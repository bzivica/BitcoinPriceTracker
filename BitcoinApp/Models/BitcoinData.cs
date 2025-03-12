using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinApp.Models;

public class BitcoinData
{
    public decimal PriceEUR { get; set; }
    public decimal PriceCZK { get; set; }
    public DateTime Timestamp { get; set; }
    public string Note { get; set; } = string.Empty;
}
