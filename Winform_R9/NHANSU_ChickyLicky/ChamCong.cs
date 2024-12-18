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
    public partial class ChamCong : Form
    {
        private string sCon = "Data Source=DESKTOP-FO8QLMB;Initial Catalog=NHANSU;Integrated Security=True";
        public ChamCong()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void ChamCong_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sCon))
                {
                    string query = "SELECT * FROM CHAMCONG";
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
                txtMaCC.Text = dataGridView1.Rows[e.RowIndex].Cells["macc"].Value.ToString();
                txtMaNV.Text = dataGridView1.Rows[e.RowIndex].Cells["manv"].Value.ToString();
                dtGiovao.Value = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["giovao"].Value.ToString());
                dtGiora.Value = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["giora"].Value.ToString());
                dtNgay.Value = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["ngay"].Value.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn dòng" + ex.Message);
            }
        }
        private bool IsInputValid()
        {
            // Kiểm tra Mã chấm công
            if (string.IsNullOrWhiteSpace(txtMaCC.Text) || !Regex.IsMatch(txtMaCC.Text, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Mã chấm công không được để trống và chỉ chứa ký tự chữ hoặc số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMaCC.Focus();
                return false;
            }

            // Kiểm tra Mã nhân viên
            if (string.IsNullOrWhiteSpace(txtMaNV.Text))
            {
                MessageBox.Show("Mã nhân viên không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMaNV.Focus();
                return false;
            }

            // Kiểm tra Ngày
            if (dtNgay.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("Ngày chấm công không được lớn hơn ngày hiện tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dtNgay.Focus();
                return false;
            }

            // Kiểm tra Giờ vào
            DateTime GioVao = dtGiovao.Value;
            if (GioVao == null)
            {
                MessageBox.Show("Giờ vào không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dtGiovao.Focus();
                return false;
            }

            // Kiểm tra Giờ ra
            DateTime GioRa = dtGiora.Value;
            if (GioRa == null)
            {
                MessageBox.Show("Giờ ra không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dtGiora.Focus();
                return false;
            }

            // Kiểm tra Giờ ra phải lớn hơn Giờ vào
            if (GioRa <= GioVao)
            {
                MessageBox.Show("Giờ ra phải lớn hơn Giờ vào.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dtGiora.Focus();
                return false;
            }

            return true; // Dữ liệu hợp lệ
        }

        //kiểm tra dữ liệu trùng khi create
        private bool IsDuplicateData(string maNV, string maCC)
        {
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "SELECT COUNT(*) FROM Chamcong WHERE MaNV = @MaNV OR MaCC = @MaCC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                cmd.Parameters.AddWithValue("@MaCC", maCC);

                conn.Open();
                int count = (int)cmd.ExecuteScalar(); // Lấy số lượng bản ghi trùng

                return count > 0; // Trả về true nếu có bản ghi trùng
            }
        }

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


        private void btnLuu_Click(object sender, EventArgs e)
        {
            //gọi hàm kiểm tra tính hợp
            if (!IsInputValid())
            {
                return;// nếu dữ liệu không hợp lệ, dừng việc thêm mới
            }
            //kiểm tra trùng mã chấm công và mã nhân viên
            if (IsDuplicateData(txtMaCC.Text, txtMaCC.Text))
            {
                MessageBox.Show("Mã nhân viên hoặc mã chấm công đã tồn tại! Vui lòng kiểm tra lại.");
                return;//nếu bị trùng, dừng việc thêm mới

            }

            //KTRA CÓ MANV này hay ko, sau đó  mới tiến hành thêm dữ liệu chấm công cho nhân viên.
            if (!IsNhanVienExist(txtMaNV.Text))
            {
                MessageBox.Show("Nhân viên với mã này không tồn tại! Vui lòng kiểm tra lại.");
                txtMaNV.Focus();
                return; // Dừng việc thêm mới
            }

            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "INSERT INTO CHAMCONG (MaCC,MaNV,Ngay,GioVao,GioRa)" +
                               "Values(@MaCC,@MaNV,@Ngay,@Giovao,@Giora)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaCC", txtMaCC.Text);
                cmd.Parameters.AddWithValue("@MaNV", txtMaNV.Text);
                cmd.Parameters.AddWithValue("@Ngay", dtNgay.Value);
                cmd.Parameters.AddWithValue("@Giovao", dtGiovao.Value);
                cmd.Parameters.AddWithValue("@Giora", dtGiora.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Thêm chấm công thành công!");
                LoadData();
            }
        }

        //KTRA TRÙNG DL KHI UPDATE
        private bool IsDuplicateDataForUpdate(string maNV, string maCC)
        {
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "SELECT COUNT(*) FROM CHAMCONG WHERE (MaNV = @MaNV OR MaCC = @MaCC) AND MaCC != @CurrentMaCC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                cmd.Parameters.AddWithValue("@MaCC", maCC);
                cmd.Parameters.AddWithValue("@CurrentMaCC", txtMaNV.Text); // MaCC hiện tại đang được chỉnh sửa

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
            if (IsDuplicateDataForUpdate(txtMaCC.Text, txtMaNV.Text))
            {
                MessageBox.Show("Mã nhân viên hoặc Mã chấm công đã tồn tại! Vui lòng kiểm tra lại.");
                return; // Nếu bị trùng, dừng việc cập nhật
            }
            //Tiến hành cập nhật dữ liệu
            using (SqlConnection conn = new SqlConnection(sCon))
            {
                string query = "UPDATE CHAMCONG SET Ngay = @Ngay, GioVao = @Giovao, GioRa = @Giora " +
                               "WHERE MaNV = @MaNV and MaCC=@MaCC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaCC", txtMaCC.Text);
                cmd.Parameters.AddWithValue("@MaNV", txtMaNV.Text);
                cmd.Parameters.AddWithValue("@Ngay", dtNgay.Value);
                cmd.Parameters.AddWithValue("@Giovao", dtGiovao.Value);
                cmd.Parameters.AddWithValue("@Giora", dtGiora.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Cập nhật thông tin chấm công thành công!");
                LoadData(); // Tải lại dữ liệu sau khi sửa
            }
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem có dòng nào được chọn hay không
                if (string.IsNullOrWhiteSpace(txtMaCC.Text))
                {
                    MessageBox.Show("Vui lòng chọn chấm công cần xóa!");
                    return;
                }

                // Xác nhận trước khi xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa chấm công này không?",
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
                    string query = "DELETE FROM CHAMCONG WHERE MaNV = @MaNV AND MaCC = @MaCC";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    // Thêm tham số
                    cmd.Parameters.AddWithValue("@MaNV", txtMaNV.Text);
                    cmd.Parameters.AddWithValue("@MaCC", txtMaCC.Text);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery(); // Lấy số dòng bị ảnh hưởng

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa chấm công thành công!");
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
                MessageBox.Show("Lỗi khi xóa chấm công: " + ex.Message);
            }
        }

        //Hàm xóa dl trên các ô của Form
        private void ClearInputFields()
        {
            txtMaCC.Text = string.Empty;
            txtMaNV.Text = string.Empty;
            dtNgay.Value = DateTime.Now;
            dtGiovao.Value = DateTime.Now;
            dtGiora.Value = DateTime.Now;
        }
    }
}
