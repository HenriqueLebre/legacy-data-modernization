using Microsoft.AspNetCore.Mvc;
using LegacyDataAccess.Core.DTOs;
using LegacyDataAccess.Core.Interfaces;

namespace LegacyDataAccess.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _repository;

    public CustomersController(ICustomerRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerListDto>>> GetAll(
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var customers = await _repository.GetAllAsync(includeInactive, cancellationToken);
        return Ok(customers);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken);
        if (customer == null)
            return NotFound(new { message = $"Customer {id} not found" });
        return Ok(customer);
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<CustomerDto>> GetByCode(string code, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetByCodeAsync(code, cancellationToken);
        if (customer == null)
            return NotFound(new { message = $"Customer with code '{code}' not found" });
        return Ok(customer);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<CustomerListDto>>> Search(
        [FromQuery] string q,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { message = "Search term is required" });

        var customers = await _repository.SearchAsync(q, cancellationToken);
        return Ok(customers);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CustomerCreateDto customer, CancellationToken cancellationToken = default)
    {
        if (await _repository.CodeExistsAsync(customer.Code, cancellationToken))
            return Conflict(new { message = $"Customer code '{customer.Code}' already exists" });

        var id = await _repository.CreateAsync(customer, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CustomerCreateDto customer, CancellationToken cancellationToken = default)
    {
        if (!await _repository.ExistsAsync(id, cancellationToken))
            return NotFound(new { message = $"Customer {id} not found" });

        await _repository.UpdateAsync(id, customer, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound(new { message = $"Customer {id} not found" });
        return NoContent();
    }

    [HttpGet("{id:int}/balance")]
    public async Task<ActionResult<decimal>> GetBalance(int id, CancellationToken cancellationToken = default)
    {
        if (!await _repository.ExistsAsync(id, cancellationToken))
            return NotFound(new { message = $"Customer {id} not found" });

        var balance = await _repository.GetBalanceAsync(id, cancellationToken);
        return Ok(new { customerId = id, balance });
    }

    [HttpPost("{id:int}/balance")]
    public async Task<IActionResult> UpdateBalance(int id, [FromBody] BalanceUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var updated = await _repository.UpdateBalanceAsync(id, request.Amount, cancellationToken);
        if (!updated)
            return NotFound(new { message = $"Customer {id} not found" });
        return Ok(new { message = "Balance updated", customerId = id, amountChanged = request.Amount });
    }
}

public record BalanceUpdateRequest(decimal Amount);