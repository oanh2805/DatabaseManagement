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
    public partial class ChucVu : Form
    {
        private string sCon = "Data Source=DESKTOP-FO8QLMB;Initial Catalog=NHANSU;Integrated Security=True";
        public ChucVu()
        {
            InitializeComponent();
        }

        private void ChucVu_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sCon))
                {
                    string query = "SELECT * FROM CHUCVU";
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
                txtMaCV.Text = dataGridView1.Rows[e.RowIndex].Cells["macv"].Value.ToString();
                txtTenCV.Text = dataGridView1.Rows[e.RowIndex].Cells["tencv"].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn dòng" + ex.Message);
            }
        }

        private bool IsInputValid()
        {
            // Kiểm tra Mã chức vụ
            if (string.IsNullOrWhiteSpace(txtMaCV.Text) || !Regex.IsMatch(txtMaCV.Text, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Mã chức vụ không được để trống và chỉ chứa ký tự chữ hoặc số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMaCV.Focus();
                return false;
            }

            // Kiểm tra tên chức vụ
            if (string.IsNullOrWhiteSpace(txtTenCV.Text))
            {
                MessageBox.Show("Tên chức vụ không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTenCV.Focus();
                return false;
            }
            return true;
        }
        //kiểm tra dữ liệu trùng khi create
        private bool IsDuplicateData(string maCV)
        {
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "SELECT COUNT(*) FROM Chucvu WHERE MaCV = @MaCV";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaCV", maCV);
               
                conn.Open();
                int count = (int)cmd.ExecuteScalar(); // Lấy số lượng bản ghi trùng

                return count > 0; // Trả về true nếu có bản ghi trùng
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            //gọi hàm kiểm tra tính hợp
            if (!IsInputValid())
            {
                return;// nếu dữ liệu không hợp lệ, dừng việc thêm mới
            }
            //kiểm tra trùng mã chấm công và mã nhân viên
            if (IsDuplicateData(txtMaCV.Text))
            {
                MessageBox.Show("Mã chức vụ đã tồn tại! Vui lòng kiểm tra lại.");
                return;//nếu bị trùng, dừng việc thêm mới

            }
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "INSERT INTO CHUCVU (MaCV,TenCV)" +
                               "Values(@MaCV,@TenCV)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaCV", txtMaCV.Text);
                cmd.Parameters.AddWithValue("@TenCV", txtTenCV.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Thêm chức vụ thành công!");
                LoadData();
            }
        }

        //KTRA TRÙNG DL KHI UPDATE
        private bool IsDuplicateDataForUpdate(string maCV, string tenCV)
        {
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "SELECT COUNT(*) FROM CHUCVU WHERE (MaCV=@MaCV)AND MaCV != @CurrentMaCV";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaCV", maCV);
                cmd.Parameters.AddWithValue("@TenCV", tenCV);
                cmd.Parameters.AddWithValue("@CurrentMaCV", txtMaCV.Text); // MaCV hiện tại đang được chỉnh sửa

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
            if (IsDuplicateDataForUpdate(txtMaCV.Text, txtTenCV.Text))
            {
                MessageBox.Show("Mã chức vụ hoặc tên chức vụ đã tồn tại ! Vui lòng kiểm tra lại.");
                return; // Nếu bị trùng, dừng việc cập nhật
            }
            //Tiến hành cập nhật dữ liệu
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "UPDATE CHUCVU SET TenCV = @TenCV " +
                               "WHERE MaCV=@MaCV";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaCV", txtMaCV.Text);
                cmd.Parameters.AddWithValue("@TenCV", txtTenCV.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Cập nhật thông tin nhân viên thành công!");
                LoadData(); // Tải lại dữ liệu sau khi sửa
            }
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem có dòng nào được chọn hay không
                if (string.IsNullOrWhiteSpace(txtMaCV.Text))
                {
                    MessageBox.Show("Vui lòng chọn chức vụ cần xóa!");
                    return;
                }

                // Xác nhận trước khi xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa chức vụ này không?\n"
                                                      + "Các nhân viên có chức vụ này sẽ được cập nhật lại.",
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

                    // Bước 1: Cập nhật MaCV trong bảng NHANVIEN về NULL
                    string updateQuery = "UPDATE NHANVIEN SET MaCV = NULL WHERE MaCV = @MaCV";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@MaCV", txtMaCV.Text);
                        updateCmd.ExecuteNonQuery();
                    }

                    // Bước 2: Xóa chức vụ trong bảng CHUCVU
                    string deleteQuery = "DELETE FROM CHUCVU WHERE MaCV = @MaCV";
                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@MaCV", txtMaCV.Text);
                        int rowsAffected = deleteCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Xóa chức vụ thành công!");
                            LoadData(); // Tải lại dữ liệu sau khi xóa
                            ClearInputFields(); // Xóa thông tin trên các ô nhập của FORM
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy chức vụ để xóa.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa chức vụ: " + ex.Message);
            }
        }

        // Hàm xóa dữ liệu trên các ô của Form
        private void ClearInputFields()
        {
            txtMaCV.Text = string.Empty;
            txtTenCV.Text = string.Empty;
        }
    }
}
