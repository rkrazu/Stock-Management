namespace Stock_Managemnet
{
    partial class RecordExpenseForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Label lblCashAccount;
        private System.Windows.Forms.ComboBox cmbCashAccount;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.NumericUpDown numAmount;
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
            this.lblCategory = new System.Windows.Forms.Label();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.lblCashAccount = new System.Windows.Forms.Label();
            this.cmbCashAccount = new System.Windows.Forms.ComboBox();
            this.lblAmount = new System.Windows.Forms.Label();
            this.numAmount = new System.Windows.Forms.NumericUpDown();
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
            //
            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new System.Drawing.Point(20, 20);
            this.lblCategory.Text = "Category:";
            //
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Location = new System.Drawing.Point(130, 17);
            this.cmbCategory.Size = new System.Drawing.Size(450, 23);
            //
            this.lblCashAccount.AutoSize = true;
            this.lblCashAccount.Location = new System.Drawing.Point(20, 56);
            this.lblCashAccount.Text = "Paid from:";
            //
            this.cmbCashAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCashAccount.Location = new System.Drawing.Point(130, 53);
            this.cmbCashAccount.Size = new System.Drawing.Size(220, 23);
            //
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(20, 92);
            this.lblAmount.Text = "Amount:";
            //
            this.numAmount.DecimalPlaces = 2;
            this.numAmount.Location = new System.Drawing.Point(130, 89);
            this.numAmount.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            this.numAmount.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            this.numAmount.Size = new System.Drawing.Size(160, 23);
            this.numAmount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            this.lblReference.AutoSize = true;
            this.lblReference.Location = new System.Drawing.Point(20, 128);
            this.lblReference.Text = "Reference:";
            //
            this.txtReference.Location = new System.Drawing.Point(130, 125);
            this.txtReference.Size = new System.Drawing.Size(450, 23);
            //
            this.lblPaidAt.AutoSize = true;
            this.lblPaidAt.Location = new System.Drawing.Point(20, 164);
            this.lblPaidAt.Text = "Paid at:";
            //
            this.dtpPaidAt.CustomFormat = "dd-MMM-yyyy HH:mm";
            this.dtpPaidAt.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPaidAt.Location = new System.Drawing.Point(130, 161);
            this.dtpPaidAt.Size = new System.Drawing.Size(200, 23);
            //
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(20, 200);
            this.lblNotes.Text = "Notes:";
            //
            this.txtNotes.Location = new System.Drawing.Point(130, 197);
            this.txtNotes.Multiline = true;
            this.txtNotes.Size = new System.Drawing.Size(450, 70);
            //
            this.btnSave.Location = new System.Drawing.Point(424, 282);
            this.btnSave.Size = new System.Drawing.Size(75, 28);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(505, 282);
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            //
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(610, 327);
            this.Controls.Add(this.lblCategory);
            this.Controls.Add(this.cmbCategory);
            this.Controls.Add(this.lblCashAccount);
            this.Controls.Add(this.cmbCashAccount);
            this.Controls.Add(this.lblAmount);
            this.Controls.Add(this.numAmount);
            this.Controls.Add(this.lblReference);
            this.Controls.Add(this.txtReference);
            this.Controls.Add(this.lblPaidAt);
            this.Controls.Add(this.dtpPaidAt);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
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
