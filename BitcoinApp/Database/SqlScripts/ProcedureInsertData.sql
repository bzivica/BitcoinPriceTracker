-- Procedura pro vložení dat
CREATE PROCEDURE InsertBitcoinPrice
    @Date DATETIME,
    @BitcoinPriceEUR DECIMAL(18, 2),
    @BitcoinPriceCZK DECIMAL(18, 2)
AS
BEGIN
    INSERT INTO BitcoinPrice (Date, BitcoinPriceEUR, BitcoinPriceCZK)
    VALUES (@Date, @BitcoinPriceEUR, @BitcoinPriceCZK);
END;