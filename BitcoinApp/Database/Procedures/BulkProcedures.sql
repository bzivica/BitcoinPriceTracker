CREATE PROCEDURE BulkInsertBitcoinData
    @DataToInsert BitcoinDataType READONLY,
    @BatchSize INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalRows INT, @ProcessedRows INT = 0;

    -- Získání celkového počtu řádků k vložení
    SELECT @TotalRows = COUNT(*) FROM @DataToInsert;

    -- Smyčka pro dávkové zpracování
    WHILE @ProcessedRows < @TotalRows
    BEGIN
        ;WITH CTE AS (
            SELECT t.Timestamp, t.PriceEUR, t.PriceCZK, t.Note,
                   ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS RowNum
            FROM @DataToInsert t
        )
        INSERT INTO BitcoinData (PriceEUR, PriceCZK, Timestamp, Note)
        SELECT CTE.PriceEUR, CTE.PriceCZK, CTE.Timestamp, CTE.Note
        FROM CTE
        WHERE CTE.RowNum > @ProcessedRows
          AND CTE.RowNum <= @ProcessedRows + @BatchSize
          AND NOT EXISTS (
              SELECT 1
              FROM BitcoinData b
              WHERE b.Timestamp = CTE.Timestamp -- Zkontrolujte, zda již existuje záznam s tímto Timestamp
          );

        -- Aktualizace počtu zpracovaných řádků
        SET @ProcessedRows = @ProcessedRows + @BatchSize;
    END
END;
GO

-- Stored procedure for bulk updating data with Note field condition
CREATE PROCEDURE BulkUpdateBitcoinData
    @DataToUpdate BitcoinDataUpdateType READONLY,
    @BatchSize INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalRows INT, @ProcessedRows INT = 0;

    -- Get total rows to update
    SELECT @TotalRows = COUNT(*) FROM @DataToUpdate;

    -- Loop for batch processing
    WHILE @ProcessedRows < @TotalRows
    BEGIN
        -- Update batch of data, only if Note field has changed
        WITH CTE AS (
            SELECT Id, PriceEUR, PriceCZK, Timestamp, Note,
                   ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS RowNum
            FROM @DataToUpdate
            WHERE Note IS NOT NULL -- Perform update only for records with changed Note field
        )
        UPDATE bd
        SET bd.PriceEUR = u.PriceEUR,
            bd.PriceCZK = u.PriceCZK,
            bd.Timestamp = u.Timestamp,
            bd.Note = u.Note
        FROM BitcoinData AS bd
        INNER JOIN CTE AS u ON bd.Id = u.Id
        WHERE u.RowNum > @ProcessedRows AND u.RowNum <= @ProcessedRows + @BatchSize;

        -- Update processed rows count
        SET @ProcessedRows = @ProcessedRows + @BatchSize;
    END
END;
GO

-- Stored procedure for bulk deleting data in batches
CREATE PROCEDURE BulkDeleteBitcoinData
    @IdsToDelete BitcoinDataDeleteType READONLY,
    @BatchSize INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalRows INT, @ProcessedRows INT = 0;

    -- Get total rows to delete
    SELECT @TotalRows = COUNT(*) FROM @IdsToDelete;

    -- Loop for batch processing
    WHILE @ProcessedRows < @TotalRows
    BEGIN
        -- Delete batch of data
        WITH CTE AS (
            SELECT Id,
                   ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS RowNum
            FROM @IdsToDelete
        )
        DELETE bd
        FROM BitcoinData AS bd
        INNER JOIN CTE AS u ON bd.Id = u.Id
        WHERE u.RowNum > @ProcessedRows AND u.RowNum <= @ProcessedRows + @BatchSize;

        -- Update processed rows count
        SET @ProcessedRows = @ProcessedRows + @BatchSize;
    END
END;
GO
