using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinApp.Database.Wrappers.Interfaces;

public interface IDatabaseConnectionFactory
{
    IDbConnectionWrapper CreateConnection(string connectionString);
}
