-- InsertBitcoinData
-- This procedure inserts a new Bitcoin record into the BitcoinData table
CREATE PROCEDURE InsertBitcoinData
    @Date DATETIME,
    @PriceBTC_EUR DECIMAL(18, 2),
    @PriceBTC_CZK DECIMAL(18, 2),
    @Note NVARCHAR(255)
AS
BEGIN
    INSERT INTO BitcoinData (Timestamp, PriceEUR, PriceCZK, Note)
    VALUES (@Date, @PriceBTC_EUR, @PriceBTC_CZK, @Note);
END;
GO

-- GetAllBitcoinData
-- This procedure retrieves all Bitcoin data from the BitcoinData table
CREATE PROCEDURE GetAllBitcoinData
AS
BEGIN
    SELECT * FROM BitcoinData;
END;
GO

-- UpdateBitcoinNote
-- This procedure updates the 'Note' field for a specific Bitcoin record based on its Id
CREATE PROCEDURE UpdateBitcoinNote
    @Id INT,
    @Note NVARCHAR(255)
AS
BEGIN
    UPDATE BitcoinData
    SET Note = @Note
    WHERE Id = @Id;
END;
GO

-- DeleteBitcoinData
-- This procedure deletes a Bitcoin record based on its Id
CREATE PROCEDURE DeleteBitcoinData
    @Id INT
AS
BEGIN
    DELETE FROM BitcoinData
    WHERE Id = @Id;
END;
GO
