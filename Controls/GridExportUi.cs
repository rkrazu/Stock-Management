using System.Drawing;
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

            var exportButton = new Button
            {
                Text = "Export",
                Width = 82,
                Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Standard,
                UseVisualStyleBackColor = true
            };

            host.Controls.Add(exportButton);
            exportButton.BringToFront();
            host.Resize += (s, e) => PositionExportButton(host, exportButton);
            exportButton.Click += (s, e) => DataGridViewExcelExporter.Export(grid, exportBaseName, host.FindForm());

            parent.Controls.Add(host);
            parent.Controls.SetChildIndex(host, childIndex);
            PositionExportButton(host, exportButton);
        }

        private static void PositionExportButton(Control host, Button exportButton)
        {
            exportButton.Location = new Point(
                System.Math.Max(8, host.ClientSize.Width - exportButton.Width - 10),
                10);
        }
    }
}
