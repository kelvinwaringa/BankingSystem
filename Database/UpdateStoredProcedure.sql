-- Update stored procedure to include TransactionTypeId
USE BankingSystemDB;
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetTransactionHistory')
    DROP PROCEDURE sp_GetTransactionHistory;
GO

CREATE PROCEDURE sp_GetTransactionHistory
    @AccountId INT,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @TransactionTypeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.TransactionId,
        t.AccountId,
        t.TransactionTypeId,
        tt.TypeName AS TransactionType,
        t.Amount,
        t.BalanceAfterTransaction,
        t.Description,
        t.RelatedAccountId,
        t.TransactionDate
    FROM Transactions t
    INNER JOIN TransactionTypes tt ON t.TransactionTypeId = tt.TransactionTypeId
    WHERE t.AccountId = @AccountId
        AND (@StartDate IS NULL OR t.TransactionDate >= @StartDate)
        AND (@EndDate IS NULL OR t.TransactionDate <= @EndDate)
        AND (@TransactionTypeId IS NULL OR t.TransactionTypeId = @TransactionTypeId)
    ORDER BY t.TransactionDate DESC;
END
GO

