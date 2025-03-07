-- Vytvoření tabulky BitcoinData
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'BitcoinData')
BEGIN
    CREATE TABLE BitcoinData (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PriceEUR DECIMAL(18,2),
        PriceCZK DECIMAL(18,2),
        Timestamp DATETIME,
        Note NVARCHAR(255)
    )
END

/*TO DO: Zvazit jeste jesti nektery z nich bude treba
-- Index na Timestamp
CREATE NONCLUSTERED INDEX IDX_BitcoinData_Timestamp
ON BitcoinData (Timestamp);

-- Index na PriceEUR
CREATE NONCLUSTERED INDEX IDX_BitcoinData_PriceEUR
ON BitcoinData (PriceEUR);

-- Index na PriceCZK
CREATE NONCLUSTERED INDEX IDX_BitcoinData_PriceCZK
ON BitcoinData (PriceCZK);

-- Kombinovaný index na Timestamp a PriceEUR
CREATE NONCLUSTERED INDEX IDX_BitcoinData_Timestamp_PriceEUR
ON BitcoinData (Timestamp, PriceEUR);

-- Kombinovaný index na Timestamp a PriceCZK
CREATE NONCLUSTERED INDEX IDX_BitcoinData_Timestamp_PriceCZK
ON BitcoinData (Timestamp, PriceCZK);
*/
