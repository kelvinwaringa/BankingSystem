using System;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class CreateAccountForm : Form
    {
        private int _userId;
        private AccountService _accountService;
        private Label lblTitle;
        private Label lblAccountType;
        private Label lblInitialBalance;
        private ComboBox cmbAccountType;
        private TextBox txtInitialBalance;
        private Button btnCreate;
        private Button btnCancel;

        private ToolTip _tooltip;

        public CreateAccountForm(int userId, AccountService accountService)
        {
            _userId = userId;
            _accountService = accountService;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadAccountTypes();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleTextBox(txtInitialBalance);
            UITheme.StyleButton(btnCreate, ButtonStyle.Success);
            UITheme.StyleButton(btnCancel, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblAccountType = new Label();
            this.lblInitialBalance = new Label();
            this.cmbAccountType = new ComboBox();
            this.txtInitialBalance = new TextBox();
            this.btnCreate = new Button();
            this.btnCancel = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(200, 20);
            this.lblTitle.Text = "Create New Account";

            // lblAccountType
            this.lblAccountType.AutoSize = true;
            this.lblAccountType.Location = new System.Drawing.Point(20, 60);
            this.lblAccountType.Size = new System.Drawing.Size(100, 20);
            this.lblAccountType.Text = "Account Type:";

            // lblInitialBalance
            this.lblInitialBalance.AutoSize = true;
            this.lblInitialBalance.Location = new System.Drawing.Point(20, 100);
            this.lblInitialBalance.Size = new System.Drawing.Size(100, 20);
            this.lblInitialBalance.Text = "Initial Balance:";

            // cmbAccountType
            this.cmbAccountType.Location = new System.Drawing.Point(130, 57);
            this.cmbAccountType.Size = new System.Drawing.Size(200, 21);
            this.cmbAccountType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbAccountType.DisplayMember = "TypeName";
            this.cmbAccountType.ValueMember = "AccountTypeId";

            // txtInitialBalance
            this.txtInitialBalance.Location = new System.Drawing.Point(130, 97);
            this.txtInitialBalance.Size = new System.Drawing.Size(200, 20);
            this.txtInitialBalance.Text = "0.00";

            // btnCreate
            this.btnCreate.Location = new System.Drawing.Point(130, 140);
            this.btnCreate.Size = new System.Drawing.Size(90, 30);
            this.btnCreate.Text = "Create";
            this.btnCreate.Click += new EventHandler(this.btnCreate_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(240, 140);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 200);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.lblAccountType, this.lblInitialBalance,
                this.cmbAccountType, this.txtInitialBalance, this.btnCreate, this.btnCancel
            });
            this.Name = "CreateAccountForm";
            this.Text = "Create Account";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadAccountTypes()
        {
            try
            {
                var accountTypes = _accountService.GetAllAccountTypes();
                cmbAccountType.Items.Clear();
                foreach (var accountType in accountTypes)
                {
                    cmbAccountType.Items.Add(accountType);
                }
                if (cmbAccountType.Items.Count > 0)
                    cmbAccountType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading account types: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbAccountType.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select an account type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtInitialBalance.Text, out decimal initialBalance) || initialBalance < 0)
                {
                    MessageBox.Show("Please enter a valid initial balance (>= 0).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedAccountType = (AccountType)cmbAccountType.SelectedItem;
                var account = _accountService.CreateAccount(_userId, selectedAccountType.AccountTypeId, initialBalance);

                MessageBox.Show($"Account created successfully!\nAccount Number: {account.AccountNumber}\nBalance: ${account.Balance:F2}", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Account Creation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

