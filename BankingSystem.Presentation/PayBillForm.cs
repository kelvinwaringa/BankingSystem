using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class PayBillForm : Form
    {
        private BillPayment _billPayment;
        private List<Account> _accounts;
        private BillPaymentService _service;
        private Label lblTitle;
        private Label lblBillInfo;
        private Label lblAccount;
        private ComboBox cmbAccount;
        private Button btnPay;
        private Button btnCancel;

        private ToolTip _tooltip;

        public PayBillForm(BillPayment billPayment, List<Account> accounts, BillPaymentService service)
        {
            _billPayment = billPayment;
            _accounts = accounts;
            _service = service;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadAccounts();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleButton(btnPay, ButtonStyle.Success);
            UITheme.StyleButton(btnCancel, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblBillInfo = new Label();
            this.lblAccount = new Label();
            this.cmbAccount = new ComboBox();
            this.btnPay = new Button();
            this.btnCancel = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Text = "Pay Bill";

            // lblBillInfo
            this.lblBillInfo.AutoSize = true;
            this.lblBillInfo.Location = new System.Drawing.Point(20, 60);
            this.lblBillInfo.Size = new System.Drawing.Size(400, 60);
            this.lblBillInfo.Text = $"Payee: {_billPayment.PayeeName}\nAmount: {_billPayment.Amount:C}\nDue Date: {_billPayment.DueDate:MM/dd/yyyy}";

            // lblAccount
            this.lblAccount.AutoSize = true;
            this.lblAccount.Location = new System.Drawing.Point(20, 140);
            this.lblAccount.Size = new System.Drawing.Size(100, 20);
            this.lblAccount.Text = "Pay From Account:";

            // cmbAccount
            this.cmbAccount.Location = new System.Drawing.Point(130, 137);
            this.cmbAccount.Size = new System.Drawing.Size(250, 21);
            this.cmbAccount.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbAccount.DisplayMember = "AccountNumber";
            this.cmbAccount.ValueMember = "AccountId";

            // btnPay
            this.btnPay.Location = new System.Drawing.Point(130, 180);
            this.btnPay.Size = new System.Drawing.Size(100, 30);
            this.btnPay.Text = "Pay Bill";
            this.btnPay.Click += new EventHandler(this.btnPay_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(240, 180);
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 230);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.lblBillInfo, this.lblAccount, this.cmbAccount,
                this.btnPay, this.btnCancel
            });
            this.Name = "PayBillForm";
            this.Text = "Pay Bill";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadAccounts()
        {
            cmbAccount.DataSource = _accounts;
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbAccount.SelectedValue == null)
                {
                    MessageBox.Show("Please select an account to pay from.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var accountId = (int)cmbAccount.SelectedValue;
                
                if (MessageBox.Show($"Are you sure you want to pay {_billPayment.Amount:C} to {_billPayment.PayeeName}?", "Confirm Payment", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _service.PayBill(_billPayment.BillPaymentId, accountId);
                    MessageBox.Show("Bill paid successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error paying bill: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

