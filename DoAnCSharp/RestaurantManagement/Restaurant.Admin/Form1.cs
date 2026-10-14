using Restaurant.Admin.Services;

namespace Restaurant.Admin
{
    public partial class Form1 : Form
    {
        private readonly MonAnApiService _monAnApiService;

        public Form1()
        {
            InitializeComponent();

            _monAnApiService = new MonAnApiService();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadMonAnAsync();
        }

        private async void btnTaiLai_Click(object sender, EventArgs e)
        {
            await LoadMonAnAsync();
        }

        private async Task LoadMonAnAsync()
        {
            try
            {
                var foods = await _monAnApiService.GetAllAsync();

                dgvMonAn.DataSource = foods;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải dữ liệu món ăn.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}