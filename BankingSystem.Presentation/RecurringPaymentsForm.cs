using System;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class RecurringPaymentsForm : Form
    {
        private int _userId;
        private RecurringPaymentService _service;
        private AccountService _accountService;
        private DataGridView dgvPayments;
        private Label lblTitle;
        private Button btnCreate;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnDeactivate;
        private Button btnRefresh;
        private Button btnClose;

        private ToolTip _tooltip;

        public RecurringPaymentsForm(int userId, RecurringPaymentService service, AccountService accountService)
        {
            _userId = userId;
            _service = service;
            _accountService = accountService;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadPayments();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleDataGridView(dgvPayments);
            UITheme.StyleButton(btnCreate, ButtonStyle.Success);
            UITheme.StyleButton(btnEdit, ButtonStyle.Primary);
            UITheme.StyleButton(btnDelete, ButtonStyle.Danger);
            UITheme.StyleButton(btnDeactivate, ButtonStyle.Warning);
            UITheme.StyleButton(btnRefresh, ButtonStyle.Secondary);
            UITheme.StyleButton(btnClose, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.dgvPayments = new DataGridView();
            this.btnCreate = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnDeactivate = new Button();
            this.btnRefresh = new Button();
            this.btnClose = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(200, 20);
            this.lblTitle.Text = "Recurring Payments";

            // dgvPayments
            this.dgvPayments.Location = new System.Drawing.Point(20, 60);
            this.dgvPayments.Size = new System.Drawing.Size(900, 350);
            this.dgvPayments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPayments.ReadOnly = true;
            this.dgvPayments.AllowUserToAddRows = false;
            this.dgvPayments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // btnCreate
            this.btnCreate.Location = new System.Drawing.Point(20, 420);
            this.btnCreate.Size = new System.Drawing.Size(120, 30);
            this.btnCreate.Text = "Create New";
            this.btnCreate.Click += new EventHandler(this.btnCreate_Click);

            // btnEdit
            this.btnEdit.Location = new System.Drawing.Point(150, 420);
            this.btnEdit.Size = new System.Drawing.Size(100, 30);
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new EventHandler(this.btnEdit_Click);

            // btnDeactivate
            this.btnDeactivate.Location = new System.Drawing.Point(260, 420);
            this.btnDeactivate.Size = new System.Drawing.Size(120, 30);
            this.btnDeactivate.Text = "Deactivate";
            this.btnDeactivate.Click += new EventHandler(this.btnDeactivate_Click);

            // btnDelete
            this.btnDelete.Location = new System.Drawing.Point(390, 420);
            this.btnDelete.Size = new System.Drawing.Size(100, 30);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);

            // btnRefresh
            this.btnRefresh.Location = new System.Drawing.Point(500, 420);
            this.btnRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);

            // btnClose
            this.btnClose.Location = new System.Drawing.Point(840, 420);
            this.btnClose.Size = new System.Drawing.Size(80, 30);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new EventHandler(this.btnClose_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 470);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.dgvPayments, this.btnCreate, this.btnEdit, 
                this.btnDeactivate, this.btnDelete, this.btnRefresh, this.btnClose
            });
            this.Name = "RecurringPaymentsForm";
            this.Text = "Recurring Payments Management";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadPayments()
        {
            try
            {
                var payments = _service.GetRecurringPaymentsByUserId(_userId);

                dgvPayments.Columns.Clear();
                dgvPayments.Columns.Add("RecurringPaymentId", "ID");
                dgvPayments.Columns.Add("RecipientName", "Recipient");
                dgvPayments.Columns.Add("Amount", "Amount");
                dgvPayments.Columns.Add("Frequency", "Frequency");
                dgvPayments.Columns.Add("NextPaymentDate", "Next Payment");
                dgvPayments.Columns.Add("LastPaymentDate", "Last Payment");
                dgvPayments.Columns.Add("EndDate", "End Date");
                dgvPayments.Columns.Add("IsActive", "Active");
                dgvPayments.Columns.Add("Description", "Description");

                dgvPayments.Columns["Amount"].DefaultCellStyle.Format = "C2";
                dgvPayments.Columns["NextPaymentDate"].DefaultCellStyle.Format = "MM/dd/yyyy";
                dgvPayments.Columns["LastPaymentDate"].DefaultCellStyle.Format = "MM/dd/yyyy";
                dgvPayments.Columns["EndDate"].DefaultCellStyle.Format = "MM/dd/yyyy";

                dgvPayments.Rows.Clear();
                foreach (var payment in payments)
                {
                    dgvPayments.Rows.Add(
                        payment.RecurringPaymentId,
                        payment.RecipientName,
                        payment.Amount,
                        payment.Frequency,
                        payment.NextPaymentDate,
                        payment.LastPaymentDate?.ToString("MM/dd/yyyy") ?? "Never",
                        payment.EndDate?.ToString("MM/dd/yyyy") ?? "Never",
                        payment.IsActive ? "Yes" : "No",
                        payment.Description ?? ""
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recurring payments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                var accounts = _accountService.GetUserAccounts(_userId);
                if (accounts.Count == 0)
                {
                    MessageBox.Show("You need at least one account to create a recurring payment.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var form = new RecurringPaymentForm(_userId, null, _service, _accountService);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadPayments();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating recurring payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPayments.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a recurring payment to edit.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var paymentId = (int)dgvPayments.SelectedRows[0].Cells["RecurringPaymentId"].Value;
                var payment = _service.GetRecurringPaymentsByUserId(_userId).FirstOrDefault(p => p.RecurringPaymentId == paymentId);
                
                if (payment == null)
                {
                    MessageBox.Show("Recurring payment not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var form = new RecurringPaymentForm(_userId, payment, _service, _accountService);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadPayments();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing recurring payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeactivate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPayments.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a recurring payment to deactivate.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var paymentId = (int)dgvPayments.SelectedRows[0].Cells["RecurringPaymentId"].Value;
                
                if (MessageBox.Show("Are you sure you want to deactivate this recurring payment?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _service.DeactivateRecurringPayment(paymentId);
                    MessageBox.Show("Recurring payment deactivated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPayments();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deactivating recurring payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPayments.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a recurring payment to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var paymentId = (int)dgvPayments.SelectedRows[0].Cells["RecurringPaymentId"].Value;
                
                if (MessageBox.Show("Are you sure you want to delete this recurring payment? This action cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _service.DeleteRecurringPayment(paymentId);
                    MessageBox.Show("Recurring payment deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPayments();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting recurring payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPayments();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

