using System;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class SavingsGoalsForm : Form
    {
        private int _userId;
        private SavingsGoalService _service;
        private AccountService _accountService;
        private DataGridView dgvGoals;
        private Label lblTitle;
        private Button btnCreate;
        private Button btnEdit;
        private Button btnAddToGoal;
        private Button btnDelete;
        private Button btnRefresh;
        private Button btnClose;

        private ToolTip _tooltip;

        public SavingsGoalsForm(int userId, SavingsGoalService service, AccountService accountService)
        {
            _userId = userId;
            _service = service;
            _accountService = accountService;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadGoals();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleDataGridView(dgvGoals);
            UITheme.StyleButton(btnCreate, ButtonStyle.Success);
            UITheme.StyleButton(btnEdit, ButtonStyle.Primary);
            UITheme.StyleButton(btnAddToGoal, ButtonStyle.Success);
            UITheme.StyleButton(btnDelete, ButtonStyle.Danger);
            UITheme.StyleButton(btnRefresh, ButtonStyle.Secondary);
            UITheme.StyleButton(btnClose, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.dgvGoals = new DataGridView();
            this.btnCreate = new Button();
            this.btnEdit = new Button();
            this.btnAddToGoal = new Button();
            this.btnDelete = new Button();
            this.btnRefresh = new Button();
            this.btnClose = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Text = "Savings Goals";

            // dgvGoals
            this.dgvGoals.Location = new System.Drawing.Point(20, 60);
            this.dgvGoals.Size = new System.Drawing.Size(900, 350);
            this.dgvGoals.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvGoals.ReadOnly = true;
            this.dgvGoals.AllowUserToAddRows = false;
            this.dgvGoals.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

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

            // btnAddToGoal
            this.btnAddToGoal.Location = new System.Drawing.Point(260, 420);
            this.btnAddToGoal.Size = new System.Drawing.Size(120, 30);
            this.btnAddToGoal.Text = "Add to Goal";
            this.btnAddToGoal.Click += new EventHandler(this.btnAddToGoal_Click);

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
                this.lblTitle, this.dgvGoals, this.btnCreate, this.btnEdit,
                this.btnAddToGoal, this.btnDelete, this.btnRefresh, this.btnClose
            });
            this.Name = "SavingsGoalsForm";
            this.Text = "Savings Goals Management";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadGoals()
        {
            try
            {
                var goals = _service.GetSavingsGoalsByUserId(_userId);

                dgvGoals.Columns.Clear();
                dgvGoals.Columns.Add("SavingsGoalId", "ID");
                dgvGoals.Columns.Add("GoalName", "Goal Name");
                dgvGoals.Columns.Add("TargetAmount", "Target Amount");
                dgvGoals.Columns.Add("CurrentAmount", "Current Amount");
                dgvGoals.Columns.Add("Progress", "Progress %");
                dgvGoals.Columns.Add("TargetDate", "Target Date");
                dgvGoals.Columns.Add("IsCompleted", "Completed");

                dgvGoals.Columns["TargetAmount"].DefaultCellStyle.Format = "C2";
                dgvGoals.Columns["CurrentAmount"].DefaultCellStyle.Format = "C2";
                dgvGoals.Columns["Progress"].DefaultCellStyle.Format = "F2";
                dgvGoals.Columns["TargetDate"].DefaultCellStyle.Format = "MM/dd/yyyy";

                dgvGoals.Rows.Clear();
                foreach (var goal in goals)
                {
                    var progress = _service.GetProgressPercentage(goal);
                    dgvGoals.Rows.Add(
                        goal.SavingsGoalId,
                        goal.GoalName,
                        goal.TargetAmount,
                        goal.CurrentAmount,
                        progress,
                        goal.TargetDate?.ToString("MM/dd/yyyy") ?? "No Date",
                        goal.IsCompleted ? "Yes" : "No"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading savings goals: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                var accounts = _accountService.GetUserAccounts(_userId);
                var form = new SavingsGoalForm(_userId, null, _service, _accountService);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadGoals();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating savings goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvGoals.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a savings goal to edit.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var goalId = (int)dgvGoals.SelectedRows[0].Cells["SavingsGoalId"].Value;
                var goal = _service.GetSavingsGoalsByUserId(_userId).FirstOrDefault(g => g.SavingsGoalId == goalId);
                
                if (goal == null)
                {
                    MessageBox.Show("Savings goal not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var accounts = _accountService.GetUserAccounts(_userId);
                var form = new SavingsGoalForm(_userId, goal, _service, _accountService);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadGoals();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing savings goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddToGoal_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvGoals.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a savings goal to add to.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var goalId = (int)dgvGoals.SelectedRows[0].Cells["SavingsGoalId"].Value;
                var goal = _service.GetSavingsGoalsByUserId(_userId).FirstOrDefault(g => g.SavingsGoalId == goalId);
                
                if (goal == null)
                {
                    MessageBox.Show("Savings goal not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (goal.IsCompleted)
                {
                    MessageBox.Show("Cannot add to a completed savings goal.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var inputForm = new Form())
                {
                    inputForm.Text = "Add to Goal";
                    inputForm.Size = new System.Drawing.Size(300, 150);
                    inputForm.StartPosition = FormStartPosition.CenterParent;
                    
                    var lblPrompt = new Label { Text = "Enter amount to add:", Location = new System.Drawing.Point(20, 20), AutoSize = true };
                    var txtAmount = new TextBox { Location = new System.Drawing.Point(20, 50), Size = new System.Drawing.Size(240, 20) };
                    var btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new System.Drawing.Point(80, 80), Size = new System.Drawing.Size(75, 30) };
                    var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new System.Drawing.Point(165, 80), Size = new System.Drawing.Size(75, 30) };
                    
                    inputForm.Controls.AddRange(new Control[] { lblPrompt, txtAmount, btnOK, btnCancel });
                    inputForm.AcceptButton = btnOK;
                    inputForm.CancelButton = btnCancel;
                    
                    if (inputForm.ShowDialog() == DialogResult.OK && decimal.TryParse(txtAmount.Text, out decimal amount) && amount > 0)
                    {
                        _service.AddToSavingsGoal(goalId, amount);
                        MessageBox.Show($"Added {amount:C} to savings goal.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadGoals();
                    }
                    else if (inputForm.DialogResult == DialogResult.OK)
                    {
                        MessageBox.Show("Please enter a valid amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding to savings goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvGoals.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a savings goal to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var goalId = (int)dgvGoals.SelectedRows[0].Cells["SavingsGoalId"].Value;
                
                if (MessageBox.Show("Are you sure you want to delete this savings goal? This action cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _service.DeleteSavingsGoal(goalId);
                    MessageBox.Show("Savings goal deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGoals();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting savings goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadGoals();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

