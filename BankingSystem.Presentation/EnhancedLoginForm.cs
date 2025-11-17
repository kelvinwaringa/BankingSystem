using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class EnhancedLoginForm : Form
    {
        private UserService _userService;
        private User _currentUser;
        private Dictionary<string, int> _failedAttempts = new Dictionary<string, int>();
        private const int MAX_FAILED_ATTEMPTS = 3;
        private const int LOCKOUT_MINUTES = 5;

        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblSecurityInfo;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnSignUp;
        private CheckBox chkShowPassword;

        public EnhancedLoginForm()
        {
            _userService = new UserService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblUsername = new Label();
            this.lblPassword = new Label();
            this.lblSecurityInfo = new Label();
            this.txtUsername = new TextBox();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.btnSignUp = new Button();
            this.chkShowPassword = new CheckBox();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(80, 30);
            this.lblTitle.Size = new System.Drawing.Size(250, 26);
            this.lblTitle.Text = "Banking System Login";

            // lblUsername
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(50, 90);
            this.lblUsername.Size = new System.Drawing.Size(58, 13);
            this.lblUsername.Text = "Username:";

            // lblPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(50, 130);
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.Text = "Password:";

            // txtUsername
            this.txtUsername.Location = new System.Drawing.Point(120, 87);
            this.txtUsername.Size = new System.Drawing.Size(200, 20);
            this.txtUsername.TabIndex = 1;

            // txtPassword
            this.txtPassword.Location = new System.Drawing.Point(120, 127);
            this.txtPassword.Size = new System.Drawing.Size(200, 20);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.TabIndex = 2;

            // chkShowPassword
            this.chkShowPassword.AutoSize = true;
            this.chkShowPassword.Location = new System.Drawing.Point(120, 155);
            this.chkShowPassword.Size = new System.Drawing.Size(95, 17);
            this.chkShowPassword.Text = "Show Password";
            this.chkShowPassword.CheckedChanged += new EventHandler(this.chkShowPassword_CheckedChanged);

            // lblSecurityInfo
            this.lblSecurityInfo.AutoSize = true;
            this.lblSecurityInfo.ForeColor = System.Drawing.Color.Red;
            this.lblSecurityInfo.Location = new System.Drawing.Point(120, 180);
            this.lblSecurityInfo.Size = new System.Drawing.Size(200, 13);
            this.lblSecurityInfo.Text = "";
            this.lblSecurityInfo.Visible = false;

            // btnLogin
            this.btnLogin.Location = new System.Drawing.Point(120, 210);
            this.btnLogin.Size = new System.Drawing.Size(90, 35);
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);

            // btnSignUp
            this.btnSignUp.Location = new System.Drawing.Point(230, 210);
            this.btnSignUp.Size = new System.Drawing.Size(90, 35);
            this.btnSignUp.TabIndex = 4;
            this.btnSignUp.Text = "Sign Up";
            this.btnSignUp.UseVisualStyleBackColor = true;
            this.btnSignUp.Click += new EventHandler(this.btnSignUp_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 280);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.lblUsername, this.lblPassword, this.lblSecurityInfo,
                this.txtUsername, this.txtPassword, this.chkShowPassword,
                this.btnLogin, this.btnSignUp
            });
            this.Name = "EnhancedLoginForm";
            this.Text = "Banking System - Secure Login";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '*';
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Please enter both username and password.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check for account lockout
                if (_failedAttempts.ContainsKey(txtUsername.Text) && 
                    _failedAttempts[txtUsername.Text] >= MAX_FAILED_ATTEMPTS)
                {
                    lblSecurityInfo.Text = $"Account locked due to {MAX_FAILED_ATTEMPTS} failed attempts. Please try again later.";
                    lblSecurityInfo.Visible = true;
                    btnLogin.Enabled = false;
                    System.Threading.Thread.Sleep(2000);
                    btnLogin.Enabled = true;
                    return;
                }

                _currentUser = _userService.Login(txtUsername.Text, txtPassword.Text);
                
                if (_currentUser != null)
                {
                    // Reset failed attempts on successful login
                    if (_failedAttempts.ContainsKey(txtUsername.Text))
                    {
                        _failedAttempts.Remove(txtUsername.Text);
                    }

                    // Log successful login
                    BankingSystem.BusinessLogic.AuditService.LogAction(
                        _currentUser.UserId, 
                        "Login", 
                        "User", 
                        _currentUser.UserId,
                        $"User {_currentUser.Username} logged in successfully",
                        BankingSystem.BusinessLogic.AuditService.GetClientIpAddress()
                    );

                    MessageBox.Show($"Welcome, {_currentUser.FullName}!", "Login Successful", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Check if user is admin
                    if (_currentUser.Role == "Admin")
                    {
                        var adminPanelForm = new AdminPanelForm(_currentUser);
                        adminPanelForm.Show();
                    }
                    else
                    {
                        var dashboardForm = new EnhancedDashboardForm(_currentUser);
                        dashboardForm.Show();
                    }
                    this.Hide();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // Log failed login attempt
                BankingSystem.BusinessLogic.AuditService.LogAction(
                    null, 
                    "FailedLogin", 
                    "User", 
                    null,
                    $"Failed login attempt for username: {txtUsername.Text}",
                    BankingSystem.BusinessLogic.AuditService.GetClientIpAddress()
                );

                // Track failed attempts
                if (!_failedAttempts.ContainsKey(txtUsername.Text))
                {
                    _failedAttempts[txtUsername.Text] = 0;
                }
                _failedAttempts[txtUsername.Text]++;

                int remaining = MAX_FAILED_ATTEMPTS - _failedAttempts[txtUsername.Text];
                if (remaining > 0)
                {
                    lblSecurityInfo.Text = $"Invalid credentials. {remaining} attempt(s) remaining.";
                    lblSecurityInfo.Visible = true;
                }
                else
                {
                    lblSecurityInfo.Text = "Account locked. Too many failed attempts.";
                    lblSecurityInfo.Visible = true;
                }

                MessageBox.Show(ex.Message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            var signUpForm = new EnhancedSignUpForm();
            if (signUpForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Account created successfully! You can now login.", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

