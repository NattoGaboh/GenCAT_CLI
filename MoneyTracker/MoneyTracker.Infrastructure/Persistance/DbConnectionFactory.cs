using System.Data;
using Microsoft.Extensions.Configuration;

namespace {{ProjectName }}.Infrastructure.Persistence;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        // PostgreSQL
        if (_configuration["Database"] == "PostgreSQL")
            return new Npgsql.NpgsqlConnection(connectionString);

        // SQL Server
        return new Microsoft.Data.SqlClient.SqlConnection(connectionString);
    }
}