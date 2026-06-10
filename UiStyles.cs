using System.Drawing;
using System.Windows.Forms;

namespace Stock_Managemnet
{
    public static class UiStyles
    {
        public static readonly Font DefaultFont = new Font("Segoe UI", 11F);
        public static readonly Font TitleFont = new Font("Segoe UI", 20F, FontStyle.Bold);
        public static readonly Font SectionFont = new Font("Segoe UI", 11F, FontStyle.Bold);
        public static readonly Font MonoFont = new Font("Consolas", 11F);

        private const int SingleLineControlHeight = 30;

        public static void Apply(Form form)
        {
            form.Font = DefaultFont;
            ApplyControls(form.Controls);
        }

        private static void ApplyControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                ApplyControl(control);
                if (control.HasChildren)
                    ApplyControls(control.Controls);
            }
        }

        private static void ApplyControl(Control control)
        {
            switch (control)
            {
                case TextBox textBox:
                    textBox.Font = textBox.Font.Name == "Consolas" ? MonoFont : DefaultFont;
                    if (textBox.BorderStyle != BorderStyle.None)
                    {
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        if (!textBox.Multiline && textBox.Height < SingleLineControlHeight)
                            textBox.Height = SingleLineControlHeight;
                    }
                    break;

                case DataGridView grid:
                    grid.Font = DefaultFont;
                    grid.DefaultCellStyle.Font = DefaultFont;
                    grid.ColumnHeadersDefaultCellStyle.Font = SectionFont;
                    grid.RowTemplate.Height = 34;
                    grid.ColumnHeadersHeight = 38;
                    break;

                case Label label:
                    if (label.Font.Bold && label.Font.Size >= 15F)
                        label.Font = TitleFont;
                    else if (label.Font.Bold)
                        label.Font = SectionFont;
                    else
                        label.Font = DefaultFont;
                    break;

                case Button button:
                    button.Font = DefaultFont;
                    if (button.Height < 30)
                        button.Height = 30;
                    break;

                case ComboBox comboBox:
                    comboBox.Font = DefaultFont;
                    if (comboBox.Height < SingleLineControlHeight)
                        comboBox.Height = SingleLineControlHeight;
                    break;

                case NumericUpDown numericUpDown:
                    numericUpDown.Font = DefaultFont;
                    if (numericUpDown.Height < SingleLineControlHeight)
                        numericUpDown.Height = SingleLineControlHeight;
                    break;

                case ListBox listBox:
                    listBox.Font = DefaultFont;
                    break;

                case TabControl tabControl:
                    tabControl.Font = DefaultFont;
                    break;

                case DateTimePicker dateTimePicker:
                    dateTimePicker.Font = DefaultFont;
                    if (dateTimePicker.Height < SingleLineControlHeight)
                        dateTimePicker.Height = SingleLineControlHeight;
                    break;

                case CheckBox checkBox:
                    checkBox.Font = DefaultFont;
                    break;

                case UserControl userControl:
                    userControl.Font = DefaultFont;
                    break;

                case StatusStrip statusStrip:
                    statusStrip.Font = DefaultFont;
                    foreach (ToolStripItem item in statusStrip.Items)
                        item.Font = DefaultFont;
                    break;

                case ToolStrip toolStrip:
                    toolStrip.Font = DefaultFont;
                    break;
            }
        }
    }
}
