﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinApp.Services.Interfaces;

public interface ICnbService
{
    Task<decimal> GetEurToCzkRateAsync();
}
