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
/// A wrapper around the SqlCommand to abstract database command execution and parameter handling.
/// Implements IDbCommandWrapper to provide an abstraction for executing SQL queries or stored procedures.
/// </summary>
public class SqlCommandWrapper : IDbCommandWrapper
{
    private readonly SqlCommand _command;

    public SqlCommandWrapper(SqlConnection connection)
    {
        _command = connection.CreateCommand();
    }

    public string CommandText
    {
        get => _command.CommandText;
        set => _command.CommandText = value;
    }

    public CommandType CommandType
    {
        get => _command.CommandType;
        set => _command.CommandType = value;
    }

    public void AddParameter(string name, object value)
    {
        // Create the parameter and assign it to the command
        SqlParameter parameter = _command.Parameters.Add(name, SqlDbType.VarChar); // Default to varChar for string types
        if (value == null)
        {
            parameter.Value = DBNull.Value;
        }
        else
        {
            parameter.Value = value;
        }
    }

    // Overload for specifying the parameter type explicitly
    public void AddParameter(string name, object value, SqlDbType dbType)
    {
        SqlParameter parameter = _command.Parameters.Add(name, dbType);
        if (value == null)
        {
            parameter.Value = DBNull.Value;
        }
        else
        {
            parameter.Value = value;
        }
    }

    public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
    {
        return await _command.ExecuteNonQueryAsync(cancellationToken);
    }

    // Vrací wrapper místo SqlDataReader
    public async Task<IDataReaderWrapper> ExecuteReaderAsync(CancellationToken cancellationToken)
    {
        var sqlDataReader = await _command.ExecuteReaderAsync(cancellationToken);
        return new SqlDataReaderWrapper(sqlDataReader);
    }
    public void Dispose()
    {
        _command?.Dispose();
    }
}
