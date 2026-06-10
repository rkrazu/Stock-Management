namespace Stock_Managemnet
{
    partial class StockAdjustForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.Label lblAvailable;
        private System.Windows.Forms.Panel panelCustomer;
        private System.Windows.Forms.Label lblCustomer;
        private Controls.CustomerSelectControl customerSelect;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.NumericUpDown numQuantity;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblProduct = new System.Windows.Forms.Label();
            this.lblAvailable = new System.Windows.Forms.Label();
            this.panelCustomer = new System.Windows.Forms.Panel();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.customerSelect = new Controls.CustomerSelectControl();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.numQuantity = new System.Windows.Forms.NumericUpDown();
            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panelCustomer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
            this.SuspendLayout();
            //
            // lblProduct
            //
            this.lblProduct.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProduct.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblProduct.Location = new System.Drawing.Point(20, 15);
            this.lblProduct.Size = new System.Drawing.Size(540, 25);
            //
            // lblAvailable
            //
            this.lblAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAvailable.Location = new System.Drawing.Point(20, 45);
            this.lblAvailable.Size = new System.Drawing.Size(540, 20);
            //
            // panelCustomer
            //
            this.panelCustomer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCustomer.Controls.Add(this.lblCustomer);
            this.panelCustomer.Controls.Add(this.customerSelect);
            this.panelCustomer.Location = new System.Drawing.Point(0, 70);
            this.panelCustomer.Size = new System.Drawing.Size(580, 65);
            this.panelCustomer.Visible = false;
            //
            // lblCustomer
            //
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(20, 6);
            this.lblCustomer.Text = "Customer (required):";
            //
            // customerSelect
            //
            this.customerSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.customerSelect.Location = new System.Drawing.Point(20, 28);
            this.customerSelect.Size = new System.Drawing.Size(540, 30);
            //
            // lblQuantity
            //
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(20, 80);
            this.lblQuantity.Text = "Quantity:";
            //
            // numQuantity
            //
            this.numQuantity.Location = new System.Drawing.Point(120, 77);
            this.numQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numQuantity.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            this.numQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.numQuantity.Size = new System.Drawing.Size(120, 23);
            //
            // lblNotes
            //
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(20, 115);
            this.lblNotes.Text = "Notes:";
            //
            // txtNotes
            //
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.Location = new System.Drawing.Point(20, 135);
            this.txtNotes.Multiline = true;
            this.txtNotes.Size = new System.Drawing.Size(540, 60);
            //
            // btnApply
            //
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(404, 220);
            this.btnApply.Size = new System.Drawing.Size(75, 28);
            this.btnApply.Text = "Apply";
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(485, 220);
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            //
            // StockAdjustForm
            //
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(580, 260);
            this.Controls.Add(this.lblProduct);
            this.Controls.Add(this.lblAvailable);
            this.Controls.Add(this.panelCustomer);
            this.Controls.Add(this.lblQuantity);
            this.Controls.Add(this.numQuantity);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(596, 200);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.panelCustomer.ResumeLayout(false);
            this.panelCustomer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
