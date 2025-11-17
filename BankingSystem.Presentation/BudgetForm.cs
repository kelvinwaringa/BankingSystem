using System;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class BudgetForm : Form
    {
        private int _userId;
        private Budget _budget;
        private BudgetService _service;
        private Label lblTitle;
        private Label lblCategory;
        private Label lblBudgetAmount;
        private Label lblPeriod;
        private Label lblStartDate;
        private Label lblEndDate;
        private TextBox txtCategory;
        private TextBox txtBudgetAmount;
        private ComboBox cmbPeriod;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private CheckBox chkNoEndDate;
        private Button btnSave;
        private Button btnCancel;

        private ToolTip _tooltip;

        public BudgetForm(int userId, Budget budget, BudgetService service)
        {
            _userId = userId;
            _budget = budget;
            _service = service;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadData();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleTextBox(txtCategory);
            UITheme.StyleTextBox(txtBudgetAmount);
            UITheme.StyleButton(btnSave, ButtonStyle.Success);
            UITheme.StyleButton(btnCancel, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblCategory = new Label();
            this.lblBudgetAmount = new Label();
            this.lblPeriod = new Label();
            this.lblStartDate = new Label();
            this.lblEndDate = new Label();
            this.txtCategory = new TextBox();
            this.txtBudgetAmount = new TextBox();
            this.cmbPeriod = new ComboBox();
            this.dtpStartDate = new DateTimePicker();
            this.dtpEndDate = new DateTimePicker();
            this.chkNoEndDate = new CheckBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            int yPos = 20;
            int labelWidth = 150;
            int controlX = 160;
            int spacing = 30;

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, yPos);
            this.lblTitle.Text = _budget == null ? "Create Budget" : "Edit Budget";

            yPos += 40;

            // lblCategory
            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new System.Drawing.Point(20, yPos);
            this.lblCategory.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblCategory.Text = "Category:";

            // txtCategory
            this.txtCategory.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.txtCategory.Size = new System.Drawing.Size(250, 20);

            yPos += spacing;

            // lblBudgetAmount
            this.lblBudgetAmount.AutoSize = true;
            this.lblBudgetAmount.Location = new System.Drawing.Point(20, yPos);
            this.lblBudgetAmount.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblBudgetAmount.Text = "Budget Amount:";

            // txtBudgetAmount
            this.txtBudgetAmount.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.txtBudgetAmount.Size = new System.Drawing.Size(250, 20);

            yPos += spacing;

            // lblPeriod
            this.lblPeriod.AutoSize = true;
            this.lblPeriod.Location = new System.Drawing.Point(20, yPos);
            this.lblPeriod.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblPeriod.Text = "Period:";

            // cmbPeriod
            this.cmbPeriod.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.cmbPeriod.Size = new System.Drawing.Size(250, 21);
            this.cmbPeriod.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbPeriod.Items.AddRange(new object[] { "Monthly", "Weekly", "Yearly" });

            yPos += spacing;

            // lblStartDate
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(20, yPos);
            this.lblStartDate.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblStartDate.Text = "Start Date:";

            // dtpStartDate
            this.dtpStartDate.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.dtpStartDate.Size = new System.Drawing.Size(250, 20);
            this.dtpStartDate.Format = DateTimePickerFormat.Short;

            yPos += spacing;

            // chkNoEndDate
            this.chkNoEndDate.AutoSize = true;
            this.chkNoEndDate.Location = new System.Drawing.Point(20, yPos);
            this.chkNoEndDate.Text = "No End Date";
            this.chkNoEndDate.CheckedChanged += new EventHandler(this.chkNoEndDate_CheckedChanged);

            yPos += spacing;

            // lblEndDate
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(20, yPos);
            this.lblEndDate.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblEndDate.Text = "End Date:";

            // dtpEndDate
            this.dtpEndDate.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.dtpEndDate.Size = new System.Drawing.Size(250, 20);
            this.dtpEndDate.Format = DateTimePickerFormat.Short;

            yPos += spacing + 20;

            // btnSave
            this.btnSave.Location = new System.Drawing.Point(controlX, yPos);
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(controlX + 110, yPos);
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, yPos + 60);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.lblCategory, this.txtCategory, this.lblBudgetAmount, this.txtBudgetAmount,
                this.lblPeriod, this.cmbPeriod, this.lblStartDate, this.dtpStartDate,
                this.chkNoEndDate, this.lblEndDate, this.dtpEndDate,
                this.btnSave, this.btnCancel
            });
            this.Name = "BudgetForm";
            this.Text = _budget == null ? "Create Budget" : "Edit Budget";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadData()
        {
            if (_budget != null)
            {
                txtCategory.Text = _budget.Category;
                txtBudgetAmount.Text = _budget.BudgetAmount.ToString("F2");
                cmbPeriod.Text = _budget.Period;
                dtpStartDate.Value = _budget.StartDate;
                if (_budget.EndDate.HasValue)
                {
                    dtpEndDate.Value = _budget.EndDate.Value;
                    chkNoEndDate.Checked = false;
                }
                else
                {
                    chkNoEndDate.Checked = true;
                }
            }
            else
            {
                dtpStartDate.Value = DateTime.Now;
                chkNoEndDate.Checked = true;
            }
        }

        private void chkNoEndDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpEndDate.Enabled = !chkNoEndDate.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCategory.Text))
                {
                    MessageBox.Show("Category is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtBudgetAmount.Text, out decimal budgetAmount) || budgetAmount <= 0)
                {
                    MessageBox.Show("Please enter a valid budget amount greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(cmbPeriod.Text))
                {
                    MessageBox.Show("Please select a period.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var startDate = dtpStartDate.Value;
                var endDate = chkNoEndDate.Checked ? (DateTime?)null : dtpEndDate.Value;

                if (_budget == null)
                {
                    _service.CreateBudget(_userId, txtCategory.Text, budgetAmount, cmbPeriod.Text, startDate, endDate);
                    MessageBox.Show("Budget created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _budget.Category = txtCategory.Text;
                    _budget.BudgetAmount = budgetAmount;
                    _budget.Period = cmbPeriod.Text;
                    _budget.StartDate = startDate;
                    _budget.EndDate = endDate;
                    
                    _service.UpdateBudget(_budget);
                    MessageBox.Show("Budget updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving budget: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

