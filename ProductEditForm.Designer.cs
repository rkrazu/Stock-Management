namespace Stock_Managemnet
{
    partial class ProductEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblSku;
        private System.Windows.Forms.TextBox txtSku;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.TextBox txtCategory;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.NumericUpDown numPrice;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.NumericUpDown numQuantity;
        private System.Windows.Forms.Label lblReorder;
        private System.Windows.Forms.NumericUpDown numReorder;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblSku = new System.Windows.Forms.Label();
            this.txtSku = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.txtCategory = new System.Windows.Forms.TextBox();
            this.lblPrice = new System.Windows.Forms.Label();
            this.numPrice = new System.Windows.Forms.NumericUpDown();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.numQuantity = new System.Windows.Forms.NumericUpDown();
            this.lblReorder = new System.Windows.Forms.Label();
            this.numReorder = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReorder)).BeginInit();
            this.SuspendLayout();
            //
            // lblSku
            //
            this.lblSku.AutoSize = true;
            this.lblSku.Location = new System.Drawing.Point(20, 20);
            this.lblSku.Name = "lblSku";
            this.lblSku.Size = new System.Drawing.Size(32, 15);
            this.lblSku.Text = "SKU:";
            //
            // txtSku
            //
            this.txtSku.Location = new System.Drawing.Point(120, 17);
            this.txtSku.MaxLength = 50;
            this.txtSku.Size = new System.Drawing.Size(460, 23);
            //
            // lblName
            //
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(20, 55);
            this.lblName.Text = "Name:";
            //
            // txtName
            //
            this.txtName.Location = new System.Drawing.Point(120, 52);
            this.txtName.MaxLength = 200;
            this.txtName.Size = new System.Drawing.Size(460, 23);
            //
            // lblCategory
            //
            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new System.Drawing.Point(20, 90);
            this.lblCategory.Text = "Category:";
            //
            // txtCategory
            //
            this.txtCategory.Location = new System.Drawing.Point(120, 87);
            this.txtCategory.MaxLength = 100;
            this.txtCategory.Size = new System.Drawing.Size(460, 23);
            //
            // lblPrice
            //
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(20, 125);
            this.lblPrice.Text = "Unit price:";
            //
            // numPrice
            //
            this.numPrice.DecimalPlaces = 2;
            this.numPrice.Location = new System.Drawing.Point(120, 122);
            this.numPrice.Maximum = new decimal(new int[] { 9999999, 0, 0, 0 });
            this.numPrice.Size = new System.Drawing.Size(120, 23);
            //
            // lblQuantity
            //
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(20, 160);
            this.lblQuantity.Text = "Initial qty:";
            //
            // numQuantity
            //
            this.numQuantity.Location = new System.Drawing.Point(120, 157);
            this.numQuantity.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            this.numQuantity.Size = new System.Drawing.Size(120, 23);
            //
            // lblReorder
            //
            this.lblReorder.AutoSize = true;
            this.lblReorder.Location = new System.Drawing.Point(20, 195);
            this.lblReorder.Text = "Reorder at:";
            //
            // numReorder
            //
            this.numReorder.Location = new System.Drawing.Point(120, 192);
            this.numReorder.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            this.numReorder.Size = new System.Drawing.Size(120, 23);
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(424, 240);
            this.btnSave.Size = new System.Drawing.Size(75, 28);
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(505, 240);
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            //
            // ProductEditForm
            //
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(600, 285);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.numReorder);
            this.Controls.Add(this.lblReorder);
            this.Controls.Add(this.numQuantity);
            this.Controls.Add(this.lblQuantity);
            this.Controls.Add(this.numPrice);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.txtCategory);
            this.Controls.Add(this.lblCategory);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtSku);
            this.Controls.Add(this.lblSku);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReorder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
