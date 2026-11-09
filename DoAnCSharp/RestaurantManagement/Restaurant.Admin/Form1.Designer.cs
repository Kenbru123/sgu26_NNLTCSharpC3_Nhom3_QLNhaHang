namespace Restaurant.Admin
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tabQuanLy = new TabControl();
            tabMonAn = new TabPage();
            dgvMonAn = new DataGridView();
            btnTaiLai = new Button();
            tabKhuVuc = new TabPage();
            dgvKhuVuc = new DataGridView();
            lblMaKhuVuc = new Label();
            txtMaKhuVuc = new TextBox();
            lblTenKhuVuc = new Label();
            txtTenKhuVuc = new TextBox();
            lblMoTaKhuVuc = new Label();
            txtMoTaKhuVuc = new TextBox();
            btnThemKhuVuc = new Button();
            btnSuaKhuVuc = new Button();
            btnXoaKhuVuc = new Button();
            btnTaiKhuVuc = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvMonAn).BeginInit();
            tabQuanLy.SuspendLayout();
            tabMonAn.SuspendLayout();
            tabKhuVuc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvKhuVuc).BeginInit();
            SuspendLayout();
            // 
            // tabQuanLy
            // 
            tabQuanLy.Controls.Add(tabMonAn);
            tabQuanLy.Controls.Add(tabKhuVuc);
            tabQuanLy.Dock = DockStyle.Fill;
            tabQuanLy.Location = new Point(0, 0);
            tabQuanLy.Name = "tabQuanLy";
            tabQuanLy.SelectedIndex = 0;
            tabQuanLy.Size = new Size(1000, 650);
            tabQuanLy.TabIndex = 0;
            // 
            // tabMonAn
            // 
            tabMonAn.Controls.Add(dgvMonAn);
            tabMonAn.Controls.Add(btnTaiLai);
            tabMonAn.Location = new Point(4, 29);
            tabMonAn.Name = "tabMonAn";
            tabMonAn.Padding = new Padding(12);
            tabMonAn.Size = new Size(992, 617);
            tabMonAn.TabIndex = 0;
            tabMonAn.Text = "Món ăn";
            tabMonAn.UseVisualStyleBackColor = true;
            // 
            // dgvMonAn
            // 
            dgvMonAn.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvMonAn.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMonAn.Location = new Point(12, 12);
            dgvMonAn.Name = "dgvMonAn";
            dgvMonAn.ReadOnly = true;
            dgvMonAn.RowHeadersWidth = 51;
            dgvMonAn.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMonAn.Size = new Size(968, 530);
            dgvMonAn.TabIndex = 0;
            // 
            // btnTaiLai
            // 
            btnTaiLai.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnTaiLai.Location = new Point(780, 553);
            btnTaiLai.Name = "btnTaiLai";
            btnTaiLai.Size = new Size(200, 45);
            btnTaiLai.TabIndex = 1;
            btnTaiLai.Text = "Tải danh sách món ăn";
            btnTaiLai.UseVisualStyleBackColor = true;
            btnTaiLai.Click += btnTaiLai_Click;
            // 
            // tabKhuVuc
            // 
            tabKhuVuc.Controls.Add(dgvKhuVuc);
            tabKhuVuc.Controls.Add(lblMaKhuVuc);
            tabKhuVuc.Controls.Add(txtMaKhuVuc);
            tabKhuVuc.Controls.Add(lblTenKhuVuc);
            tabKhuVuc.Controls.Add(txtTenKhuVuc);
            tabKhuVuc.Controls.Add(lblMoTaKhuVuc);
            tabKhuVuc.Controls.Add(txtMoTaKhuVuc);
            tabKhuVuc.Controls.Add(btnThemKhuVuc);
            tabKhuVuc.Controls.Add(btnSuaKhuVuc);
            tabKhuVuc.Controls.Add(btnXoaKhuVuc);
            tabKhuVuc.Controls.Add(btnTaiKhuVuc);
            tabKhuVuc.Location = new Point(4, 29);
            tabKhuVuc.Name = "tabKhuVuc";
            tabKhuVuc.Padding = new Padding(12);
            tabKhuVuc.Size = new Size(992, 617);
            tabKhuVuc.TabIndex = 1;
            tabKhuVuc.Text = "Khu vực";
            tabKhuVuc.UseVisualStyleBackColor = true;
            // 
            // dgvKhuVuc
            // 
            dgvKhuVuc.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvKhuVuc.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvKhuVuc.Location = new Point(20, 190);
            dgvKhuVuc.MultiSelect = false;
            dgvKhuVuc.Name = "dgvKhuVuc";
            dgvKhuVuc.ReadOnly = true;
            dgvKhuVuc.RowHeadersWidth = 51;
            dgvKhuVuc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvKhuVuc.Size = new Size(952, 405);
            dgvKhuVuc.TabIndex = 10;
            // 
            // lblMaKhuVuc
            // 
            lblMaKhuVuc.AutoSize = true;
            lblMaKhuVuc.Location = new Point(20, 20);
            lblMaKhuVuc.Name = "lblMaKhuVuc";
            lblMaKhuVuc.Size = new Size(85, 20);
            lblMaKhuVuc.TabIndex = 0;
            lblMaKhuVuc.Text = "Mã khu vực";
            // 
            // txtMaKhuVuc
            // 
            txtMaKhuVuc.Location = new Point(20, 45);
            txtMaKhuVuc.Name = "txtMaKhuVuc";
            txtMaKhuVuc.ReadOnly = true;
            txtMaKhuVuc.Size = new Size(120, 27);
            txtMaKhuVuc.TabIndex = 1;
            // 
            // lblTenKhuVuc
            // 
            lblTenKhuVuc.AutoSize = true;
            lblTenKhuVuc.Location = new Point(165, 20);
            lblTenKhuVuc.Name = "lblTenKhuVuc";
            lblTenKhuVuc.Size = new Size(92, 20);
            lblTenKhuVuc.TabIndex = 2;
            lblTenKhuVuc.Text = "Tên khu vực";
            // 
            // txtTenKhuVuc
            // 
            txtTenKhuVuc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTenKhuVuc.Location = new Point(165, 45);
            txtTenKhuVuc.MaxLength = 100;
            txtTenKhuVuc.Name = "txtTenKhuVuc";
            txtTenKhuVuc.Size = new Size(807, 27);
            txtTenKhuVuc.TabIndex = 3;
            // 
            // lblMoTaKhuVuc
            // 
            lblMoTaKhuVuc.AutoSize = true;
            lblMoTaKhuVuc.Location = new Point(20, 83);
            lblMoTaKhuVuc.Name = "lblMoTaKhuVuc";
            lblMoTaKhuVuc.Size = new Size(47, 20);
            lblMoTaKhuVuc.TabIndex = 4;
            lblMoTaKhuVuc.Text = "Mô tả";
            // 
            // txtMoTaKhuVuc
            // 
            txtMoTaKhuVuc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMoTaKhuVuc.Location = new Point(20, 108);
            txtMoTaKhuVuc.MaxLength = 255;
            txtMoTaKhuVuc.Name = "txtMoTaKhuVuc";
            txtMoTaKhuVuc.Size = new Size(952, 27);
            txtMoTaKhuVuc.TabIndex = 5;
            // 
            // btnThemKhuVuc
            // 
            btnThemKhuVuc.Location = new Point(20, 148);
            btnThemKhuVuc.Name = "btnThemKhuVuc";
            btnThemKhuVuc.Size = new Size(125, 32);
            btnThemKhuVuc.TabIndex = 6;
            btnThemKhuVuc.Text = "Thêm khu vực";
            btnThemKhuVuc.UseVisualStyleBackColor = true;
            btnThemKhuVuc.Click += btnThemKhuVuc_Click;
            // 
            // btnSuaKhuVuc
            // 
            btnSuaKhuVuc.Enabled = false;
            btnSuaKhuVuc.Location = new Point(155, 148);
            btnSuaKhuVuc.Name = "btnSuaKhuVuc";
            btnSuaKhuVuc.Size = new Size(125, 32);
            btnSuaKhuVuc.TabIndex = 7;
            btnSuaKhuVuc.Text = "Cập nhật";
            btnSuaKhuVuc.UseVisualStyleBackColor = true;
            btnSuaKhuVuc.Click += btnSuaKhuVuc_Click;
            // 
            // btnXoaKhuVuc
            // 
            btnXoaKhuVuc.Enabled = false;
            btnXoaKhuVuc.Location = new Point(290, 148);
            btnXoaKhuVuc.Name = "btnXoaKhuVuc";
            btnXoaKhuVuc.Size = new Size(125, 32);
            btnXoaKhuVuc.TabIndex = 8;
            btnXoaKhuVuc.Text = "Xóa";
            btnXoaKhuVuc.UseVisualStyleBackColor = true;
            btnXoaKhuVuc.Click += btnXoaKhuVuc_Click;
            // 
            // btnTaiKhuVuc
            // 
            btnTaiKhuVuc.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTaiKhuVuc.Location = new Point(847, 148);
            btnTaiKhuVuc.Name = "btnTaiKhuVuc";
            btnTaiKhuVuc.Size = new Size(125, 32);
            btnTaiKhuVuc.TabIndex = 9;
            btnTaiKhuVuc.Text = "Tải lại";
            btnTaiKhuVuc.UseVisualStyleBackColor = true;
            btnTaiKhuVuc.Click += btnTaiKhuVuc_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 650);
            Controls.Add(tabQuanLy);
            MinimumSize = new Size(800, 550);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý nhà hàng";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dgvMonAn).EndInit();
            tabQuanLy.ResumeLayout(false);
            tabMonAn.ResumeLayout(false);
            tabKhuVuc.ResumeLayout(false);
            tabKhuVuc.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvKhuVuc).EndInit();
            ResumeLayout(false);
        }

        private TabControl tabQuanLy;
        private TabPage tabMonAn;
        private TabPage tabKhuVuc;
        private DataGridView dgvMonAn;
        private Button btnTaiLai;
        private DataGridView dgvKhuVuc;
        private Label lblMaKhuVuc;
        private TextBox txtMaKhuVuc;
        private Label lblTenKhuVuc;
        private TextBox txtTenKhuVuc;
        private Label lblMoTaKhuVuc;
        private TextBox txtMoTaKhuVuc;
        private Button btnThemKhuVuc;
        private Button btnSuaKhuVuc;
        private Button btnXoaKhuVuc;
        private Button btnTaiKhuVuc;
    }
}
