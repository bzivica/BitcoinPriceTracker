using Moq;  
using NUnit.Framework;
using System;  
using System.Collections.Generic;
using System.Data;  
using System.IO;  
using System.Threading;
using System.Threading.Tasks;
using BitcoinApp.Database.Wrappers; 
using BitcoinApp.Database.Wrappers.Interfaces;
using BitcoinApp.Models;  
using BitcoinApp.Services; 
using Microsoft.Extensions.Configuration;

namespace BitcoinApp.Tests;

[TestFixture]
public class DatabaseServiceTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<IDatabaseConnectionFactory> _mockConnectionFactory;
    private DatabaseService _databaseService;

    [SetUp]
    public void SetUp()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConnectionFactory = new Mock<IDatabaseConnectionFactory>();

        _mockConfiguration.Setup(c => c[It.Is<string>(s => s == "ConnectionStrings:DefaultConnection")])
            .Returns("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;");
        _mockConfiguration.Setup(c => c[It.Is<string>(s => s == "Database:SqlScriptPath")])
            .Returns("path/to/sqlscript.sql");

        _databaseService = new DatabaseService(
            _mockConfiguration.Object,
            _mockConnectionFactory.Object
        );
    }


    [Test]
    public void CreateDatabaseSchemaAsync_ShouldThrowException_WhenScriptPathIsInvalid()
    {
        // Arrange
        _mockConfiguration.Setup(c => c[It.Is<string>(s => s == "Database:SqlScriptPath")])
            .Returns("invalid/path");

        var cancellationToken = CancellationToken.None;

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _databaseService.CreateDatabaseSchemaAsync(cancellationToken));
        Assert.That(ex.Message, Is.EqualTo("SQL script path is not configured or the script file does not exist: path/to/sqlscript.sql."));
    }

    [Test]
    public async Task InsertBitcoinDataAsync_ShouldInsertData_WhenValidDataIsProvided()
    {
        // Arrange
        var date = DateTime.Now;
        decimal priceBTC_EUR = 40000.0m;
        decimal priceBTC_CZK = 1000000.0m;
        string note = "Test data";
        var cancellationToken = CancellationToken.None;

        var mockConnection = new Mock<IDbConnectionWrapper>();
        var mockCommandWrapper = new Mock<IDbCommandWrapper>();
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommandWrapper.Object);
        mockCommandWrapper.Setup(cmd => cmd.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockConnectionFactory.Setup(factory => factory.CreateConnection(It.IsAny<string>())).Returns(mockConnection.Object);

        // Act
        await _databaseService.InsertBitcoinDataAsync(date, priceBTC_EUR, priceBTC_CZK, note, cancellationToken);

        // Assert
        mockCommandWrapper.Verify(cmd => cmd.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetAllBitcoinDataAsync_ShouldReturnBitcoinData_WhenDataExists()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var mockData = new List<BitcoinDataTable>
        {
            new BitcoinDataTable { Id = 1, PriceEUR = 40000.0m, PriceCZK = 1000000.0m, Timestamp = DateTime.Now, Note = "Test data" }
        };

        // Mock the IDataReaderWrapper
        var mockReaderWrapper = new Mock<IDataReaderWrapper>();

        // Mock ReadAsync
        mockReaderWrapper.SetupSequence(r => r.ReadAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true)  // First call to ReadAsync returns true (data available)
                         .ReturnsAsync(false); // Second call returns false (no more data)

        // Mock GetOrdinal for each column in the data
        mockReaderWrapper.Setup(r => r.GetOrdinal("Id")).Returns(0);  
        mockReaderWrapper.Setup(r => r.GetOrdinal("PriceEUR")).Returns(1);
        mockReaderWrapper.Setup(r => r.GetOrdinal("PriceCZK")).Returns(2);
        mockReaderWrapper.Setup(r => r.GetOrdinal("Timestamp")).Returns(3);
        mockReaderWrapper.Setup(r => r.GetOrdinal("Note")).Returns(4);  

        // Mock the Get methods for each column
        mockReaderWrapper.Setup(r => r.GetInt32(0)).Returns(mockData[0].Id); 
        mockReaderWrapper.Setup(r => r.GetDecimal(1)).Returns(mockData[0].PriceEUR);
        mockReaderWrapper.Setup(r => r.GetDecimal(2)).Returns(mockData[0].PriceCZK);
        mockReaderWrapper.Setup(r => r.GetDateTime(3)).Returns(mockData[0].Timestamp); 
        mockReaderWrapper.Setup(r => r.IsDBNull(It.IsAny<int>())).Returns(false);  // No DBNull values in this mock
        mockReaderWrapper.Setup(r => r.GetString(4)).Returns(mockData[0].Note);  

        // Mock IDbCommandWrapper
        var mockCommandWrapper = new Mock<IDbCommandWrapper>();

        // Setup ExecuteReaderAsync to return Task<IDataReaderWrapper>
        mockCommandWrapper.Setup(cmd => cmd.ExecuteReaderAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(mockReaderWrapper.Object);  // Return mocked IDataReaderWrapper

        // Mock IDbConnectionWrapper
        var mockConnection = new Mock<IDbConnectionWrapper>();
        mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommandWrapper.Object);

        // Setup for ConnectionFactory
        _mockConnectionFactory.Setup(factory => factory.CreateConnection(It.IsAny<string>())).Returns(mockConnection.Object);

        // Act
        var result = await _databaseService.GetAllBitcoinDataAsync(cancellationToken);

        // Verify that ExecuteReaderAsync was called exactly once
        mockCommandWrapper.Verify(cmd => cmd.ExecuteReaderAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));  
        Assert.That(result[0].Id, Is.EqualTo(mockData[0].Id)); 
        Assert.That(result[0].PriceEUR, Is.EqualTo(mockData[0].PriceEUR)); 
        Assert.That(result[0].PriceCZK, Is.EqualTo(mockData[0].PriceCZK));  
        Assert.That(result[0].Timestamp, Is.EqualTo(mockData[0].Timestamp)); 
        Assert.That(result[0].Note, Is.EqualTo(mockData[0].Note));  
    }



    [Test]
    public async Task UpdateBitcoinNoteAsync_ShouldUpdateNote_WhenValidIdAndNoteAreProvided()
    {
        // Arrange
        var id = 1;
        var note = "Updated Note";
        var cancellationToken = CancellationToken.None;

        // Mocking connection and ISqlCommandWrapper
        var mockConnection = new Mock<IDbConnectionWrapper>();
        var mockCommandWrapper = new Mock<IDbCommandWrapper>();
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommandWrapper.Object);
        mockCommandWrapper.Setup(cmd => cmd.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockConnectionFactory.Setup(factory => factory.CreateConnection(It.IsAny<string>())).Returns(mockConnection.Object);

        // Act
        await _databaseService.UpdateBitcoinNoteAsync(id, note, cancellationToken);

        // Assert
        mockCommandWrapper.Verify(cmd => cmd.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeleteBitcoinDataAsync_ShouldDeleteData_WhenValidIdIsProvided()
    {
        // Arrange
        var id = 1;
        var cancellationToken = CancellationToken.None;

        // Mocking connection and ISqlCommandWrapper
        var mockConnection = new Mock<IDbConnectionWrapper>();
        var mockCommandWrapper = new Mock<IDbCommandWrapper>();
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommandWrapper.Object);
        mockCommandWrapper.Setup(cmd => cmd.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockConnectionFactory.Setup(factory => factory.CreateConnection(It.IsAny<string>())).Returns(mockConnection.Object);

        // Act
        await _databaseService.DeleteBitcoinDataAsync(id, cancellationToken);

        // Assert
        mockCommandWrapper.Verify(cmd => cmd.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}
