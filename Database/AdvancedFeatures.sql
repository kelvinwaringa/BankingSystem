-- Advanced Features Database Schema
-- Recurring Payments, Bill Payments, Budgets, Savings Goals, Audit Logs

-- Recurring Payments Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RecurringPayments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RecurringPayments] (
        [RecurringPaymentId] INT IDENTITY(1,1) PRIMARY KEY,
        [AccountId] INT NOT NULL,
        [RecipientName] NVARCHAR(100) NOT NULL,
        [RecipientAccountNumber] NVARCHAR(50) NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [Frequency] NVARCHAR(20) NOT NULL, -- Daily, Weekly, Monthly, Yearly
        [NextPaymentDate] DATETIME NOT NULL,
        [LastPaymentDate] DATETIME NULL,
        [EndDate] DATETIME NULL,
        [Description] NVARCHAR(255) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([AccountId]) REFERENCES [Accounts]([AccountId])
    );
END

-- Bill Payments Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BillPayments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BillPayments] (
        [BillPaymentId] INT IDENTITY(1,1) PRIMARY KEY,
        [AccountId] INT NOT NULL,
        [PayeeName] NVARCHAR(100) NOT NULL,
        [PayeeAccountNumber] NVARCHAR(50) NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [DueDate] DATETIME NOT NULL,
        [PaymentDate] DATETIME NULL,
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, Paid, Overdue, Cancelled
        [Description] NVARCHAR(255) NULL,
        [IsRecurring] BIT NOT NULL DEFAULT 0,
        [RecurringPaymentId] INT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([AccountId]) REFERENCES [Accounts]([AccountId]),
        FOREIGN KEY ([RecurringPaymentId]) REFERENCES [RecurringPayments]([RecurringPaymentId])
    );
END

-- Budgets Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Budgets]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Budgets] (
        [BudgetId] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [Category] NVARCHAR(50) NOT NULL, -- Food, Transportation, Entertainment, Utilities, etc.
        [BudgetAmount] DECIMAL(18,2) NOT NULL,
        [Period] NVARCHAR(20) NOT NULL, -- Monthly, Weekly, Yearly
        [StartDate] DATETIME NOT NULL,
        [EndDate] DATETIME NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId])
    );
END

-- Savings Goals Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SavingsGoals]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SavingsGoals] (
        [SavingsGoalId] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [AccountId] INT NULL,
        [GoalName] NVARCHAR(100) NOT NULL,
        [TargetAmount] DECIMAL(18,2) NOT NULL,
        [CurrentAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [TargetDate] DATETIME NULL,
        [Description] NVARCHAR(255) NULL,
        [IsCompleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]),
        FOREIGN KEY ([AccountId]) REFERENCES [Accounts]([AccountId])
    );
END

-- Audit Logs Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AuditLogs] (
        [AuditLogId] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NULL,
        [Action] NVARCHAR(50) NOT NULL, -- Login, Logout, Transaction, AccountCreated, etc.
        [EntityType] NVARCHAR(50) NULL, -- User, Account, Transaction, Loan, etc.
        [EntityId] INT NULL,
        [Details] NVARCHAR(MAX) NULL,
        [IpAddress] NVARCHAR(50) NULL,
        [Timestamp] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId])
    );
    
    CREATE INDEX IX_AuditLogs_UserId ON [AuditLogs]([UserId]);
    CREATE INDEX IX_AuditLogs_Timestamp ON [AuditLogs]([Timestamp]);
    CREATE INDEX IX_AuditLogs_Action ON [AuditLogs]([Action]);
END

-- Transaction Categories Table (for better analytics)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransactionCategories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TransactionCategories] (
        [CategoryId] INT IDENTITY(1,1) PRIMARY KEY,
        [CategoryName] NVARCHAR(50) NOT NULL UNIQUE,
        [Description] NVARCHAR(255) NULL
    );
    
    -- Insert default categories
    INSERT INTO [TransactionCategories] ([CategoryName], [Description]) VALUES
    ('Food & Dining', 'Restaurants, groceries, food delivery'),
    ('Transportation', 'Gas, public transport, parking, car maintenance'),
    ('Shopping', 'Retail purchases, online shopping'),
    ('Bills & Utilities', 'Electricity, water, internet, phone bills'),
    ('Entertainment', 'Movies, concerts, subscriptions, hobbies'),
    ('Healthcare', 'Medical expenses, pharmacy, insurance'),
    ('Education', 'Tuition, books, courses'),
    ('Travel', 'Hotels, flights, vacation expenses'),
    ('Transfer', 'Money transfers between accounts'),
    ('Income', 'Salary, deposits, refunds'),
    ('Other', 'Miscellaneous expenses');
END

-- Add CategoryId to Transactions table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Transactions]') AND name = 'CategoryId')
BEGIN
    ALTER TABLE [Transactions] ADD [CategoryId] INT NULL;
    ALTER TABLE [Transactions] ADD FOREIGN KEY ([CategoryId]) REFERENCES [TransactionCategories]([CategoryId]);
    CREATE INDEX IX_Transactions_CategoryId ON [Transactions]([CategoryId]);
END

-- Add Notes field to Transactions if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Transactions]') AND name = 'Notes')
BEGIN
    ALTER TABLE [Transactions] ADD [Notes] NVARCHAR(500) NULL;
END

PRINT 'Advanced features database schema created successfully!';

