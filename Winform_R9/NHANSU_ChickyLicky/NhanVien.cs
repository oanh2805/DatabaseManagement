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

    public partial class NhanVien : Form
    {
        private string sCon = "Data Source=DESKTOP-FO8QLMB;Initial Catalog=NHANSU;Integrated Security=True";

        public NhanVien()
        {
            InitializeComponent();
        }

        private void NhanVien_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sCon))
                {
                    string query = "SELECT * FROM NHANVIEN"; 
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
   

    private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtMaCV_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void txtHoTen_TextChanged(object sender, EventArgs e)
        {

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
                txtMaCV.Text = dataGridView1.Rows[e.RowIndex].Cells["macv"].Value.ToString();
                txtHoTen.Text = dataGridView1.Rows[e.RowIndex].Cells["hoten"].Value.ToString();
                txtSDT.Text = dataGridView1.Rows[e.RowIndex].Cells["sdt"].Value.ToString();
                txtDiaChi.Text = dataGridView1.Rows[e.RowIndex].Cells["diachi"].Value.ToString();
                dtpNgaySinh.Value = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["ngaysinh"].Value);

                string sgioitinh = dataGridView1.Rows[e.RowIndex].Cells["gioitinh"].Value.ToString();
                if (sgioitinh == "Nam")
                {
                    rbNam.Checked = true;
                }
                else if (sgioitinh == "Nữ")
                {
                    rbNu.Checked = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn dòng: " + ex.Message);
            }
        }




        //KIỂM TRA DỮ LIỆU NHẬP VÀO TRƯỚC KHI CREATE
        private bool IsInputValid()
        {
            // Kiểm tra mã nhân viên không được để trống
            if (string.IsNullOrWhiteSpace(txtMaNV.Text))
            {
                MessageBox.Show("Mã nhân viên không được để trống!");
                txtMaNV.Focus();
                return false;
            }

            // Kiểm tra họ tên không được để trống
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Họ tên không được để trống!");
                txtHoTen.Focus();
                return false;
            }

            // Kiểm tra số điện thoại không được để trống và phải là số hợp lệ
            if (string.IsNullOrWhiteSpace(txtSDT.Text))
            {
                MessageBox.Show("Số điện thoại không được để trống!");
                txtSDT.Focus();
                return false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtSDT.Text, @"^\d{10}$"))
            {
                MessageBox.Show("Số điện thoại phải là 10 chữ số!");
                txtSDT.Focus();
                return false;
            }

            // Kiểm tra địa chỉ không được để trống
            if (string.IsNullOrWhiteSpace(txtDiaChi.Text))
            {
                MessageBox.Show("Địa chỉ không được để trống!");
                txtDiaChi.Focus();
                return false;
            }

            // Kiểm tra mã chức vụ không được để trống
            if (string.IsNullOrWhiteSpace(txtMaCV.Text))
            {
                MessageBox.Show("Mã chức vụ không được để trống!");
                txtMaCV.Focus();
                return false;
            }

            // Kiểm tra ngày sinh phải nhỏ hơn ngày hiện tại
            if (dtpNgaySinh.Value.Date >= DateTime.Now.Date)
            {
                MessageBox.Show("Ngày sinh phải nhỏ hơn ngày hiện tại!");
                dtpNgaySinh.Focus();
                return false;
            }

            return true; // Dữ liệu hợp lệ
        }

        //KTRA DL TRÙNG KHI CREATE
        private bool IsDuplicateData(string maNV, string sdt)
        {
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "SELECT COUNT(*) FROM NHANVIEN WHERE MaNV = @MaNV OR SDT = @SDT";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                cmd.Parameters.AddWithValue("@SDT", sdt);

                conn.Open();
                int count = (int)cmd.ExecuteScalar(); // Lấy số lượng bản ghi trùng

                return count > 0; // Trả về true nếu có bản ghi trùng
            }
        }


        //CREATE
        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Gọi hàm kiểm tra tính hợp lệ
            if (!IsInputValid())
            {
                return; // Nếu dữ liệu không hợp lệ, dừng việc thêm mới
            }

            // Kiểm tra trùng Mã nhân viên hoặc Số điện thoại
            if (IsDuplicateData(txtMaNV.Text, txtSDT.Text))
            {
                MessageBox.Show("Mã nhân viên hoặc số điện thoại đã tồn tại! Vui lòng kiểm tra lại.");
                return; // Nếu bị trùng, dừng việc thêm mới
            }

            using (SqlConnection conn = new SqlConnection(sCon))
            {

                string query = "INSERT INTO NHANVIEN (MaNV, HoTen, NgaySinh, GioiTinh, DiaChi, SDT, MaCV) " +
                               "VALUES (@MaNV, @HoTen, @NgaySinh, @GioiTinh, @DiaChi, @SDT, @MaCV)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaNV", txtMaNV.Text);
                cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text);
                cmd.Parameters.AddWithValue("@NgaySinh", dtpNgaySinh.Value);
                cmd.Parameters.AddWithValue("@GioiTinh", rbNam.Checked ? "Nam" : "Nữ");
                cmd.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text);
                cmd.Parameters.AddWithValue("@SDT", txtSDT.Text);
                cmd.Parameters.AddWithValue("@MaCV", txtMaCV.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Thêm nhân viên thành công!");
                LoadData();
            }

        }

        //KTRA DỮ LIỆU HỢP LỆ TƯƠNG TỰ NHƯ CREATE

        //KTRA TRÙNG DỮ LIỆU TRƯỚC KHI UPDATE
        private bool IsDuplicateDataForUpdate(string maNV, string sdt)
        {
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "SELECT COUNT(*) FROM NHANVIEN WHERE (MaNV = @MaNV OR SDT = @SDT) AND MaNV != @CurrentMaNV";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                cmd.Parameters.AddWithValue("@SDT", sdt);
                cmd.Parameters.AddWithValue("@CurrentMaNV", txtMaNV.Text); // MaNV hiện tại đang được chỉnh sửa

                conn.Open();
                int count = (int)cmd.ExecuteScalar();

                return count > 0; // Trả về true nếu có bản ghi trùng
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // Kiểm tra tính hợp lệ
            if (!IsInputValid())
            {
                return; // Nếu dữ liệu không hợp lệ, dừng việc cập nhật
            }
            // Kiểm tra trùng dữ liệu (ngoại trừ bản ghi đang sửa)
            if (IsDuplicateDataForUpdate(txtMaNV.Text, txtSDT.Text))
            {
                MessageBox.Show("Mã nhân viên hoặc số điện thoại đã tồn tại! Vui lòng kiểm tra lại.");
                return; // Nếu bị trùng, dừng việc cập nhật
            }

            // UPDATE
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "UPDATE NHANVIEN SET HoTen = @HoTen, NgaySinh = @NgaySinh, GioiTinh = @GioiTinh, " +
                               "DiaChi = @DiaChi, SDT = @SDT, MaCV = @MaCV WHERE MaNV = @MaNV";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@MaNV", txtMaNV.Text);
                cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text);
                cmd.Parameters.AddWithValue("@NgaySinh", dtpNgaySinh.Value);
                cmd.Parameters.AddWithValue("@GioiTinh", rbNam.Checked ? "Nam" : "Nữ");
                cmd.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text);
                cmd.Parameters.AddWithValue("@SDT", txtSDT.Text);
                cmd.Parameters.AddWithValue("@MaCV", txtMaCV.Text);

                conn.Open();
                cmd.ExecuteNonQuery();

                MessageBox.Show("Cập nhật thông tin nhân viên thành công!");
                LoadData(); // Tải lại dữ liệu sau khi sửa
            }
        }



        //DELETE
        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem có dòng nào được chọn hay không
                if (string.IsNullOrWhiteSpace(txtMaNV.Text))
                {
                    MessageBox.Show("Vui lòng chọn nhân viên cần xóa!");
                    return;
                }

                // Xác nhận trước khi xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này không?\n"
                                                      + "Dữ liệu liên quan sẽ bị xóa!",
                                                      "Xác nhận xóa",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return; // Nếu người dùng chọn "No", thoát
                }

                using (SqlConnection conn = new SqlConnection(sCon))
                {
                    conn.Open();

                    // Bước 1: Xóa dữ liệu liên quan từ bảng LUONG, CHAMCONG và TAIKHOAN
                    string deleteLuong = "DELETE FROM LUONG WHERE MaNV = @MaNV";
                    string deleteChamCong = "DELETE FROM CHAMCONG WHERE MaNV = @MaNV";
                    string deleteTaiKhoan = "DELETE FROM TAIKHOAN WHERE MaNV = @MaNV";

                    using (SqlCommand cmdLuong = new SqlCommand(deleteLuong, conn))
                    using (SqlCommand cmdChamCong = new SqlCommand(deleteChamCong, conn))
                    using (SqlCommand cmdTaiKhoan = new SqlCommand(deleteTaiKhoan, conn))
                    {
                        cmdLuong.Parameters.AddWithValue("@MaNV", txtMaNV.Text);
                        cmdChamCong.Parameters.AddWithValue("@MaNV", txtMaNV.Text);
                        cmdTaiKhoan.Parameters.AddWithValue("@MaNV", txtMaNV.Text);

                        cmdLuong.ExecuteNonQuery();
                        cmdChamCong.ExecuteNonQuery();
                        cmdTaiKhoan.ExecuteNonQuery();
                    }

                    // Bước 2: Xóa nhân viên trong bảng NHANVIEN
                    string deleteNhanVien = "DELETE FROM NHANVIEN WHERE MaNV = @MaNV";
                    using (SqlCommand cmdNhanVien = new SqlCommand(deleteNhanVien, conn))
                    {
                        cmdNhanVien.Parameters.AddWithValue("@MaNV", txtMaNV.Text);
                        int rowsAffected = cmdNhanVien.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Xóa nhân viên thành công!");
                            LoadData(); // Tải lại dữ liệu sau khi xóa
                            ClearInputFields(); // Xóa thông tin trên các ô nhập của FORM
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy nhân viên cần xóa. Vui lòng thử lại.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa nhân viên: " + ex.Message);
            }
        }

        private void ClearInputFields()
        {
            txtMaNV.Text = string.Empty;
            txtHoTen.Text = string.Empty;
            txtSDT.Text = string.Empty;
            txtDiaChi.Text = string.Empty;
            txtMaCV.Text = string.Empty;
            rbNam.Checked = false;
            rbNu.Checked = false;
            dtpNgaySinh.Value = DateTime.Now;
        }
    }
}
