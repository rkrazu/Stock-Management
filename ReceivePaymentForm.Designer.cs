namespace Stock_Managemnet
{
    partial class ReceivePaymentForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.ComboBox cmbCustomer;
        private System.Windows.Forms.Label lblCustomerBalance;
        private System.Windows.Forms.Label lblCashAccount;
        private System.Windows.Forms.ComboBox cmbCashAccount;
        private System.Windows.Forms.Label lblInvoice;
        private System.Windows.Forms.ComboBox cmbInvoice;
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
            this.lblCustomer = new System.Windows.Forms.Label();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.lblCustomerBalance = new System.Windows.Forms.Label();
            this.lblCashAccount = new System.Windows.Forms.Label();
            this.cmbCashAccount = new System.Windows.Forms.ComboBox();
            this.lblInvoice = new System.Windows.Forms.Label();
            this.cmbInvoice = new System.Windows.Forms.ComboBox();
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
            //
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(20, 20);
            this.lblCustomer.Text = "Customer:";
            //
            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomer.Location = new System.Drawing.Point(130, 17);
            this.cmbCustomer.Size = new System.Drawing.Size(450, 23);
            this.cmbCustomer.SelectedIndexChanged += new System.EventHandler(this.CmbCustomer_SelectedIndexChanged);
            //
            this.lblCustomerBalance.AutoSize = true;
            this.lblCustomerBalance.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.lblCustomerBalance.Location = new System.Drawing.Point(130, 44);
            this.lblCustomerBalance.Size = new System.Drawing.Size(450, 15);
            //
            this.lblCashAccount.AutoSize = true;
            this.lblCashAccount.Location = new System.Drawing.Point(20, 72);
            this.lblCashAccount.Text = "Cash/Bank:";
            //
            this.cmbCashAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCashAccount.Location = new System.Drawing.Point(130, 69);
            this.cmbCashAccount.Size = new System.Drawing.Size(220, 23);
            //
            this.lblInvoice.AutoSize = true;
            this.lblInvoice.Location = new System.Drawing.Point(20, 108);
            this.lblInvoice.Text = "Invoice:";
            //
            this.cmbInvoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInvoice.Location = new System.Drawing.Point(130, 105);
            this.cmbInvoice.Size = new System.Drawing.Size(450, 23);
            this.cmbInvoice.SelectedIndexChanged += new System.EventHandler(this.CmbInvoice_SelectedIndexChanged);
            //
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(20, 144);
            this.lblAmount.Text = "Amount:";
            //
            this.numAmount.DecimalPlaces = 2;
            this.numAmount.Location = new System.Drawing.Point(130, 141);
            this.numAmount.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            this.numAmount.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numAmount.Size = new System.Drawing.Size(160, 23);
            this.numAmount.Value = new decimal(new int[] { 0, 0, 0, 0 });
            //
            this.lblMethod.AutoSize = true;
            this.lblMethod.Location = new System.Drawing.Point(20, 180);
            this.lblMethod.Text = "Method:";
            //
            this.cmbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMethod.Location = new System.Drawing.Point(130, 177);
            this.cmbMethod.Size = new System.Drawing.Size(200, 23);
            //
            this.lblReference.AutoSize = true;
            this.lblReference.Location = new System.Drawing.Point(20, 216);
            this.lblReference.Text = "Reference:";
            //
            this.txtReference.Location = new System.Drawing.Point(130, 213);
            this.txtReference.Size = new System.Drawing.Size(450, 23);
            //
            this.lblPaidAt.AutoSize = true;
            this.lblPaidAt.Location = new System.Drawing.Point(20, 252);
            this.lblPaidAt.Text = "Paid at:";
            //
            this.dtpPaidAt.CustomFormat = "dd-MMM-yyyy HH:mm";
            this.dtpPaidAt.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPaidAt.Location = new System.Drawing.Point(130, 249);
            this.dtpPaidAt.Size = new System.Drawing.Size(200, 23);
            //
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(20, 288);
            this.lblNotes.Text = "Notes:";
            //
            this.txtNotes.Location = new System.Drawing.Point(130, 285);
            this.txtNotes.Multiline = true;
            this.txtNotes.Size = new System.Drawing.Size(450, 70);
            //
            this.btnSave.Location = new System.Drawing.Point(424, 370);
            this.btnSave.Size = new System.Drawing.Size(75, 28);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(505, 370);
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            //
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(610, 415);
            this.Controls.Add(this.lblCustomer);
            this.Controls.Add(this.cmbCustomer);
            this.Controls.Add(this.lblCustomerBalance);
            this.Controls.Add(this.lblCashAccount);
            this.Controls.Add(this.cmbCashAccount);
            this.Controls.Add(this.lblInvoice);
            this.Controls.Add(this.cmbInvoice);
            this.Controls.Add(this.lblAmount);
            this.Controls.Add(this.numAmount);
            this.Controls.Add(this.lblMethod);
            this.Controls.Add(this.cmbMethod);
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
