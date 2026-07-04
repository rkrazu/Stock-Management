namespace LicenseKeyGenerator
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.TextBox txtCustomer;
        private System.Windows.Forms.Label lblMachineId;
        private System.Windows.Forms.TextBox txtMachineId;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label lblActivationKey;
        private System.Windows.Forms.TextBox txtActivationKey;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnCreateKeys;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.txtCustomer = new System.Windows.Forms.TextBox();
            this.lblMachineId = new System.Windows.Forms.Label();
            this.txtMachineId = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.lblActivationKey = new System.Windows.Forms.Label();
            this.txtActivationKey = new System.Windows.Forms.TextBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnCreateKeys = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(16, 16);
            this.lblTitle.Text = "ELECTRONICS License Key Generator";
            //
            // lblCustomer
            //
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(16, 60);
            this.lblCustomer.Text = "Customer name:";
            //
            // txtCustomer
            //
            this.txtCustomer.Location = new System.Drawing.Point(16, 84);
            this.txtCustomer.Size = new System.Drawing.Size(552, 27);
            //
            // lblMachineId
            //
            this.lblMachineId.AutoSize = true;
            this.lblMachineId.Location = new System.Drawing.Point(16, 124);
            this.lblMachineId.Text = "Machine ID (from customer):";
            //
            // txtMachineId
            //
            this.txtMachineId.Font = new System.Drawing.Font("Consolas", 11F);
            this.txtMachineId.Location = new System.Drawing.Point(16, 148);
            this.txtMachineId.Size = new System.Drawing.Size(552, 25);
            //
            // btnGenerate
            //
            this.btnGenerate.Location = new System.Drawing.Point(16, 188);
            this.btnGenerate.Size = new System.Drawing.Size(160, 32);
            this.btnGenerate.Text = "Generate Key";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.BtnGenerate_Click);
            //
            // lblActivationKey
            //
            this.lblActivationKey.AutoSize = true;
            this.lblActivationKey.Location = new System.Drawing.Point(16, 236);
            this.lblActivationKey.Text = "Activation key (send to customer):";
            //
            // txtActivationKey
            //
            this.txtActivationKey.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtActivationKey.Location = new System.Drawing.Point(16, 260);
            this.txtActivationKey.Multiline = true;
            this.txtActivationKey.ReadOnly = true;
            this.txtActivationKey.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtActivationKey.Size = new System.Drawing.Size(552, 120);
            //
            // btnCopy
            //
            this.btnCopy.Location = new System.Drawing.Point(488, 388);
            this.btnCopy.Size = new System.Drawing.Size(80, 32);
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.BtnCopy_Click);
            //
            // btnCreateKeys
            //
            this.btnCreateKeys.Location = new System.Drawing.Point(192, 188);
            this.btnCreateKeys.Size = new System.Drawing.Size(180, 32);
            this.btnCreateKeys.Text = "Create Developer Keys";
            this.btnCreateKeys.UseVisualStyleBackColor = true;
            this.btnCreateKeys.Click += new System.EventHandler(this.BtnCreateKeys_Click);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 436);
            this.Controls.Add(this.btnCreateKeys);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.txtActivationKey);
            this.Controls.Add(this.lblActivationKey);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.txtMachineId);
            this.Controls.Add(this.lblMachineId);
            this.Controls.Add(this.txtCustomer);
            this.Controls.Add(this.lblCustomer);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "License Key Generator";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
