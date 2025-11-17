using System;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class LoanPaymentForm : Form
    {
        private Loan _loan;
        private System.Collections.Generic.List<Account> _accounts;
        private LoanService _loanService;
        private TransactionService _transactionService;
        private Label lblTitle;
        private Label lblLoanInfo;
        private Label lblAccount;
        private Label lblAmount;
        private ComboBox cmbAccount;
        private TextBox txtAmount;
        private Button btnPay;
        private Button btnCancel;

        private ToolTip _tooltip;

        public LoanPaymentForm(Loan loan, System.Collections.Generic.List<Account> accounts, LoanService loanService, TransactionService transactionService)
        {
            _loan = loan;
            _accounts = accounts;
            _loanService = loanService;
            _transactionService = transactionService;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleTextBox(txtAmount);
            UITheme.StyleButton(btnPay, ButtonStyle.Success);
            UITheme.StyleButton(btnCancel, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblLoanInfo = new Label();
            this.lblAccount = new Label();
            this.lblAmount = new Label();
            this.cmbAccount = new ComboBox();
            this.txtAmount = new TextBox();
            this.btnPay = new Button();
            this.btnCancel = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(200, 20);
            this.lblTitle.Text = "Make Loan Payment";

            // lblLoanInfo
            this.lblLoanInfo.AutoSize = true;
            this.lblLoanInfo.Location = new System.Drawing.Point(20, 60);
            this.lblLoanInfo.Size = new System.Drawing.Size(400, 60);
            this.lblLoanInfo.Text = $"Loan ID: {_loan.LoanId}\n" +
                $"Remaining Balance: ${_loan.RemainingBalance:F2}\n" +
                $"Monthly Payment: ${_loan.MonthlyPayment:F2}";

            // lblAccount
            this.lblAccount.AutoSize = true;
            this.lblAccount.Location = new System.Drawing.Point(20, 140);
            this.lblAccount.Size = new System.Drawing.Size(100, 20);
            this.lblAccount.Text = "From Account:";

            // lblAmount
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(20, 180);
            this.lblAmount.Size = new System.Drawing.Size(100, 20);
            this.lblAmount.Text = "Payment Amount:";

            // cmbAccount
            this.cmbAccount.Location = new System.Drawing.Point(130, 137);
            this.cmbAccount.Size = new System.Drawing.Size(250, 21);
            this.cmbAccount.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var account in _accounts)
            {
                cmbAccount.Items.Add($"{account.AccountNumber} - {account.AccountType.TypeName} - Balance: ${account.Balance:F2}");
            }
            if (cmbAccount.Items.Count > 0)
                cmbAccount.SelectedIndex = 0;

            // txtAmount
            this.txtAmount.Location = new System.Drawing.Point(130, 177);
            this.txtAmount.Size = new System.Drawing.Size(250, 20);
            this.txtAmount.Text = _loan.MonthlyPayment.ToString("F2");

            // btnPay
            this.btnPay.Location = new System.Drawing.Point(130, 220);
            this.btnPay.Size = new System.Drawing.Size(120, 30);
            this.btnPay.Text = "Make Payment";
            this.btnPay.Click += new EventHandler(this.btnPay_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(260, 220);
            this.btnCancel.Size = new System.Drawing.Size(120, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 280);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.lblLoanInfo, this.lblAccount, this.lblAmount,
                this.cmbAccount, this.txtAmount, this.btnPay, this.btnCancel
            });
            this.Name = "LoanPaymentForm";
            this.Text = "Loan Payment";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbAccount.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select an account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out decimal paymentAmount) || paymentAmount <= 0)
                {
                    MessageBox.Show("Please enter a valid payment amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedText = cmbAccount.SelectedItem.ToString();
                var accountNumber = selectedText.Split('-')[0].Trim();
                var account = _accounts.First(a => a.AccountNumber == accountNumber);

                _loanService.ProcessLoanPayment(_loan.LoanId, account.AccountId, paymentAmount);

                var updatedLoan = _loanService.GetLoanById(_loan.LoanId);
                var message = $"Payment processed successfully!\n\nRemaining Balance: ${updatedLoan.RemainingBalance:F2}";
                if (updatedLoan.RemainingBalance <= 0)
                {
                    message += "\n\nLoan fully paid!";
                }

                MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Payment Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

