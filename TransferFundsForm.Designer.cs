namespace Stock_Managemnet
{
    partial class TransferFundsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblFromAccount;
        private System.Windows.Forms.ComboBox cmbFromAccount;
        private System.Windows.Forms.Label lblFromBalance;
        private System.Windows.Forms.Label lblToAccount;
        private System.Windows.Forms.ComboBox cmbToAccount;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.NumericUpDown numAmount;
        private System.Windows.Forms.Label lblReference;
        private System.Windows.Forms.TextBox txtReference;
        private System.Windows.Forms.Label lblTransferredAt;
        private System.Windows.Forms.DateTimePicker dtpTransferredAt;
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
            this.lblFromAccount = new System.Windows.Forms.Label();
            this.cmbFromAccount = new System.Windows.Forms.ComboBox();
            this.lblFromBalance = new System.Windows.Forms.Label();
            this.lblToAccount = new System.Windows.Forms.Label();
            this.cmbToAccount = new System.Windows.Forms.ComboBox();
            this.lblAmount = new System.Windows.Forms.Label();
            this.numAmount = new System.Windows.Forms.NumericUpDown();
            this.lblReference = new System.Windows.Forms.Label();
            this.txtReference = new System.Windows.Forms.TextBox();
            this.lblTransferredAt = new System.Windows.Forms.Label();
            this.dtpTransferredAt = new System.Windows.Forms.DateTimePicker();
            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).BeginInit();
            this.SuspendLayout();
            //
            this.lblFromAccount.AutoSize = true;
            this.lblFromAccount.Location = new System.Drawing.Point(20, 20);
            this.lblFromAccount.Text = "From:";
            //
            this.cmbFromAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFromAccount.Location = new System.Drawing.Point(130, 17);
            this.cmbFromAccount.Size = new System.Drawing.Size(280, 23);
            this.cmbFromAccount.SelectedIndexChanged += new System.EventHandler(this.CmbFromAccount_SelectedIndexChanged);
            //
            this.lblFromBalance.AutoSize = true;
            this.lblFromBalance.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.lblFromBalance.Location = new System.Drawing.Point(420, 20);
            this.lblFromBalance.Text = "Balance: —";
            //
            this.lblToAccount.AutoSize = true;
            this.lblToAccount.Location = new System.Drawing.Point(20, 56);
            this.lblToAccount.Text = "To:";
            //
            this.cmbToAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbToAccount.Location = new System.Drawing.Point(130, 53);
            this.cmbToAccount.Size = new System.Drawing.Size(280, 23);
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
            this.lblTransferredAt.AutoSize = true;
            this.lblTransferredAt.Location = new System.Drawing.Point(20, 164);
            this.lblTransferredAt.Text = "Date:";
            //
            this.dtpTransferredAt.CustomFormat = "dd-MMM-yyyy HH:mm";
            this.dtpTransferredAt.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTransferredAt.Location = new System.Drawing.Point(130, 161);
            this.dtpTransferredAt.Size = new System.Drawing.Size(200, 23);
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
            this.btnSave.Text = "Transfer";
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
            this.Controls.Add(this.lblFromAccount);
            this.Controls.Add(this.cmbFromAccount);
            this.Controls.Add(this.lblFromBalance);
            this.Controls.Add(this.lblToAccount);
            this.Controls.Add(this.cmbToAccount);
            this.Controls.Add(this.lblAmount);
            this.Controls.Add(this.numAmount);
            this.Controls.Add(this.lblReference);
            this.Controls.Add(this.txtReference);
            this.Controls.Add(this.lblTransferredAt);
            this.Controls.Add(this.dtpTransferredAt);
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
