using Microsoft.Data.SqlClient;

namespace Application.Common.Data;

public interface ISqlConnectionFactory
{
    SqlConnection Create();
}
