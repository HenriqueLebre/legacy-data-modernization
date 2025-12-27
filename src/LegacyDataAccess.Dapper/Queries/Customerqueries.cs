namespace LegacyDataAccess.Dapper.Queries;

/// <summary>
/// SQL queries for customer operations
/// Organized separately for easy maintenance and DBA review
/// </summary>
public static class CustomerQueries
{
    public const string GetById = @"
        SELECT 
            id, code, name, email, phone, document,
            address, city, state, zip_code,
            is_active, credit_limit, current_balance,
            created_at, updated_at
        FROM customers
        WHERE id = @Id";

    public const string GetByCode = @"
        SELECT 
            id, code, name, email, phone, document,
            address, city, state, zip_code,
            is_active, credit_limit, current_balance,
            created_at, updated_at
        FROM customers
        WHERE code = @Code";

    public const string GetAll = @"
        SELECT id, code, name, phone, is_active
        FROM customers
        WHERE (@IncludeInactive = true OR is_active = true)
        ORDER BY name";

    public const string Search = @"
        SELECT id, code, name, phone, is_active
        FROM customers
        WHERE (
            name ILIKE @SearchTerm OR
            code ILIKE @SearchTerm OR
            document ILIKE @SearchTerm OR
            email ILIKE @SearchTerm
        )
        AND is_active = true
        ORDER BY name
        LIMIT 100";

    public const string Exists = @"
        SELECT EXISTS(SELECT 1 FROM customers WHERE id = @Id)";

    public const string CodeExists = @"
        SELECT EXISTS(SELECT 1 FROM customers WHERE code = @Code)";

    public const string Insert = @"
        INSERT INTO customers (
            code, name, email, phone, document,
            address, city, state, zip_code,
            credit_limit, is_active, created_at
        ) VALUES (
            @Code, @Name, @Email, @Phone, @Document,
            @Address, @City, @State, @ZipCode,
            @CreditLimit, true, NOW()
        )
        RETURNING id";

    public const string Update = @"
        UPDATE customers SET
            code = @Code,
            name = @Name,
            email = @Email,
            phone = @Phone,
            document = @Document,
            address = @Address,
            city = @City,
            state = @State,
            zip_code = @ZipCode,
            credit_limit = @CreditLimit,
            updated_at = NOW()
        WHERE id = @Id";

    public const string Delete = @"
        UPDATE customers SET 
            is_active = false,
            updated_at = NOW()
        WHERE id = @Id";

    public const string GetBalance = @"
        SELECT current_balance FROM customers WHERE id = @Id";

    public const string UpdateBalance = @"
        UPDATE customers SET
            current_balance = current_balance + @Amount,
            updated_at = NOW()
        WHERE id = @Id";
}