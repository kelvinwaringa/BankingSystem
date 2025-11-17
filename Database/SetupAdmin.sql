-- Admin User Setup Script
-- This script creates an admin user with default credentials
-- Username: admin
-- Password: admin123
--
-- To create a custom admin user, modify the values below

USE BankingSystemDB;
GO

-- Check if admin user already exists
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    -- Create admin user
    -- Password hash for 'admin123' is: jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=
    INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedDate, LastModifiedDate)
    VALUES (
        'admin',
        'admin@bank.com',
        'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=',  -- Hash for password: admin123
        'Admin',
        'User',
        'Admin',
        1,
        GETDATE(),
        GETDATE()
    );
    
    PRINT 'Admin user created successfully!';
    PRINT 'Username: admin';
    PRINT 'Password: admin123';
END
ELSE
BEGIN
    -- Update existing admin user to ensure it has Admin role
    UPDATE Users 
    SET Role = 'Admin', LastModifiedDate = GETDATE()
    WHERE Username = 'admin';
    
    PRINT 'Admin user already exists. Role updated to Admin.';
END

GO

-- Display admin users
SELECT UserId, Username, Email, FirstName, LastName, Role, IsActive
FROM Users
WHERE Role = 'Admin';

GO

-- To update an existing user to Admin role, use:
-- UPDATE Users SET Role = 'Admin', LastModifiedDate = GETDATE() WHERE Username = 'your_username';

GO

