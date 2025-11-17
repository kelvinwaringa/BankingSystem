-- Banking System Database Creation Script
-- Microsoft SQL Server

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BankingSystemDB')
BEGIN
    CREATE DATABASE BankingSystemDB;
END
GO

USE BankingSystemDB;
GO

-- Create Users Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(50) UNIQUE NOT NULL,
        Email NVARCHAR(100) UNIQUE NOT NULL,
        PasswordHash NVARCHAR(256) NOT NULL,
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        PhoneNumber NVARCHAR(20),
        Address NVARCHAR(200),
        DateOfBirth DATE,
        Role NVARCHAR(20) NOT NULL DEFAULT 'Customer', -- 'Customer' or 'Admin'
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        LastModifiedDate DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- Create AccountTypes Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AccountTypes')
BEGIN
    CREATE TABLE AccountTypes (
        AccountTypeId INT PRIMARY KEY IDENTITY(1,1),
        TypeName NVARCHAR(50) NOT NULL UNIQUE, -- 'Savings', 'Checking', 'Credit'
        Description NVARCHAR(200),
        InterestRate DECIMAL(5,2) DEFAULT 0.00,
        MinimumBalance DECIMAL(18,2) DEFAULT 0.00
    );
    
    -- Insert default account types
    INSERT INTO AccountTypes (TypeName, Description, InterestRate, MinimumBalance) VALUES
    ('Savings', 'Savings Account', 2.5, 100.00),
    ('Checking', 'Checking Account', 0.0, 0.00),
    ('Credit', 'Credit Card Account', 0.0, 0.00);
END
GO

-- Create Accounts Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Accounts')
BEGIN
    CREATE TABLE Accounts (
        AccountId INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        AccountTypeId INT NOT NULL,
        AccountNumber NVARCHAR(20) UNIQUE NOT NULL,
        Balance DECIMAL(18,2) NOT NULL DEFAULT 0.00,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        LastModifiedDate DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
        FOREIGN KEY (AccountTypeId) REFERENCES AccountTypes(AccountTypeId)
    );
END
GO

-- Create TransactionTypes Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TransactionTypes')
BEGIN
    CREATE TABLE TransactionTypes (
        TransactionTypeId INT PRIMARY KEY IDENTITY(1,1),
        TypeName NVARCHAR(50) NOT NULL UNIQUE -- 'Deposit', 'Withdrawal', 'Transfer', 'Loan Payment'
    );
    
    -- Insert default transaction types
    INSERT INTO TransactionTypes (TypeName) VALUES
    ('Deposit'),
    ('Withdrawal'),
    ('Transfer'),
    ('Loan Payment');
END
GO

-- Create Transactions Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Transactions')
BEGIN
    CREATE TABLE Transactions (
        TransactionId INT PRIMARY KEY IDENTITY(1,1),
        AccountId INT NOT NULL,
        TransactionTypeId INT NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        BalanceAfterTransaction DECIMAL(18,2) NOT NULL,
        Description NVARCHAR(200),
        RelatedAccountId INT NULL, -- For transfers, the destination account
        TransactionDate DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId) ON DELETE CASCADE,
        FOREIGN KEY (TransactionTypeId) REFERENCES TransactionTypes(TransactionTypeId),
        FOREIGN KEY (RelatedAccountId) REFERENCES Accounts(AccountId)
    );
END
GO

-- Create Loans Table (Optional Enhancement)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Loans')
BEGIN
    CREATE TABLE Loans (
        LoanId INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        AccountId INT NULL, -- Linked account for loan payments
        LoanAmount DECIMAL(18,2) NOT NULL,
        InterestRate DECIMAL(5,2) NOT NULL,
        MonthlyPayment DECIMAL(18,2) NOT NULL,
        RemainingBalance DECIMAL(18,2) NOT NULL,
        LoanStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- 'Pending', 'Approved', 'Active', 'Paid', 'Rejected'
        ApplicationDate DATETIME NOT NULL DEFAULT GETDATE(),
        ApprovalDate DATETIME NULL,
        DueDate DATE NULL,
        FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
        FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId)
    );
END
GO

-- Create Indexes for better performance
CREATE NONCLUSTERED INDEX IX_Users_Username ON Users(Username);
CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
CREATE NONCLUSTERED INDEX IX_Accounts_UserId ON Accounts(UserId);
CREATE NONCLUSTERED INDEX IX_Accounts_AccountNumber ON Accounts(AccountNumber);
CREATE NONCLUSTERED INDEX IX_Transactions_AccountId ON Transactions(AccountId);
CREATE NONCLUSTERED INDEX IX_Transactions_TransactionDate ON Transactions(TransactionDate);
CREATE NONCLUSTERED INDEX IX_Loans_UserId ON Loans(UserId);
GO

-- Create Stored Procedures

-- Stored Procedure: Create User Account
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_CreateUser')
    DROP PROCEDURE sp_CreateUser;
GO

