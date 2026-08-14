using System.Data;
using Microsoft.Data.SqlClient;

namespace SmartHealthcare.Infrastructure.Persistence.AdoNet;

public class SqlConnectionFactory : SmartHealthcare.Application.Common.Interfaces.ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
