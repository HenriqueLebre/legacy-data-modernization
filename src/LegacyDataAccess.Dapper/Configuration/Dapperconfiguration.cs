using Microsoft.Extensions.Configuration;
using Npgsql;

namespace LegacyDataAccess.Dapper.Configuration;

public class DapperConfiguration
{
    private readonly string _connectionString;
    private readonly int _commandTimeout;
    private readonly int _maxRetryCount;

    public DapperConfiguration(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        _commandTimeout = configuration.GetValue<int>("Dapper:CommandTimeout", 30);
        _maxRetryCount = configuration.GetValue<int>("Dapper:MaxRetryCount", 3);
        
        ConfigureNpgsql();
    }

    public DapperConfiguration(string connectionString)
    {
        _connectionString = connectionString;
        _commandTimeout = 30;
        _maxRetryCount = 3;
        
        ConfigureNpgsql();
    }

    private static void ConfigureNpgsql()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        global::Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    public async Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        
        var retryCount = 0;
        while (true)
        {
            try
            {
                await connection.OpenAsync(cancellationToken);
                return connection;
            }
            catch (NpgsqlException) when (retryCount < _maxRetryCount)
            {
                retryCount++;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount)), cancellationToken);
            }
        }
    }

    public int CommandTimeout => _commandTimeout;
}