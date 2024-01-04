using Microsoft.Data.SqlClient;

namespace Application.Common.DatabaseAbstraction;

public interface ISqlConnectionFactory
{
    SqlConnection Create();
}
