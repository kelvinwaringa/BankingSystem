using System;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class BudgetsForm : Form
    {
        private int _userId;
        private BudgetService _service;
        private DataGridView dgvBudgets;
        private Label lblTitle;
        private Button btnCreate;
        private Button btnEdit;
        private Button btnViewStatus;
        private Button btnDelete;
        private Button btnRefresh;
        private Button btnClose;

        private ToolTip _tooltip;

        public BudgetsForm(int userId, BudgetService service)
        {
            _userId = userId;
            _service = service;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadBudgets();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleDataGridView(dgvBudgets);
            UITheme.StyleButton(btnCreate, ButtonStyle.Success);
            UITheme.StyleButton(btnEdit, ButtonStyle.Primary);
            UITheme.StyleButton(btnViewStatus, ButtonStyle.Secondary);
            UITheme.StyleButton(btnDelete, ButtonStyle.Danger);
            UITheme.StyleButton(btnRefresh, ButtonStyle.Secondary);
            UITheme.StyleButton(btnClose, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.dgvBudgets = new DataGridView();
            this.btnCreate = new Button();
            this.btnEdit = new Button();
            this.btnViewStatus = new Button();
            this.btnDelete = new Button();
            this.btnRefresh = new Button();
            this.btnClose = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Text = "Budgets";

            // dgvBudgets
            this.dgvBudgets.Location = new System.Drawing.Point(20, 60);
            this.dgvBudgets.Size = new System.Drawing.Size(900, 350);
            this.dgvBudgets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBudgets.ReadOnly = true;
            this.dgvBudgets.AllowUserToAddRows = false;
            this.dgvBudgets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

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

            // btnViewStatus
            this.btnViewStatus.Location = new System.Drawing.Point(260, 420);
            this.btnViewStatus.Size = new System.Drawing.Size(120, 30);
            this.btnViewStatus.Text = "View Status";
            this.btnViewStatus.Click += new EventHandler(this.btnViewStatus_Click);

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
                this.lblTitle, this.dgvBudgets, this.btnCreate, this.btnEdit,
                this.btnViewStatus, this.btnDelete, this.btnRefresh, this.btnClose
            });
            this.Name = "BudgetsForm";
            this.Text = "Budget Management";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadBudgets()
        {
            try
            {
                var budgets = _service.GetBudgetsByUserId(_userId);

                dgvBudgets.Columns.Clear();
                dgvBudgets.Columns.Add("BudgetId", "ID");
                dgvBudgets.Columns.Add("Category", "Category");
                dgvBudgets.Columns.Add("BudgetAmount", "Budget Amount");
                dgvBudgets.Columns.Add("Period", "Period");
                dgvBudgets.Columns.Add("StartDate", "Start Date");
                dgvBudgets.Columns.Add("EndDate", "End Date");
                dgvBudgets.Columns.Add("IsActive", "Active");

                dgvBudgets.Columns["BudgetAmount"].DefaultCellStyle.Format = "C2";
                dgvBudgets.Columns["StartDate"].DefaultCellStyle.Format = "MM/dd/yyyy";
                dgvBudgets.Columns["EndDate"].DefaultCellStyle.Format = "MM/dd/yyyy";

                dgvBudgets.Rows.Clear();
                foreach (var budget in budgets)
                {
                    dgvBudgets.Rows.Add(
                        budget.BudgetId,
                        budget.Category,
                        budget.BudgetAmount,
                        budget.Period,
                        budget.StartDate,
                        budget.EndDate?.ToString("MM/dd/yyyy") ?? "No End",
                        budget.IsActive ? "Yes" : "No"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading budgets: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                var form = new BudgetForm(_userId, null, _service);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadBudgets();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating budget: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvBudgets.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a budget to edit.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var budgetId = (int)dgvBudgets.SelectedRows[0].Cells["BudgetId"].Value;
                var budget = _service.GetBudgetsByUserId(_userId).FirstOrDefault(b => b.BudgetId == budgetId);
                
                if (budget == null)
                {
                    MessageBox.Show("Budget not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var form = new BudgetForm(_userId, budget, _service);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadBudgets();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing budget: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnViewStatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvBudgets.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a budget to view status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var budgetId = (int)dgvBudgets.SelectedRows[0].Cells["BudgetId"].Value;
                var status = _service.GetBudgetStatus(budgetId);
                
                var message = $"Budget: {status.Budget.Category}\n" +
                             $"Budget Amount: {status.Budget.BudgetAmount:C}\n" +
                             $"Total Spent: {status.TotalSpent:C}\n" +
                             $"Remaining: {status.RemainingAmount:C}\n" +
                             $"Percentage Used: {status.PercentageUsed:F2}%\n" +
                             $"Status: {(status.IsOverBudget ? "OVER BUDGET" : "Within Budget")}";
                
                MessageBox.Show(message, "Budget Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error viewing budget status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvBudgets.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a budget to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var budgetId = (int)dgvBudgets.SelectedRows[0].Cells["BudgetId"].Value;
                
                if (MessageBox.Show("Are you sure you want to delete this budget? This action cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _service.DeleteBudget(budgetId);
                    MessageBox.Show("Budget deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBudgets();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting budget: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadBudgets();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

