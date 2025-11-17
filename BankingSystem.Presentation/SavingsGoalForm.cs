using System;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class SavingsGoalForm : Form
    {
        private int _userId;
        private SavingsGoal _goal;
        private SavingsGoalService _service;
        private AccountService _accountService;
        private Label lblTitle;
        private Label lblAccount;
        private Label lblGoalName;
        private Label lblTargetAmount;
        private Label lblTargetDate;
        private Label lblDescription;
        private ComboBox cmbAccount;
        private TextBox txtGoalName;
        private TextBox txtTargetAmount;
        private DateTimePicker dtpTargetDate;
        private TextBox txtDescription;
        private CheckBox chkNoTargetDate;
        private Button btnSave;
        private Button btnCancel;

        private ToolTip _tooltip;

        public SavingsGoalForm(int userId, SavingsGoal goal, SavingsGoalService service, AccountService accountService)
        {
            _userId = userId;
            _goal = goal;
            _service = service;
            _accountService = accountService;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadAccounts();
            LoadData();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleTextBox(txtGoalName);
            UITheme.StyleTextBox(txtTargetAmount);
            UITheme.StyleTextBox(txtDescription);
            UITheme.StyleButton(btnSave, ButtonStyle.Success);
            UITheme.StyleButton(btnCancel, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblAccount = new Label();
            this.lblGoalName = new Label();
            this.lblTargetAmount = new Label();
            this.lblTargetDate = new Label();
            this.lblDescription = new Label();
            this.cmbAccount = new ComboBox();
            this.txtGoalName = new TextBox();
            this.txtTargetAmount = new TextBox();
            this.dtpTargetDate = new DateTimePicker();
            this.txtDescription = new TextBox();
            this.chkNoTargetDate = new CheckBox();
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
            this.lblTitle.Text = _goal == null ? "Create Savings Goal" : "Edit Savings Goal";

            yPos += 40;

            // lblAccount
            this.lblAccount.AutoSize = true;
            this.lblAccount.Location = new System.Drawing.Point(20, yPos);
            this.lblAccount.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblAccount.Text = "Account (Optional):";

            // cmbAccount
            this.cmbAccount.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.cmbAccount.Size = new System.Drawing.Size(250, 21);
            this.cmbAccount.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbAccount.DisplayMember = "AccountNumber";
            this.cmbAccount.ValueMember = "AccountId";

            yPos += spacing;

            // lblGoalName
            this.lblGoalName.AutoSize = true;
            this.lblGoalName.Location = new System.Drawing.Point(20, yPos);
            this.lblGoalName.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblGoalName.Text = "Goal Name:";

            // txtGoalName
            this.txtGoalName.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.txtGoalName.Size = new System.Drawing.Size(250, 20);

            yPos += spacing;

            // lblTargetAmount
            this.lblTargetAmount.AutoSize = true;
            this.lblTargetAmount.Location = new System.Drawing.Point(20, yPos);
            this.lblTargetAmount.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblTargetAmount.Text = "Target Amount:";

            // txtTargetAmount
            this.txtTargetAmount.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.txtTargetAmount.Size = new System.Drawing.Size(250, 20);

            yPos += spacing;

            // chkNoTargetDate
            this.chkNoTargetDate.AutoSize = true;
            this.chkNoTargetDate.Location = new System.Drawing.Point(20, yPos);
            this.chkNoTargetDate.Text = "No Target Date";
            this.chkNoTargetDate.CheckedChanged += new EventHandler(this.chkNoTargetDate_CheckedChanged);

            yPos += spacing;

            // lblTargetDate
            this.lblTargetDate.AutoSize = true;
            this.lblTargetDate.Location = new System.Drawing.Point(20, yPos);
            this.lblTargetDate.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblTargetDate.Text = "Target Date:";

            // dtpTargetDate
            this.dtpTargetDate.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.dtpTargetDate.Size = new System.Drawing.Size(250, 20);
            this.dtpTargetDate.Format = DateTimePickerFormat.Short;

            yPos += spacing;

            // lblDescription
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(20, yPos);
            this.lblDescription.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblDescription.Text = "Description:";

            // txtDescription
            this.txtDescription.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.txtDescription.Size = new System.Drawing.Size(250, 60);
            this.txtDescription.Multiline = true;

            yPos += 80;

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
                this.lblTitle, this.lblAccount, this.cmbAccount, this.lblGoalName, this.txtGoalName,
                this.lblTargetAmount, this.txtTargetAmount, this.chkNoTargetDate, this.lblTargetDate, this.dtpTargetDate,
                this.lblDescription, this.txtDescription, this.btnSave, this.btnCancel
            });
            this.Name = "SavingsGoalForm";
            this.Text = _goal == null ? "Create Savings Goal" : "Edit Savings Goal";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadAccounts()
        {
            try
            {
                var accounts = _accountService.GetUserAccounts(_userId);
                cmbAccount.DataSource = accounts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accounts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            if (_goal != null)
            {
                if (_goal.AccountId.HasValue)
                {
                    cmbAccount.SelectedValue = _goal.AccountId.Value;
                }
                txtGoalName.Text = _goal.GoalName;
                txtTargetAmount.Text = _goal.TargetAmount.ToString("F2");
                if (_goal.TargetDate.HasValue)
                {
                    dtpTargetDate.Value = _goal.TargetDate.Value;
                    chkNoTargetDate.Checked = false;
                }
                else
                {
                    chkNoTargetDate.Checked = true;
                }
                txtDescription.Text = _goal.Description ?? "";
            }
            else
            {
                chkNoTargetDate.Checked = true;
            }
        }

        private void chkNoTargetDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpTargetDate.Enabled = !chkNoTargetDate.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtGoalName.Text))
                {
                    MessageBox.Show("Goal name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtTargetAmount.Text, out decimal targetAmount) || targetAmount <= 0)
                {
                    MessageBox.Show("Please enter a valid target amount greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int? accountId = null;
                if (cmbAccount.SelectedValue != null && cmbAccount.SelectedValue is int)
                {
                    accountId = (int)cmbAccount.SelectedValue;
                }

                var targetDate = chkNoTargetDate.Checked ? (DateTime?)null : dtpTargetDate.Value;

                if (_goal == null)
                {
                    _service.CreateSavingsGoal(_userId, accountId, txtGoalName.Text, targetAmount, targetDate, 
                        string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text);
                    MessageBox.Show("Savings goal created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _goal.AccountId = accountId;
                    _goal.GoalName = txtGoalName.Text;
                    _goal.TargetAmount = targetAmount;
                    _goal.TargetDate = targetDate;
                    _goal.Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text;
                    
                    _service.UpdateSavingsGoal(_goal);
                    MessageBox.Show("Savings goal updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving savings goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

