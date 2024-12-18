using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace NHANSU_ChickyLicky
{
    using System.Data;
    using System.Data.SqlClient;
    public partial class TaiKhoan : Form
    {
        private string sCon = "Data Source=DESKTOP-FO8QLMB;Initial Catalog=NHANSU;Integrated Security=True";
        public TaiKhoan()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void TaiKhoan_Load(object sender, EventArgs e)
        {
            LoadData();

        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sCon))
                {
                    string query = "SELECT * FROM TAIKHOAN";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Đổ dữ liệu vào DataGridView
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Kiểm tra xem người dùng có click vào dòng hợp lệ không

                // Lấy dòng được chọn
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                //hiển thị dữ liệu từ DataGridView lên Textbox
                txtMaTK.Text = dataGridView1.Rows[e.RowIndex].Cells["matk"].Value.ToString();
                txtMaNV.Text = dataGridView1.Rows[e.RowIndex].Cells["manv"].Value.ToString();
                txtTenDangNhap.Text = dataGridView1.Rows[e.RowIndex].Cells["tendangnhap"].Value.ToString();
                txtMatKhau.Text = dataGridView1.Rows[e.RowIndex].Cells["matkhau"].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn dòng" + ex.Message);
            }
        }

        private bool IsInputValid()
        {
            // Kiểm tra Mã tài khoản
            if (string.IsNullOrWhiteSpace(txtMaTK.Text) || !Regex.IsMatch(txtMaTK.Text, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Mã tài khoản không được để trống và chỉ chứa ký tự chữ hoặc số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMaTK.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtMaNV.Text) || !Regex.IsMatch(txtMaNV.Text, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Mã nhân viên không được để trống và chỉ chứa ký tự chữ hoặc số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMaTK.Focus();
                return false;
            }

            // Kiểm tra tên đăng nhập
            if (string.IsNullOrWhiteSpace(txtTenDangNhap.Text))
            {
                MessageBox.Show("Tên đăng nhập không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTenDangNhap.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Mật khẩu không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTenDangNhap.Focus();
                return false;
            }

            return true;
        }

        private bool IsDuplicateData(string maTK, string maNV)
        {
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "SELECT COUNT(*) FROM Taikhoan WHERE MaTK = @MaTK and MaNV = @MaNV";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaTK", maTK);
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                conn.Open();
                int count = (int)cmd.ExecuteScalar(); // Lấy số lượng bản ghi trùng

                return count > 0; // Trả về true nếu có bản ghi trùng
            }
        }

        // Kiểm tra xem MaNV có tồn tại trong bảng NHANVIEN hay không
        private bool IsNhanVienExist(string maNV)
        {
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "SELECT COUNT(*) FROM NHANVIEN WHERE MaNV = @MaNV";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaNV", maNV);

                conn.Open();
                int count = (int)cmd.ExecuteScalar(); // Trả về số lượng nhân viên có MaNV

                return count > 0; // Nếu tồn tại thì trả về true
            }
        }


        private void btnTao_Click(object sender, EventArgs e)
        {
            //gọi hàm kiểm tra tính hợp
            if (!IsInputValid())
            {
                return;// nếu dữ liệu không hợp lệ, dừng việc thêm mới
            }
            //kiểm tra trùng mã tài khoản và mã nhân viên
            if (IsDuplicateData(txtMaTK.Text, txtMaNV.Text))
            {
                MessageBox.Show("Mã tài khoản hoặc Mã nhân viên đã tồn tại! Vui lòng kiểm tra lại.");
                return;//nếu bị trùng, dừng việc thêm mới

            }

            //MaNV tồn tại thì mới thêm TK mới cho NHANVIEN
            if (!IsNhanVienExist(txtMaNV.Text))
            {
                MessageBox.Show("Nhân viên với mã này không tồn tại! Vui lòng kiểm tra lại.");
                txtMaNV.Focus();
                return; // Dừng việc thêm mới
            }

            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "INSERT INTO Taikhoan (MaTK,TenDangNhap,MatKhau,MaNV)" +
                               "Values(@MaTK,@TenDangNhap,@MatKhau,@MaNV)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaTK", txtMaTK.Text);
                cmd.Parameters.AddWithValue("@TenDangNhap", txtTenDangNhap.Text);
                cmd.Parameters.AddWithValue("@Matkhau", txtMatKhau.Text);
                cmd.Parameters.AddWithValue("@MaNV", txtMaNV.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Thêm tài khoản thành công!");
                LoadData();
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem có dòng nào được chọn hay không
                if (string.IsNullOrWhiteSpace(txtMaTK.Text))
                {
                    MessageBox.Show("Vui lòng chọn tài khoản cần xóa!");
                    return;
                }

                // Xác nhận trước khi xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa tài khoản này không?",
                                                      "Xác nhận xóa",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return; // Nếu người dùng chọn "No", thoát
                }
                // Thực hiện xóa nhân viên
                using (SqlConnection conn = new SqlConnection(sCon))
                {
                    string query = "DELETE FROM Taikhoan WHERE MaTK = @MaTK AND MaNV = @MaNV";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    // Thêm tham số
                    cmd.Parameters.AddWithValue("@MaTK", txtMaTK.Text);
                    cmd.Parameters.AddWithValue("@MaNV", txtMaNV.Text);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery(); // Lấy số dòng bị ảnh hưởng

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa tài khoản thành công!");
                        LoadData(); // Tải lại dữ liệu sau khi xóa
                        ClearInputFields(); // Xóa thông tin trên các ô nhập của FORM
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy bản ghi để xóa.");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa tài khoản: " + ex.Message);
            }
        }

        //Hàm xóa dl trên các ô của Form
        private void ClearInputFields()
        {
            txtMaTK.Text = string.Empty;
            txtMaNV.Text = string.Empty;
            txtTenDangNhap.Text = string.Empty;
            txtMatKhau.Text = string.Empty;
        }
    }
}
