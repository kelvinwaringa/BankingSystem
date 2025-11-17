using System;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class TransactionForm : Form
    {
        private Account _account;
        private string _transactionType;
        private TransactionService _transactionService;
        private Label lblTitle;
        private Label lblAccount;
        private Label lblAmount;
        private Label lblDescription;
        private TextBox txtAmount;
        private TextBox txtDescription;
        private Button btnProcess;
        private Button btnCancel;

        private ToolTip _tooltip;

        public TransactionForm(Account account, string transactionType, TransactionService transactionService)
        {
            _account = account;
            _transactionType = transactionType;
            _transactionService = transactionService;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleTextBox(txtAmount);
            UITheme.StyleTextBox(txtDescription);
            UITheme.StyleButton(btnProcess, _transactionType == "Deposit" ? ButtonStyle.Success : ButtonStyle.Warning);
            UITheme.StyleButton(btnCancel, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblAccount = new Label();
            this.lblAmount = new Label();
            this.lblDescription = new Label();
            this.txtAmount = new TextBox();
            this.txtDescription = new TextBox();
            this.btnProcess = new Button();
            this.btnCancel = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(200, 20);
            this.lblTitle.Text = $"{_transactionType} Transaction";

            // lblAccount
            this.lblAccount.AutoSize = true;
            this.lblAccount.Location = new System.Drawing.Point(20, 60);
            this.lblAccount.Size = new System.Drawing.Size(300, 20);
            this.lblAccount.Text = $"Account: {_account.AccountNumber} ({_account.AccountType.TypeName}) - Balance: ${_account.Balance:F2}";

            // lblAmount
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(20, 100);
            this.lblAmount.Size = new System.Drawing.Size(100, 20);
            this.lblAmount.Text = "Amount:";

            // lblDescription
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(20, 140);
            this.lblDescription.Size = new System.Drawing.Size(100, 20);
            this.lblDescription.Text = "Description:";

            // txtAmount
            this.txtAmount.Location = new System.Drawing.Point(130, 97);
            this.txtAmount.Size = new System.Drawing.Size(200, 20);

            // txtDescription
            this.txtDescription.Location = new System.Drawing.Point(130, 137);
            this.txtDescription.Size = new System.Drawing.Size(200, 60);
            this.txtDescription.Multiline = true;

            // btnProcess
            this.btnProcess.Location = new System.Drawing.Point(130, 210);
            this.btnProcess.Size = new System.Drawing.Size(90, 30);
            this.btnProcess.Text = _transactionType;
            this.btnProcess.Click += new EventHandler(this.btnProcess_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(240, 210);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 270);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.lblAccount, this.lblAmount, this.lblDescription,
                this.txtAmount, this.txtDescription, this.btnProcess, this.btnCancel
            });
            this.Name = "TransactionForm";
            this.Text = _transactionType;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Please enter a valid amount greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Transaction transaction = null;
                if (_transactionType == "Deposit")
                {
                    transaction = _transactionService.ProcessDeposit(_account.AccountId, amount, txtDescription.Text);
                }
                else if (_transactionType == "Withdrawal")
                {
                    transaction = _transactionService.ProcessWithdrawal(_account.AccountId, amount, txtDescription.Text);
                }

                if (transaction != null)
                {
                    MessageBox.Show($"{_transactionType} processed successfully!\nNew Balance: ${transaction.BalanceAfterTransaction:F2}", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Transaction Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

