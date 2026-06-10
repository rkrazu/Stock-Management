using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Stock_Managemnet.Controls
{
    public class SearchableSelectControl<T> : UserControl where T : class
    {
        private const int DropDownHeight = 160;
        private const int InputHeight = 30;
        private const int WM_LBUTTONDOWN = 0x0201;

        private readonly Panel _inputPanel;
        private readonly TextBox _input;
        private readonly Button _btnToggle;
        private readonly Button _btnClear;
        private readonly Panel _dropDownPanel;
        private readonly ListBox _list;
        private readonly ClickOutsideFilter _clickOutsideFilter;

        private Func<string, IEnumerable<T>> _search;
        private Func<T, string, bool> _matches;
        private T _selectedItem;
        private bool _suppressTextChange;
        private bool _dropDownOpen;
        private Form _hostForm;

        private static SearchableSelectControl<T> _openInstance;

        public event EventHandler SelectedItemChanged;

        public Func<T, string> FormatItem { get; set; }

        public T SelectedItem => _selectedItem;

        public SearchableSelectControl()
        {
            Height = InputHeight;
            MinimumSize = new Size(200, InputHeight);

            _inputPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = InputHeight,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = SystemColors.Window
            };

            _btnClear = new Button
            {
                Dock = DockStyle.Right,
                Width = 30,
                Text = "×",
                FlatStyle = FlatStyle.Flat,
                TabStop = false,
                Visible = false,
                Cursor = Cursors.Hand
            };
            _btnClear.FlatAppearance.BorderSize = 0;

            _btnToggle = new Button
            {
                Dock = DockStyle.Right,
                Width = 30,
                Text = "▾",
                FlatStyle = FlatStyle.Flat,
                TabStop = false,
                Cursor = Cursors.Hand
            };
            _btnToggle.FlatAppearance.BorderSize = 0;

            _input = new TextBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None
            };

            _list = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                IntegralHeight = false
            };
            _list.FormattingEnabled = true;
            _list.Format += List_Format;

            _dropDownPanel = new Panel
            {
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = SystemColors.Window
            };
            _dropDownPanel.Controls.Add(_list);

            _inputPanel.Controls.Add(_input);
            _inputPanel.Controls.Add(_btnClear);
            _inputPanel.Controls.Add(_btnToggle);
            Controls.Add(_inputPanel);

            _clickOutsideFilter = new ClickOutsideFilter(this);

            _input.TextChanged += Input_TextChanged;
            _input.KeyDown += Input_KeyDown;
            _input.KeyPress += Input_KeyPress;
            _input.Click += Input_Click;
            _inputPanel.MouseDown += InputPanel_MouseDown;
            _btnToggle.Click += (s, e) => ToggleDropDown();
            _btnClear.Click += (s, e) => ClearSelection();
            _list.Click += List_Click;
            _list.KeyDown += List_KeyDown;
            Resize += (s, e) =>
            {
                if (_dropDownOpen)
                    PositionDropDown();
            };
        }

        public void Bind(Func<string, IEnumerable<T>> search, Func<T, string, bool> matches = null)
        {
            _search = search;
            _matches = matches;
        }

        public void SetSelectedItem(T item)
        {
            _selectedItem = item;
            _suppressTextChange = true;
            _input.Text = item == null ? string.Empty : Format(item);
            _suppressTextChange = false;
            _btnClear.Visible = item != null;
        }

        public bool TrySelectHighlightedItem()
        {
            if (!_dropDownOpen || _list.Items.Count == 0)
                return false;

            var item = (_list.SelectedItem ?? _list.Items[0]) as T;
            if (item == null)
                return false;

            SelectItem(item);
            return true;
        }

        public bool IsInputFocused => _input.Focused;

        public bool IsDropDownOpen => _dropDownOpen;

        public void HideDropDownIfOpen()
        {
            HideDropDown();
        }

        public void ClearSelection()
        {
            _selectedItem = null;
            _suppressTextChange = true;
            _input.Text = string.Empty;
            _suppressTextChange = false;
            _btnClear.Visible = false;
            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Input_Click(object sender, EventArgs e)
        {
            OpenDropDownForInputInteraction();
        }

        private void InputPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            _input.Focus();
            OpenDropDownForInputInteraction();
        }

        private void OpenDropDownForInputInteraction()
        {
            if (_selectedItem != null)
            {
                _suppressTextChange = true;
                _input.SelectAll();
                _suppressTextChange = false;
            }

            ShowDropDown();
        }

        private void Input_TextChanged(object sender, EventArgs e)
        {
            if (_suppressTextChange)
                return;

            if (_selectedItem != null && _input.Text != Format(_selectedItem))
            {
                var stillMatches = _matches != null && _matches(_selectedItem, _input.Text);
                if (!stillMatches)
                {
                    _selectedItem = null;
                    _btnClear.Visible = false;
                    SelectedItemChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            if (!_input.Focused)
                return;

            if (!_dropDownOpen)
                ShowDropDown();
            else
                RefreshList();
        }

        private void Input_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                e.Handled = true;
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                ShowDropDown();
                _list.Focus();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                HideDropDown();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (TrySelectHighlightedItem())
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void List_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && _list.SelectedItem is T item)
            {
                SelectItem(item);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                HideDropDown();
                _input.Focus();
                e.Handled = true;
            }
        }

        private void List_Click(object sender, EventArgs e)
        {
            if (_list.SelectedItem is T item)
                SelectItem(item);
        }

        private void ToggleDropDown()
        {
            if (_dropDownOpen)
                HideDropDown();
            else
                ShowDropDown();
        }

        private void ShowDropDown()
        {
            if (_search == null)
                return;

            var host = FindForm();
            if (host == null)
                return;

            if (_selectedItem != null && string.IsNullOrEmpty(_input.Text))
            {
                _suppressTextChange = true;
                _input.Text = Format(_selectedItem);
                _input.SelectAll();
                _suppressTextChange = false;
            }

            if (_dropDownPanel.Parent != host)
                host.Controls.Add(_dropDownPanel);

            RefreshList();
            PositionDropDown();
            _dropDownPanel.Visible = true;
            _dropDownPanel.BringToFront();

            if (!_dropDownOpen)
            {
                if (_openInstance != null && _openInstance != this)
                    _openInstance.HideDropDown();

                _openInstance = this;
                _hostForm = host;
                _hostForm.Move += Host_MoveOrResize;
                _hostForm.Resize += Host_MoveOrResize;
                Application.AddMessageFilter(_clickOutsideFilter);
                _dropDownOpen = true;
            }
        }

        internal void HideDropDown()
        {
            if (!_dropDownOpen)
                return;

            _dropDownPanel.Visible = false;
            _dropDownOpen = false;

            if (_openInstance == this)
                _openInstance = null;

            Application.RemoveMessageFilter(_clickOutsideFilter);

            if (_hostForm != null)
            {
                _hostForm.Move -= Host_MoveOrResize;
                _hostForm.Resize -= Host_MoveOrResize;
                _hostForm = null;
            }

            if (_selectedItem != null)
            {
                _suppressTextChange = true;
                _input.Text = Format(_selectedItem);
                _suppressTextChange = false;
            }
        }

        private void Host_MoveOrResize(object sender, EventArgs e)
        {
            if (_dropDownOpen)
                PositionDropDown();
        }

        private void PositionDropDown()
        {
            var host = _dropDownPanel.Parent as Form ?? FindForm();
            if (host == null)
                return;

            var screenPoint = _inputPanel.PointToScreen(new Point(0, _inputPanel.Height));
            _dropDownPanel.Location = host.PointToClient(screenPoint);
            _dropDownPanel.Size = new Size(_inputPanel.Width, DropDownHeight);
        }

        private void RefreshList()
        {
            var term = _input.Text ?? string.Empty;
            if (_selectedItem != null &&
                string.Equals(term, Format(_selectedItem), StringComparison.Ordinal) &&
                _input.SelectionLength > 0)
            {
                term = string.Empty;
            }

            _list.BeginUpdate();
            _list.Items.Clear();
            if (_search != null)
            {
                foreach (var item in _search(term))
                    _list.Items.Add(item);
            }

            if (_list.Items.Count > 0)
                _list.SelectedIndex = 0;
            _list.EndUpdate();
        }

        private void List_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is T item)
                e.Value = Format(item);
        }

        private void SelectItem(T item)
        {
            if (item == null)
                return;

            _selectedItem = item;
            _suppressTextChange = true;
            _input.Text = Format(item);
            _input.SelectionStart = _input.Text.Length;
            _suppressTextChange = false;
            _btnClear.Visible = true;
            HideDropDown();
            _input.Focus();
            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }

        private string Format(T item)
        {
            if (item == null)
                return string.Empty;

            return FormatItem != null ? FormatItem(item) : item.ToString();
        }

        internal bool IsRelatedControl(Control control)
        {
            while (control != null)
            {
                if (control == this || control == _dropDownPanel)
                    return true;
                control = control.Parent;
            }

            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                HideDropDown();
                _dropDownPanel.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _input.Font = Font;
            _list.Font = Font;
            _btnToggle.Font = Font;
            _btnClear.Font = Font;
        }

        private sealed class ClickOutsideFilter : IMessageFilter
        {
            private readonly SearchableSelectControl<T> _owner;

            public ClickOutsideFilter(SearchableSelectControl<T> owner)
            {
                _owner = owner;
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (!_owner._dropDownOpen || m.Msg != WM_LBUTTONDOWN)
                    return false;

                var clicked = Control.FromHandle(m.HWnd);
                if (clicked != null && _owner.IsRelatedControl(clicked))
                    return false;

                _owner.HideDropDown();
                return false;
            }
        }
    }
}
