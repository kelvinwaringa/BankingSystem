using System;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class TransferForm : Form
    {
        private Account _fromAccount;
        private TransactionService _transactionService;
        private AccountService _accountService;
        private Label lblTitle;
        private Label lblFromAccount;
        private Label lblToAccount;
        private Label lblAmount;
        private Label lblDescription;
        private ComboBox cmbToAccount;
        private TextBox txtAmount;
        private TextBox txtDescription;
        private Button btnTransfer;
        private Button btnCancel;

        private ToolTip _tooltip;

        public TransferForm(Account fromAccount, TransactionService transactionService, AccountService accountService)
        {
            _fromAccount = fromAccount;
            _transactionService = transactionService;
            _accountService = accountService;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleTextBox(txtAmount);
            UITheme.StyleTextBox(txtDescription);
            UITheme.StyleButton(btnTransfer, ButtonStyle.Primary);
            UITheme.StyleButton(btnCancel, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblFromAccount = new Label();
            this.lblToAccount = new Label();
            this.lblAmount = new Label();
            this.lblDescription = new Label();
            this.cmbToAccount = new ComboBox();
            this.txtAmount = new TextBox();
            this.txtDescription = new TextBox();
            this.btnTransfer = new Button();
            this.btnCancel = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(200, 20);
            this.lblTitle.Text = "Transfer Funds";

            // lblFromAccount
            this.lblFromAccount.AutoSize = true;
            this.lblFromAccount.Location = new System.Drawing.Point(20, 60);
            this.lblFromAccount.Size = new System.Drawing.Size(300, 20);
            this.lblFromAccount.Text = $"From: {_fromAccount.AccountNumber} ({_fromAccount.AccountType.TypeName}) - Balance: ${_fromAccount.Balance:F2}";

            // lblToAccount
            this.lblToAccount.AutoSize = true;
            this.lblToAccount.Location = new System.Drawing.Point(20, 100);
            this.lblToAccount.Size = new System.Drawing.Size(100, 20);
            this.lblToAccount.Text = "To Account:";

            // lblAmount
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(20, 140);
            this.lblAmount.Size = new System.Drawing.Size(100, 20);
            this.lblAmount.Text = "Amount:";

            // lblDescription
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(20, 180);
            this.lblDescription.Size = new System.Drawing.Size(100, 20);
            this.lblDescription.Text = "Description:";

            // cmbToAccount
            this.cmbToAccount.Location = new System.Drawing.Point(130, 97);
            this.cmbToAccount.Size = new System.Drawing.Size(200, 21);
            this.cmbToAccount.DropDownStyle = ComboBoxStyle.DropDownList;
            LoadDestinationAccounts();

            // txtAmount
            this.txtAmount.Location = new System.Drawing.Point(130, 137);
            this.txtAmount.Size = new System.Drawing.Size(200, 20);

            // txtDescription
            this.txtDescription.Location = new System.Drawing.Point(130, 177);
            this.txtDescription.Size = new System.Drawing.Size(200, 40);
            this.txtDescription.Multiline = true;

            // btnTransfer
            this.btnTransfer.Location = new System.Drawing.Point(130, 230);
            this.btnTransfer.Size = new System.Drawing.Size(90, 30);
            this.btnTransfer.Text = "Transfer";
            this.btnTransfer.Click += new EventHandler(this.btnTransfer_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(240, 230);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 290);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.lblFromAccount, this.lblToAccount, this.lblAmount, this.lblDescription,
                this.cmbToAccount, this.txtAmount, this.txtDescription, this.btnTransfer, this.btnCancel
            });
            this.Name = "TransferForm";
            this.Text = "Transfer Funds";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadDestinationAccounts()
        {
            try
            {
                var accounts = _accountService.GetUserAccounts(_fromAccount.UserId)
                    .Where(a => a.AccountId != _fromAccount.AccountId && a.IsActive)
                    .ToList();

                cmbToAccount.Items.Clear();
                foreach (var account in accounts)
                {
                    cmbToAccount.Items.Add($"{account.AccountNumber} - {account.AccountType.TypeName}");
                }

                if (cmbToAccount.Items.Count == 0)
                {
                    MessageBox.Show("No other accounts available for transfer.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accounts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbToAccount.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a destination account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Please enter a valid amount greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedText = cmbToAccount.SelectedItem.ToString();
                var toAccountNumber = selectedText.Split('-')[0].Trim();
                var toAccount = _accountService.GetAccountByAccountNumber(toAccountNumber);

                var transaction = _transactionService.ProcessTransfer(
                    _fromAccount.AccountId,
                    toAccount.AccountId,
                    amount,
                    txtDescription.Text ?? $"Transfer to {toAccountNumber}"
                );

                MessageBox.Show($"Transfer processed successfully!\nNew Balance: ${transaction.BalanceAfterTransaction:F2}", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Transfer Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

