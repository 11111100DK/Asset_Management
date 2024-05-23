using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsserManagement
{

    public partial class Notice : UserControl
    {
        private bool isSelected;
        public event EventHandler UserControlClicked; // Khai báo sự kiện click cho UserControl

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
  

                UpdateAppearance();
            }
        }
        public event EventHandler ItemClick;

        public Notice()
        {
            InitializeComponent();
            this.MouseEnter += MyUserControl_MouseEnter;
            this.MouseLeave += MyUserControl_MouseLeave;
        }
        public void SetData(string name, string warrantyDate)
        {
            label2.Text = name;
            label4.Text = warrantyDate;
        }
        public void SetImage(string image)
        {
            pictureBox1.ImageLocation = image;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }
        private void UpdateAppearance()
        {
            this.BackColor = isSelected ? Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(230)))), ((int)(((byte)(200))))) : SystemColors.Control;
        }

        private void YourUserControl_Click(object sender, EventArgs e)
        {
            IsSelected = !IsSelected;
        }
        private void MyUserControl_MouseEnter(object sender, EventArgs e)
        {
            // Thay đổi giao diện của UserControl khi hover
            this.BackColor = Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(210)))), ((int)(((byte)(159)))));
        }

        // Sự kiện xảy ra khi con trỏ chuột rời khỏi UserControl
        private void MyUserControl_MouseLeave(object sender, EventArgs e)
        {
            // Đặt lại giao diện của UserControl khi không hover
            this.BackColor = Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(230)))), ((int)(((byte)(200)))));
        }
        private void Notice_Load(object sender, EventArgs e)
        {

        }
        private void MyUserControl_Click(object sender, EventArgs e)
        {
            // Kích hoạt sự kiện UserControlClicked nếu đã đăng ký
            UserControlClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
