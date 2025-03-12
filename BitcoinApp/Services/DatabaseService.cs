using BitcoinApp.Database.Wrappers;
using BitcoinApp.Database.Wrappers.Interfaces;
using BitcoinApp.Models;
using BitcoinApp.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BitcoinApp.Services;

/// <summary>
/// Service to interact with the database, including operations such as creating the schema, inserting, updating, deleting, and bulk operations for Bitcoin data.
/// </summary>
public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;
    private readonly string _scriptPath;
    private readonly IDatabaseConnectionFactory _connectionFactory;

    private const string DbScirptsPathKey = "Database:SqlScriptPath";
    private const string DbDdefaultConnectionKey = "ConnectionStrings:DefaultConnection";

    public DatabaseService(IConfiguration configuration, IDatabaseConnectionFactory databaseConnectionFactory)
    {
        _connectionString = configuration[DbDdefaultConnectionKey] ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _connectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        _scriptPath = configuration[DbScirptsPathKey] ?? throw new ArgumentNullException(nameof(configuration), "SQL script path is not configured.");
    }


    // Helper method to convert list of BitcoinData to DataTable
    private DataTable CreateBitcoinData(List<BitcoinData> bitcoinDataList)
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add("PriceEUR", typeof(decimal));
        dataTable.Columns.Add("PriceCZK", typeof(decimal));
        dataTable.Columns.Add("Timestamp", typeof(DateTime));
        dataTable.Columns.Add("Note", typeof(string));

        foreach (var data in bitcoinDataList)
        {
            dataTable.Rows.Add(data.PriceEUR, data.PriceCZK, data.Timestamp, data.Note);
        }

        return dataTable;
    }

    // Helper method to convert list of BitcoinData to DataTable
    private DataTable CreateBitcoinDataTable(List<BitcoinDataTable> bitcoinDataList)
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id", typeof(int));
        dataTable.Columns.Add("PriceEUR", typeof(decimal));
        dataTable.Columns.Add("PriceCZK", typeof(decimal));
        dataTable.Columns.Add("Timestamp", typeof(DateTime));
        dataTable.Columns.Add("Note", typeof(string));

        foreach (var data in bitcoinDataList)
        {
            dataTable.Rows.Add(data.Id, data.PriceEUR, data.PriceCZK, data.Timestamp, data.Note);
        }

        return dataTable;
    }

    // Helper method to convert list of Ids to DataTable
    private DataTable CreateIdListTable(List<int> idsToDelete)
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id", typeof(int));

        foreach (var id in idsToDelete)
        {
            dataTable.Rows.Add(id);
        }

        return dataTable;
    }

    public async Task CreateDatabaseSchemaAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_scriptPath) || !File.Exists(_scriptPath))
        {
            throw new InvalidOperationException($"SQL script path is not configured or the script file does not exist: {_scriptPath}.");
        }

        string script = await File.ReadAllTextAsync(_scriptPath, cancellationToken);
        await ExecuteSqlScriptAsync(script, cancellationToken);
    }

    private async Task ExecuteSqlScriptAsync(string script, CancellationToken cancellationToken)
    {
        using (var connection = _connectionFactory.CreateConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = script;
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }

    public async Task InsertBitcoinDataAsync(DateTime date, decimal priceBTC_EUR, decimal priceBTC_CZK, string note, CancellationToken cancellationToken)
    {
        using (var connection = _connectionFactory.CreateConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "InsertBitcoinData";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@Date", date);
                command.AddParameter("@PriceBTC_EUR", priceBTC_EUR);
                command.AddParameter("@PriceBTC_CZK", priceBTC_CZK);
                command.AddParameter("@Note", note);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }
    public async Task<List<BitcoinDataTable>> GetAllBitcoinDataAsync(CancellationToken cancellationToken)
    {
        var dataList = new List<BitcoinDataTable>();

        using (var connection = _connectionFactory.CreateConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "GetAllBitcoinData";
                command.CommandType = CommandType.StoredProcedure;

                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var data = new BitcoinDataTable
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PriceEUR = reader.GetDecimal(reader.GetOrdinal("PriceEUR")),
                            PriceCZK = reader.GetDecimal(reader.GetOrdinal("PriceCZK")),
                            Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp")),
                            Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? string.Empty : reader.GetString(reader.GetOrdinal("Note"))
                        };
                        dataList.Add(data);
                    }
                }
            }
        }

        return dataList;
    }

    public async Task UpdateBitcoinNoteAsync(int id, string note, CancellationToken cancellationToken)
    {
        using (var connection = _connectionFactory.CreateConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UpdateBitcoinNote";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@Id", id);
                command.AddParameter("@Note", note);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }

    public async Task DeleteBitcoinDataAsync(int id, CancellationToken cancellationToken)
    {
        using (var connection = _connectionFactory.CreateConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DeleteBitcoinData";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@Id", id);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }

    #region Bulk operations
    public async Task BulkInsertBitcoinDataAsync(List<BitcoinData> bitcoinDataList, int batchSize, CancellationToken cancellationToken)
    {
        if (bitcoinDataList == null || bitcoinDataList.Count == 0) return;

        using (var connection = _connectionFactory.CreateConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "BulkInsertBitcoinData"; // Name of the stored procedure for bulk insert
                command.CommandType = CommandType.StoredProcedure;

                // Create a table-valued parameter
                var dataTable = CreateBitcoinData(bitcoinDataList); // Convert to DataTable

                // Use AddParameter to add parameters
                command.AddParameter("@DataToInsert", dataTable, SqlDbType.Structured);
                command.AddParameter("@BatchSize", batchSize, SqlDbType.Int);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }

    public async Task BulkUpdateBitcoinDataAsync(List<BitcoinDataTable> bitcoinDataList, int batchSize, CancellationToken cancellationToken)
    {
        if (bitcoinDataList == null || bitcoinDataList.Count == 0) return;

        using (var connection = _connectionFactory.CreateConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "BulkUpdateBitcoinData"; // Name of the stored procedure for bulk update
                command.CommandType = CommandType.StoredProcedure;

                // Create a table-valued parameter
                var dataTable = CreateBitcoinDataTable(bitcoinDataList); // Convert to DataTable

                // Use AddParameter to add parameters
                command.AddParameter("@DataToUpdate", dataTable, SqlDbType.Structured);
                command.AddParameter("@BatchSize", batchSize, SqlDbType.Int);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }

    public async Task BulkDeleteBitcoinDataAsync(List<int> idsToDelete, int batchSize, CancellationToken cancellationToken)
    {
        if (idsToDelete == null || idsToDelete.Count == 0) return;

        using (var connection = _connectionFactory.CreateConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "BulkDeleteBitcoinData"; // Name of the stored procedure for bulk delete
                command.CommandType = CommandType.StoredProcedure;

                // Create a table-valued parameter
                var dataTable = CreateIdListTable(idsToDelete); // Convert to DataTable

                // Use AddParameter to add parameters
                command.AddParameter("@IdsToDelete", dataTable, SqlDbType.Structured);
                command.AddParameter("@BatchSize", batchSize, SqlDbType.Int);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }
    #endregion

}
