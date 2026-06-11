using System;
using System.Drawing;
using System.Windows.Forms;

namespace Stock_Managemnet.Controls
{
    public class PasswordFieldControl : UserControl
    {
        private const string ShowLabel = "Show";
        private const string HideLabel = "Hide";

        private readonly Panel _hostPanel;
        private readonly TextBox _textBox;
        private readonly Button _toggleButton;
        private bool _passwordVisible;

        public PasswordFieldControl()
        {
            Height = 30;
            MinimumSize = new Size(120, 30);
            BackColor = SystemColors.Control;

            _hostPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SystemColors.Window,
                BorderStyle = BorderStyle.FixedSingle
            };

            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                UseSystemPasswordChar = true,
                Font = UiStyles.DefaultFont,
                BackColor = SystemColors.Window
            };

            _toggleButton = new Button
            {
                Text = ShowLabel,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TabStop = false,
                BackColor = SystemColors.Window,
                ForeColor = Color.FromArgb(37, 99, 235),
                Font = UiStyles.DefaultFont,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _toggleButton.FlatAppearance.BorderSize = 0;
            _toggleButton.Click += ToggleButton_Click;

            _hostPanel.Controls.Add(_textBox);
            _hostPanel.Controls.Add(_toggleButton);
            Controls.Add(_hostPanel);

            _hostPanel.Resize += (s, e) => LayoutChildren();
            Resize += (s, e) => LayoutChildren();
            LayoutChildren();
        }

        public override string Text
        {
            get => _textBox.Text;
            set => _textBox.Text = value ?? string.Empty;
        }

        public void Clear()
        {
            _textBox.Clear();
            SetPasswordVisible(false);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            _textBox.Focus();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textBox.Font = Font;
            _toggleButton.Font = Font;
            LayoutChildren();
        }

        private void ToggleButton_Click(object sender, EventArgs e)
        {
            SetPasswordVisible(!_passwordVisible);
            _textBox.Focus();
            _textBox.SelectionStart = _textBox.Text.Length;
        }

        private void SetPasswordVisible(bool visible)
        {
            _passwordVisible = visible;
            _textBox.UseSystemPasswordChar = !visible;
            _toggleButton.Text = visible ? HideLabel : ShowLabel;
        }

        private void LayoutChildren()
        {
            var buttonWidth = GetToggleButtonWidth();
            var hostHeight = Math.Max(28, Height);
            _hostPanel.Height = hostHeight;

            _toggleButton.SetBounds(_hostPanel.Width - buttonWidth, 0, buttonWidth, hostHeight);
            _textBox.SetBounds(4, 0, Math.Max(0, _hostPanel.Width - buttonWidth - 4), hostHeight);
            _toggleButton.BringToFront();
        }

        private int GetToggleButtonWidth()
        {
            var showWidth = TextRenderer.MeasureText(ShowLabel, _toggleButton.Font).Width;
            var hideWidth = TextRenderer.MeasureText(HideLabel, _toggleButton.Font).Width;
            return Math.Max(showWidth, hideWidth) + 16;
        }
    }
}
