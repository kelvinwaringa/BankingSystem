# Banking System - Enterprise Application

## CNIT 450 - Spring 2025

A comprehensive online banking management system built with C#, .NET Framework 4.8, Windows Forms, and Microsoft SQL Server.

---

## 📋 Table of Contents

- [Project Structure](#project-structure)
- [Features](#features)
- [Database Setup](#database-setup)
- [Configuration](#configuration)
- [Building and Running](#building-and-running)
- [Usage Guide](#usage-guide)
- [Technical Architecture](#technical-architecture)
- [Project Deliverables](#project-deliverables)

---

## 🏗️ Project Structure

The solution follows a layered architecture pattern:

```
BankingSystem/
├── BankingSystem.Models/          # Domain models and entities
├── BankingSystem.DataAccess/       # Data access layer (repositories)
├── BankingSystem.BusinessLogic/   # Business logic layer (services)
└── BankingSystem.Presentation/     # Windows Forms UI layer
```

---

## ✨ Features

### Core Functionalities

#### 1. **User Authentication & Security**
- Secure login and sign-up for customers
- Password encryption using SHA-256
- Password strength validation (8+ chars, uppercase, lowercase, number, special char)
- Account lockout protection (3 failed attempts)
- Role-based access control (Customer/Admin)
- Show/hide password toggle
- Enhanced security feedback

#### 2. **Account Management**
- Customer profile management
- Multi-account support (Savings, Checking, Credit)
- View and manage account details
- Account balance viewing with real-time updates
- Account creation with initial balance
- Account closure (when balance is zero)
- Interest rate display for savings accounts

#### 3. **Fund Transactions**
- **Deposit**: Add funds to accounts
- **Withdraw**: Remove funds from accounts
- **Transfer**: Move money between accounts
- Real-time transaction processing with validation
- Transaction limits:
  - Daily withdrawal limit: $5,000
  - Daily transfer limit: $10,000
  - Maximum transaction: $50,000
- Overdraft protection
- Minimum balance protection

#### 4. **Transaction History & Statements**
- Detailed log of all transactions with timestamps
- Search and filter transactions by date and type
- View transaction history for each account
- **Account Statements**:
  - Printable formatted statements
  - Export to text file (.txt)
  - Date range filtering
  - Transaction summary (opening/closing balances, totals)

#### 5. **Loan and Credit Management**
- Loan application with eligibility checking
- AI-based loan approval system
- Loan payment processing
- Installment tracking
- Admin loan approval/rejection interface
- Automatic loan disbursement to accounts

#### 6. **Recurring Payments**
- Create, edit, and manage recurring payments
- Support for Daily, Weekly, Monthly, and Yearly frequencies
- Track next payment dates and end dates
- Link to accounts for automatic processing
- Deactivate/activate recurring payments

#### 7. **Bill Payments**
- Create and manage bill payments
- Track due dates and payment status (Pending, Paid, Overdue, Cancelled)
- Pay bills directly from accounts
- View overdue bills
- Link bills to recurring payments

#### 8. **Budget Management**
- Create budgets by category (Food, Transportation, Entertainment, etc.)
- Set budget periods (Monthly, Weekly, Yearly)
- View budget status with spending vs. budget
- Track progress and get over-budget alerts
- Budget breakdown by category

#### 9. **Savings Goals**
- Create savings goals with target amounts and dates
- Track progress with percentage completion
- Add funds to goals manually
- Link goals to specific accounts (optional)
- Calculate required monthly savings
- Goal completion tracking

#### 10. **Spending Analytics**
- Comprehensive spending analysis
- Total income vs expenses tracking
- Net savings calculation
- Savings rate percentage
- Category-based spending breakdown
- Monthly spending trends
- Customizable date ranges

#### 11. **Interest Calculation**
- Automatic interest calculation for savings accounts
- Projected interest display on account details
- Monthly interest service (ready for automation)
- Interest rate display in account listings

#### 12. **Admin Features**
- User management panel (activate/deactivate accounts)
- Loan approval/rejection interface
- Comprehensive views (all users, accounts, transactions, loans)
- Real-time data refresh
- Audit log viewing

#### 13. **Audit Logging**
- Complete activity tracking:
  - User login/logout events
  - Failed login attempts
  - All financial transactions
  - Account creation/modification
  - Loan applications and approvals
- IP address tracking
- Timestamp recording
- Entity-level logging
- Compliance-ready audit trail

#### 14. **Modern UI/UX**
- Professional color scheme and modern theme
- Card-based layouts
- Consistent styling across all forms
- Tooltips on interactive elements
- Loading indicators
- Responsive design
- Enhanced visual hierarchy

---

## 🗄️ Database Setup

### Step 1: Create Database Schema

1. Open SQL Server Management Studio (SSMS)
2. Execute `Database/CreateDatabase.sql` to create:
   - Database: `BankingSystemDB`
   - Core tables (Users, Accounts, Transactions, Loans, etc.)
   - Stored procedures
   - Indexes for performance

### Step 2: Add Advanced Features

Execute `Database/AdvancedFeatures.sql` to add:
- Recurring Payments table
- Bill Payments table
- Budgets table
- Savings Goals table
- Transaction Categories table
- Audit Logs table

### Step 3: Create Admin User

Execute `Database/SetupAdmin.sql` to create an admin user:
- **Username**: `admin`
- **Password**: `admin123`

Or use the script to create a custom admin user.

---

## ⚙️ Configuration

### Connection String

Update the connection string in `BankingSystem.Presentation/App.config`:

```xml
<connectionStrings>
    <add name="BankingSystemDB" 
         connectionString="Server=localhost\SQLEXPRESS;Database=BankingSystemDB;Integrated Security=True;TrustServerCertificate=True;" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

**For different SQL Server instances:**
- Default instance: `Server=localhost;Database=BankingSystemDB;...`
- Named instance: `Server=localhost\INSTANCENAME;Database=BankingSystemDB;...`
- SQL Authentication: `Server=...;Database=...;User ID=...;Password=...;`

---

## 🔨 Building and Running

### Option 1: Using Visual Studio

1. Open `BankingSystem.sln` in Visual Studio
2. Restore NuGet packages (if needed)
3. Build the solution (Build > Build Solution or Ctrl+Shift+B)
4. Set `BankingSystem.Presentation` as the startup project
5. Run the application (F5)

### Option 2: Using Command Line

**Build:**
```powershell
dotnet build BankingSystem.sln
```

**Run:**
```powershell
dotnet run --project BankingSystem.Presentation
```

Or use the provided PowerShell scripts:
```powershell
.\build.ps1    # Build the solution
.\run.ps1      # Build and run the application
```

---

## 📖 Usage Guide

### First Time Setup

1. **Create Database**: Run `Database/CreateDatabase.sql` and `Database/AdvancedFeatures.sql`
2. **Create Admin**: Run `Database/SetupAdmin.sql`
3. **Launch Application**: Build and run the solution

### Customer Workflow

1. **Sign Up**: Click "Sign Up" to create a new account
2. **Login**: Enter username and password
3. **Dashboard**: View accounts and access banking operations
4. **Create Account**: Select account type and initial balance
5. **Transactions**: Deposit, withdraw, or transfer funds
6. **View History**: Check transaction history and statements
7. **Manage Features**: Use Recurring Payments, Bill Payments, Budgets, and Savings Goals

### Admin Workflow

1. **Login**: Use admin credentials (default: `admin`/`admin123`)
2. **Admin Panel**: Access comprehensive management interface
3. **User Management**: Activate/deactivate user accounts
4. **Loan Approval**: Review and approve/reject loan applications
5. **View All Data**: Monitor users, accounts, transactions, and loans

---

## 🏛️ Technical Architecture

### Layered Architecture

- **Presentation Layer**: Windows Forms UI with modern theme system
- **Business Logic Layer**: Services for validation, processing, and business rules
- **Data Access Layer**: Repositories for database operations
- **Models Layer**: Domain entities and data transfer objects

### Key Technologies

- ✅ Microsoft Visual Studio (IDE)
- ✅ C# programming language
- ✅ .NET Framework 4.8
- ✅ Microsoft SQL Server (Database)
- ✅ Windows Forms (UI Framework)
- ✅ ADO.NET (Data Access)
- ✅ SHA-256 (Password Hashing)

### Design Patterns

- Repository Pattern (Data Access)
- Service Layer Pattern (Business Logic)
- Dependency Injection (Service Dependencies)
- Layered Architecture (Separation of Concerns)

---

## 📦 Project Deliverables

### Deliverable 1: Analysis Models

- **Entity-Relationship Diagram (ERD)**: See `Documentation/ERD_Description.txt`
- **Use Case Diagram**: See `Documentation/UseCaseDiagram.txt`
- **Class Diagram**: See `Documentation/ClassDiagram.txt`

### Deliverable 2: Implementation

- ✅ Complete Visual Studio solution
- ✅ Database implementation with stored procedures
- ✅ All core functionalities implemented
- ✅ User interface with Windows Forms
- ✅ Comprehensive error handling and validation
- ✅ Security features and audit logging

---

## 📝 Notes

- Default connection uses Windows Authentication (Integrated Security=True)
- For SQL Server Authentication, modify the connection string
- All transactions are logged with timestamps
- Account numbers are auto-generated: `ACCYYYYMMDDHHMMSS####`
- The application includes comprehensive error handling
- Audit logs track all user actions for compliance

---

## 🛠️ Development

### Code Structure

- **Models**: Domain entities (`User`, `Account`, `Transaction`, `Loan`, etc.)
- **Repositories**: Data access methods (`UserRepository`, `AccountRepository`, etc.)
- **Services**: Business logic (`UserService`, `TransactionService`, etc.)
- **Forms**: UI components (`LoginForm`, `DashboardForm`, etc.)

### Adding New Features

1. Add model class in `BankingSystem.Models`
2. Create repository in `BankingSystem.DataAccess`
3. Create service in `BankingSystem.BusinessLogic`
4. Create UI form in `BankingSystem.Presentation`
5. Integrate into dashboard or appropriate form

---

## 📄 License

This project is developed for educational purposes as part of CNIT 450 course requirements.

---

## 👥 Development Team

[Add your team members and responsibilities here]

---

## 🔗 Quick Links

- **Database Scripts**: `Database/` folder
- **Documentation**: `Documentation/` folder
- **Build Scripts**: `build.ps1`, `run.ps1`

---

**Last Updated**: 2025
