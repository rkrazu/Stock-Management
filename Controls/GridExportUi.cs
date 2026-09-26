using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Stock_Managemnet.Services;

namespace Stock_Managemnet.Controls
{
    public static class GridExportUi
    {
        private const string ExportEnabledTag = "GridExportEnabled";

        public static void Enable(DataGridView grid, string exportBaseName)
        {
            if (grid == null || grid.Parent == null)
                return;

            if (Equals(grid.Tag, ExportEnabledTag))
                return;

            grid.Tag = ExportEnabledTag;

            var parent = grid.Parent;
            var dock = grid.Dock;
            var margin = grid.Margin;
            var location = grid.Location;
            var size = grid.Size;
            var anchor = grid.Anchor;
            var childIndex = parent.Controls.GetChildIndex(grid);

            parent.Controls.Remove(grid);

            var host = new Panel
            {
                Dock = dock,
                Margin = margin,
                Location = location,
                Size = size,
                Anchor = anchor
            };

            grid.Dock = DockStyle.Fill;
            grid.Margin = Padding.Empty;
            host.Controls.Add(grid);

            var menuButton = new Button
            {
                Text = string.Empty,
                Width = 34,
                Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                AccessibleName = "More actions"
            };
            menuButton.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            menuButton.Paint += PaintMenuDots;

            var menu = new ContextMenuStrip
            {
                Font = new Font("Segoe UI", 10F),
                ShowImageMargin = false
            };
            menu.Items.Add("Export to Excel", null, (s, e) => DataGridViewExcelExporter.Export(grid, exportBaseName, host.FindForm()));
            menu.Items.Add("Download as PDF", null, (s, e) => DataGridViewDocumentExporter.ExportPdf(grid, exportBaseName, host.FindForm()));
            menu.Items.Add("Print", null, (s, e) => DataGridViewDocumentExporter.Print(grid, exportBaseName, host.FindForm()));

            host.Controls.Add(menuButton);
            menuButton.BringToFront();
            host.Resize += (s, e) => PositionMenuButton(host, menuButton);
            menuButton.Click += (s, e) => menu.Show(menuButton, new Point(menuButton.Width, menuButton.Height), ToolStripDropDownDirection.BelowLeft);
            host.Disposed += (s, e) => menu.Dispose();

            parent.Controls.Add(host);
            parent.Controls.SetChildIndex(host, childIndex);
            PositionMenuButton(host, menuButton);
        }

        private static void PaintMenuDots(object sender, PaintEventArgs e)
        {
            var button = (Control)sender;
            var x = (button.Width - 4) / 2;
            var y = (button.Height / 2) - 7;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var brush = new SolidBrush(Color.FromArgb(55, 65, 81)))
            {
                e.Graphics.FillEllipse(brush, x, y, 4, 4);
                e.Graphics.FillEllipse(brush, x, y + 6, 4, 4);
                e.Graphics.FillEllipse(brush, x, y + 12, 4, 4);
            }
        }

        private static void PositionMenuButton(Control host, Button menuButton)
        {
            menuButton.Location = new Point(
                System.Math.Max(8, host.ClientSize.Width - menuButton.Width - 10),
                8);
        }
    }
}
