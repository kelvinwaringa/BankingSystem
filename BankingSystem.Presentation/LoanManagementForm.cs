using System;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class LoanManagementForm : Form
    {
        private int _userId;
        private LoanService _loanService;
        private AccountService _accountService;
        private TransactionService _transactionService;
        private DataGridView dgvLoans;
        private Label lblTitle;
        private Button btnMakePayment;
        private Button btnClose;
        private Button btnRefresh;

        private ToolTip _tooltip;

        public LoanManagementForm(int userId, LoanService loanService, AccountService accountService, TransactionService transactionService)
        {
            _userId = userId;
            _loanService = loanService;
            _accountService = accountService;
            _transactionService = transactionService;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadLoans();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleDataGridView(dgvLoans);
            UITheme.StyleButton(btnMakePayment, ButtonStyle.Success);
            UITheme.StyleButton(btnRefresh, ButtonStyle.Secondary);
            UITheme.StyleButton(btnClose, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.dgvLoans = new DataGridView();
            this.btnMakePayment = new Button();
            this.btnRefresh = new Button();
            this.btnClose = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(200, 20);
            this.lblTitle.Text = "My Loans";

            // dgvLoans
            this.dgvLoans.Location = new System.Drawing.Point(20, 60);
            this.dgvLoans.Size = new System.Drawing.Size(800, 350);
            this.dgvLoans.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLoans.ReadOnly = true;
            this.dgvLoans.AllowUserToAddRows = false;
            this.dgvLoans.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // btnMakePayment
            this.btnMakePayment.Location = new System.Drawing.Point(20, 420);
            this.btnMakePayment.Size = new System.Drawing.Size(150, 30);
            this.btnMakePayment.Text = "Make Payment";
            this.btnMakePayment.Click += new EventHandler(this.btnMakePayment_Click);

            // btnRefresh
            this.btnRefresh.Location = new System.Drawing.Point(180, 420);
            this.btnRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);

            // btnClose
            this.btnClose.Location = new System.Drawing.Point(740, 420);
            this.btnClose.Size = new System.Drawing.Size(80, 30);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new EventHandler(this.btnClose_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 470);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.dgvLoans, this.btnMakePayment, this.btnRefresh, this.btnClose
            });
            this.Name = "LoanManagementForm";
            this.Text = "Loan Management";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadLoans()
        {
            try
            {
                var loans = _loanService.GetUserLoans(_userId);

                dgvLoans.Columns.Clear();
                dgvLoans.Columns.Add("LoanId", "Loan ID");
                dgvLoans.Columns.Add("LoanAmount", "Loan Amount");
                dgvLoans.Columns.Add("InterestRate", "Interest Rate");
                dgvLoans.Columns.Add("MonthlyPayment", "Monthly Payment");
                dgvLoans.Columns.Add("RemainingBalance", "Remaining Balance");
                dgvLoans.Columns.Add("LoanStatus", "Status");
                dgvLoans.Columns.Add("ApplicationDate", "Application Date");
                dgvLoans.Columns.Add("DueDate", "Due Date");

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
                        loan.LoanAmount,
                        loan.InterestRate,
                        loan.MonthlyPayment,
                        loan.RemainingBalance,
                        loan.LoanStatus,
                        loan.ApplicationDate,
                        loan.DueDate?.ToString("MM/dd/yyyy") ?? "N/A"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading loans: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMakePayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvLoans.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a loan to make a payment.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var loanId = (int)dgvLoans.SelectedRows[0].Cells["LoanId"].Value;
                var loan = _loanService.GetLoanById(loanId);

                if (loan.LoanStatus != "Active" && loan.LoanStatus != "Approved")
                {
                    MessageBox.Show("This loan is not active. Only active or approved loans can receive payments.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (loan.RemainingBalance <= 0)
                {
                    MessageBox.Show("This loan is already paid off.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Get user accounts for payment
                var accounts = _accountService.GetUserAccounts(_userId);
                if (accounts.Count == 0)
                {
                    MessageBox.Show("No accounts available for payment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Show payment form
                var paymentForm = new LoanPaymentForm(loan, accounts, _loanService, _transactionService);
                paymentForm.ShowDialog();
                LoadLoans();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadLoans();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

