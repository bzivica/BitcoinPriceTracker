using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace BitcoinApp.Database.Wrappers.Interfaces;

public interface IDbCommandWrapper : IDisposable
{
    // Execute SQL command and return the number of affected rows
    Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken);

    // Execute SQL command and return a SqlDataReader for reading the result set
    Task<IDataReaderWrapper> ExecuteReaderAsync(CancellationToken cancellationToken);

    // Add parameter to the SQL command
    void AddParameter(string name, object value);
    public void AddParameter(string name, object value, SqlDbType dbType);

    // Command text (the SQL query or stored procedure name)
    string CommandText { get; set; }

    // Command type (e.g., stored procedure, text, etc.)
    CommandType CommandType { get; set; }
}
