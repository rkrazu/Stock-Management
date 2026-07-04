namespace Stock_Managemnet
{
    partial class ActivationForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblInstructions;
        private System.Windows.Forms.Label lblMachineIdCaption;
        private System.Windows.Forms.Label lblMachineIdValue;
        private System.Windows.Forms.Button btnCopyMachineId;
        private System.Windows.Forms.Label lblActivationKeyCaption;
        private System.Windows.Forms.TextBox txtActivationKey;
        private System.Windows.Forms.Button btnActivate;
        private System.Windows.Forms.Button btnExit;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.lblMachineIdCaption = new System.Windows.Forms.Label();
            this.lblMachineIdValue = new System.Windows.Forms.Label();
            this.btnCopyMachineId = new System.Windows.Forms.Button();
            this.lblActivationKeyCaption = new System.Windows.Forms.Label();
            this.txtActivationKey = new System.Windows.Forms.TextBox();
            this.btnActivate = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            //
            // pbLogo
            //
            this.pbLogo.Location = new System.Drawing.Point(168, 16);
            this.pbLogo.Size = new System.Drawing.Size(80, 80);
            this.pbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLogo.TabStop = false;
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(118, 102);
            this.lblTitle.Text = "ELECTRONICS";
            //
            // lblInstructions
            //
            this.lblInstructions.Location = new System.Drawing.Point(24, 132);
            this.lblInstructions.Size = new System.Drawing.Size(368, 72);
            this.lblInstructions.Text = "This software must be activated for this computer.\r\nSend the Machine ID below to the developer and enter the activation key you receive.";
            //
            // lblMachineIdCaption
            //
            this.lblMachineIdCaption.AutoSize = true;
            this.lblMachineIdCaption.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblMachineIdCaption.Location = new System.Drawing.Point(24, 214);
            this.lblMachineIdCaption.Text = "Machine ID:";
            //
            // lblMachineIdValue
            //
            this.lblMachineIdValue.AutoSize = true;
            this.lblMachineIdValue.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
            this.lblMachineIdValue.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.lblMachineIdValue.Location = new System.Drawing.Point(24, 240);
            this.lblMachineIdValue.Text = "XXXX-XXXX-XXXX-XXXX";
            //
            // btnCopyMachineId
            //
            this.btnCopyMachineId.Location = new System.Drawing.Point(24, 272);
            this.btnCopyMachineId.Size = new System.Drawing.Size(120, 30);
            this.btnCopyMachineId.Text = "Copy Machine ID";
            this.btnCopyMachineId.UseVisualStyleBackColor = true;
            this.btnCopyMachineId.Click += new System.EventHandler(this.BtnCopyMachineId_Click);
            //
            // lblActivationKeyCaption
            //
            this.lblActivationKeyCaption.AutoSize = true;
            this.lblActivationKeyCaption.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblActivationKeyCaption.Location = new System.Drawing.Point(24, 318);
            this.lblActivationKeyCaption.Text = "Activation key:";
            //
            // txtActivationKey
            //
            this.txtActivationKey.Location = new System.Drawing.Point(24, 344);
            this.txtActivationKey.Multiline = true;
            this.txtActivationKey.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtActivationKey.Size = new System.Drawing.Size(368, 88);
            //
            // btnActivate
            //
            this.btnActivate.Location = new System.Drawing.Point(236, 446);
            this.btnActivate.Size = new System.Drawing.Size(75, 30);
            this.btnActivate.Text = "Activate";
            this.btnActivate.UseVisualStyleBackColor = true;
            this.btnActivate.Click += new System.EventHandler(this.BtnActivate_Click);
            //
            // btnExit
            //
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(317, 446);
            this.btnExit.Size = new System.Drawing.Size(75, 30);
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            //
            // ActivationForm
            //
            this.AcceptButton = this.btnActivate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(416, 492);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnActivate);
            this.Controls.Add(this.txtActivationKey);
            this.Controls.Add(this.lblActivationKeyCaption);
            this.Controls.Add(this.btnCopyMachineId);
            this.Controls.Add(this.lblMachineIdValue);
            this.Controls.Add(this.lblMachineIdCaption);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pbLogo);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Activation";
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
