using BitcoinApp.Database.Wrappers.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinApp.Database.Wrappers;

// <summary>
/// A factory class responsible for creating instances of IDbConnectionWrapper for SQL Server connections.
/// Implements IDatabaseConnectionFactory to abstract the creation of database connections.
/// </summary>
public class SqlDatabaseConnectionFactory : IDatabaseConnectionFactory
{
    public IDbConnectionWrapper CreateConnection(string connectionString)
    {
        return new SqlConnectionWrapper(connectionString);
    }
}
