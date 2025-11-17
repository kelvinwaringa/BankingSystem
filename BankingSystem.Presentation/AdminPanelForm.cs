using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.DataAccess;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class AdminPanelForm : Form
    {
        private User _currentUser;
        private UserService _userService;
        private AccountService _accountService;
        private TransactionService _transactionService;
        private DataGridView dgvUsers;
        private DataGridView dgvAccounts;
        private DataGridView dgvTransactions;
        private TabControl tabControl;
        private Label lblWelcome;
        private Button btnLogout;
        private Button btnRefresh;

        private ToolTip _tooltip;

        public AdminPanelForm(User user)
        {
            _currentUser = user;
            _userService = new UserService();
            _accountService = new AccountService();
            _transactionService = new TransactionService();
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadAllData();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleDataGridView(dgvUsers);
            UITheme.StyleDataGridView(dgvAccounts);
            UITheme.StyleDataGridView(dgvTransactions);
            UITheme.StyleButton(btnRefresh, ButtonStyle.Secondary);
            UITheme.StyleButton(btnLogout, ButtonStyle.Danger);
            UITheme.StyleLabel(lblWelcome, LabelStyle.Title);
        }

        private void InitializeComponent()
        {
            this.lblWelcome = new Label();
            this.tabControl = new TabControl();
            this.dgvUsers = new DataGridView();
            this.dgvAccounts = new DataGridView();
            this.dgvTransactions = new DataGridView();
            this.btnRefresh = new Button();
            this.btnLogout = new Button();

            // lblWelcome
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.Location = new System.Drawing.Point(20, 20);
            this.lblWelcome.Size = new System.Drawing.Size(400, 24);
            this.lblWelcome.Text = $"Admin Panel - Welcome, {_currentUser.FullName}";

            // tabControl
            this.tabControl.Location = new System.Drawing.Point(20, 60);
            this.tabControl.Size = new System.Drawing.Size(900, 500);
            
            // Users Tab
            TabPage tabUsers = new TabPage("All Users");
            this.dgvUsers.Dock = DockStyle.Fill;
            this.dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsers.ReadOnly = true;
            this.dgvUsers.AllowUserToAddRows = false;
            tabUsers.Controls.Add(this.dgvUsers);
            this.tabControl.TabPages.Add(tabUsers);

            // Accounts Tab
            TabPage tabAccounts = new TabPage("All Accounts");
            this.dgvAccounts.Dock = DockStyle.Fill;
            this.dgvAccounts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAccounts.ReadOnly = true;
            this.dgvAccounts.AllowUserToAddRows = false;
            tabAccounts.Controls.Add(this.dgvAccounts);
            this.tabControl.TabPages.Add(tabAccounts);

            // Transactions Tab
            TabPage tabTransactions = new TabPage("All Transactions");
            this.dgvTransactions.Dock = DockStyle.Fill;
            this.dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTransactions.ReadOnly = true;
            this.dgvTransactions.AllowUserToAddRows = false;
            tabTransactions.Controls.Add(this.dgvTransactions);
            this.tabControl.TabPages.Add(tabTransactions);

            // Loans Tab
            TabPage tabLoans = new TabPage("All Loans");
            DataGridView dgvLoans = new DataGridView();
            dgvLoans.Dock = DockStyle.Fill;
            dgvLoans.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLoans.ReadOnly = true;
            dgvLoans.AllowUserToAddRows = false;
            dgvLoans.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            UITheme.StyleDataGridView(dgvLoans);
            dgvLoans.CellDoubleClick += new DataGridViewCellEventHandler((s, e) => {
                if (e.RowIndex >= 0)
                {
                    var loanId = (int)dgvLoans.Rows[e.RowIndex].Cells["LoanId"].Value;
                    var loanService = new LoanService();
                    var loan = loanService.GetLoanById(loanId);
                    if (loan != null && loan.LoanStatus == "Pending")
                    {
                        var result = MessageBox.Show($"Loan ID: {loan.LoanId}\nAmount: ${loan.LoanAmount:F2}\n\nApprove or Reject?", 
                            "Loan Action", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            loanService.ApproveLoan(loanId);
                            MessageBox.Show("Loan approved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            loanService.RejectLoan(loanId);
                            MessageBox.Show("Loan rejected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        LoadAllData();
                    }
                }
            });
            tabLoans.Controls.Add(dgvLoans);
            this.tabControl.TabPages.Add(tabLoans);

            // btnRefresh
            this.btnRefresh.Location = new System.Drawing.Point(20, 570);
            this.btnRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);

            // Add User Management button
            var btnUserManagement = new Button();
            btnUserManagement.Location = new System.Drawing.Point(130, 570);
            btnUserManagement.Size = new System.Drawing.Size(130, 30);
            btnUserManagement.Text = "User Management";
            btnUserManagement.Click += new EventHandler((s, e) => {
                var userMgmtForm = new AdminUserManagementForm();
                userMgmtForm.ShowDialog();
                LoadUsers();
            });

            // btnLogout
            this.btnLogout.Location = new System.Drawing.Point(820, 570);
            this.btnLogout.Size = new System.Drawing.Size(100, 30);
            this.btnLogout.Text = "Logout";
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 620);
            this.Controls.AddRange(new Control[] {
                this.lblWelcome, this.tabControl, this.btnRefresh, btnUserManagement, this.btnLogout
            });
            this.Name = "AdminPanelForm";
            this.Text = "Banking System - Admin Panel";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = UITheme.BackgroundColor;
        }

        private void LoadAllData()
        {
            LoadUsers();
            LoadAccounts();
            LoadTransactions();
            LoadLoans();
        }

        private void LoadLoans()
        {
            try
            {
                var loanService = new LoanService();
                var loans = loanService.GetAllLoans();
                
                var dgvLoans = (DataGridView)this.tabControl.TabPages[3].Controls[0];
                dgvLoans.Columns.Clear();
                dgvLoans.Columns.Add("LoanId", "Loan ID");
                dgvLoans.Columns.Add("UserId", "User ID");
                dgvLoans.Columns.Add("LoanAmount", "Loan Amount");
                dgvLoans.Columns.Add("InterestRate", "Interest Rate");
                dgvLoans.Columns.Add("MonthlyPayment", "Monthly Payment");
                dgvLoans.Columns.Add("RemainingBalance", "Remaining Balance");
                dgvLoans.Columns.Add("LoanStatus", "Status");
                dgvLoans.Columns.Add("ApplicationDate", "Application Date");

                dgvLoans.Columns["LoanAmount"].DefaultCellStyle.Format = "C2";
                dgvLoans.Columns["MonthlyPayment"].DefaultCellStyle.Format = "C2";
                dgvLoans.Columns["RemainingBalance"].DefaultCellStyle.Format = "C2";
                dgvLoans.Columns["InterestRate"].DefaultCellStyle.Format = "F2%";
                dgvLoans.Columns["ApplicationDate"].DefaultCellStyle.Format = "MM/dd/yyyy";

                dgvLoans.Rows.Clear();
                foreach (var loan in loans)
                {
                    dgvLoans.Rows.Add(
                        loan.LoanId,
                        loan.UserId,
                        loan.LoanAmount,
                        loan.InterestRate,
                        loan.MonthlyPayment,
                        loan.RemainingBalance,
                        loan.LoanStatus,
                        loan.ApplicationDate
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading loans: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUsers()
        {
            try
            {
                var userRepo = new UserRepository();
                dgvUsers.Columns.Clear();
                dgvUsers.Columns.Add("UserId", "User ID");
                dgvUsers.Columns.Add("Username", "Username");
                dgvUsers.Columns.Add("Email", "Email");
                dgvUsers.Columns.Add("FullName", "Full Name");
                dgvUsers.Columns.Add("Role", "Role");
                dgvUsers.Columns.Add("IsActive", "Active");
                dgvUsers.Columns.Add("CreatedDate", "Created Date");

                dgvUsers.Rows.Clear();
                
                // Get all users from database
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT UserId, Username, Email, FirstName, LastName, Role, IsActive, CreatedDate FROM Users", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dgvUsers.Rows.Add(
                                    (int)reader["UserId"],
                                    (string)reader["Username"],
                                    (string)reader["Email"],
                                    (string)reader["FirstName"] + " " + (string)reader["LastName"],
                                    (string)reader["Role"],
                                    (bool)reader["IsActive"],
                                    ((DateTime)reader["CreatedDate"]).ToString("MM/dd/yyyy")
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAccounts()
        {
            try
            {
                var accountRepo = new AccountRepository();
                dgvAccounts.Columns.Clear();
                dgvAccounts.Columns.Add("AccountId", "Account ID");
                dgvAccounts.Columns.Add("AccountNumber", "Account Number");
                dgvAccounts.Columns.Add("UserId", "User ID");
                dgvAccounts.Columns.Add("AccountType", "Type");
                dgvAccounts.Columns.Add("Balance", "Balance");
                dgvAccounts.Columns.Add("IsActive", "Active");
                dgvAccounts.Columns.Add("CreatedDate", "Created Date");

                dgvAccounts.Columns["Balance"].DefaultCellStyle.Format = "C2";

                dgvAccounts.Rows.Clear();
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        "SELECT a.AccountId, a.AccountNumber, a.UserId, at.TypeName, a.Balance, a.IsActive, a.CreatedDate " +
                        "FROM Accounts a INNER JOIN AccountTypes at ON a.AccountTypeId = at.AccountTypeId", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dgvAccounts.Rows.Add(
                                    (int)reader["AccountId"],
                                    (string)reader["AccountNumber"],
                                    (int)reader["UserId"],
                                    (string)reader["TypeName"],
                                    (decimal)reader["Balance"],
                                    (bool)reader["IsActive"],
                                    ((DateTime)reader["CreatedDate"]).ToString("MM/dd/yyyy")
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accounts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTransactions()
        {
            try
            {
                dgvTransactions.Columns.Clear();
                dgvTransactions.Columns.Add("TransactionId", "Transaction ID");
                dgvTransactions.Columns.Add("AccountId", "Account ID");
                dgvTransactions.Columns.Add("TransactionType", "Type");
                dgvTransactions.Columns.Add("Amount", "Amount");
                dgvTransactions.Columns.Add("BalanceAfter", "Balance After");
                dgvTransactions.Columns.Add("Description", "Description");
                dgvTransactions.Columns.Add("TransactionDate", "Date");

                dgvTransactions.Columns["Amount"].DefaultCellStyle.Format = "C2";
                dgvTransactions.Columns["BalanceAfter"].DefaultCellStyle.Format = "C2";

                dgvTransactions.Rows.Clear();
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        "SELECT t.TransactionId, t.AccountId, tt.TypeName, t.Amount, t.BalanceAfterTransaction, t.Description, t.TransactionDate " +
                        "FROM Transactions t INNER JOIN TransactionTypes tt ON t.TransactionTypeId = tt.TransactionTypeId " +
                        "ORDER BY t.TransactionDate DESC", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dgvTransactions.Rows.Add(
                                    (int)reader["TransactionId"],
                                    (int)reader["AccountId"],
                                    (string)reader["TypeName"],
                                    (decimal)reader["Amount"],
                                    (decimal)reader["BalanceAfterTransaction"],
                                    reader["Description"] == DBNull.Value ? "" : (string)reader["Description"],
                                    ((DateTime)reader["TransactionDate"]).ToString("MM/dd/yyyy HH:mm")
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transactions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAllData();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            var loginForm = new EnhancedLoginForm();
            loginForm.Show();
        }
    }
}

