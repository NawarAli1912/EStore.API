using System.Data.Common;
using Application.Common.DatabaseAbstraction;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Persistence;
public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbConnection Create()
    {
        return new NpgsqlConnection(
            _configuration.GetConnectionString("Default"));
    }
}
