using System;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class TransactionHistoryForm : Form
    {
        private Account _account;
        private TransactionService _transactionService;
        private DataGridView dgvTransactions;
        private Label lblTitle;
        private Label lblAccount;
        private Button btnClose;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Label lblStartDate;
        private Label lblEndDate;
        private Button btnFilter;

        private ToolTip _tooltip;

        public TransactionHistoryForm(Account account)
        {
            _account = account;
            _transactionService = new TransactionService();
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadTransactions();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleDataGridView(dgvTransactions);
            UITheme.StyleButton(btnFilter, ButtonStyle.Secondary);
            UITheme.StyleButton(btnClose, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblAccount = new Label();
            this.dgvTransactions = new DataGridView();
            this.btnClose = new Button();
            this.dtpStartDate = new DateTimePicker();
            this.dtpEndDate = new DateTimePicker();
            this.lblStartDate = new Label();
            this.lblEndDate = new Label();
            this.btnFilter = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(200, 20);
            this.lblTitle.Text = "Transaction History";

            // lblAccount
            this.lblAccount.AutoSize = true;
            this.lblAccount.Location = new System.Drawing.Point(20, 50);
            this.lblAccount.Size = new System.Drawing.Size(400, 20);
            this.lblAccount.Text = $"Account: {_account.AccountNumber} ({_account.AccountType.TypeName}) - Balance: ${_account.Balance:F2}";

            // Date filters
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(20, 80);
            this.lblStartDate.Size = new System.Drawing.Size(80, 20);
            this.lblStartDate.Text = "Start Date:";
            this.dtpStartDate.Location = new System.Drawing.Point(100, 77);
            this.dtpStartDate.Size = new System.Drawing.Size(150, 20);
            this.dtpStartDate.Value = DateTime.Now.AddMonths(-1);

            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(270, 80);
            this.lblEndDate.Size = new System.Drawing.Size(80, 20);
            this.lblEndDate.Text = "End Date:";
            this.dtpEndDate.Location = new System.Drawing.Point(350, 77);
            this.dtpEndDate.Size = new System.Drawing.Size(150, 20);
            this.dtpEndDate.Value = DateTime.Now;

            this.btnFilter.Location = new System.Drawing.Point(520, 75);
            this.btnFilter.Size = new System.Drawing.Size(80, 25);
            this.btnFilter.Text = "Filter";
            this.btnFilter.Click += new EventHandler(this.btnFilter_Click);

            // dgvTransactions
            this.dgvTransactions.Location = new System.Drawing.Point(20, 110);
            this.dgvTransactions.Size = new System.Drawing.Size(750, 300);
            this.dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTransactions.ReadOnly = true;
            this.dgvTransactions.AllowUserToAddRows = false;

            // btnClose
            this.btnClose.Location = new System.Drawing.Point(690, 420);
            this.btnClose.Size = new System.Drawing.Size(80, 30);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new EventHandler(this.btnClose_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 470);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.lblAccount, this.lblStartDate, this.lblEndDate,
                this.dtpStartDate, this.dtpEndDate, this.btnFilter,
                this.dgvTransactions, this.btnClose
            });
            this.Name = "TransactionHistoryForm";
            this.Text = "Transaction History";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadTransactions(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var transactions = _transactionService.GetTransactionHistory(_account.AccountId, startDate, endDate);

                dgvTransactions.Columns.Clear();
                dgvTransactions.Columns.Add("TransactionDate", "Date");
                dgvTransactions.Columns.Add("TransactionType", "Type");
                dgvTransactions.Columns.Add("Amount", "Amount");
                dgvTransactions.Columns.Add("BalanceAfter", "Balance After");
                dgvTransactions.Columns.Add("Description", "Description");

                dgvTransactions.Columns["Amount"].DefaultCellStyle.Format = "C2";
                dgvTransactions.Columns["BalanceAfter"].DefaultCellStyle.Format = "C2";
                dgvTransactions.Columns["TransactionDate"].DefaultCellStyle.Format = "MM/dd/yyyy HH:mm";

                dgvTransactions.Rows.Clear();
                foreach (var transaction in transactions)
                {
                    dgvTransactions.Rows.Add(
                        transaction.TransactionDate,
                        transaction.TransactionType.TypeName,
                        transaction.Amount,
                        transaction.BalanceAfterTransaction,
                        transaction.Description
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transactions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            LoadTransactions(dtpStartDate.Value, dtpEndDate.Value);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

