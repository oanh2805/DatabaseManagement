using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NHANSU_ChickyLicky
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void btnNhanVien_Click(object sender, EventArgs e)
        {
           NhanVien nv = new NhanVien();
            nv.Show();

        }

        private void btnChucVu_Click(object sender, EventArgs e)
        {
            ChucVu cv = new ChucVu();
            cv.Show();
        }

        private void btnChamCong_Click(object sender, EventArgs e)
        {
            ChamCong cc = new ChamCong();
            cc.Show();
        }

        private void btnLuong_Click(object sender, EventArgs e)
        {
            Luong l = new Luong();
            l.Show();
        }

        private void btnTaiKhoan_Click(object sender, EventArgs e)
        {
            TaiKhoan tk = new TaiKhoan();
            tk.Show();
        }

        private void Menu_Load(object sender, EventArgs e)
        {

        }
    }
}
