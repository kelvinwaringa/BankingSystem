using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BankingSystem.BusinessLogic;
using BankingSystem.Models;

namespace BankingSystem.Presentation
{
    public partial class AccountStatementForm : Form
    {
        private Account _account;
        private TransactionService _transactionService;
        private RichTextBox rtbStatement;
        private Button btnExport;
        private Button btnPrint;
        private Button btnClose;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Label lblTitle;
        private Label lblStartDate;
        private Label lblEndDate;

        private ToolTip _tooltip;

        public AccountStatementForm(Account account, TransactionService transactionService)
        {
            _account = account;
            _transactionService = transactionService;
            _tooltip = new ToolTip();
            InitializeComponent();
            ApplyTheme();
            GenerateStatement();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModernTheme(this);
            UITheme.StyleButton(btnExport, ButtonStyle.Primary);
            UITheme.StyleButton(btnPrint, ButtonStyle.Secondary);
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
            this.rtbStatement = new RichTextBox();
            this.btnExport = new Button();
            this.btnPrint = new Button();
            this.btnClose = new Button();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(300, 24);
            this.lblTitle.Text = "Account Statement";

            // Date pickers
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(20, 60);
            this.lblStartDate.Size = new System.Drawing.Size(80, 20);
            this.lblStartDate.Text = "Start Date:";
            this.dtpStartDate.Location = new System.Drawing.Point(100, 57);
            this.dtpStartDate.Size = new System.Drawing.Size(150, 20);
            this.dtpStartDate.Value = DateTime.Now.AddMonths(-1);

            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(270, 60);
            this.lblEndDate.Size = new System.Drawing.Size(80, 20);
            this.lblEndDate.Text = "End Date:";
            this.dtpEndDate.Location = new System.Drawing.Point(350, 57);
            this.dtpEndDate.Size = new System.Drawing.Size(150, 20);
            this.dtpEndDate.Value = DateTime.Now;

            // rtbStatement
            this.rtbStatement.Location = new System.Drawing.Point(20, 90);
            this.rtbStatement.Size = new System.Drawing.Size(750, 400);
            this.rtbStatement.Font = new System.Drawing.Font("Courier New", 9F);
            this.rtbStatement.ReadOnly = true;

            // Buttons
            this.btnExport.Location = new System.Drawing.Point(520, 500);
            this.btnExport.Size = new System.Drawing.Size(80, 30);
            this.btnExport.Text = "Export";
            this.btnExport.Click += new EventHandler(this.btnExport_Click);

            this.btnPrint.Location = new System.Drawing.Point(610, 500);
            this.btnPrint.Size = new System.Drawing.Size(80, 30);
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new EventHandler(this.btnPrint_Click);

            this.btnClose.Location = new System.Drawing.Point(690, 500);
            this.btnClose.Size = new System.Drawing.Size(80, 30);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new EventHandler(this.btnClose_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 550);
            this.Controls.AddRange(new Control[] {
                this.lblTitle, this.lblStartDate, this.lblEndDate,
                this.dtpStartDate, this.dtpEndDate, this.rtbStatement,
                this.btnExport, this.btnPrint, this.btnClose
            });
            this.Name = "AccountStatementForm";
            this.Text = "Account Statement";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void GenerateStatement()
        {
            try
            {
                var transactions = _transactionService.GetTransactionHistory(
                    _account.AccountId, 
                    dtpStartDate.Value, 
                    dtpEndDate.Value
                );

                var statement = new StringBuilder();
                statement.AppendLine("═══════════════════════════════════════════════════════════════");
                statement.AppendLine("                    BANKING SYSTEM");
                statement.AppendLine("                  ACCOUNT STATEMENT");
                statement.AppendLine("═══════════════════════════════════════════════════════════════");
                statement.AppendLine();
                statement.AppendLine($"Account Number: {_account.AccountNumber}");
                statement.AppendLine($"Account Type: {_account.AccountType.TypeName}");
                statement.AppendLine($"Statement Period: {dtpStartDate.Value:MM/dd/yyyy} to {dtpEndDate.Value:MM/dd/yyyy}");
                statement.AppendLine($"Generated: {DateTime.Now:MM/dd/yyyy HH:mm:ss}");
                statement.AppendLine();
                statement.AppendLine("═══════════════════════════════════════════════════════════════");
                statement.AppendLine();
                statement.AppendLine($"Opening Balance: ${_account.Balance - transactions.Sum(t => t.Amount):F2}");
                statement.AppendLine();
                statement.AppendLine("TRANSACTIONS:");
                statement.AppendLine("───────────────────────────────────────────────────────────────");
                statement.AppendLine(String.Format("{0,-12} {1,-15} {2,15} {3,15} {4}", 
                    "Date", "Type", "Amount", "Balance", "Description"));
                statement.AppendLine("───────────────────────────────────────────────────────────────");

                decimal runningBalance = _account.Balance - transactions.Sum(t => t.Amount);
                foreach (var transaction in transactions.OrderBy(t => t.TransactionDate))
                {
                    if (transaction.TransactionType.TypeName == "Deposit" || 
                        (transaction.TransactionType.TypeName == "Transfer" && transaction.RelatedAccountId == null))
                    {
                        runningBalance += transaction.Amount;
                    }
                    else
                    {
                        runningBalance -= transaction.Amount;
                    }

                    statement.AppendLine(String.Format("{0,-12} {1,-15} {2,15:C2} {3,15:C2} {4}",
                        transaction.TransactionDate.ToString("MM/dd/yyyy"),
                        transaction.TransactionType.TypeName,
                        transaction.Amount,
                        runningBalance,
                        transaction.Description ?? ""));
                }

                statement.AppendLine("───────────────────────────────────────────────────────────────");
                statement.AppendLine();
                statement.AppendLine($"Closing Balance: ${_account.Balance:F2}");
                statement.AppendLine();
                statement.AppendLine($"Total Transactions: {transactions.Count}");
                statement.AppendLine($"Total Deposits: ${transactions.Where(t => t.TransactionType.TypeName == "Deposit").Sum(t => t.Amount):F2}");
                statement.AppendLine($"Total Withdrawals: ${transactions.Where(t => t.TransactionType.TypeName == "Withdrawal").Sum(t => t.Amount):F2}");
                statement.AppendLine();
                statement.AppendLine("═══════════════════════════════════════════════════════════════");
                statement.AppendLine("This is an official statement from Banking System.");
                statement.AppendLine("Please retain this statement for your records.");

                rtbStatement.Text = statement.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating statement: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                    saveDialog.FileName = $"Statement_{_account.AccountNumber}_{DateTime.Now:yyyyMMdd}.txt";
                    
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveDialog.FileName, rtbStatement.Text);
                        MessageBox.Show("Statement exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting statement: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                using (var printDialog = new PrintDialog())
                {
                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        var printDocument = new PrintDocument();
                        printDocument.PrintPage += (s, args) =>
                        {
                            args.Graphics.DrawString(rtbStatement.Text, rtbStatement.Font, 
                                Brushes.Black, args.MarginBounds);
                        };
                        printDocument.Print();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing statement: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

