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
    //a
    using System.Data;
    using System.Data.SqlClient;
    public partial class Luong : Form
    {
        //b
        private string sCon = "Data Source=DESKTOP-FO8QLMB;Initial Catalog=NHANSU;Integrated Security=True";
        public Luong()
        {
            InitializeComponent();
        }

        private void Luong_Load(object sender, EventArgs e)
        {
            //c
            LoadData();
        }

        //d
        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sCon))
                {
                    string query = "SELECT * FROM LUONG"; 
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

                // Hiển thị dữ liệu từ DataGridView lên TextBox
                txtMaNV.Text = dataGridView1.Rows[e.RowIndex].Cells["manv"].Value.ToString();
                txtMaLuong.Text = dataGridView1.Rows[e.RowIndex].Cells["maluong"].Value.ToString();
                txtLuongTheoGio.Text = dataGridView1.Rows[e.RowIndex].Cells["luongtheogio"].Value.ToString();
                txtThuong.Text = dataGridView1.Rows[e.RowIndex].Cells["thuong"].Value.ToString();
                txtPhat.Text = dataGridView1.Rows[e.RowIndex].Cells["phat"].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn dòng: " + ex.Message);
            }
        }

        private bool IsInputValid()
        {
            // Kiểm tra MaLuong không được để trống
            if (string.IsNullOrWhiteSpace(txtMaLuong.Text))
            {
                MessageBox.Show("Mã lương không được để trống!");
                txtMaLuong.Focus();
                return false;
            }

            // Kiểm tra LuongTheoGio phải là số hợp lệ và >= 0
            if (!decimal.TryParse(txtLuongTheoGio.Text, out decimal luongTheoGio) || luongTheoGio < 0)
            {
                MessageBox.Show("Lương theo giờ phải là số và lớn hơn hoặc bằng 0!");
                txtLuongTheoGio.Focus();
                return false;
            }

            // Kiểm tra Thuong  phải là số hợp lệ và >= 0
            if (!decimal.TryParse(txtThuong.Text, out decimal thuong) || thuong < 0)
            {
                MessageBox.Show("Thưởng phải là số và lớn hơn hoặc bằng 0!");
                txtThuong.Focus();
                return false;
            }

            // Kiểm tra Phat phải là số hợp lệ và >= 0
            if (!decimal.TryParse(txtPhat.Text, out decimal phat) || phat < 0)
            {
                MessageBox.Show("Phạt phải là số và lớn hơn hoặc bằng 0!");
                txtPhat.Focus();
                return false;
            }

            return true; // Dữ liệu hợp lệ
        }

        private bool IsDuplicateData(string maLuong)
        {
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "SELECT COUNT(*) FROM LUONG WHERE MaLuong = @MaLuong";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaLuong", maLuong);

                conn.Open();
                int count = (int)cmd.ExecuteScalar(); // Lấy số lượng bản ghi trùng

                return count > 0; // Trả về true nếu có bản ghi trùng
            }
        }

        //KTRA MaNV đã tồn tại trong bảng NHANVIEN chưa. Nếu ko tồn tại, báo lỗi
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



        //CREATE
        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Gọi hàm kiểm tra tính hợp lệ
            if (!IsInputValid())
            {
                return; // Dừng nếu dữ liệu không hợp lệ
            }

            // Kiểm tra trùng mã lương
            if (IsDuplicateData(txtMaLuong.Text))
            {
                MessageBox.Show("Mã lương đã tồn tại! Vui lòng nhập lại.");
                return; // Dừng nếu mã lương bị trùng
            }

            //ktra manv có tồn tại hay ko, nếu có thì mới thêm dữ liệu lương cho nhân viên.
            if (!IsNhanVienExist(txtMaNV.Text))
            {
                MessageBox.Show("Nhân viên với mã này không tồn tại! Vui lòng kiểm tra lại.");
                txtMaNV.Focus();
                return; // Dừng việc thêm mới
            }

            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "INSERT INTO LUONG (MaLuong, LuongTheoGio, Thuong, Phat) " +
                               "VALUES (@MaLuong, @LuongTheoGio, @Thuong, @Phat)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaLuong", txtMaLuong.Text);
                cmd.Parameters.AddWithValue("@LuongTheoGio", Convert.ToDecimal(txtLuongTheoGio.Text));
                cmd.Parameters.AddWithValue("@Thuong", Convert.ToDecimal(txtThuong.Text));
                cmd.Parameters.AddWithValue("@Phat", Convert.ToDecimal(txtPhat.Text));

                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Thêm dữ liệu lương thành công!");

                // Tải lại dữ liệu
                LoadData();
            }
        }

        private bool IsDuplicateDataForUpdate(string maLuong)
        {
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "SELECT COUNT(*) FROM LUONG WHERE MaLuong = @MaLuong AND MaLuong != @CurrentMaLuong";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaLuong", maLuong);
                cmd.Parameters.AddWithValue("@CurrentMaLuong", txtMaLuong.Text); // Mã lương hiện tại đang được chỉnh sửa

                conn.Open();
                int count = (int)cmd.ExecuteScalar();

                return count > 0; // Trả về true nếu có bản ghi trùng
            }
        }

        //UPDATE
        private void btnSua_Click(object sender, EventArgs e)
        {
            // Kiểm tra tính hợp lệ
            if (!IsInputValid())
            {
                return; // Dừng nếu dữ liệu không hợp lệ
            }

            // Kiểm tra trùng dữ liệu (ngoại trừ bản ghi đang sửa)
            if (IsDuplicateDataForUpdate(txtMaLuong.Text))
            {
                MessageBox.Show("Mã lương đã tồn tại! Vui lòng kiểm tra lại.");
                return; // Dừng nếu bị trùng
            }

            // Thực hiện UPDATE
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "UPDATE LUONG SET LuongTheoGio = @LuongTheoGio, Thuong = @Thuong, Phat = @Phat " +
                               "WHERE MaLuong = @MaLuong";
                SqlCommand cmd = new SqlCommand(query, conn);

                // Truyền tham số
                cmd.Parameters.AddWithValue("@MaLuong", txtMaLuong.Text);
                cmd.Parameters.AddWithValue("@LuongTheoGio", Convert.ToDecimal(txtLuongTheoGio.Text));
                cmd.Parameters.AddWithValue("@Thuong", Convert.ToDecimal(txtThuong.Text));
                cmd.Parameters.AddWithValue("@Phat", Convert.ToDecimal(txtPhat.Text));

                conn.Open();
                cmd.ExecuteNonQuery();

                MessageBox.Show("Cập nhật thông tin lương thành công!");

                LoadData(); // Tải lại dữ liệu sau khi sửa
            }
        }


        //DELETE
        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem có dòng nào được chọn hay không
                if (string.IsNullOrWhiteSpace(txtMaLuong.Text))
                {
                    MessageBox.Show("Vui lòng chọn mã lương cần xóa!");
                    return;
                }

                // Xác nhận trước khi xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa bản ghi lương này không?",
                                                      "Xác nhận xóa",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return; // Nếu người dùng chọn "No", thoát
                }

                // Thực hiện xóa bản ghi lương
                using (SqlConnection conn = new SqlConnection(sCon))
                {
                    string query = "DELETE FROM LUONG WHERE MaLuong = @MaLuong";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    // Thêm tham số
                    cmd.Parameters.AddWithValue("@MaLuong", txtMaLuong.Text);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery(); // Lấy số dòng bị ảnh hưởng

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa bản ghi lương thành công!");
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
                MessageBox.Show("Lỗi khi xóa bản ghi lương: " + ex.Message);
            }
        }

        private void ClearInputFields()
        {
            txtMaLuong.Text = string.Empty;
            txtLuongTheoGio.Text = string.Empty;
            txtThuong.Text = string.Empty;
            txtPhat.Text = string.Empty;
        }


    }
}
