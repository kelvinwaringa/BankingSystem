using System;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;

namespace BankingSystem.Presentation
{
    public partial class EnhancedSignUpForm : Form
    {
        private UserService _userService;
        private Label lblTitle;
        private Label lblUsername;
        private Label lblEmail;
        private Label lblPassword;
        private Label lblConfirmPassword;
        private Label lblFirstName;
        private Label lblLastName;
        private Label lblPhoneNumber;
        private Label lblPasswordStrength;
        private TextBox txtUsername;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtPhoneNumber;
        private Button btnSignUp;
        private Button btnCancel;
        private CheckBox chkShowPassword;
        private ProgressBar pbPasswordStrength;

        private ToolTip _tooltip;

        public EnhancedSignUpForm()
        {
            _userService = new UserService();
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleTextBox(txtUsername);
            UITheme.StyleTextBox(txtEmail);
            UITheme.StyleTextBox(txtPassword);
            UITheme.StyleTextBox(txtConfirmPassword);
            UITheme.StyleTextBox(txtFirstName);
            UITheme.StyleTextBox(txtLastName);
            UITheme.StyleTextBox(txtPhoneNumber);
            UITheme.StyleButton(btnSignUp, ButtonStyle.Success);
            UITheme.StyleButton(btnCancel, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblUsername = new Label();
            this.lblEmail = new Label();
            this.lblPassword = new Label();
            this.lblConfirmPassword = new Label();
            this.lblFirstName = new Label();
            this.lblLastName = new Label();
            this.lblPhoneNumber = new Label();
            this.lblPasswordStrength = new Label();
            this.txtUsername = new TextBox();
            this.txtEmail = new TextBox();
            this.txtPassword = new TextBox();
            this.txtConfirmPassword = new TextBox();
            this.txtFirstName = new TextBox();
            this.txtLastName = new TextBox();
            this.txtPhoneNumber = new TextBox();
            this.btnSignUp = new Button();
            this.btnCancel = new Button();
            this.chkShowPassword = new CheckBox();
            this.pbPasswordStrength = new ProgressBar();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(100, 20);
            this.lblTitle.Size = new System.Drawing.Size(180, 24);
            this.lblTitle.Text = "Create New Account";

            // Labels
            int yPos = 60;
            this.lblUsername.Location = new System.Drawing.Point(30, yPos);
            this.lblUsername.Size = new System.Drawing.Size(100, 20);
            this.lblUsername.Text = "Username:";
            yPos += 30;
            this.lblEmail.Location = new System.Drawing.Point(30, yPos);
            this.lblEmail.Size = new System.Drawing.Size(100, 20);
            this.lblEmail.Text = "Email:";
            yPos += 30;
            this.lblPassword.Location = new System.Drawing.Point(30, yPos);
            this.lblPassword.Size = new System.Drawing.Size(100, 20);
            this.lblPassword.Text = "Password:";
            yPos += 30;
            this.lblConfirmPassword.Location = new System.Drawing.Point(30, yPos);
            this.lblConfirmPassword.Size = new System.Drawing.Size(100, 20);
            this.lblConfirmPassword.Text = "Confirm Password:";
            yPos += 30;
            this.lblFirstName.Location = new System.Drawing.Point(30, yPos);
            this.lblFirstName.Size = new System.Drawing.Size(100, 20);
            this.lblFirstName.Text = "First Name:";
            yPos += 30;
            this.lblLastName.Location = new System.Drawing.Point(30, yPos);
            this.lblLastName.Size = new System.Drawing.Size(100, 20);
            this.lblLastName.Text = "Last Name:";
            yPos += 30;
            this.lblPhoneNumber.Location = new System.Drawing.Point(30, yPos);
            this.lblPhoneNumber.Size = new System.Drawing.Size(100, 20);
            this.lblPhoneNumber.Text = "Phone Number:";

            // TextBoxes
            yPos = 57;
            this.txtUsername.Location = new System.Drawing.Point(140, yPos);
            this.txtUsername.Size = new System.Drawing.Size(200, 20);
            yPos += 30;
            this.txtEmail.Location = new System.Drawing.Point(140, yPos);
            this.txtEmail.Size = new System.Drawing.Size(200, 20);
            yPos += 30;
            this.txtPassword.Location = new System.Drawing.Point(140, yPos);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(200, 20);
            this.txtPassword.TextChanged += new EventHandler(this.txtPassword_TextChanged);
            yPos += 30;
            this.txtConfirmPassword.Location = new System.Drawing.Point(140, yPos);
            this.txtConfirmPassword.PasswordChar = '*';
            this.txtConfirmPassword.Size = new System.Drawing.Size(200, 20);
            yPos += 30;
            this.txtFirstName.Location = new System.Drawing.Point(140, yPos);
            this.txtFirstName.Size = new System.Drawing.Size(200, 20);
            yPos += 30;
            this.txtLastName.Location = new System.Drawing.Point(140, yPos);
            this.txtLastName.Size = new System.Drawing.Size(200, 20);
            yPos += 30;
            this.txtPhoneNumber.Location = new System.Drawing.Point(140, yPos);
            this.txtPhoneNumber.Size = new System.Drawing.Size(200, 20);

            // Password strength
            this.lblPasswordStrength.AutoSize = true;
            this.lblPasswordStrength.Location = new System.Drawing.Point(140, 147);
            this.lblPasswordStrength.Size = new System.Drawing.Size(200, 15);
            this.lblPasswordStrength.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.pbPasswordStrength.Location = new System.Drawing.Point(140, 165);
            this.pbPasswordStrength.Size = new System.Drawing.Size(200, 10);
            this.pbPasswordStrength.Maximum = 5;

            // chkShowPassword
            this.chkShowPassword.AutoSize = true;
            this.chkShowPassword.Location = new System.Drawing.Point(140, 180);
            this.chkShowPassword.Size = new System.Drawing.Size(95, 17);
            this.chkShowPassword.Text = "Show Password";
            this.chkShowPassword.CheckedChanged += new EventHandler(this.chkShowPassword_CheckedChanged);

            // Buttons
            this.btnSignUp.Location = new System.Drawing.Point(140, 310);
            this.btnSignUp.Size = new System.Drawing.Size(90, 30);
            this.btnSignUp.Text = "Sign Up";
            this.btnSignUp.Click += new EventHandler(this.btnSignUp_Click);
            this.btnCancel.Location = new System.Drawing.Point(250, 310);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 370);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.lblUsername, this.lblEmail, this.lblPassword, this.lblConfirmPassword,
                this.lblFirstName, this.lblLastName, this.lblPhoneNumber, this.lblPasswordStrength,
                this.txtUsername, this.txtEmail, this.txtPassword, this.txtConfirmPassword,
                this.txtFirstName, this.txtLastName, this.txtPhoneNumber,
                this.pbPasswordStrength, this.chkShowPassword,
                this.btnSignUp, this.btnCancel
            });
            this.Name = "EnhancedSignUpForm";
            this.Text = "Sign Up";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            var validation = SecurityEnhancements.ValidatePasswordStrength(txtPassword.Text);
            int strength = 0;
            
