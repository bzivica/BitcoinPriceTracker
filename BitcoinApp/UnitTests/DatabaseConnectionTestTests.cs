using Moq;
using NUnit.Framework;
using System;
using Microsoft.Data.SqlClient;
namespace BitcoinApp.UnitTests

{
    [TestFixture]
    public class DatabaseConnectionTestTests
    {
        [Test]
        public void TestDatabaseConnection_Success()
        {
            // Mock pro SqlConnection
            var mockConnection = new Mock<SqlConnection>("Server = (localdb)\\MSSQLLocalDB; Database = BitcoinPriceTrackerDb; Trusted_Connection = True; ");

            // Mock pro úspěšné otevření připojení
            mockConnection.Setup(m => m.Open()).Verifiable();

            // Test, že metoda Open() je volána (simulujeme úspěšné připojení)
            mockConnection.Object.Open();

            // Ověření, že Open byla zavolána
            mockConnection.Verify(m => m.Open(), Times.Once);
        }

        [Test]
        public void TestDatabaseConnection_Failure()
        {
            // Mock pro SqlConnection s chybou při připojení
            var mockConnection = new Mock<SqlConnection>("Server=myInvalidServer;Database=BitcoinPriceTrackerDb;Integrated Security=True;");

            // Setup pro vyvolání výjimky při pokusu o připojení
            mockConnection.Setup(m => m.Open()).Throws(new InvalidOperationException("Failed to connect to database"));

            // Test, že při pokusu o otevření připojení vyvolá výjimku
            Assert.Throws<InvalidOperationException>(() => mockConnection.Object.Open());
        }
    }
}
