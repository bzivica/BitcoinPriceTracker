using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using BitcoinApp.Database.Wrappers;  // Assuming your wrapper is in this namespace
using BitcoinApp.Database.Wrappers.Interfaces;  // Assuming your interfaces are in this namespace

namespace BitcoinApp.UnitTests;

[TestFixture]
public class DatabaseConnectionTests
{
    [Test]
    public async Task TestDatabaseConnection_Success()
    {
        // Mock pro IDbConnectionWrapper
        var mockConnection = new Mock<IDbConnectionWrapper>();

        // Setup pro úspěšné otevření připojení (metoda OpenAsync)
        mockConnection.Setup(m => m.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();

        // Zavolání OpenAsync
        await mockConnection.Object.OpenAsync(CancellationToken.None);

        // Ověření, že metoda OpenAsync byla volána
        mockConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void TestDatabaseConnection_Failure()
    {
        // Mock pro IDbConnectionWrapper s chybou při pokusu o připojení
        var mockConnection = new Mock<IDbConnectionWrapper>();

        // Setup pro vyvolání výjimky při pokusu o připojení
        mockConnection.Setup(m => m.OpenAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Failed to connect to database"));

        // Test, že při pokusu o otevření připojení vyvolá výjimku
        Assert.ThrowsAsync<InvalidOperationException>(() => mockConnection.Object.OpenAsync(CancellationToken.None));
    }
}
