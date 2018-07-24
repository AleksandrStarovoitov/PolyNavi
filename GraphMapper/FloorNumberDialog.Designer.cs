namespace GraphMapper
{
	partial class FloorNumberDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.floorNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.floorPartNumericUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.floorNumberNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.floorPartNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(25, 71);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(56, 19);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "Ок";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(148, 71);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(56, 19);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Номер этажа:";
            // 
            // floorNumberNumericUpDown
            // 
            this.floorNumberNumericUpDown.Location = new System.Drawing.Point(129, 17);
            this.floorNumberNumericUpDown.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.floorNumberNumericUpDown.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.floorNumberNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.floorNumberNumericUpDown.Name = "floorNumberNumericUpDown";
            this.floorNumberNumericUpDown.Size = new System.Drawing.Size(90, 20);
            this.floorNumberNumericUpDown.TabIndex = 6;
            this.floorNumberNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 47);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Номер части этажа: ";
            // 
            // floorPartNumericUpDown
            // 
            this.floorPartNumericUpDown.Location = new System.Drawing.Point(129, 45);
            this.floorPartNumericUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.floorPartNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.floorPartNumericUpDown.Name = "floorPartNumericUpDown";
            this.floorPartNumericUpDown.Size = new System.Drawing.Size(90, 20);
            this.floorPartNumericUpDown.TabIndex = 8;
            this.floorPartNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // FloorNumberDialoge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 99);
            this.Controls.Add(this.floorPartNumericUpDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.floorNumberNumericUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FloorNumberDialoge";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FloorNumberDialoge";
            ((System.ComponentModel.ISupportInitialize)(this.floorNumberNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.floorPartNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown floorNumberNumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown floorPartNumericUpDown;
    }
}