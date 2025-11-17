using System;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class LoanApplicationForm : Form
    {
        private int _userId;
        private LoanService _loanService;
        private AccountService _accountService;
        private Label lblTitle;
        private Label lblLoanAmount;
        private Label lblLoanTerm;
        private Label lblAccount;
        private Label lblEligibilityResult;
        private TextBox txtLoanAmount;
        private NumericUpDown numLoanTerm;
        private ComboBox cmbAccount;
        private Button btnCheckEligibility;
        private Button btnApply;
        private Button btnCancel;
        private RichTextBox txtEligibilityDetails;

        private ToolTip _tooltip;

        public LoanApplicationForm(int userId, LoanService loanService, AccountService accountService)
        {
            _userId = userId;
            _loanService = loanService;
            _accountService = accountService;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadAccounts();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleTextBox(txtLoanAmount);
            UITheme.StyleButton(btnCheckEligibility, ButtonStyle.Secondary);
            UITheme.StyleButton(btnApply, ButtonStyle.Success);
            UITheme.StyleButton(btnCancel, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblLoanAmount = new Label();
            this.lblLoanTerm = new Label();
            this.lblAccount = new Label();
            this.lblEligibilityResult = new Label();
            this.txtLoanAmount = new TextBox();
            this.numLoanTerm = new NumericUpDown();
            this.cmbAccount = new ComboBox();
            this.btnCheckEligibility = new Button();
            this.btnApply = new Button();
            this.btnCancel = new Button();
            this.txtEligibilityDetails = new RichTextBox();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(200, 20);
            this.lblTitle.Text = "Apply for Loan";

            // lblLoanAmount
            this.lblLoanAmount.AutoSize = true;
            this.lblLoanAmount.Location = new System.Drawing.Point(20, 60);
            this.lblLoanAmount.Size = new System.Drawing.Size(100, 20);
            this.lblLoanAmount.Text = "Loan Amount ($):";

            // lblLoanTerm
            this.lblLoanTerm.AutoSize = true;
            this.lblLoanTerm.Location = new System.Drawing.Point(20, 100);
            this.lblLoanTerm.Size = new System.Drawing.Size(100, 20);
            this.lblLoanTerm.Text = "Loan Term (Months):";

            // lblAccount
            this.lblAccount.AutoSize = true;
            this.lblAccount.Location = new System.Drawing.Point(20, 140);
            this.lblAccount.Size = new System.Drawing.Size(100, 20);
            this.lblAccount.Text = "Linked Account:";

            // txtLoanAmount
            this.txtLoanAmount.Location = new System.Drawing.Point(130, 57);
            this.txtLoanAmount.Size = new System.Drawing.Size(200, 20);

            // numLoanTerm
            this.numLoanTerm.Location = new System.Drawing.Point(130, 97);
            this.numLoanTerm.Size = new System.Drawing.Size(200, 20);
            this.numLoanTerm.Minimum = 6;
            this.numLoanTerm.Maximum = 60;
            this.numLoanTerm.Value = 12;

            // cmbAccount
            this.cmbAccount.Location = new System.Drawing.Point(130, 137);
            this.cmbAccount.Size = new System.Drawing.Size(200, 21);
            this.cmbAccount.DropDownStyle = ComboBoxStyle.DropDownList;

            // txtEligibilityDetails
            this.txtEligibilityDetails.Location = new System.Drawing.Point(20, 180);
            this.txtEligibilityDetails.Size = new System.Drawing.Size(400, 100);
            this.txtEligibilityDetails.ReadOnly = true;
            this.txtEligibilityDetails.BackColor = System.Drawing.SystemColors.Control;

            // lblEligibilityResult
            this.lblEligibilityResult.AutoSize = true;
            this.lblEligibilityResult.Location = new System.Drawing.Point(20, 290);
            this.lblEligibilityResult.Size = new System.Drawing.Size(400, 20);
            this.lblEligibilityResult.Text = "";

            // btnCheckEligibility
            this.btnCheckEligibility.Location = new System.Drawing.Point(20, 320);
            this.btnCheckEligibility.Size = new System.Drawing.Size(150, 30);
            this.btnCheckEligibility.Text = "Check Eligibility";
            this.btnCheckEligibility.Click += new EventHandler(this.btnCheckEligibility_Click);

            // btnApply
            this.btnApply.Location = new System.Drawing.Point(180, 320);
            this.btnApply.Size = new System.Drawing.Size(120, 30);
            this.btnApply.Text = "Apply for Loan";
            this.btnApply.Enabled = false;
            this.btnApply.Click += new EventHandler(this.btnApply_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(310, 320);
            this.btnCancel.Size = new System.Drawing.Size(110, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 380);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.lblLoanAmount, this.lblLoanTerm, this.lblAccount,
                this.txtLoanAmount, this.numLoanTerm, this.cmbAccount,
                this.txtEligibilityDetails, this.lblEligibilityResult,
                this.btnCheckEligibility, this.btnApply, this.btnCancel
            });
            this.Name = "LoanApplicationForm";
            this.Text = "Apply for Loan";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadAccounts()
        {
            try
            {
                var accounts = _accountService.GetUserAccounts(_userId);
                cmbAccount.Items.Clear();
                foreach (var account in accounts)
                {
                    cmbAccount.Items.Add($"{account.AccountNumber} - {account.AccountType.TypeName}");
                }
                if (cmbAccount.Items.Count > 0)
                    cmbAccount.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accounts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCheckEligibility_Click(object sender, EventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtLoanAmount.Text, out decimal loanAmount) || loanAmount <= 0)
                {
                    MessageBox.Show("Please enter a valid loan amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int loanTerm = (int)numLoanTerm.Value;
                var eligibility = _loanService.CheckLoanEligibility(_userId, loanAmount, loanTerm);

                txtEligibilityDetails.Clear();
                if (eligibility.IsEligible)
                {
                    txtEligibilityDetails.Text = $"✓ ELIGIBLE FOR LOAN\n\n";
                    txtEligibilityDetails.Text += $"Recommended Amount: ${eligibility.RecommendedAmount:F2}\n";
                    txtEligibilityDetails.Text += $"Interest Rate: {eligibility.InterestRate:F2}%\n";
                    var monthlyRate = eligibility.InterestRate / 100 / 12;
                    var powFactor = (double)Math.Pow(1 + (double)monthlyRate, loanTerm);
                    var estimatedPayment = loanAmount * (monthlyRate * (decimal)powFactor) / ((decimal)powFactor - 1);
                    txtEligibilityDetails.Text += $"Monthly Payment (approx): ${estimatedPayment:F2}\n";
                    txtEligibilityDetails.ForeColor = System.Drawing.Color.Green;
                    lblEligibilityResult.Text = "✓ You are eligible for this loan!";
                    lblEligibilityResult.ForeColor = System.Drawing.Color.Green;
                    btnApply.Enabled = true;
                }
                else
                {
                    txtEligibilityDetails.Text = "✗ NOT ELIGIBLE FOR LOAN\n\n";
                    txtEligibilityDetails.Text += "Reasons:\n";
                    foreach (var reason in eligibility.Reasons)
                    {
                        txtEligibilityDetails.Text += $"• {reason}\n";
                    }
                    if (eligibility.RecommendedAmount > 0)
                    {
                        txtEligibilityDetails.Text += $"\nRecommended Amount: ${eligibility.RecommendedAmount:F2}";
                    }
                    txtEligibilityDetails.ForeColor = System.Drawing.Color.Red;
                    lblEligibilityResult.Text = "✗ You are not eligible for this loan amount.";
                    lblEligibilityResult.ForeColor = System.Drawing.Color.Red;
                    btnApply.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking eligibility: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtLoanAmount.Text, out decimal loanAmount) || loanAmount <= 0)
                {
                    MessageBox.Show("Please enter a valid loan amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbAccount.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select an account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int loanTerm = (int)numLoanTerm.Value;
                var selectedText = cmbAccount.SelectedItem.ToString();
                var accountNumber = selectedText.Split('-')[0].Trim();
                var account = _accountService.GetAccountByAccountNumber(accountNumber);

                var loan = _loanService.ApplyForLoan(_userId, loanAmount, loanTerm, account.AccountId);

                MessageBox.Show($"Loan application submitted successfully!\n\n" +
                    $"Loan ID: {loan.LoanId}\n" +
                    $"Amount: ${loan.LoanAmount:F2}\n" +
                    $"Interest Rate: {loan.InterestRate:F2}%\n" +
                    $"Monthly Payment: ${loan.MonthlyPayment:F2}\n" +
                    $"Status: {loan.LoanStatus}\n\n" +
                    $"Your application is pending approval.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Application Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

