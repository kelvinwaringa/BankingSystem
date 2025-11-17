using System;
using System.Drawing;
using System.Windows.Forms;

namespace BankingSystem.Presentation
{
    public partial class LoadingForm : Form
    {
        private Label lblMessage;
        private ProgressBar progressBar;

        public LoadingForm(string message = "Loading...")
        {
            InitializeComponent(message);
        }

        private void InitializeComponent(string message)
        {
            this.SuspendLayout();

            // Form Properties
            this.Text = "Loading";
            this.Size = new Size(300, 120);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.BackColor = UITheme.BackgroundColor;

            // Message Label
            lblMessage = new Label();
            lblMessage.Text = message;
            lblMessage.Font = new Font("Segoe UI", 10F);
            lblMessage.ForeColor = UITheme.TextColor;
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(20, 20);
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;

            // Progress Bar
            progressBar = new ProgressBar();
            progressBar.Location = new Point(20, 50);
            progressBar.Size = new Size(260, 23);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 30;

            // Center label
            lblMessage.Left = (this.ClientSize.Width - lblMessage.Width) / 2;

            this.Controls.AddRange(new Control[] { lblMessage, progressBar });
            this.ResumeLayout(false);
        }

        public void UpdateMessage(string message)
        {
            if (lblMessage.InvokeRequired)
            {
                lblMessage.Invoke(new Action<string>(UpdateMessage), message);
            }
            else
            {
                lblMessage.Text = message;
                lblMessage.Left = (this.ClientSize.Width - lblMessage.Width) / 2;
            }
        }
    }
}

