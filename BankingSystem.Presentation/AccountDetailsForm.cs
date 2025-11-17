using System;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class AccountDetailsForm : Form
    {
        private Account _account;
        private AccountService _accountService;
        private AccountInterestService _interestService;
        private Label lblTitle;
        private Label lblAccountNumber;
        private Label lblAccountType;
        private Label lblBalance;
        private Label lblInterestRate;
        private Label lblMinimumBalance;
        private Label lblCreatedDate;
        private Label lblProjectedInterest;
        private Button btnClose;
        private Button btnCloseAccount;

        private ToolTip _tooltip;

        public AccountDetailsForm(Account account, AccountService accountService)
        {
            _account = account;
            _accountService = accountService;
            _interestService = new AccountInterestService();
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadAccountDetails();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleButton(btnCloseAccount, ButtonStyle.Danger);
            UITheme.StyleButton(btnClose, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
            UITheme.StyleLabel(lblBalance, LabelStyle.Success);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblAccountNumber = new Label();
            this.lblAccountType = new Label();
            this.lblBalance = new Label();
            this.lblInterestRate = new Label();
            this.lblMinimumBalance = new Label();
            this.lblCreatedDate = new Label();
            this.lblProjectedInterest = new Label();
            this.btnClose = new Button();
            this.btnCloseAccount = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(200, 20);
            this.lblTitle.Text = "Account Details";

            int yPos = 60;
            this.lblAccountNumber.Location = new System.Drawing.Point(20, yPos);
            this.lblAccountNumber.Size = new System.Drawing.Size(300, 20);
            yPos += 30;
            this.lblAccountType.Location = new System.Drawing.Point(20, yPos);
            this.lblAccountType.Size = new System.Drawing.Size(300, 20);
            yPos += 30;
            this.lblBalance.Location = new System.Drawing.Point(20, yPos);
            this.lblBalance.Size = new System.Drawing.Size(300, 20);
            this.lblBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblBalance.ForeColor = System.Drawing.Color.Green;
            yPos += 30;
            this.lblInterestRate.Location = new System.Drawing.Point(20, yPos);
            this.lblInterestRate.Size = new System.Drawing.Size(300, 20);
            yPos += 30;
            this.lblMinimumBalance.Location = new System.Drawing.Point(20, yPos);
            this.lblMinimumBalance.Size = new System.Drawing.Size(300, 20);
            yPos += 30;
            this.lblCreatedDate.Location = new System.Drawing.Point(20, yPos);
            this.lblCreatedDate.Size = new System.Drawing.Size(300, 20);
            yPos += 30;
            this.lblProjectedInterest.Location = new System.Drawing.Point(20, yPos);
            this.lblProjectedInterest.Size = new System.Drawing.Size(300, 20);
            this.lblProjectedInterest.ForeColor = System.Drawing.Color.Blue;

            // Buttons
            this.btnCloseAccount.Location = new System.Drawing.Point(20, 280);
            this.btnCloseAccount.Size = new System.Drawing.Size(140, 30);
            this.btnCloseAccount.Text = "Close Account";
            this.btnCloseAccount.Enabled = _account.Balance == 0;
            this.btnCloseAccount.Click += new EventHandler(this.btnCloseAccount_Click);

            this.btnClose.Location = new System.Drawing.Point(180, 280);
            this.btnClose.Size = new System.Drawing.Size(140, 30);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new EventHandler(this.btnClose_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 330);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.lblAccountNumber, this.lblAccountType, this.lblBalance,
                this.lblInterestRate, this.lblMinimumBalance, this.lblCreatedDate, this.lblProjectedInterest,
                this.btnCloseAccount, this.btnClose
            });
            this.Name = "AccountDetailsForm";
            this.Text = "Account Details";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadAccountDetails()
        {
            lblAccountNumber.Text = $"Account Number: {_account.AccountNumber}";
            lblAccountType.Text = $"Account Type: {_account.AccountType.TypeName}";
            lblBalance.Text = $"Current Balance: ${_account.Balance:F2}";
            lblInterestRate.Text = $"Interest Rate: {_account.AccountType.InterestRate:F2}%";
            lblMinimumBalance.Text = $"Minimum Balance: ${_account.AccountType.MinimumBalance:F2}";
            lblCreatedDate.Text = $"Created: {_account.CreatedDate:MM/dd/yyyy}";

            if (_account.AccountType.TypeName == "Savings" && _account.AccountType.InterestRate > 0)
            {
                var projectedInterest = _interestService.CalculateProjectedInterest(_account.AccountId, 12);
                lblProjectedInterest.Text = $"Projected Annual Interest: ${projectedInterest:F2}";
            }
            else
            {
                lblProjectedInterest.Text = "";
            }
        }

        private void btnCloseAccount_Click(object sender, EventArgs e)
        {
            if (_account.Balance != 0)
            {
                MessageBox.Show("Account must have a zero balance before it can be closed.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to close account {_account.AccountNumber}?\n\nThis action cannot be undone.",
                "Confirm Account Closure", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var accountRepo = new BankingSystem.DataAccess.AccountRepository();
                    if (accountRepo.CloseAccount(_account.AccountId))
                    {
                        MessageBox.Show("Account closed successfully.", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error closing account: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

