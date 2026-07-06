namespace Stock_Managemnet
{
    partial class RecipeEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblOutput;
        private Controls.ProductSelectControl outputProductSelect;
        private System.Windows.Forms.Label lblMaterial;
        private Controls.ProductSelectControl materialProductSelect;
        private System.Windows.Forms.Label lblMaterialQty;
        private System.Windows.Forms.NumericUpDown numMaterialQty;
        private System.Windows.Forms.Button btnAddMaterial;
        private System.Windows.Forms.Button btnRemoveMaterial;
        private System.Windows.Forms.DataGridView dgvMaterials;
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
            this.lblOutput = new System.Windows.Forms.Label();
            this.outputProductSelect = new Controls.ProductSelectControl();
            this.lblMaterial = new System.Windows.Forms.Label();
            this.materialProductSelect = new Controls.ProductSelectControl();
            this.lblMaterialQty = new System.Windows.Forms.Label();
            this.numMaterialQty = new System.Windows.Forms.NumericUpDown();
            this.btnAddMaterial = new System.Windows.Forms.Button();
            this.btnRemoveMaterial = new System.Windows.Forms.Button();
            this.dgvMaterials = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numMaterialQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterials)).BeginInit();
            this.SuspendLayout();
            //
            this.lblName.AutoSize = false;
            this.lblName.Location = new System.Drawing.Point(20, 20);
            this.lblName.Size = new System.Drawing.Size(125, 23);
            this.lblName.Text = "Production name:";
            //
            this.txtName.Location = new System.Drawing.Point(150, 17);
            this.txtName.Size = new System.Drawing.Size(490, 23);
            //
            this.lblOutput.AutoSize = false;
            this.lblOutput.Location = new System.Drawing.Point(20, 55);
            this.lblOutput.Size = new System.Drawing.Size(125, 23);
            this.lblOutput.Text = "Output product:";
            //
            this.outputProductSelect.Location = new System.Drawing.Point(150, 52);
            this.outputProductSelect.Size = new System.Drawing.Size(490, 30);
            //
            this.lblMaterial.AutoSize = false;
            this.lblMaterial.Location = new System.Drawing.Point(20, 90);
            this.lblMaterial.Size = new System.Drawing.Size(125, 23);
            this.lblMaterial.Text = "Material:";
            //
            this.materialProductSelect.Location = new System.Drawing.Point(150, 87);
            this.materialProductSelect.Size = new System.Drawing.Size(350, 30);
            //
            this.lblMaterialQty.AutoSize = true;
            this.lblMaterialQty.Location = new System.Drawing.Point(510, 90);
            this.lblMaterialQty.Text = "Qty/unit:";
            //
            this.numMaterialQty.Location = new System.Drawing.Point(570, 87);
            this.numMaterialQty.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numMaterialQty.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            this.numMaterialQty.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.numMaterialQty.Size = new System.Drawing.Size(70, 23);
            //
            this.btnAddMaterial.Location = new System.Drawing.Point(150, 125);
            this.btnAddMaterial.Size = new System.Drawing.Size(90, 27);
            this.btnAddMaterial.Text = "Add Material";
            this.btnAddMaterial.Click += new System.EventHandler(this.BtnAddMaterial_Click);
            //
            this.btnRemoveMaterial.Location = new System.Drawing.Point(246, 125);
            this.btnRemoveMaterial.Size = new System.Drawing.Size(100, 27);
            this.btnRemoveMaterial.Text = "Remove Material";
            this.btnRemoveMaterial.Click += new System.EventHandler(this.BtnRemoveMaterial_Click);
            //
            this.dgvMaterials.AllowUserToAddRows = false;
            this.dgvMaterials.AllowUserToDeleteRows = false;
            this.dgvMaterials.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMaterials.Location = new System.Drawing.Point(20, 162);
            this.dgvMaterials.MultiSelect = false;
            this.dgvMaterials.ReadOnly = true;
            this.dgvMaterials.RowHeadersVisible = false;
            this.dgvMaterials.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMaterials.Size = new System.Drawing.Size(640, 280);
            //
            this.btnSave.Location = new System.Drawing.Point(504, 457);
            this.btnSave.Size = new System.Drawing.Size(75, 28);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            //
            this.btnCancel.Location = new System.Drawing.Point(585, 457);
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            //
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(680, 502);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvMaterials);
            this.Controls.Add(this.btnRemoveMaterial);
            this.Controls.Add(this.btnAddMaterial);
            this.Controls.Add(this.numMaterialQty);
            this.Controls.Add(this.lblMaterialQty);
            this.Controls.Add(this.materialProductSelect);
            this.Controls.Add(this.lblMaterial);
            this.Controls.Add(this.outputProductSelect);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.numMaterialQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterials)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
