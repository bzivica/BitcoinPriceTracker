using BitcoinApp.Database.Wrappers.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinApp.Database.Wrappers;

/// <summary>
/// A wrapper around the SqlDataReader to enable easier mocking, testing, and resource management.
/// Implements IDataReaderWrapper to provide an abstraction for asynchronous reading of database records.
/// </summary>
public class SqlDataReaderWrapper : IDataReaderWrapper
{
    private readonly SqlDataReader _reader;
    private bool _disposed;

    public SqlDataReaderWrapper(SqlDataReader reader)
    {
        _reader = reader;
        _disposed = false;
    }

    public async Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
        return await _reader.ReadAsync(cancellationToken);
    }

    public int GetOrdinal(string name)
    {
        return _reader.GetOrdinal(name);
    }

    public int GetInt32(int ordinal)
    {
        return _reader.GetInt32(ordinal);
    }

    public decimal GetDecimal(int ordinal)
    {
        return _reader.GetDecimal(ordinal);
    }

    public DateTime GetDateTime(int ordinal)
    {
        return _reader.GetDateTime(ordinal);
    }

    public string GetString(int ordinal)
    {
        return _reader.GetString(ordinal);
    }

    public bool IsDBNull(int ordinal)
    {
        return _reader.IsDBNull(ordinal);
    }

    // IDisposable implementation
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Resoruces dispose
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _reader?.Dispose();
            }

            _disposed = true;
        }
    }

    // Finalizer
    ~SqlDataReaderWrapper()
    {
        Dispose(false);
    }
}