CREATE PROCEDURE sp_CreateUser
    @Username NVARCHAR(50),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(256),
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @PhoneNumber NVARCHAR(20) = NULL,
    @Address NVARCHAR(200) = NULL,
    @DateOfBirth DATE = NULL,
    @Role NVARCHAR(20) = 'Customer',
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, PhoneNumber, Address, DateOfBirth, Role)
    VALUES (@Username, @Email, @PasswordHash, @FirstName, @LastName, @PhoneNumber, @Address, @DateOfBirth, @Role);
    
    SET @UserId = SCOPE_IDENTITY();
END
GO

-- Stored Procedure: Get User by Username
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetUserByUsername')
    DROP PROCEDURE sp_GetUserByUsername;
GO

CREATE PROCEDURE sp_GetUserByUsername
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT UserId, Username, Email, PasswordHash, FirstName, LastName, PhoneNumber, Address, DateOfBirth, Role, IsActive, CreatedDate
    FROM Users
    WHERE Username = @Username AND IsActive = 1;
END
GO

-- Stored Procedure: Create Account
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_CreateAccount')
    DROP PROCEDURE sp_CreateAccount;
GO

CREATE PROCEDURE sp_CreateAccount
    @UserId INT,
    @AccountTypeId INT,
    @AccountNumber NVARCHAR(20),
    @InitialBalance DECIMAL(18,2) = 0.00,
    @AccountId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Accounts (UserId, AccountTypeId, AccountNumber, Balance)
    VALUES (@UserId, @AccountTypeId, @AccountNumber, @InitialBalance);
    
    SET @AccountId = SCOPE_IDENTITY();
    
    -- Create initial deposit transaction if balance > 0
    IF @InitialBalance > 0
    BEGIN
        DECLARE @DepositTypeId INT = (SELECT TransactionTypeId FROM TransactionTypes WHERE TypeName = 'Deposit');
        INSERT INTO Transactions (AccountId, TransactionTypeId, Amount, BalanceAfterTransaction, Description)
        VALUES (@AccountId, @DepositTypeId, @InitialBalance, @InitialBalance, 'Initial Deposit');
    END
END
GO

-- Stored Procedure: Process Transaction
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_ProcessTransaction')
    DROP PROCEDURE sp_ProcessTransaction;
GO

CREATE PROCEDURE sp_ProcessTransaction
    @AccountId INT,
    @TransactionTypeId INT,
    @Amount DECIMAL(18,2),
    @Description NVARCHAR(200) = NULL,
    @RelatedAccountId INT = NULL,
    @TransactionId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    DECLARE @CurrentBalance DECIMAL(18,2);
    DECLARE @NewBalance DECIMAL(18,2);
    DECLARE @TransactionTypeName NVARCHAR(50);
    
    SELECT @CurrentBalance = Balance FROM Accounts WHERE AccountId = @AccountId;
    SELECT @TransactionTypeName = TypeName FROM TransactionTypes WHERE TransactionTypeId = @TransactionTypeId;
    
    -- Calculate new balance based on transaction type
    IF @TransactionTypeName = 'Deposit'
        SET @NewBalance = @CurrentBalance + @Amount;
    ELSE IF @TransactionTypeName = 'Withdrawal'
    BEGIN
        IF @CurrentBalance < @Amount
        BEGIN
            ROLLBACK TRANSACTION;
            RAISERROR('Insufficient funds', 16, 1);
            RETURN;
        END
        SET @NewBalance = @CurrentBalance - @Amount;
    END
    ELSE IF @TransactionTypeName = 'Transfer'
    BEGIN
        IF @CurrentBalance < @Amount
        BEGIN
            ROLLBACK TRANSACTION;
            RAISERROR('Insufficient funds for transfer', 16, 1);
            RETURN;
        END
        SET @NewBalance = @CurrentBalance - @Amount;
        
        -- Update destination account if transfer
        IF @RelatedAccountId IS NOT NULL
        BEGIN
            UPDATE Accounts SET Balance = Balance + @Amount WHERE AccountId = @RelatedAccountId;
            
            -- Create transaction record for destination account
            INSERT INTO Transactions (AccountId, TransactionTypeId, Amount, BalanceAfterTransaction, Description, RelatedAccountId)
            VALUES (@RelatedAccountId, @TransactionTypeId, @Amount, (SELECT Balance FROM Accounts WHERE AccountId = @RelatedAccountId), 
                    'Transfer from Account ' + (SELECT AccountNumber FROM Accounts WHERE AccountId = @AccountId), @AccountId);
        END
    END
    
    -- Update account balance
    UPDATE Accounts SET Balance = @NewBalance, LastModifiedDate = GETDATE() WHERE AccountId = @AccountId;
    
    -- Create transaction record
    INSERT INTO Transactions (AccountId, TransactionTypeId, Amount, BalanceAfterTransaction, Description, RelatedAccountId)
    VALUES (@AccountId, @TransactionTypeId, @Amount, @NewBalance, @Description, @RelatedAccountId);
    
    SET @TransactionId = SCOPE_IDENTITY();
    
    COMMIT TRANSACTION;
END
GO

-- Stored Procedure: Get Transaction History
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

GO

