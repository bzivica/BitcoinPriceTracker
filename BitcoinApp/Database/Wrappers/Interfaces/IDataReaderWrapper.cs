using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinApp.Database.Wrappers.Interfaces;

public interface IDataReaderWrapper : IDisposable
{
    Task<bool> ReadAsync(CancellationToken cancellationToken);
    int GetOrdinal(string name);
    int GetInt32(int ordinal);
    decimal GetDecimal(int ordinal);
    DateTime GetDateTime(int ordinal);
    string GetString(int ordinal);
    bool IsDBNull(int ordinal);
}