            if (txtPassword.Text.Length >= 8) strength++;
            if (System.Text.RegularExpressions.Regex.IsMatch(txtPassword.Text, @"[A-Z]")) strength++;
            if (System.Text.RegularExpressions.Regex.IsMatch(txtPassword.Text, @"[a-z]")) strength++;
            if (System.Text.RegularExpressions.Regex.IsMatch(txtPassword.Text, @"[0-9]")) strength++;
            if (System.Text.RegularExpressions.Regex.IsMatch(txtPassword.Text, @"[!@#$%^&*(),.?\"":{}|<>]")) strength++;

            pbPasswordStrength.Value = strength;
            
            if (strength <= 2)
            {
                lblPasswordStrength.Text = "Weak";
                lblPasswordStrength.ForeColor = System.Drawing.Color.Red;
            }
            else if (strength <= 3)
            {
                lblPasswordStrength.Text = "Medium";
                lblPasswordStrength.ForeColor = System.Drawing.Color.Orange;
            }
            else
            {
                lblPasswordStrength.Text = "Strong";
                lblPasswordStrength.ForeColor = System.Drawing.Color.Green;
            }
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '*';
            txtConfirmPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '*';
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Text) || string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                    string.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    MessageBox.Show("Please fill in all required fields.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Password strength validation
                var passwordValidation = SecurityEnhancements.ValidatePasswordStrength(txtPassword.Text);
                if (!passwordValidation.IsValid)
                {
                    MessageBox.Show("Password does not meet requirements:\n" + 
                        string.Join("\n", passwordValidation.Errors), "Password Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("Passwords do not match.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _userService.RegisterUser(
                    txtUsername.Text,
                    txtEmail.Text,
                    txtPassword.Text,
                    txtFirstName.Text,
                    txtLastName.Text,
                    string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ? null : txtPhoneNumber.Text
                );

                MessageBox.Show("Account created successfully! You can now login.", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Sign Up Failed", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

