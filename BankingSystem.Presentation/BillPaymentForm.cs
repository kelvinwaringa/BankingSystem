using System;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class BillPaymentForm : Form
    {
        private int _userId;
        private BillPayment _billPayment;
        private BillPaymentService _service;
        private AccountService _accountService;
        private Label lblTitle;
        private Label lblAccount;
        private Label lblPayeeName;
        private Label lblPayeeAccount;
        private Label lblAmount;
        private Label lblDueDate;
        private Label lblDescription;
        private ComboBox cmbAccount;
        private TextBox txtPayeeName;
        private TextBox txtPayeeAccount;
        private TextBox txtAmount;
        private DateTimePicker dtpDueDate;
        private TextBox txtDescription;
        private Button btnSave;
        private Button btnCancel;

        private ToolTip _tooltip;

        public BillPaymentForm(int userId, BillPayment billPayment, BillPaymentService service, AccountService accountService)
        {
            _userId = userId;
            _billPayment = billPayment;
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
            UITheme.StyleTextBox(txtPayeeName);
            UITheme.StyleTextBox(txtPayeeAccount);
            UITheme.StyleTextBox(txtAmount);
            UITheme.StyleTextBox(txtDescription);
            UITheme.StyleButton(btnSave, ButtonStyle.Success);
            UITheme.StyleButton(btnCancel, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblAccount = new Label();
            this.lblPayeeName = new Label();
            this.lblPayeeAccount = new Label();
            this.lblAmount = new Label();
            this.lblDueDate = new Label();
            this.lblDescription = new Label();
            this.cmbAccount = new ComboBox();
            this.txtPayeeName = new TextBox();
            this.txtPayeeAccount = new TextBox();
            this.txtAmount = new TextBox();
            this.dtpDueDate = new DateTimePicker();
            this.txtDescription = new TextBox();
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
            this.lblTitle.Text = _billPayment == null ? "Create Bill Payment" : "Edit Bill Payment";

            yPos += 40;

            // lblAccount
            this.lblAccount.AutoSize = true;
            this.lblAccount.Location = new System.Drawing.Point(20, yPos);
            this.lblAccount.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblAccount.Text = "Account:";

            // cmbAccount
            this.cmbAccount.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.cmbAccount.Size = new System.Drawing.Size(250, 21);
            this.cmbAccount.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbAccount.DisplayMember = "AccountNumber";
            this.cmbAccount.ValueMember = "AccountId";

            yPos += spacing;

            // lblPayeeName
            this.lblPayeeName.AutoSize = true;
            this.lblPayeeName.Location = new System.Drawing.Point(20, yPos);
            this.lblPayeeName.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblPayeeName.Text = "Payee Name:";

            // txtPayeeName
            this.txtPayeeName.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.txtPayeeName.Size = new System.Drawing.Size(250, 20);

            yPos += spacing;

            // lblPayeeAccount
            this.lblPayeeAccount.AutoSize = true;
            this.lblPayeeAccount.Location = new System.Drawing.Point(20, yPos);
            this.lblPayeeAccount.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblPayeeAccount.Text = "Payee Account:";

            // txtPayeeAccount
            this.txtPayeeAccount.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.txtPayeeAccount.Size = new System.Drawing.Size(250, 20);

            yPos += spacing;

            // lblAmount
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(20, yPos);
            this.lblAmount.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblAmount.Text = "Amount:";

            // txtAmount
            this.txtAmount.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.txtAmount.Size = new System.Drawing.Size(250, 20);

            yPos += spacing;

            // lblDueDate
            this.lblDueDate.AutoSize = true;
            this.lblDueDate.Location = new System.Drawing.Point(20, yPos);
            this.lblDueDate.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblDueDate.Text = "Due Date:";

            // dtpDueDate
            this.dtpDueDate.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.dtpDueDate.Size = new System.Drawing.Size(250, 20);
            this.dtpDueDate.Format = DateTimePickerFormat.Short;

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
                this.lblTitle, this.lblAccount, this.cmbAccount, this.lblPayeeName, this.txtPayeeName,
                this.lblPayeeAccount, this.txtPayeeAccount, this.lblAmount, this.txtAmount,
                this.lblDueDate, this.dtpDueDate, this.lblDescription, this.txtDescription,
                this.btnSave, this.btnCancel
            });
            this.Name = "BillPaymentForm";
            this.Text = _billPayment == null ? "Create Bill Payment" : "Edit Bill Payment";
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
            if (_billPayment != null)
            {
                cmbAccount.SelectedValue = _billPayment.AccountId;
                txtPayeeName.Text = _billPayment.PayeeName;
                txtPayeeAccount.Text = _billPayment.PayeeAccountNumber ?? "";
                txtAmount.Text = _billPayment.Amount.ToString("F2");
                dtpDueDate.Value = _billPayment.DueDate;
                txtDescription.Text = _billPayment.Description ?? "";
            }
            else
            {
                dtpDueDate.Value = DateTime.Now.AddDays(30);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbAccount.SelectedValue == null)
                {
                    MessageBox.Show("Please select an account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPayeeName.Text))
                {
                    MessageBox.Show("Payee name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Please enter a valid amount greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var accountId = (int)cmbAccount.SelectedValue;
                var dueDate = dtpDueDate.Value;

                if (_billPayment == null)
                {
                    _service.CreateBillPayment(
                        accountId,
                        txtPayeeName.Text,
                        string.IsNullOrWhiteSpace(txtPayeeAccount.Text) ? null : txtPayeeAccount.Text,
                        amount,
                        dueDate,
                        string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text
                    );
                    MessageBox.Show("Bill payment created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _billPayment.AccountId = accountId;
                    _billPayment.PayeeName = txtPayeeName.Text;
                    _billPayment.PayeeAccountNumber = string.IsNullOrWhiteSpace(txtPayeeAccount.Text) ? null : txtPayeeAccount.Text;
                    _billPayment.Amount = amount;
                    _billPayment.DueDate = dueDate;
                    _billPayment.Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text;
                    
                    _service.UpdateBillPayment(_billPayment);
                    MessageBox.Show("Bill payment updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving bill payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

