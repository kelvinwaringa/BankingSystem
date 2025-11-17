using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using BankingSystem.DataAccess;

namespace BankingSystem.Presentation
{
    public partial class AdminUserManagementForm : Form
    {
        private DataGridView dgvUsers;
        private Button btnActivate;
        private Button btnDeactivate;
        private Button btnClose;
        private Label lblTitle;

        private ToolTip _tooltip;

        public AdminUserManagementForm()
        {
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            LoadUsers();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleDataGridView(dgvUsers);
            UITheme.StyleButton(btnActivate, ButtonStyle.Success);
            UITheme.StyleButton(btnDeactivate, ButtonStyle.Warning);
            UITheme.StyleButton(btnClose, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.dgvUsers = new DataGridView();
            this.btnActivate = new Button();
            this.btnDeactivate = new Button();
            this.btnClose = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(200, 20);
            this.lblTitle.Text = "User Management";

            // dgvUsers
            this.dgvUsers.Location = new System.Drawing.Point(20, 60);
            this.dgvUsers.Size = new System.Drawing.Size(800, 400);
            this.dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsers.ReadOnly = true;
            this.dgvUsers.AllowUserToAddRows = false;
            this.dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Buttons
            this.btnActivate.Location = new System.Drawing.Point(20, 470);
            this.btnActivate.Size = new System.Drawing.Size(120, 30);
            this.btnActivate.Text = "Activate User";
            this.btnActivate.Click += new EventHandler(this.btnActivate_Click);

            this.btnDeactivate.Location = new System.Drawing.Point(150, 470);
            this.btnDeactivate.Size = new System.Drawing.Size(120, 30);
            this.btnDeactivate.Text = "Deactivate User";
            this.btnDeactivate.Click += new EventHandler(this.btnDeactivate_Click);

            this.btnClose.Location = new System.Drawing.Point(740, 470);
            this.btnClose.Size = new System.Drawing.Size(80, 30);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new EventHandler(this.btnClose_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 520);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.dgvUsers, this.btnActivate, this.btnDeactivate, this.btnClose
            });
            this.Name = "AdminUserManagementForm";
            this.Text = "User Management";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadUsers()
        {
            try
            {
                dgvUsers.Columns.Clear();
                dgvUsers.Columns.Add("UserId", "User ID");
                dgvUsers.Columns.Add("Username", "Username");
                dgvUsers.Columns.Add("Email", "Email");
                dgvUsers.Columns.Add("FullName", "Full Name");
                dgvUsers.Columns.Add("Role", "Role");
                dgvUsers.Columns.Add("IsActive", "Active");
                dgvUsers.Columns.Add("CreatedDate", "Created Date");

                dgvUsers.Rows.Clear();
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT UserId, Username, Email, FirstName, LastName, Role, IsActive, CreatedDate FROM Users", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dgvUsers.Rows.Add(
                                    (int)reader["UserId"],
                                    (string)reader["Username"],
                                    (string)reader["Email"],
                                    (string)reader["FirstName"] + " " + (string)reader["LastName"],
                                    (string)reader["Role"],
                                    (bool)reader["IsActive"],
                                    ((DateTime)reader["CreatedDate"]).ToString("MM/dd/yyyy")
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var userId = (int)dgvUsers.SelectedRows[0].Cells["UserId"].Value;
            UpdateUserStatus(userId, true);
        }

        private void btnDeactivate_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var userId = (int)dgvUsers.SelectedRows[0].Cells["UserId"].Value;
            UpdateUserStatus(userId, false);
        }

        private void UpdateUserStatus(int userId, bool isActive)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("UPDATE Users SET IsActive = @IsActive WHERE UserId = @UserId", connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@IsActive", isActive);
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show($"User {(isActive ? "activated" : "deactivated")} successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

