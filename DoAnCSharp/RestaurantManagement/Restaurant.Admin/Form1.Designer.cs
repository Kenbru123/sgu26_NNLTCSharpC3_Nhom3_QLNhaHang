namespace Restaurant.Admin
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dgvMonAn = new DataGridView();
            btnTaiLai = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvMonAn).BeginInit();
            SuspendLayout();
            // 
            // dgvMonAn
            // 
            dgvMonAn.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMonAn.Location = new Point(0, 0);
            dgvMonAn.Name = "dgvMonAn";
            dgvMonAn.RowHeadersWidth = 51;
            dgvMonAn.Size = new Size(455, 327);
            dgvMonAn.TabIndex = 0;
            // 
            // btnTaiLai
            // 
            btnTaiLai.Location = new Point(264, 333);
            btnTaiLai.Name = "btnTaiLai";
            btnTaiLai.Size = new Size(191, 47);
            btnTaiLai.TabIndex = 1;
            btnTaiLai.Text = "Tải danh sách món ăn";
            btnTaiLai.UseVisualStyleBackColor = true;
            btnTaiLai.Click += btnTaiLai_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnTaiLai);
            Controls.Add(dgvMonAn);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dgvMonAn).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvMonAn;
        private Button btnTaiLai;
    }
}
