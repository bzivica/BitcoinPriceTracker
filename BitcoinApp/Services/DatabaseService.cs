using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using BitcoinApp.Models;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace BitcoinApp.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;
        private readonly string _scriptPath;

        public DatabaseService(string connectionString, IConfiguration configuration)
        {
            _connectionString = connectionString;
            _scriptPath = configuration.GetValue<string>("Database:SqlScriptPath") ?? throw new ArgumentNullException(nameof(configuration), "SQL script path is not configured.");
        }

        // Vytvoření databáze, pokud neexistuje – pouze pro vývojové účely. 
        // Tento přístup není doporučen pro produkční nasazení.

        /*public void CreateDatabaseIfNotExists()
        {
            // Získání názvu databáze z connection stringu
            var connectionStringBuilder = new SqlConnectionStringBuilder(_connectionString);
            string databaseName = connectionStringBuilder.InitialCatalog;

            // Připojení k master databázi pro vytvoření nové databáze, pokud neexistuje
            string masterConnectionString = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master"
            }.ToString();

            using (SqlConnection connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();

                // SQL příkaz pro vytvoření databáze, pokud neexistuje
                string checkDatabaseExistsQuery = $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{databaseName}') " +
                                                  $"BEGIN CREATE DATABASE {databaseName} END";

                SqlCommand command = new SqlCommand(checkDatabaseExistsQuery, connection);
                command.ExecuteNonQuery();
            }

            // Po vytvoření databáze spustíme SQL skript pro vytvoření tabulek
            CreateDatabaseSchema();
        }*/

        // Vytvoření databázového schématu (tabulek)
        public void CreateDatabaseSchema()
        {
            if (string.IsNullOrEmpty(_scriptPath))
            {
                throw new InvalidOperationException("SQL script path is not configured.");
            }

            // Načtení SQL skriptu ze souboru
            string script = File.ReadAllText(_scriptPath);

            // Spuštění SQL skriptu
            ExecuteSqlScript(script);
        }

        // Metoda pro vykonání SQL skriptu
        private void ExecuteSqlScript(string script)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(script, connection);
                command.ExecuteNonQuery();
            }
        }

        // Uložení dat o Bitcoinu
        public void SaveBitcoinData(decimal priceEUR, decimal priceCZK, DateTime timestamp)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO BitcoinData (PriceEUR, PriceCZK, Timestamp, Note) VALUES (@PriceEUR, @PriceCZK, @Timestamp, @Note)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PriceEUR", priceEUR);
                command.Parameters.AddWithValue("@PriceCZK", priceCZK);
                command.Parameters.AddWithValue("@Timestamp", timestamp);
                command.Parameters.AddWithValue("@Note", string.Empty);
                command.ExecuteNonQuery();
            }
        }

        // Získání uložených dat o Bitcoinu
        public List<BitcoinData> GetSavedBitcoinData()
        {
            List<BitcoinData> dataList = new List<BitcoinData>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM BitcoinData";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    BitcoinData data = new BitcoinData
                    {
                        Id = (int)reader["Id"],
                        PriceEUR = (decimal)reader["PriceEUR"],
                        PriceCZK = (decimal)reader["PriceCZK"],
                        Timestamp = (DateTime)reader["Timestamp"],
                        Note = reader["Note"] as string ?? string.Empty
                    };
                    dataList.Add(data);
                }
            }
            return dataList;
        }

        // Aktualizace dat o Bitcoinu
        public void UpdateBitcoinData(BitcoinData data)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE BitcoinData SET Note = @Note WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Note", data.Note);
                command.Parameters.AddWithValue("@Id", data.Id);
                command.ExecuteNonQuery();
            }
        }

        // Smazání dat o Bitcoinu
        public void DeleteBitcoinData(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM BitcoinData WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
