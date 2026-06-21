using System.Data.Common;

namespace Application.Common.DatabaseAbstraction;

public interface ISqlConnectionFactory
{
    DbConnection Create();
}
