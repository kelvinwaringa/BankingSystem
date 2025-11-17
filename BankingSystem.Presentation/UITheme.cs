using System.Drawing;
using System.Windows.Forms;

namespace BankingSystem.Presentation
{
    public static class UITheme
    {
        // Modern Banking Color Scheme
        public static readonly Color PrimaryColor = Color.FromArgb(0, 102, 204);      // Professional Blue
        public static readonly Color SecondaryColor = Color.FromArgb(0, 153, 76);     // Success Green
        public static readonly Color DangerColor = Color.FromArgb(220, 53, 69);       // Warning Red
        public static readonly Color WarningColor = Color.FromArgb(255, 193, 7);      // Warning Yellow
        public static readonly Color BackgroundColor = Color.FromArgb(248, 249, 250); // Light Gray
        public static readonly Color CardColor = Color.White;
        public static readonly Color TextColor = Color.FromArgb(33, 37, 41);
        public static readonly Color BorderColor = Color.FromArgb(222, 226, 230);
        public static readonly Color HoverColor = Color.FromArgb(0, 122, 204);

        public static void ApplyModernTheme(Form form)
        {
            form.BackColor = BackgroundColor;
            form.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            form.ForeColor = TextColor;
        }

        public static void StyleButton(Button button, ButtonStyle style = ButtonStyle.Primary)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            button.Cursor = Cursors.Hand;
            button.Padding = new Padding(10, 5, 10, 5);

            switch (style)
            {
                case ButtonStyle.Primary:
                    button.BackColor = PrimaryColor;
                    button.ForeColor = Color.White;
                    button.FlatAppearance.MouseOverBackColor = HoverColor;
                    break;
                case ButtonStyle.Success:
                    button.BackColor = SecondaryColor;
                    button.ForeColor = Color.White;
                    button.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 133, 86);
                    break;
                case ButtonStyle.Danger:
                    button.BackColor = DangerColor;
                    button.ForeColor = Color.White;
                    button.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 35, 51);
                    break;
                case ButtonStyle.Warning:
                    button.BackColor = WarningColor;
                    button.ForeColor = TextColor;
                    button.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 203, 17);
                    break;
                case ButtonStyle.Secondary:
                    button.BackColor = Color.White;
                    button.ForeColor = TextColor;
                    button.FlatAppearance.BorderColor = BorderColor;
                    button.FlatAppearance.BorderSize = 1;
                    button.FlatAppearance.MouseOverBackColor = BackgroundColor;
                    break;
            }
        }

        public static void StyleTextBox(TextBox textBox)
        {
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = CardColor;
            textBox.ForeColor = TextColor;
            textBox.Font = new Font("Segoe UI", 9F);
            textBox.Padding = new Padding(5);
        }

        public static void StyleLabel(Label label, LabelStyle style = LabelStyle.Normal)
        {
            label.ForeColor = TextColor;
            label.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            switch (style)
            {
                case LabelStyle.Title:
                    label.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
                    label.ForeColor = PrimaryColor;
                    break;
                case LabelStyle.Subtitle:
                    label.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                    break;
                case LabelStyle.Success:
                    label.ForeColor = SecondaryColor;
                    label.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    break;
                case LabelStyle.Danger:
                    label.ForeColor = DangerColor;
                    label.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    break;
            }
        }

        public static void StyleDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = CardColor;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = BorderColor;
            dgv.DefaultCellStyle.BackColor = CardColor;
            dgv.DefaultCellStyle.ForeColor = TextColor;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgv.DefaultCellStyle.SelectionBackColor = PrimaryColor;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = BackgroundColor;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
        }

        public static void StyleListBox(ListBox listBox)
        {
            listBox.BackColor = CardColor;
            listBox.ForeColor = TextColor;
            listBox.BorderStyle = BorderStyle.FixedSingle;
            listBox.Font = new Font("Segoe UI", 9F);
        }

        public static void StylePanel(Panel panel)
        {
            panel.BackColor = CardColor;
            panel.BorderStyle = BorderStyle.FixedSingle;
        }

        public static void StyleGroupBox(GroupBox groupBox)
        {
            groupBox.ForeColor = TextColor;
            groupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        }

        public static void AddTooltip(ToolTip tooltip, Control control, string text)
        {
            tooltip.SetToolTip(control, text);
            tooltip.ToolTipTitle = "Help";
            tooltip.ToolTipIcon = ToolTipIcon.Info;
        }
    }

    public enum ButtonStyle
    {
        Primary,
        Success,
        Danger,
        Warning,
        Secondary
    }

    public enum LabelStyle
    {
        Normal,
        Title,
        Subtitle,
        Success,
        Danger
    }
}

