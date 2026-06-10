namespace Stock_Managemnet
{
    partial class ProductionRunForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblRecipe;
        private System.Windows.Forms.Label lblBatchQty;
        private System.Windows.Forms.NumericUpDown numBatchQty;
        private System.Windows.Forms.DataGridView dgvMaterials;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblRecipe = new System.Windows.Forms.Label();
            this.lblBatchQty = new System.Windows.Forms.Label();
            this.numBatchQty = new System.Windows.Forms.NumericUpDown();
            this.dgvMaterials = new System.Windows.Forms.DataGridView();
            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numBatchQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterials)).BeginInit();
            this.SuspendLayout();
            //
            this.lblRecipe.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRecipe.Location = new System.Drawing.Point(20, 15);
            this.lblRecipe.Size = new System.Drawing.Size(620, 25);
            //
            this.lblBatchQty.AutoSize = true;
            this.lblBatchQty.Location = new System.Drawing.Point(20, 50);
            this.lblBatchQty.Text = "Batch quantity:";
            //
            this.numBatchQty.Location = new System.Drawing.Point(130, 47);
            this.numBatchQty.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numBatchQty.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            this.numBatchQty.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.numBatchQty.Size = new System.Drawing.Size(100, 23);
            this.numBatchQty.ValueChanged += new System.EventHandler(this.NumBatchQty_ValueChanged);
            //
            this.dgvMaterials.AllowUserToAddRows = false;
            this.dgvMaterials.AllowUserToDeleteRows = false;
            this.dgvMaterials.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMaterials.Location = new System.Drawing.Point(20, 80);
            this.dgvMaterials.MultiSelect = false;
            this.dgvMaterials.ReadOnly = true;
            this.dgvMaterials.RowHeadersVisible = false;
            this.dgvMaterials.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMaterials.Size = new System.Drawing.Size(620, 280);
            //
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(20, 370);
            this.lblNotes.Text = "Notes:";
            //
            this.txtNotes.Location = new System.Drawing.Point(20, 390);
            this.txtNotes.Multiline = true;
            this.txtNotes.Size = new System.Drawing.Size(620, 70);
            //
            this.btnRun.Location = new System.Drawing.Point(484, 475);
            this.btnRun.Size = new System.Drawing.Size(75, 28);
            this.btnRun.Text = "Run";
            this.btnRun.Click += new System.EventHandler(this.BtnRun_Click);
            //
            this.btnCancel.Location = new System.Drawing.Point(565, 475);
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            //
            this.AcceptButton = this.btnRun;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(660, 520);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.dgvMaterials);
            this.Controls.Add(this.numBatchQty);
            this.Controls.Add(this.lblBatchQty);
            this.Controls.Add(this.lblRecipe);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Run Production";
            ((System.ComponentModel.ISupportInitialize)(this.numBatchQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterials)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
