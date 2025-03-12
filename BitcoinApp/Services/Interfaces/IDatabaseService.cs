using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinApp.Services.Interfaces;

using BitcoinApp.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IDatabaseService
{
    Task CreateDatabaseSchemaAsync(CancellationToken cancellationToken);
    Task InsertBitcoinDataAsync(DateTime date, decimal priceBTC_EUR, decimal priceBTC_CZK, string note, CancellationToken cancellationToken);
    Task<List<BitcoinDataTable>> GetAllBitcoinDataAsync(CancellationToken cancellationToken);
    Task UpdateBitcoinNoteAsync(int id, string note, CancellationToken cancellationToken);
    Task DeleteBitcoinDataAsync(int id, CancellationToken cancellationToken);
    Task BulkInsertBitcoinDataAsync(List<BitcoinData> bitcoinDataList, int batchSize, CancellationToken cancellationToken);
    Task BulkUpdateBitcoinDataAsync(List<BitcoinDataTable> bitcoinDataList, int batchSize, CancellationToken cancellationToken);
    Task BulkDeleteBitcoinDataAsync(List<int> idsToDelete, int batchSize, CancellationToken cancellationToken);
}
