-- BitcoinDataType (for BulkInsert)
CREATE TYPE BitcoinDataType AS TABLE
(
    PriceEUR DECIMAL(18, 2),
    PriceCZK DECIMAL(18, 2),
    Timestamp DATETIME,
    Note NVARCHAR(255)
);
GO

-- BitcoinDataUpdateType (for BulkUpdate)
CREATE TYPE BitcoinDataUpdateType AS TABLE
(
    Id INT,
    PriceEUR DECIMAL(18, 2),
    PriceCZK DECIMAL(18, 2),
    Timestamp DATETIME,
    Note NVARCHAR(255)
);
GO

-- BitcoinDataDeleteType (for BulkDelete)
CREATE TYPE BitcoinDataDeleteType AS TABLE
(
    Id INT
);
GO
