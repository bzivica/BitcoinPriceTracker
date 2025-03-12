using BitcoinApp.Database.Wrappers.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinApp.Database.Wrappers;

/// <summary>
/// A wrapper around the SqlConnection to abstract database connection operations.
/// Implements IDbConnectionWrapper to provide an abstraction for managing a SQL Server connection.
/// </summary>
public class SqlConnectionWrapper : IDbConnectionWrapper
{
    private readonly SqlConnection _connection;

    public SqlConnectionWrapper(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
    }

    public async Task OpenAsync(CancellationToken cancellationToken)
    {
        await _connection.OpenAsync(cancellationToken);
    }

    public IDbCommandWrapper CreateCommand()
    {
        return new SqlCommandWrapper(_connection);
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
