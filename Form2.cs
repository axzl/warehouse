using System.Windows.Forms;

namespace 元器件管理
{
    public partial class Form2 : Form
    {
        Form1 form1;
        public Form2()
        {
            InitializeComponent();
            form1 = new Form1();
            initial();
        }
        private void initial()
        {
            string[] items = { "车总","林鸿杰","谭力","谢振利","学长","张茜岚"};
            comboBox1.Items.AddRange(items);
        }
        private void button1_Click(object sender, System.EventArgs e)
        {
            string username = comboBox1.Text.Trim();
            string password = textBox1.Text.Trim();
            int connected = form1.checkUser(username, password);
            if(connected < 1) //已连接
            {
                Hide();
                form1.Show();
            }
            else
            {
                MessageBox.Show("用户名或密码错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
