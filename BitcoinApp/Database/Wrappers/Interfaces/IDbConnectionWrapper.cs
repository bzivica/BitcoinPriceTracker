using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace BitcoinApp.Database.Wrappers.Interfaces;

public interface IDbConnectionWrapper : IDisposable
{
    Task OpenAsync(CancellationToken cancellationToken);
    IDbCommandWrapper CreateCommand();
}
