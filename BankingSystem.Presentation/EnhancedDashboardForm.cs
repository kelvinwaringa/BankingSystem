using System;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class EnhancedDashboardForm : Form
    {
        private User _currentUser;
        private UserService _userService;
        private AccountService _accountService;
        private TransactionService _transactionService;
        private LoanService _loanService;
        private ListBox lstAccounts;
        private Label lblWelcome;
        private Label lblAccountBalance;
        private Label lblTotalBalance;
        private Label lblAccountSummary;
        private Button btnViewTransactions;
        private Button btnDeposit;
        private Button btnWithdraw;
        private Button btnTransfer;
        private Button btnCreateAccount;
        private Button btnUpdateProfile;
        private Button btnLoanManagement;
        private Button btnApplyLoan;
        private Button btnStatement;
        private Button btnAccountDetails;
        private Button btnAnalytics;
        private Button btnLogout;
        private Button btnRecurringPayments;
        private Button btnBillPayments;
        private Button btnBudgets;
        private Button btnSavingsGoals;

        private ToolTip _tooltip;

        public EnhancedDashboardForm(User user)
        {
            _currentUser = user;
            _userService = new UserService();
            _accountService = new AccountService();
            _transactionService = new TransactionService();
            _loanService = new LoanService();
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadAccounts();
            UpdateAccountSummary();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleListBox(lstAccounts);
            UITheme.StyleButton(btnViewTransactions, ButtonStyle.Primary);
            UITheme.StyleButton(btnStatement, ButtonStyle.Primary);
            UITheme.StyleButton(btnAnalytics, ButtonStyle.Primary);
            UITheme.StyleButton(btnAccountDetails, ButtonStyle.Secondary);
            UITheme.StyleButton(btnDeposit, ButtonStyle.Success);
            UITheme.StyleButton(btnWithdraw, ButtonStyle.Warning);
            UITheme.StyleButton(btnTransfer, ButtonStyle.Primary);
            UITheme.StyleButton(btnCreateAccount, ButtonStyle.Secondary);
            UITheme.StyleButton(btnUpdateProfile, ButtonStyle.Secondary);
            UITheme.StyleButton(btnLoanManagement, ButtonStyle.Primary);
            UITheme.StyleButton(btnApplyLoan, ButtonStyle.Primary);
            UITheme.StyleButton(btnRecurringPayments, ButtonStyle.Primary);
            UITheme.StyleButton(btnBillPayments, ButtonStyle.Primary);
            UITheme.StyleButton(btnBudgets, ButtonStyle.Primary);
            UITheme.StyleButton(btnSavingsGoals, ButtonStyle.Primary);
            UITheme.StyleButton(btnLogout, ButtonStyle.Danger);
            UITheme.StyleLabel(lblWelcome, LabelStyle.Title);
            UITheme.StyleLabel(lblTotalBalance, LabelStyle.Success);
            
            // Add tooltips
            UITheme.AddTooltip(_tooltip, btnViewTransactions, "View all your transaction history");
            UITheme.AddTooltip(_tooltip, btnStatement, "Generate and export account statements");
            UITheme.AddTooltip(_tooltip, btnAnalytics, "View spending analytics and reports");
            UITheme.AddTooltip(_tooltip, btnDeposit, "Deposit money into your account");
            UITheme.AddTooltip(_tooltip, btnWithdraw, "Withdraw money from your account");
            UITheme.AddTooltip(_tooltip, btnTransfer, "Transfer money between accounts");
        }

        private void InitializeComponent()
        {
            this.lblWelcome = new Label();
            this.lstAccounts = new ListBox();
            this.lblAccountBalance = new Label();
            this.lblTotalBalance = new Label();
            this.lblAccountSummary = new Label();
            this.btnViewTransactions = new Button();
            this.btnDeposit = new Button();
            this.btnWithdraw = new Button();
            this.btnTransfer = new Button();
            this.btnCreateAccount = new Button();
            this.btnUpdateProfile = new Button();
            this.btnLoanManagement = new Button();
            this.btnApplyLoan = new Button();
            this.btnStatement = new Button();
            this.btnAccountDetails = new Button();
            this.btnAnalytics = new Button();
            this.btnLogout = new Button();
            this.btnRecurringPayments = new Button();
            this.btnBillPayments = new Button();
            this.btnBudgets = new Button();
            this.btnSavingsGoals = new Button();

            // lblWelcome
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.Location = new System.Drawing.Point(20, 20);
            this.lblWelcome.Size = new System.Drawing.Size(400, 24);
            this.lblWelcome.Text = $"Welcome, {_currentUser.FullName}";

            // lblAccountSummary
            this.lblAccountSummary.AutoSize = true;
            this.lblAccountSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblAccountSummary.Location = new System.Drawing.Point(20, 50);
            this.lblAccountSummary.Size = new System.Drawing.Size(300, 17);
            this.lblAccountSummary.Text = "Account Summary";

            // lstAccounts
            this.lstAccounts.Location = new System.Drawing.Point(20, 75);
            this.lstAccounts.Size = new System.Drawing.Size(350, 150);
            this.lstAccounts.SelectedIndexChanged += new EventHandler(this.lstAccounts_SelectedIndexChanged);

            // lblAccountBalance
            this.lblAccountBalance.AutoSize = true;
            this.lblAccountBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblAccountBalance.Location = new System.Drawing.Point(20, 235);
            this.lblAccountBalance.Size = new System.Drawing.Size(200, 17);
            this.lblAccountBalance.Text = "Selected Account Balance: $0.00";

            // lblTotalBalance
            this.lblTotalBalance.AutoSize = true;
            this.lblTotalBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblTotalBalance.ForeColor = System.Drawing.Color.Green;
            this.lblTotalBalance.Location = new System.Drawing.Point(20, 260);
            this.lblTotalBalance.Size = new System.Drawing.Size(250, 18);
            this.lblTotalBalance.Text = "Total Balance: $0.00";

            // Buttons - Row 1
            this.btnViewTransactions.Location = new System.Drawing.Point(20, 290);
            this.btnViewTransactions.Size = new System.Drawing.Size(160, 35);
            this.btnViewTransactions.Text = "View Transactions";
            this.btnViewTransactions.Click += new EventHandler(this.btnViewTransactions_Click);

            this.btnStatement.Location = new System.Drawing.Point(190, 290);
            this.btnStatement.Size = new System.Drawing.Size(90, 35);
            this.btnStatement.Text = "Statement";
            this.btnStatement.Click += new EventHandler(this.btnStatement_Click);

            this.btnAnalytics.Location = new System.Drawing.Point(290, 290);
            this.btnAnalytics.Size = new System.Drawing.Size(90, 35);
            this.btnAnalytics.Text = "Analytics";
            this.btnAnalytics.Click += new EventHandler(this.btnAnalytics_Click);

            // Buttons - Row 2
            this.btnAccountDetails.Location = new System.Drawing.Point(20, 335);
            this.btnAccountDetails.Size = new System.Drawing.Size(110, 35);
            this.btnAccountDetails.Text = "Account Details";
            this.btnAccountDetails.Click += new EventHandler(this.btnAccountDetails_Click);

            this.btnDeposit.Location = new System.Drawing.Point(140, 335);
            this.btnDeposit.Size = new System.Drawing.Size(110, 35);
            this.btnDeposit.Text = "Deposit";
            this.btnDeposit.Click += new EventHandler(this.btnDeposit_Click);

            this.btnWithdraw.Location = new System.Drawing.Point(260, 335);
            this.btnWithdraw.Size = new System.Drawing.Size(110, 35);
            this.btnWithdraw.Text = "Withdraw";
            this.btnWithdraw.Click += new EventHandler(this.btnWithdraw_Click);

            // Buttons - Row 3
            this.btnTransfer.Location = new System.Drawing.Point(20, 380);
            this.btnTransfer.Size = new System.Drawing.Size(110, 35);
            this.btnTransfer.Text = "Transfer";
            this.btnTransfer.Click += new EventHandler(this.btnTransfer_Click);

            this.btnCreateAccount.Location = new System.Drawing.Point(140, 380);
            this.btnCreateAccount.Size = new System.Drawing.Size(110, 35);
            this.btnCreateAccount.Text = "Create Account";
            this.btnCreateAccount.Click += new EventHandler(this.btnCreateAccount_Click);

            this.btnUpdateProfile.Location = new System.Drawing.Point(260, 380);
            this.btnUpdateProfile.Size = new System.Drawing.Size(110, 35);
            this.btnUpdateProfile.Text = "Update Profile";
            this.btnUpdateProfile.Click += new EventHandler(this.btnUpdateProfile_Click);

            // Buttons - Row 4
            this.btnLoanManagement.Location = new System.Drawing.Point(20, 425);
            this.btnLoanManagement.Size = new System.Drawing.Size(110, 35);
            this.btnLoanManagement.Text = "My Loans";
            this.btnLoanManagement.Click += new EventHandler(this.btnLoanManagement_Click);

            this.btnApplyLoan.Location = new System.Drawing.Point(140, 425);
            this.btnApplyLoan.Size = new System.Drawing.Size(110, 35);
            this.btnApplyLoan.Text = "Apply for Loan";
            this.btnApplyLoan.Click += new EventHandler(this.btnApplyLoan_Click);

            // Buttons - Row 5 (New Features)
            this.btnRecurringPayments.Location = new System.Drawing.Point(20, 470);
            this.btnRecurringPayments.Size = new System.Drawing.Size(110, 35);
            this.btnRecurringPayments.Text = "Recurring Payments";
            this.btnRecurringPayments.Click += new EventHandler(this.btnRecurringPayments_Click);

            this.btnBillPayments.Location = new System.Drawing.Point(140, 470);
            this.btnBillPayments.Size = new System.Drawing.Size(110, 35);
            this.btnBillPayments.Text = "Bill Payments";
            this.btnBillPayments.Click += new EventHandler(this.btnBillPayments_Click);

            this.btnBudgets.Location = new System.Drawing.Point(260, 470);
            this.btnBudgets.Size = new System.Drawing.Size(110, 35);
            this.btnBudgets.Text = "Budgets";
            this.btnBudgets.Click += new EventHandler(this.btnBudgets_Click);

            // Buttons - Row 6
            this.btnSavingsGoals.Location = new System.Drawing.Point(20, 515);
            this.btnSavingsGoals.Size = new System.Drawing.Size(110, 35);
            this.btnSavingsGoals.Text = "Savings Goals";
            this.btnSavingsGoals.Click += new EventHandler(this.btnSavingsGoals_Click);

            this.btnLogout.Location = new System.Drawing.Point(140, 515);
            this.btnLogout.Size = new System.Drawing.Size(110, 35);
            this.btnLogout.Text = "Logout";
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 570);
            this.Controls.AddRange(new Control[] {
                this.lblWelcome, this.lblAccountSummary, this.lstAccounts, 
                this.lblAccountBalance, this.lblTotalBalance,
                this.btnViewTransactions, this.btnStatement, this.btnAnalytics, this.btnAccountDetails, 
                this.btnDeposit, this.btnWithdraw, this.btnTransfer, this.btnCreateAccount, 
                this.btnUpdateProfile, this.btnLoanManagement, this.btnApplyLoan,
                this.btnRecurringPayments, this.btnBillPayments, this.btnBudgets, this.btnSavingsGoals, this.btnLogout
            });
            this.Name = "EnhancedDashboardForm";
            this.Text = "Banking System - Dashboard";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadAccounts()
        {
            try
            {
                var accounts = _accountService.GetUserAccounts(_currentUser.UserId);
                lstAccounts.Items.Clear();
                foreach (var account in accounts)
                {
                    var interestInfo = account.AccountType.TypeName == "Savings" && account.AccountType.InterestRate > 0
                        ? $" (Interest: {account.AccountType.InterestRate}%)"
                        : "";
                    lstAccounts.Items.Add($"{account.AccountNumber} - {account.AccountType.TypeName} - ${account.Balance:F2}{interestInfo}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accounts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateAccountSummary()
        {
            try
            {
                var accounts = _accountService.GetUserAccounts(_currentUser.UserId);
                var totalBalance = accounts.Sum(a => a.Balance);
                lblTotalBalance.Text = $"Total Balance: ${totalBalance:F2}";
            }
            catch
            {
                // Silent fail for summary
            }
        }

        private Account GetSelectedAccount()
        {
            if (lstAccounts.SelectedIndex < 0)
                return null;

            var selectedText = lstAccounts.SelectedItem.ToString();
            var accountNumber = selectedText.Split('-')[0].Trim();
            return _accountService.GetAccountByAccountNumber(accountNumber);
        }

        private void lstAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            var account = GetSelectedAccount();
            if (account != null)
            {
                lblAccountBalance.Text = $"Selected Account Balance: ${account.Balance:F2}";
            }
        }

        private void btnViewTransactions_Click(object sender, EventArgs e)
        {
            var account = GetSelectedAccount();
            if (account == null)
            {
                MessageBox.Show("Please select an account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var transactionForm = new TransactionHistoryForm(account);
            transactionForm.ShowDialog();
        }

        private void btnStatement_Click(object sender, EventArgs e)
        {
            var account = GetSelectedAccount();
            if (account == null)
            {
                MessageBox.Show("Please select an account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var statementForm = new AccountStatementForm(account, _transactionService);
            statementForm.ShowDialog();
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            var account = GetSelectedAccount();
            if (account == null)
            {
                MessageBox.Show("Please select an account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var depositForm = new TransactionForm(account, "Deposit", _transactionService);
            if (depositForm.ShowDialog() == DialogResult.OK)
            {
                LoadAccounts();
                UpdateAccountSummary();
            }
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            var account = GetSelectedAccount();
            if (account == null)
            {
                MessageBox.Show("Please select an account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var withdrawForm = new TransactionForm(account, "Withdrawal", _transactionService);
            if (withdrawForm.ShowDialog() == DialogResult.OK)
            {
                LoadAccounts();
                UpdateAccountSummary();
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            var account = GetSelectedAccount();
            if (account == null)
            {
                MessageBox.Show("Please select an account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var transferForm = new TransferForm(account, _transactionService, _accountService);
            if (transferForm.ShowDialog() == DialogResult.OK)
            {
                LoadAccounts();
                UpdateAccountSummary();
            }
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            var createAccountForm = new CreateAccountForm(_currentUser.UserId, _accountService);
            if (createAccountForm.ShowDialog() == DialogResult.OK)
            {
                LoadAccounts();
                UpdateAccountSummary();
            }
        }

        private void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            var profileForm = new ProfileForm(_currentUser, _userService);
            profileForm.ShowDialog();
            _currentUser = _userService.GetUserById(_currentUser.UserId);
            lblWelcome.Text = $"Welcome, {_currentUser.FullName}";
        }

        private void btnLoanManagement_Click(object sender, EventArgs e)
        {
            var loanForm = new LoanManagementForm(_currentUser.UserId, _loanService, _accountService, _transactionService);
            loanForm.ShowDialog();
        }

        private void btnAccountDetails_Click(object sender, EventArgs e)
        {
            var account = GetSelectedAccount();
            if (account == null)
            {
                MessageBox.Show("Please select an account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var detailsForm = new AccountDetailsForm(account, _accountService);
            if (detailsForm.ShowDialog() == DialogResult.OK)
            {
                LoadAccounts();
                UpdateAccountSummary();
            }
        }

        private void btnAnalytics_Click(object sender, EventArgs e)
        {
            var analyticsForm = new AnalyticsDashboardForm(_currentUser);
            analyticsForm.ShowDialog();
        }

        private void btnApplyLoan_Click(object sender, EventArgs e)
        {
            var applyForm = new LoanApplicationForm(_currentUser.UserId, _loanService, _accountService);
            applyForm.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
                var loginForm = new EnhancedLoginForm();
                loginForm.Show();
            }
        }

        private void btnRecurringPayments_Click(object sender, EventArgs e)
        {
            var recurringPaymentService = new RecurringPaymentService();
            var form = new RecurringPaymentsForm(_currentUser.UserId, recurringPaymentService, _accountService);
            form.ShowDialog();
        }

        private void btnBillPayments_Click(object sender, EventArgs e)
        {
            var billPaymentService = new BillPaymentService();
            var form = new BillPaymentsForm(_currentUser.UserId, billPaymentService, _accountService);
            form.ShowDialog();
        }

        private void btnBudgets_Click(object sender, EventArgs e)
        {
            var budgetService = new BudgetService();
            var form = new BudgetsForm(_currentUser.UserId, budgetService);
            form.ShowDialog();
        }

        private void btnSavingsGoals_Click(object sender, EventArgs e)
        {
            var savingsGoalService = new SavingsGoalService();
            var form = new SavingsGoalsForm(_currentUser.UserId, savingsGoalService, _accountService);
            form.ShowDialog();
        }
    }
}

