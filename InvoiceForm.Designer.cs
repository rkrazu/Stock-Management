namespace Stock_Managemnet
{
    partial class InvoiceForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelPreview;
        private Stock_Managemnet.Controls.InvoicePreviewControl invoicePreview;
        private System.Windows.Forms.Panel panelPayment;
        private System.Windows.Forms.Label lblPaymentTotalCaption;
        private System.Windows.Forms.Label lblPaymentTotal;
        private System.Windows.Forms.Label lblAmountPaidCaption;
        private System.Windows.Forms.NumericUpDown numAmountPaid;
        private System.Windows.Forms.Label lblBalanceDueCaption;
        private System.Windows.Forms.Label lblBalanceDue;
        private System.Windows.Forms.Label lblCashAccountCaption;
        private System.Windows.Forms.ComboBox cmbCashAccount;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.FlowLayoutPanel flowButtons;
        private System.Windows.Forms.Button btnSubmit;
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
            this.panelPayment = new System.Windows.Forms.Panel();
            this.lblPaymentTotalCaption = new System.Windows.Forms.Label();
            this.lblPaymentTotal = new System.Windows.Forms.Label();
            this.lblAmountPaidCaption = new System.Windows.Forms.Label();
            this.numAmountPaid = new System.Windows.Forms.NumericUpDown();
            this.lblBalanceDueCaption = new System.Windows.Forms.Label();
            this.lblBalanceDue = new System.Windows.Forms.Label();
            this.lblCashAccountCaption = new System.Windows.Forms.Label();
            this.cmbCashAccount = new System.Windows.Forms.ComboBox();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnPdf = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelButtons.SuspendLayout();
            this.flowButtons.SuspendLayout();
            this.panelPayment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAmountPaid)).BeginInit();
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
            // panelPayment
            //
            this.panelPayment.Controls.Add(this.lblPaymentTotalCaption);
            this.panelPayment.Controls.Add(this.lblPaymentTotal);
            this.panelPayment.Controls.Add(this.lblAmountPaidCaption);
            this.panelPayment.Controls.Add(this.numAmountPaid);
            this.panelPayment.Controls.Add(this.lblBalanceDueCaption);
            this.panelPayment.Controls.Add(this.lblBalanceDue);
            this.panelPayment.Controls.Add(this.lblCashAccountCaption);
            this.panelPayment.Controls.Add(this.cmbCashAccount);
            this.panelPayment.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelPayment.Height = 56;
            this.panelPayment.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.panelPayment.Visible = false;
            //
            // lblPaymentTotalCaption
            //
            this.lblPaymentTotalCaption.AutoSize = true;
            this.lblPaymentTotalCaption.Location = new System.Drawing.Point(12, 18);
            this.lblPaymentTotalCaption.Text = "Total:";
            //
            // lblPaymentTotal
            //
            this.lblPaymentTotal.AutoSize = true;
            this.lblPaymentTotal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblPaymentTotal.Location = new System.Drawing.Point(58, 17);
            //
            // lblAmountPaidCaption
            //
            this.lblAmountPaidCaption.AutoSize = true;
            this.lblAmountPaidCaption.Location = new System.Drawing.Point(170, 18);
            this.lblAmountPaidCaption.Text = "Paid now:";
            //
            // numAmountPaid
            //
            this.numAmountPaid.DecimalPlaces = 2;
            this.numAmountPaid.Location = new System.Drawing.Point(240, 15);
            this.numAmountPaid.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            this.numAmountPaid.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numAmountPaid.Size = new System.Drawing.Size(120, 27);
            //
            // lblBalanceDueCaption
            //
            this.lblBalanceDueCaption.AutoSize = true;
            this.lblBalanceDueCaption.Location = new System.Drawing.Point(380, 18);
            this.lblBalanceDueCaption.Text = "Due:";
            //
            // lblBalanceDue
            //
            this.lblBalanceDue.AutoSize = true;
            this.lblBalanceDue.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblBalanceDue.ForeColor = System.Drawing.Color.FromArgb(153, 27, 27);
            this.lblBalanceDue.Location = new System.Drawing.Point(420, 17);
            //
            // lblCashAccountCaption
            //
            this.lblCashAccountCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCashAccountCaption.AutoSize = true;
            this.lblCashAccountCaption.Location = new System.Drawing.Point(560, 18);
            this.lblCashAccountCaption.Text = "Receive in:";
            //
            // cmbCashAccount
            //
            this.cmbCashAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCashAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCashAccount.Enabled = false;
            this.cmbCashAccount.Location = new System.Drawing.Point(640, 15);
            this.cmbCashAccount.Size = new System.Drawing.Size(140, 27);
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
            this.flowButtons.Controls.Add(this.btnSubmit);
            this.flowButtons.Dock = System.Windows.Forms.DockStyle.Right;
            this.flowButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowButtons.WrapContents = false;
            //
            // btnSubmit
            //
            this.btnSubmit.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.btnSubmit.Size = new System.Drawing.Size(120, 30);
            this.btnSubmit.Text = "Submit Invoice";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.BtnSubmit_Click);
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
            this.btnClose.Text = "Cancel";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            //
            // InvoiceForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(980, 760);
            this.Controls.Add(this.panelPreview);
            this.Controls.Add(this.panelPayment);
            this.Controls.Add(this.panelButtons);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(720, 520);
            this.Padding = new System.Windows.Forms.Padding(12);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Invoice";
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.flowButtons.ResumeLayout(false);
            this.flowButtons.PerformLayout();
            this.panelPayment.ResumeLayout(false);
            this.panelPayment.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAmountPaid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
