using System;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class BillPaymentsForm : Form
    {
        private int _userId;
        private BillPaymentService _service;
        private AccountService _accountService;
        private DataGridView dgvBills;
        private Label lblTitle;
        private Button btnCreate;
        private Button btnPay;
        private Button btnEdit;
        private Button btnCancel;
        private Button btnDelete;
        private Button btnRefresh;
        private Button btnClose;

        private ToolTip _tooltip;

        public BillPaymentsForm(int userId, BillPaymentService service, AccountService accountService)
        {
            _userId = userId;
            _service = service;
            _accountService = accountService;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadBills();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleDataGridView(dgvBills);
            UITheme.StyleButton(btnCreate, ButtonStyle.Success);
            UITheme.StyleButton(btnPay, ButtonStyle.Primary);
            UITheme.StyleButton(btnEdit, ButtonStyle.Secondary);
            UITheme.StyleButton(btnCancel, ButtonStyle.Warning);
            UITheme.StyleButton(btnDelete, ButtonStyle.Danger);
            UITheme.StyleButton(btnRefresh, ButtonStyle.Secondary);
            UITheme.StyleButton(btnClose, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.dgvBills = new DataGridView();
            this.btnCreate = new Button();
            this.btnPay = new Button();
            this.btnEdit = new Button();
            this.btnCancel = new Button();
            this.btnDelete = new Button();
            this.btnRefresh = new Button();
            this.btnClose = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Text = "Bill Payments";

            // dgvBills
            this.dgvBills.Location = new System.Drawing.Point(20, 60);
            this.dgvBills.Size = new System.Drawing.Size(900, 350);
            this.dgvBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBills.ReadOnly = true;
            this.dgvBills.AllowUserToAddRows = false;
            this.dgvBills.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // btnCreate
            this.btnCreate.Location = new System.Drawing.Point(20, 420);
            this.btnCreate.Size = new System.Drawing.Size(100, 30);
            this.btnCreate.Text = "Create New";
            this.btnCreate.Click += new EventHandler(this.btnCreate_Click);

            // btnPay
            this.btnPay.Location = new System.Drawing.Point(130, 420);
            this.btnPay.Size = new System.Drawing.Size(100, 30);
            this.btnPay.Text = "Pay Bill";
            this.btnPay.Click += new EventHandler(this.btnPay_Click);

            // btnEdit
            this.btnEdit.Location = new System.Drawing.Point(240, 420);
            this.btnEdit.Size = new System.Drawing.Size(100, 30);
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new EventHandler(this.btnEdit_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(350, 420);
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.Text = "Cancel Bill";
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // btnDelete
            this.btnDelete.Location = new System.Drawing.Point(460, 420);
            this.btnDelete.Size = new System.Drawing.Size(100, 30);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);

            // btnRefresh
            this.btnRefresh.Location = new System.Drawing.Point(570, 420);
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
                this.lblTitle, this.dgvBills, this.btnCreate, this.btnPay, this.btnEdit,
                this.btnCancel, this.btnDelete, this.btnRefresh, this.btnClose
            });
            this.Name = "BillPaymentsForm";
            this.Text = "Bill Payments Management";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadBills()
        {
            try
            {
                var bills = _service.GetBillPaymentsByUserId(_userId);

                dgvBills.Columns.Clear();
                dgvBills.Columns.Add("BillPaymentId", "ID");
                dgvBills.Columns.Add("PayeeName", "Payee");
                dgvBills.Columns.Add("Amount", "Amount");
                dgvBills.Columns.Add("DueDate", "Due Date");
                dgvBills.Columns.Add("PaymentDate", "Payment Date");
                dgvBills.Columns.Add("Status", "Status");
                dgvBills.Columns.Add("Description", "Description");

                dgvBills.Columns["Amount"].DefaultCellStyle.Format = "C2";
                dgvBills.Columns["DueDate"].DefaultCellStyle.Format = "MM/dd/yyyy";
                dgvBills.Columns["PaymentDate"].DefaultCellStyle.Format = "MM/dd/yyyy";

                dgvBills.Rows.Clear();
                foreach (var bill in bills)
                {
                    dgvBills.Rows.Add(
                        bill.BillPaymentId,
                        bill.PayeeName,
                        bill.Amount,
                        bill.DueDate,
                        bill.PaymentDate?.ToString("MM/dd/yyyy") ?? "Not Paid",
                        bill.Status,
                        bill.Description ?? ""
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bill payments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                var accounts = _accountService.GetUserAccounts(_userId);
                if (accounts.Count == 0)
                {
                    MessageBox.Show("You need at least one account to create a bill payment.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var form = new BillPaymentForm(_userId, null, _service, _accountService);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadBills();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating bill payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvBills.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a bill to pay.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var billId = (int)dgvBills.SelectedRows[0].Cells["BillPaymentId"].Value;
                var bill = _service.GetBillPaymentsByUserId(_userId).FirstOrDefault(b => b.BillPaymentId == billId);
                
                if (bill == null)
                {
                    MessageBox.Show("Bill payment not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (bill.Status == "Paid")
                {
                    MessageBox.Show("This bill has already been paid.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var accounts = _accountService.GetUserAccounts(_userId);
                var form = new PayBillForm(bill, accounts, _service);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadBills();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error paying bill: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvBills.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a bill to edit.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var billId = (int)dgvBills.SelectedRows[0].Cells["BillPaymentId"].Value;
                var bill = _service.GetBillPaymentsByUserId(_userId).FirstOrDefault(b => b.BillPaymentId == billId);
                
                if (bill == null)
                {
                    MessageBox.Show("Bill payment not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (bill.Status == "Paid")
                {
                    MessageBox.Show("Cannot edit a paid bill.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var form = new BillPaymentForm(_userId, bill, _service, _accountService);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadBills();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing bill payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvBills.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a bill to cancel.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var billId = (int)dgvBills.SelectedRows[0].Cells["BillPaymentId"].Value;
                
                if (MessageBox.Show("Are you sure you want to cancel this bill payment?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _service.CancelBillPayment(billId);
                    MessageBox.Show("Bill payment cancelled successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBills();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cancelling bill payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvBills.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a bill to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var billId = (int)dgvBills.SelectedRows[0].Cells["BillPaymentId"].Value;
                
                if (MessageBox.Show("Are you sure you want to delete this bill payment? This action cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _service.DeleteBillPayment(billId);
                    MessageBox.Show("Bill payment deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBills();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting bill payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadBills();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

