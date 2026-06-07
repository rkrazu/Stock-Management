namespace Stock_Managemnet
{
    partial class StockAdjustForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.Label lblAvailable;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.TextBox txtCustomerSearch;
        private System.Windows.Forms.ListBox lstCustomers;
        private System.Windows.Forms.Button btnClearCustomer;
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
            this.lblCustomer = new System.Windows.Forms.Label();
            this.txtCustomerSearch = new System.Windows.Forms.TextBox();
            this.lstCustomers = new System.Windows.Forms.ListBox();
            this.btnClearCustomer = new System.Windows.Forms.Button();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.numQuantity = new System.Windows.Forms.NumericUpDown();
            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
            this.SuspendLayout();
            //
            this.lblProduct.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblProduct.Location = new System.Drawing.Point(20, 15);
            this.lblProduct.Size = new System.Drawing.Size(360, 25);
            //
            this.lblAvailable.Location = new System.Drawing.Point(20, 45);
            this.lblAvailable.Size = new System.Drawing.Size(360, 20);
            //
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(20, 75);
            this.lblCustomer.Text = "Customer:";
            this.lblCustomer.Visible = false;
            //
            this.txtCustomerSearch.Location = new System.Drawing.Point(20, 95);
            this.txtCustomerSearch.Size = new System.Drawing.Size(280, 23);
            this.txtCustomerSearch.Visible = false;
            //
            this.btnClearCustomer.Location = new System.Drawing.Point(305, 94);
            this.btnClearCustomer.Size = new System.Drawing.Size(75, 25);
            this.btnClearCustomer.Text = "Clear";
            this.btnClearCustomer.Visible = false;
            //
            this.lstCustomers.Location = new System.Drawing.Point(20, 125);
            this.lstCustomers.Size = new System.Drawing.Size(360, 95);
            this.lstCustomers.Visible = false;
            //
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(20, 80);
            this.lblQuantity.Text = "Quantity:";
            //
            this.numQuantity.Location = new System.Drawing.Point(120, 77);
            this.numQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numQuantity.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            this.numQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.numQuantity.Size = new System.Drawing.Size(120, 23);
            //
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(20, 115);
            this.lblNotes.Text = "Notes:";
            //
            this.txtNotes.Location = new System.Drawing.Point(20, 135);
            this.txtNotes.Multiline = true;
            this.txtNotes.Size = new System.Drawing.Size(360, 60);
            //
            this.btnApply.Location = new System.Drawing.Point(224, 215);
            this.btnApply.Size = new System.Drawing.Size(75, 28);
            this.btnApply.Text = "Apply";
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            //
            this.btnCancel.Location = new System.Drawing.Point(305, 215);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            //
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 260);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.numQuantity);
            this.Controls.Add(this.lblQuantity);
            this.Controls.Add(this.lstCustomers);
            this.Controls.Add(this.btnClearCustomer);
            this.Controls.Add(this.txtCustomerSearch);
            this.Controls.Add(this.lblCustomer);
            this.Controls.Add(this.lblAvailable);
            this.Controls.Add(this.lblProduct);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
