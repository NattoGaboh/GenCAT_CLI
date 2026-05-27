using System.Data;

namespace {{ProjectName }}.Infrastructure.Persistence;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}