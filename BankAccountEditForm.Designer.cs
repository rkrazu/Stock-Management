namespace Stock_Managemnet
{
    partial class BankAccountEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblAccountNumber;
        private System.Windows.Forms.TextBox txtAccountNumber;
        private System.Windows.Forms.Label lblBranch;
        private System.Windows.Forms.TextBox txtBranch;
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
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblAccountNumber = new System.Windows.Forms.Label();
            this.txtAccountNumber = new System.Windows.Forms.TextBox();
            this.lblBranch = new System.Windows.Forms.Label();
            this.txtBranch = new System.Windows.Forms.TextBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(20, 20);
            this.lblName.Text = "Account name:";
            this.txtName.Location = new System.Drawing.Point(140, 17);
            this.txtName.MaxLength = 200;
            this.txtName.Size = new System.Drawing.Size(440, 23);
            this.lblAccountNumber.AutoSize = true;
            this.lblAccountNumber.Location = new System.Drawing.Point(20, 55);
            this.lblAccountNumber.Text = "Account no:";
            this.txtAccountNumber.Location = new System.Drawing.Point(140, 52);
            this.txtAccountNumber.MaxLength = 100;
            this.txtAccountNumber.Size = new System.Drawing.Size(440, 23);
            this.lblBranch.AutoSize = true;
            this.lblBranch.Location = new System.Drawing.Point(20, 90);
            this.lblBranch.Text = "Branch:";
            this.txtBranch.Location = new System.Drawing.Point(140, 87);
            this.txtBranch.MaxLength = 200;
            this.txtBranch.Size = new System.Drawing.Size(440, 23);
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(20, 125);
            this.lblNotes.Text = "Notes:";
            this.txtNotes.Location = new System.Drawing.Point(140, 122);
            this.txtNotes.MaxLength = 500;
            this.txtNotes.Multiline = true;
            this.txtNotes.Size = new System.Drawing.Size(440, 70);
            this.btnSave.Location = new System.Drawing.Point(424, 210);
            this.btnSave.Size = new System.Drawing.Size(75, 28);
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(505, 210);
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(600, 255);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.txtBranch);
            this.Controls.Add(this.lblBranch);
            this.Controls.Add(this.txtAccountNumber);
            this.Controls.Add(this.lblAccountNumber);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
