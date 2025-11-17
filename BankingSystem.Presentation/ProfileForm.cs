using System;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class ProfileForm : Form
    {
        private User _user;
        private UserService _userService;
        private Label lblTitle;
        private Label lblFirstName;
        private Label lblLastName;
        private Label lblEmail;
        private Label lblPhoneNumber;
        private Label lblAddress;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtEmail;
        private TextBox txtPhoneNumber;
        private TextBox txtAddress;
        private Button btnSave;
        private Button btnCancel;

        private ToolTip _tooltip;

        public ProfileForm(User user, UserService userService)
        {
            _user = user;
            _userService = userService;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadUserData();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleTextBox(txtFirstName);
            UITheme.StyleTextBox(txtLastName);
            UITheme.StyleTextBox(txtEmail);
            UITheme.StyleTextBox(txtPhoneNumber);
            UITheme.StyleTextBox(txtAddress);
            UITheme.StyleButton(btnSave, ButtonStyle.Success);
            UITheme.StyleButton(btnCancel, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblFirstName = new Label();
            this.lblLastName = new Label();
            this.lblEmail = new Label();
            this.lblPhoneNumber = new Label();
            this.lblAddress = new Label();
            this.txtFirstName = new TextBox();
            this.txtLastName = new TextBox();
            this.txtEmail = new TextBox();
            this.txtPhoneNumber = new TextBox();
            this.txtAddress = new TextBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(200, 20);
            this.lblTitle.Text = "Update Profile";

            // Labels
            this.lblFirstName.Location = new System.Drawing.Point(20, 60);
            this.lblFirstName.Size = new System.Drawing.Size(100, 20);
            this.lblFirstName.Text = "First Name:";
            this.lblLastName.Location = new System.Drawing.Point(20, 90);
            this.lblLastName.Size = new System.Drawing.Size(100, 20);
            this.lblLastName.Text = "Last Name:";
            this.lblEmail.Location = new System.Drawing.Point(20, 120);
            this.lblEmail.Size = new System.Drawing.Size(100, 20);
            this.lblEmail.Text = "Email:";
            this.lblPhoneNumber.Location = new System.Drawing.Point(20, 150);
            this.lblPhoneNumber.Size = new System.Drawing.Size(100, 20);
            this.lblPhoneNumber.Text = "Phone Number:";
            this.lblAddress.Location = new System.Drawing.Point(20, 180);
            this.lblAddress.Size = new System.Drawing.Size(100, 20);
            this.lblAddress.Text = "Address:";

            // TextBoxes
            this.txtFirstName.Location = new System.Drawing.Point(130, 57);
            this.txtFirstName.Size = new System.Drawing.Size(200, 20);
            this.txtLastName.Location = new System.Drawing.Point(130, 87);
            this.txtLastName.Size = new System.Drawing.Size(200, 20);
            this.txtEmail.Location = new System.Drawing.Point(130, 117);
            this.txtEmail.ReadOnly = true;
            this.txtEmail.Size = new System.Drawing.Size(200, 20);
            this.txtPhoneNumber.Location = new System.Drawing.Point(130, 147);
            this.txtPhoneNumber.Size = new System.Drawing.Size(200, 20);
            this.txtAddress.Location = new System.Drawing.Point(130, 177);
            this.txtAddress.Size = new System.Drawing.Size(200, 40);
            this.txtAddress.Multiline = true;

            // Buttons
            this.btnSave.Location = new System.Drawing.Point(130, 230);
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            this.btnCancel.Location = new System.Drawing.Point(240, 230);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 290);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.lblFirstName, this.lblLastName, this.lblEmail,
                this.lblPhoneNumber, this.lblAddress,
                this.txtFirstName, this.txtLastName, this.txtEmail, this.txtPhoneNumber, this.txtAddress,
                this.btnSave, this.btnCancel
            });
            this.Name = "ProfileForm";
            this.Text = "Update Profile";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadUserData()
        {
            txtFirstName.Text = _user.FirstName;
            txtLastName.Text = _user.LastName;
            txtEmail.Text = _user.Email;
            txtPhoneNumber.Text = _user.PhoneNumber ?? "";
            txtAddress.Text = _user.Address ?? "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    MessageBox.Show("First name and last name are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _user.FirstName = txtFirstName.Text;
                _user.LastName = txtLastName.Text;
                _user.PhoneNumber = string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ? null : txtPhoneNumber.Text;
                _user.Address = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text;

                if (_userService.UpdateUserProfile(_user))
                {
                    MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

