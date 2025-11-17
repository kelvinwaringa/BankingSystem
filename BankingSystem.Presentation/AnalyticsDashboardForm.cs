using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class AnalyticsDashboardForm : Form
    {
        private User _currentUser;
        private AnalyticsService _analyticsService;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnGenerate;
        private Button btnClose;
        private RichTextBox rtbAnalysis;
        private Label lblTitle;
        private Label lblStartDate;
        private Label lblEndDate;

        private ToolTip _tooltip;

        public AnalyticsDashboardForm(User user)
        {
            _currentUser = user;
            _analyticsService = new AnalyticsService();
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleButton(btnGenerate, ButtonStyle.Primary);
            UITheme.StyleButton(btnClose, ButtonStyle.Secondary);
            UITheme.StyleLabel(lblTitle, LabelStyle.Subtitle);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblStartDate = new Label();
            this.lblEndDate = new Label();
            this.dtpStartDate = new DateTimePicker();
            this.dtpEndDate = new DateTimePicker();
            this.btnGenerate = new Button();
            this.btnClose = new Button();
            this.rtbAnalysis = new RichTextBox();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(300, 24);
            this.lblTitle.Text = "Spending Analytics Dashboard";

            // Date pickers
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(20, 60);
            this.lblStartDate.Size = new System.Drawing.Size(80, 20);
            this.lblStartDate.Text = "Start Date:";
            this.dtpStartDate.Location = new System.Drawing.Point(100, 57);
            this.dtpStartDate.Size = new System.Drawing.Size(150, 20);
            this.dtpStartDate.Value = DateTime.Now.AddMonths(-6);

            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(270, 60);
            this.lblEndDate.Size = new System.Drawing.Size(80, 20);
            this.lblEndDate.Text = "End Date:";
            this.dtpEndDate.Location = new System.Drawing.Point(350, 57);
            this.dtpEndDate.Size = new System.Drawing.Size(150, 20);
            this.dtpEndDate.Value = DateTime.Now;

            // rtbAnalysis
            this.rtbAnalysis.Location = new System.Drawing.Point(20, 90);
            this.rtbAnalysis.Size = new System.Drawing.Size(750, 450);
            this.rtbAnalysis.Font = new System.Drawing.Font("Courier New", 9F);
            this.rtbAnalysis.ReadOnly = true;

            // Buttons
            this.btnGenerate.Location = new System.Drawing.Point(520, 550);
            this.btnGenerate.Size = new System.Drawing.Size(120, 30);
            this.btnGenerate.Text = "Generate Analysis";
            this.btnGenerate.Click += new EventHandler(this.btnGenerate_Click);

            this.btnClose.Location = new System.Drawing.Point(650, 550);
            this.btnClose.Size = new System.Drawing.Size(120, 30);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new EventHandler(this.btnClose_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.lblStartDate, this.lblEndDate,
                this.dtpStartDate, this.dtpEndDate, this.rtbAnalysis,
                this.btnGenerate, this.btnClose
            });
            this.Name = "AnalyticsDashboardForm";
            this.Text = "Spending Analytics";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var analysis = _analyticsService.GetSpendingAnalysis(
                    _currentUser.UserId, 
                    dtpStartDate.Value, 
                    dtpEndDate.Value
                );

                var report = new System.Text.StringBuilder();
                report.AppendLine("═══════════════════════════════════════════════════════════════");
                report.AppendLine("                    SPENDING ANALYSIS REPORT");
                report.AppendLine("═══════════════════════════════════════════════════════════════");
                report.AppendLine();
                report.AppendLine($"Analysis Period: {dtpStartDate.Value:MM/dd/yyyy} to {dtpEndDate.Value:MM/dd/yyyy}");
                report.AppendLine($"Generated: {DateTime.Now:MM/dd/yyyy HH:mm:ss}");
                report.AppendLine();
                report.AppendLine("═══════════════════════════════════════════════════════════════");
                report.AppendLine();
                report.AppendLine("FINANCIAL SUMMARY:");
                report.AppendLine("───────────────────────────────────────────────────────────────");
                report.AppendLine($"Total Income:        ${analysis.TotalIncome:F2}");
                report.AppendLine($"Total Expenses:     ${analysis.TotalExpenses:F2}");
                report.AppendLine($"Net Savings:        ${analysis.NetSavings:F2}");
                report.AppendLine($"Savings Rate:       {(analysis.TotalIncome > 0 ? (analysis.NetSavings / analysis.TotalIncome * 100):0):F1}%");
                report.AppendLine();
                report.AppendLine("═══════════════════════════════════════════════════════════════");
                report.AppendLine();
                report.AppendLine("CATEGORY BREAKDOWN:");
                report.AppendLine("───────────────────────────────────────────────────────────────");
                
                if (analysis.CategoryBreakdown.Count > 0)
                {
                    var sortedCategories = analysis.CategoryBreakdown.OrderByDescending(c => c.Value);
                    foreach (var category in sortedCategories)
                    {
                        var percentage = analysis.TotalExpenses > 0 
                            ? (category.Value / analysis.TotalExpenses * 100) 
                            : 0;
                        report.AppendLine($"{category.Key,-25} ${category.Value,12:F2} ({percentage,5:F1}%)");
                    }
                }
                else
                {
                    report.AppendLine("No expense data available for this period.");
                }

                report.AppendLine();
                report.AppendLine("═══════════════════════════════════════════════════════════════");
                report.AppendLine();
                report.AppendLine("MONTHLY TREND:");
                report.AppendLine("───────────────────────────────────────────────────────────────");
                
                if (analysis.MonthlyTrend.Count > 0)
                {
                    var sortedMonths = analysis.MonthlyTrend.OrderBy(m => m.Key);
                    foreach (var month in sortedMonths)
                    {
                        report.AppendLine($"{month.Key,-15} ${month.Value,12:F2}");
                    }
                }
                else
                {
                    report.AppendLine("No monthly data available for this period.");
                }

                report.AppendLine();
                report.AppendLine("═══════════════════════════════════════════════════════════════");

                rtbAnalysis.Text = report.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating analysis: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

