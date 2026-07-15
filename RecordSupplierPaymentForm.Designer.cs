namespace Stock_Managemnet
{
    partial class RecordSupplierPaymentForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblSupplier;
        private System.Windows.Forms.ComboBox cmbSupplier;
        private System.Windows.Forms.Label lblSupplierBalance;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.NumericUpDown numAmount;
        private System.Windows.Forms.Label lblMethod;
        private System.Windows.Forms.ComboBox cmbMethod;
        private System.Windows.Forms.Label lblReference;
        private System.Windows.Forms.TextBox txtReference;
        private System.Windows.Forms.Label lblPaidAt;
        private System.Windows.Forms.DateTimePicker dtpPaidAt;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.TextBox txtNotes;
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
            this.lblSupplier = new System.Windows.Forms.Label();
            this.cmbSupplier = new System.Windows.Forms.ComboBox();
            this.lblSupplierBalance = new System.Windows.Forms.Label();
            this.lblAmount = new System.Windows.Forms.Label();
            this.numAmount = new System.Windows.Forms.NumericUpDown();
            this.lblMethod = new System.Windows.Forms.Label();
            this.cmbMethod = new System.Windows.Forms.ComboBox();
            this.lblReference = new System.Windows.Forms.Label();
            this.txtReference = new System.Windows.Forms.TextBox();
            this.lblPaidAt = new System.Windows.Forms.Label();
            this.dtpPaidAt = new System.Windows.Forms.DateTimePicker();
            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).BeginInit();
            this.SuspendLayout();
            this.lblSupplier.AutoSize = true;
            this.lblSupplier.Location = new System.Drawing.Point(20, 20);
            this.lblSupplier.Text = "Supplier:";
            this.cmbSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSupplier.Location = new System.Drawing.Point(130, 17);
            this.cmbSupplier.Size = new System.Drawing.Size(450, 23);
            this.cmbSupplier.SelectedIndexChanged += new System.EventHandler(this.CmbSupplier_SelectedIndexChanged);
            this.lblSupplierBalance.AutoSize = true;
            this.lblSupplierBalance.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.lblSupplierBalance.Location = new System.Drawing.Point(130, 44);
            this.lblSupplierBalance.Size = new System.Drawing.Size(450, 15);
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(20, 78);
            this.lblAmount.Text = "Amount:";
            this.numAmount.DecimalPlaces = 2;
            this.numAmount.Location = new System.Drawing.Point(130, 75);
            this.numAmount.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            this.numAmount.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            this.numAmount.Size = new System.Drawing.Size(160, 23);
            this.numAmount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.lblMethod.AutoSize = true;
            this.lblMethod.Location = new System.Drawing.Point(20, 114);
            this.lblMethod.Text = "Method:";
            this.cmbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMethod.Location = new System.Drawing.Point(130, 111);
            this.cmbMethod.Size = new System.Drawing.Size(220, 23);
            this.lblReference.AutoSize = true;
            this.lblReference.Location = new System.Drawing.Point(20, 150);
            this.lblReference.Text = "Reference:";
            this.txtReference.Location = new System.Drawing.Point(130, 147);
            this.txtReference.MaxLength = 100;
            this.txtReference.Size = new System.Drawing.Size(450, 23);
            this.lblPaidAt.AutoSize = true;
            this.lblPaidAt.Location = new System.Drawing.Point(20, 186);
            this.lblPaidAt.Text = "Paid at:";
            this.dtpPaidAt.CustomFormat = "dd MMM yyyy hh:mm tt";
            this.dtpPaidAt.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPaidAt.Location = new System.Drawing.Point(130, 183);
            this.dtpPaidAt.Size = new System.Drawing.Size(220, 23);
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(20, 222);
            this.lblNotes.Text = "Notes:";
            this.txtNotes.Location = new System.Drawing.Point(130, 219);
            this.txtNotes.MaxLength = 500;
            this.txtNotes.Multiline = true;
            this.txtNotes.Size = new System.Drawing.Size(450, 60);
            this.btnSave.Location = new System.Drawing.Point(424, 300);
            this.btnSave.Size = new System.Drawing.Size(75, 28);
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(505, 300);
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(600, 350);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.dtpPaidAt);
            this.Controls.Add(this.lblPaidAt);
            this.Controls.Add(this.txtReference);
            this.Controls.Add(this.lblReference);
            this.Controls.Add(this.cmbMethod);
            this.Controls.Add(this.lblMethod);
            this.Controls.Add(this.numAmount);
            this.Controls.Add(this.lblAmount);
            this.Controls.Add(this.lblSupplierBalance);
            this.Controls.Add(this.cmbSupplier);
            this.Controls.Add(this.lblSupplier);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
