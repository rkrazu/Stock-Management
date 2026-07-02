namespace Stock_Managemnet
{
    partial class MultiStockOutForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblCustomer;
        private Controls.CustomerSelectControl customerSelect;
        private System.Windows.Forms.Label lblCart;
        private System.Windows.Forms.Panel panelCart;
        private System.Windows.Forms.FlowLayoutPanel flowCart;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblCustomer = new System.Windows.Forms.Label();
            this.customerSelect = new Controls.CustomerSelectControl();
            this.lblCart = new System.Windows.Forms.Label();
            this.panelCart = new System.Windows.Forms.Panel();
            this.flowCart = new System.Windows.Forms.FlowLayoutPanel();
            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnContinue = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panelCart.SuspendLayout();
            this.SuspendLayout();
            //
            // lblCustomer
            //
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(20, 15);
            this.lblCustomer.Text = "Customer (required):";
            //
            // customerSelect
            //
            this.customerSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.customerSelect.Location = new System.Drawing.Point(20, 35);
            this.customerSelect.Size = new System.Drawing.Size(640, 30);
            //
            // lblCart
            //
            this.lblCart.AutoSize = true;
            this.lblCart.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblCart.Location = new System.Drawing.Point(20, 78);
            this.lblCart.Text = "Cart";
            //
            // panelCart
            //
            this.panelCart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCart.AutoScroll = true;
            this.panelCart.BackColor = System.Drawing.Color.White;
            this.panelCart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCart.Controls.Add(this.flowCart);
            this.panelCart.Location = new System.Drawing.Point(20, 100);
            this.panelCart.Size = new System.Drawing.Size(640, 200);
            //
            // flowCart
            //
            this.flowCart.AutoSize = true;
            this.flowCart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowCart.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowCart.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowCart.Location = new System.Drawing.Point(0, 0);
            this.flowCart.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.flowCart.WrapContents = false;
            //
            // lblNotes
            //
            this.lblNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(20, 312);
            this.lblNotes.Text = "Notes:";
            //
            // txtNotes
            //
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.Location = new System.Drawing.Point(20, 332);
            this.txtNotes.Multiline = true;
            this.txtNotes.Size = new System.Drawing.Size(640, 60);
            //
            // btnContinue
            //
            this.btnContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnContinue.Location = new System.Drawing.Point(504, 408);
            this.btnContinue.Size = new System.Drawing.Size(75, 28);
            this.btnContinue.Text = "Continue";
            this.btnContinue.Click += new System.EventHandler(this.BtnContinue_Click);
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(585, 408);
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            //
            // MultiStockOutForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(680, 450);
            this.Controls.Add(this.lblCustomer);
            this.Controls.Add(this.customerSelect);
            this.Controls.Add(this.lblCart);
            this.Controls.Add(this.panelCart);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize = new System.Drawing.Size(696, 420);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.panelCart.ResumeLayout(false);
            this.panelCart.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
