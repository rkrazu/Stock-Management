namespace Stock_Managemnet
{
    partial class InvoiceForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtInvoice;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnSubmit;
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
            this.txtInvoice = new System.Windows.Forms.TextBox();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // txtInvoice
            //
            this.txtInvoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInvoice.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtInvoice.Location = new System.Drawing.Point(12, 12);
            this.txtInvoice.Margin = new System.Windows.Forms.Padding(12);
            this.txtInvoice.Multiline = true;
            this.txtInvoice.ReadOnly = true;
            this.txtInvoice.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInvoice.TabStop = false;
            this.txtInvoice.WordWrap = false;
            //
            // panelButtons
            //
            this.panelButtons.Controls.Add(this.btnSubmit);
            this.panelButtons.Controls.Add(this.btnPrint);
            this.panelButtons.Controls.Add(this.btnClose);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Height = 48;
            this.panelButtons.Padding = new System.Windows.Forms.Padding(0, 8, 16, 8);
            //
            // btnSubmit
            //
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.Location = new System.Drawing.Point(498, 10);
            this.btnSubmit.Size = new System.Drawing.Size(110, 28);
            this.btnSubmit.Text = "Submit Invoice";
            this.btnSubmit.Click += new System.EventHandler(this.BtnSubmit_Click);
            //
            // btnPrint
            //
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.Location = new System.Drawing.Point(614, 10);
            this.btnPrint.Size = new System.Drawing.Size(95, 28);
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(715, 10);
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            //
            // InvoiceForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(820, 520);
            this.Controls.Add(this.txtInvoice);
            this.Controls.Add(this.panelButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 420);
            this.Padding = new System.Windows.Forms.Padding(12);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
