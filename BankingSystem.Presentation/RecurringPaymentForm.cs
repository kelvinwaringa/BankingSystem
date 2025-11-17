using System;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class RecurringPaymentForm : Form
    {
        private int _userId;
        private RecurringPayment _payment;
        private RecurringPaymentService _service;
        private AccountService _accountService;
        private Label lblTitle;
        private Label lblAccount;
        private Label lblRecipientName;
        private Label lblRecipientAccount;
        private Label lblAmount;
        private Label lblFrequency;
        private Label lblNextPaymentDate;
        private Label lblEndDate;
        private Label lblDescription;
        private ComboBox cmbAccount;
        private TextBox txtRecipientName;
        private TextBox txtRecipientAccount;
        private TextBox txtAmount;
        private ComboBox cmbFrequency;
        private DateTimePicker dtpNextPaymentDate;
        private DateTimePicker dtpEndDate;
        private TextBox txtDescription;
        private CheckBox chkNoEndDate;
        private Button btnSave;
        private Button btnCancel;

        private ToolTip _tooltip;

        public RecurringPaymentForm(int userId, RecurringPayment payment, RecurringPaymentService service, AccountService accountService)
        {
            _userId = userId;
            _payment = payment;
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
            UITheme.StyleTextBox(txtRecipientName);
            UITheme.StyleTextBox(txtRecipientAccount);
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
            this.lblRecipientName = new Label();
            this.lblRecipientAccount = new Label();
            this.lblAmount = new Label();
            this.lblFrequency = new Label();
            this.lblNextPaymentDate = new Label();
            this.lblEndDate = new Label();
            this.lblDescription = new Label();
            this.cmbAccount = new ComboBox();
            this.txtRecipientName = new TextBox();
            this.txtRecipientAccount = new TextBox();
            this.txtAmount = new TextBox();
            this.cmbFrequency = new ComboBox();
            this.dtpNextPaymentDate = new DateTimePicker();
            this.dtpEndDate = new DateTimePicker();
            this.txtDescription = new TextBox();
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
            this.lblTitle.Text = _payment == null ? "Create Recurring Payment" : "Edit Recurring Payment";

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

            // lblRecipientName
            this.lblRecipientName.AutoSize = true;
            this.lblRecipientName.Location = new System.Drawing.Point(20, yPos);
            this.lblRecipientName.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblRecipientName.Text = "Recipient Name:";

            // txtRecipientName
            this.txtRecipientName.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.txtRecipientName.Size = new System.Drawing.Size(250, 20);

            yPos += spacing;

            // lblRecipientAccount
            this.lblRecipientAccount.AutoSize = true;
            this.lblRecipientAccount.Location = new System.Drawing.Point(20, yPos);
            this.lblRecipientAccount.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblRecipientAccount.Text = "Recipient Account:";

            // txtRecipientAccount
            this.txtRecipientAccount.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.txtRecipientAccount.Size = new System.Drawing.Size(250, 20);

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

            // lblFrequency
            this.lblFrequency.AutoSize = true;
            this.lblFrequency.Location = new System.Drawing.Point(20, yPos);
            this.lblFrequency.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblFrequency.Text = "Frequency:";

            // cmbFrequency
            this.cmbFrequency.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.cmbFrequency.Size = new System.Drawing.Size(250, 21);
            this.cmbFrequency.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFrequency.Items.AddRange(new object[] { "Daily", "Weekly", "Monthly", "Yearly" });

            yPos += spacing;

            // lblNextPaymentDate
            this.lblNextPaymentDate.AutoSize = true;
            this.lblNextPaymentDate.Location = new System.Drawing.Point(20, yPos);
            this.lblNextPaymentDate.Size = new System.Drawing.Size(labelWidth, 20);
            this.lblNextPaymentDate.Text = "Next Payment Date:";

            // dtpNextPaymentDate
            this.dtpNextPaymentDate.Location = new System.Drawing.Point(controlX, yPos - 3);
            this.dtpNextPaymentDate.Size = new System.Drawing.Size(250, 20);
            this.dtpNextPaymentDate.Format = DateTimePickerFormat.Short;

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
                this.lblTitle, this.lblAccount, this.cmbAccount, this.lblRecipientName, this.txtRecipientName,
                this.lblRecipientAccount, this.txtRecipientAccount, this.lblAmount, this.txtAmount,
                this.lblFrequency, this.cmbFrequency, this.lblNextPaymentDate, this.dtpNextPaymentDate,
                this.chkNoEndDate, this.lblEndDate, this.dtpEndDate, this.lblDescription, this.txtDescription,
                this.btnSave, this.btnCancel
            });
            this.Name = "RecurringPaymentForm";
            this.Text = _payment == null ? "Create Recurring Payment" : "Edit Recurring Payment";
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
            if (_payment != null)
            {
                cmbAccount.SelectedValue = _payment.AccountId;
                txtRecipientName.Text = _payment.RecipientName;
                txtRecipientAccount.Text = _payment.RecipientAccountNumber ?? "";
                txtAmount.Text = _payment.Amount.ToString("F2");
                cmbFrequency.Text = _payment.Frequency;
                dtpNextPaymentDate.Value = _payment.NextPaymentDate;
                if (_payment.EndDate.HasValue)
                {
                    dtpEndDate.Value = _payment.EndDate.Value;
                    chkNoEndDate.Checked = false;
                }
                else
                {
                    chkNoEndDate.Checked = true;
                }
                txtDescription.Text = _payment.Description ?? "";
            }
            else
            {
                dtpNextPaymentDate.Value = DateTime.Now.AddDays(1);
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
                if (cmbAccount.SelectedValue == null)
                {
                    MessageBox.Show("Please select an account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtRecipientName.Text))
                {
                    MessageBox.Show("Recipient name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Please enter a valid amount greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(cmbFrequency.Text))
                {
                    MessageBox.Show("Please select a frequency.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var accountId = (int)cmbAccount.SelectedValue;
                var nextPaymentDate = dtpNextPaymentDate.Value;
                var endDate = chkNoEndDate.Checked ? (DateTime?)null : dtpEndDate.Value;

                if (_payment == null)
                {
                    _service.CreateRecurringPayment(
                        accountId,
                        txtRecipientName.Text,
                        string.IsNullOrWhiteSpace(txtRecipientAccount.Text) ? null : txtRecipientAccount.Text,
                        amount,
                        cmbFrequency.Text,
                        nextPaymentDate,
                        endDate,
                        string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text
                    );
                    MessageBox.Show("Recurring payment created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _payment.AccountId = accountId;
                    _payment.RecipientName = txtRecipientName.Text;
                    _payment.RecipientAccountNumber = string.IsNullOrWhiteSpace(txtRecipientAccount.Text) ? null : txtRecipientAccount.Text;
                    _payment.Amount = amount;
                    _payment.Frequency = cmbFrequency.Text;
                    _payment.NextPaymentDate = nextPaymentDate;
                    _payment.EndDate = endDate;
                    _payment.Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text;
                    
                    _service.UpdateRecurringPayment(_payment);
                    MessageBox.Show("Recurring payment updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving recurring payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

