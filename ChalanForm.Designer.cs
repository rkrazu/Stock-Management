namespace Stock_Managemnet
{
    partial class ChalanForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelPreview;
        private Stock_Managemnet.Controls.InvoicePreviewControl invoicePreview;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.FlowLayoutPanel flowButtons;
        private System.Windows.Forms.Button btnPdf;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelPreview = new System.Windows.Forms.Panel();
            this.invoicePreview = new Stock_Managemnet.Controls.InvoicePreviewControl();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPdf = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelButtons.SuspendLayout();
            this.flowButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // panelPreview
            //
            this.panelPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPreview.Padding = new System.Windows.Forms.Padding(0);
            this.panelPreview.Controls.Add(this.invoicePreview);
            //
            // invoicePreview
            //
            this.invoicePreview.Dock = System.Windows.Forms.DockStyle.Fill;
            //
            // panelButtons
            //
            this.panelButtons.Controls.Add(this.flowButtons);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Height = 50;
            this.panelButtons.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            //
            // flowButtons
            //
            this.flowButtons.AutoSize = true;
            this.flowButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowButtons.Controls.Add(this.btnClose);
            this.flowButtons.Controls.Add(this.btnPrint);
            this.flowButtons.Controls.Add(this.btnPdf);
            this.flowButtons.Dock = System.Windows.Forms.DockStyle.Right;
            this.flowButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowButtons.WrapContents = false;
            //
            // btnPdf
            //
            this.btnPdf.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.btnPdf.Size = new System.Drawing.Size(95, 30);
            this.btnPdf.Text = "Download";
            this.btnPdf.UseVisualStyleBackColor = true;
            this.btnPdf.Click += new System.EventHandler(this.BtnPdf_Click);
            //
            // btnPrint
            //
            this.btnPrint.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.btnPrint.Size = new System.Drawing.Size(90, 30);
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            //
            // btnClose
            //
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Size = new System.Drawing.Size(85, 30);
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            //
            // ChalanForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(980, 760);
            this.Controls.Add(this.panelPreview);
            this.Controls.Add(this.panelButtons);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(720, 520);
            this.Padding = new System.Windows.Forms.Padding(12);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chalan";
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.flowButtons.ResumeLayout(false);
            this.flowButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
