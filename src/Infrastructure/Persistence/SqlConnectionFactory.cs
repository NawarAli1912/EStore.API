using Application.Common.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence;
public sealed class SqlConnectionFactory(IConfiguration configuration) : ISqlConnectionFactory
{
    private readonly IConfiguration _configuration = configuration;

    public SqlConnection Create()
    {
        return new SqlConnection(
            _configuration.GetConnectionString("Default"));
    }
}
