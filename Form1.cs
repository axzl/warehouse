using System;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace 元器件管理
{
    public partial class Form1 : Form
    {//action, compon, number
        string[] items_category = { "元器件", "模块", "成品" };
        string[] items_other = {"用户", "日志" };
        string[][] items_family; 
        string[] items_compon = { "Induct_1mH", "Resist_10", "Resist_100", "Resist_1k" };
        string[] items_module = { "ESP8266", "HC-12", "HC-18", "LORA" };
        string[] items_product = { "GeekSatAH", "GeekSatBJ", "GeekSatHA", "GeekSatTJ" };
        string[] items_action = { "增加", "减少", "设置" ,"出货", "退货"};
        string[] items_number = { "10", "20", "50", "100", "200", "500", "1000", "2000", "5000" };

        string db = "server=LAPTOP-IJDV6JIH\\SQLEXPRESS;uid=user;pwd=user;database=mydb";

        int connected = 1; //未连接
        int init_Height;
        int state = 0; //正常

        public Form1()
        {
            InitializeComponent();
            init_Height = label2.Location.Y;
            initial();
        }
        public void initial()
        {
            items_family = new string[items_category.Length][];
            items_family[0] = items_compon;
            items_family[1] = items_module;
            items_family[2] = items_product;
            comboBox4.Items.AddRange(items_category);
            comboBox1.Items.AddRange(items_family[0]);
            comboBox2.Items.AddRange(items_action);
            comboBox3.Items.AddRange(items_number);
            comboBox1.Text = comboBox1.Items[0].ToString();
            comboBox2.Text = comboBox2.Items[0].ToString();
            comboBox3.Text = comboBox3.Items[0].ToString();
            comboBox4.Text = comboBox4.Items[0].ToString();
            
            //this.Height = init_Height + label3.Height + 9;
        }
        
        public int checkUser(string usename, string password)
        {
            string sql_select_user = "SELECT * FROM 用户表";
            string sql_select_all = "SELECT * FROM 元器件表 WHERE 类型 = '" + comboBox4.Text + "'";
            string[] userpass = MyData.select_user(db, sql_select_user).Split(';');
            string[] user = userpass[0].Split(',');
            string[] pass = userpass[1].Split(',');
            for (int i = 0; i < user.Length - 1; i++)
            {
                //Console.WriteLine(user[i].Trim()+" "+pass[i].Trim());
                if (user[i].Trim() == usename && pass[i].Trim() == password)
                {
                    display(MyData.select(db, sql_select_all));
                    Console.WriteLine("abc"+ comboBox1.Text);
                    Text = usename;
                    connected = 0;
                    return 0;
                }
            }
            return 1;
        }

        private string[][] addItems(string compon, string number,string type)
        {
            /*string sql_insert = "INSERT INTO 元器件表 VALUES('ESP8268', '800', '670')"; //t1(a1, a2) 
            string sql_delete = "DELETE FROM 元器件表 WHERE 库存数量 = 'ooo'";
            string sql_update = "UPDATE 元器件表 SET 库存数量 = '1200',下一编号 = '1' WHERE 元器件名 = 'ESP8268'";
            */
            string sql_select_all = "SELECT * FROM 元器件表";
            string sql_select = "SELECT * FROM 元器件表 WHERE 元器件名 = '" + compon + "'";
            string oldnum = MyData.select_number(db, sql_select);
            int newnum = Convert.ToInt32(oldnum) + Convert.ToInt32(number);
            string sql_insert_00 = "INSERT INTO 元器件表 VALUES('" + type + "','" + compon + "','" + newnum.ToString() + "','1')";
            string sql_update_00 = "UPDATE 元器件表 SET 库存数量 = '" + newnum.ToString() + "' WHERE 元器件名 = '" + compon + "'";
            MyData.update(db, MyData.select_bool(db, sql_select) == "True" ? sql_update_00 : sql_insert_00);
            state = 0;//执行完成
            return MyData.select(db, sql_select_all);
        }

        private string[][] minusItems(string compon = "", string number = "")
        {
            string sql_select_all = "SELECT * FROM 元器件表";
            string sql_select = "SELECT * FROM 元器件表 WHERE 元器件名 = '" + compon + "'";
            string oldnum = MyData.select_number(db, sql_select);
            int newnum = Convert.ToInt32(oldnum) - Convert.ToInt32(number);
            string sql_update_01 = "UPDATE 元器件表 SET 库存数量 = '" + newnum.ToString() + "' WHERE 元器件名 = '" + compon + "'";
            if (newnum >= 0) //可以减少
            {
                if (MyData.select_bool(db, sql_select) == "True") //有此元器件
                {
                    MyData.update(db, sql_update_01);
                    state = 0;
                }
            }
            return MyData.select(db, sql_select_all);
        }
        private string[][] setItems(string compon, string number, string type)
        {
            string sql_select_all = "SELECT * FROM 元器件表";
            string sql_select = "SELECT * FROM 元器件表 WHERE 元器件名 = '" + compon + "'";
            string sql_insert_02 = "INSERT INTO 元器件表 VALUES('" + type + "','" + compon + "','" + number + "','1')";
            string sql_update_02 = "UPDATE 元器件表 SET 库存数量 = '" + number + "' WHERE 元器件名 = '" + compon + "'";
            MyData.update(db, MyData.select_bool(db, sql_select) == "True" ? sql_update_02 : sql_insert_02);
            state = 0;
            return MyData.select(db, sql_select_all);
        }

        private string addNumber(string compon, string number, string type)
        {//出货
            string sql_select = "SELECT * FROM 元器件表 WHERE 元器件名 = '" + compon + "'";
            string oldnum = MyData.select_next(db, sql_select);
            int newnum = Convert.ToInt32(oldnum) + Convert.ToInt32(number);
            string sql_update_03 = "UPDATE 元器件表 SET 下一编号 = '" + newnum.ToString() + "' WHERE 元器件名 = '" + compon + "'";
            if (Convert.ToInt32(MyData.select_number(db, sql_select)) >= Convert.ToInt32(number)) //可以增加
            {
                Console.WriteLine(MyData.select_number(db, sql_select)+"---"+ number);
                if (MyData.select_bool(db, sql_select) == "True") //有此元器件
                {
                    MyData.update(db, sql_update_03);
                    minusItems(compon, number);
                }
            }
            return oldnum;
        }
        
        private string[][] minusNumber(string compon, string number, string type)
        {//退货
            string sql_select_all = "SELECT * FROM 元器件表";
            string sql_select = "SELECT * FROM 元器件表 WHERE 元器件名 = '" + compon + "'";
            string oldnum = MyData.select_next(db, sql_select);
            int newnum = Convert.ToInt32(oldnum) - Convert.ToInt32(number);
            string sql_update_04 = "UPDATE 元器件表 SET 下一编号 = '" + newnum.ToString() + "' WHERE 元器件名 = '" + compon + "'";
            if (newnum > 0) //可以减少
            {
                if (MyData.select_bool(db, sql_select) == "True") //有此元器件
                {
                    MyData.update(db, sql_update_04);
                    addItems(compon, number, type);
                }
            }
            return MyData.select(db, sql_select_all);
        }

        private void writeNumber(string compon = "", string oldnum = "")
        {
            if (state > 0) return;
            string sql_select = "SELECT * FROM 元器件表 WHERE 元器件名 = '" + compon + "'";
            string content = DateTime.Now.ToString() + "\n";
            int newnum = Convert.ToInt32(MyData.select_next(db, sql_select));
            for (int i = Convert.ToInt32(oldnum); i < newnum; i++)
            {
                content += compon + "_" + i.ToString().PadLeft(4).Replace(' ', '0') + "\n";
            }
            Console.WriteLine(content);
            MyFile.writeline("出货编号.txt", content);
        }

        private void findClass(string type)
        {
            string sql_select_all = "SELECT * FROM 元器件表 WHERE 类型 = '" + type + "'";
            string compon_all = MyData.select_name(db, sql_select_all);
            string[] compon_arr = compon_all.Split(',');
            for (int i = 0; i < compon_arr.Length - 1; i++)
            {
                comboBoxInit(comboBox1, compon_arr[i].Trim());
            }
        }

        private string[][] dbProcess(string action, string compon, string number, string type)
        {
            string[] items_action = { "增加", "减少", "设置", "出货", "退货" };
            state = 1;
            switch (action)
            {
                case "增加": addItems(compon, number, type); break;
                case "减少": minusItems(compon, number); break;
                case "设置": setItems(compon, number, type); break;
                case "出货": writeNumber(compon, addNumber(compon, number, type)); break;
                case "退货": minusNumber(compon, number, type); break;
                default: break;
            }
            return MyData.select(db, "SELECT * FROM 元器件表 WHERE 类型 = '"+ type + "'");
        }
        private void message(string[][] value)
        {
            display(value);
            if (state > 0)
            {
                MessageBox.Show("操作失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //MessageBox.Show("操作完成！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private bool isnumeric(string str)
        {
            char[] ch = str.ToCharArray();
            for (int i = 0; i< ch.Length; i++)
            {
                if (ch[i] < 48 || ch[i] > 57)
                    return false;
            }
            return true;
        }

        private void comboBoxInit(ComboBox cb, string value, bool isnum = false)
        {
            if (value == "") return;
            if (!(cb.Items.Contains(value)))
            {
                for(int i = 0; i < cb.Items.Count; i++)
                {
                    string item = cb.Items[i].ToString();
                    if (isnumeric(item))
                    {
                        if (Convert.ToInt32(item) > Convert.ToInt32(value))
                        {
                            cb.Items.Insert(i, value);
                            return;
                        }
                    }
                    else if(item.CompareTo(value) > 0)
                    {
                        cb.Items.Insert(i, value);
                        return;
                    }
                }
                cb.Items.Add(value);
            }
        }

        private void log(string compon, string action, string number, string user)
        {
            string now = DateTime.Now.ToString();
            string sql_insert_log = "INSERT INTO 日志表 VALUES('"+ now +"', '" + compon + "', '" + action + "', '" + number + "', '"+ user + "')";
            MyData.update(db, sql_insert_log);
        }

        private void button1_Click(object sender, EventArgs e)
        {//提交
            string compon = comboBox1.Text; //元器件名
            string action = comboBox2.Text; //元器件操作
            string number = comboBox3.Text; //元器件数量
            string type = comboBox4.Text; //元器件类型
            comboBoxInit(comboBox1, compon.Trim());
            comboBoxInit(comboBox2, action.Trim());
            comboBoxInit(comboBox3, number.Trim());
            string mess = "库存中，" + compon + "将" + action + number + "个，确定要继续？";
            DialogResult result = MessageBox.Show(mess, "执行", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.OK == result)
            {
                message(dbProcess(action, compon, number, type));
                log(compon, action, number, Text);
                //this.Height = init_Height + label3.Height + 10;
            } 
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("将退出登录并关闭程序，确定要继续？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            e.Cancel = DialogResult.Cancel == result;   // Cancel属性如果为true，表示取消该事件的执行。
            if (!e.Cancel)
            {
                Dispose();
                Environment.Exit(0);
            } 
        }

        private void comboBox4_TextChanged(object sender, EventArgs e)
        {
            if (connected > 0) return;
            string sql_select_all = "SELECT * FROM 元器件表 WHERE 类型 = '" + comboBox4.Text + "'";
            comboBox1.Items.Clear();
            for(int n = 0; n < items_category.Length; n++)
            {
                if (comboBox4.Text == items_category[n])
                {
                    comboBox1.Items.AddRange(items_family[n]);
                    findClass(items_category[n]);
                    break;
                }
            }
            display(MyData.select(db, sql_select_all));
            comboBox1.Text = comboBox1.Items[0].ToString();
            comboBox2.Text = comboBox2.Items[0].ToString();
            comboBox3.Text = comboBox3.Items[0].ToString();
            //this.Height = init_Height + label3.Height + 10;
        }

        private void 查询库存FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //groupBox1.Visible = !groupBox1.Visible;
        }

        private void display(string[][] value)
        {
            dataGridView1.Rows.Clear();
            for (int m = 0; m < value.Length; m++)
            {
                int index = dataGridView1.Rows.Add();
                for (int n = 0; n < value[0].Length; n++)
                {
                    dataGridView1.Rows[m].Cells[n].Value = n > 1 ? value[m][n].Trim().PadLeft(8): value[m][n].Trim();
                }
            }
        }

        private void 填物料表AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
        }
    }

    class MyFile
    {
        public static int writeline(string file, string content)
        {
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(@file, System.IO.FileMode.Append);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                sw.WriteLine(content);
                sw.Close();
                fs.Close();//这里要注意fs一定要在sw后面关闭，否则会抛异常
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("MyFile.writeline():" + ex.Message);
            }
            return 1;
        }

        public static string readline(string file)
        {
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(@file, System.IO.FileMode.Open);
                System.IO.StreamReader sr = new System.IO.StreamReader(fs);
                string line = sr.ReadLine();//直接读取一行
                sr.Close();
                fs.Close();
                return line;
            }
            catch (Exception ex)
            {
                Console.WriteLine("MyFile.readline():" + ex.Message);
            }
            return null;
        }
    }

    class MyData
    {
        public static string[][] select(string db, string sql_query)
        {//二维数据
            string[][] value = null;
            System.Collections.ArrayList array = new System.Collections.ArrayList();
            SqlConnection MyConnection = new SqlConnection(db);
            MyConnection.Open();
            try
            {
                SqlCommand MyCommand = new SqlCommand(sql_query, MyConnection);
                SqlDataReader sdr = MyCommand.ExecuteReader();
                while (sdr.Read())
                {
                    string[] temp2 = new string[sdr.FieldCount];
                    for (int i = 0; i < sdr.FieldCount; i++)
                    {
                        temp2[i] = sdr[i].ToString();
                    }
                    array.Add(temp2);
                }
                value = new string[array.Count][];
                for (int n = 0; n < value.Length; n++)
                {
                    value[n] = array[n] as string[];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MyDate.select():" + ex.Message);
            }
            MyConnection.Close();
            //Console.WriteLine(result);
            return value;
        }

        public static string select_bool(string db, string sql_query)
        {//是否有数据
            SqlConnection MyConnection = new SqlConnection(db);
            string result = string.Empty;
            try
            {
                MyConnection.Open();
                if (MyConnection.State == System.Data.ConnectionState.Open)
                {
                    //Console.WriteLine("select:" + sql_query);
                    SqlCommand MyCommand = new SqlCommand(sql_query, MyConnection);
                    SqlDataReader sdr = MyCommand.ExecuteReader();
                    result = sdr.HasRows.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MyDate.select_bool():" + ex.Message);
            }
            MyConnection.Close();
            //Console.WriteLine(result);
            return result;
        }

        public static string select_number(string db, string sql_query)
        {//元器件数量
            SqlConnection MyConnection = new SqlConnection(db);
            string result = string.Empty;
            try
            {
                MyConnection.Open();
                if (MyConnection.State == System.Data.ConnectionState.Open)
                {
                    //Console.WriteLine("select:" + sql_query);
                    SqlCommand MyCommand = new SqlCommand(sql_query, MyConnection);
                    SqlDataReader sdr = MyCommand.ExecuteReader();
                    result = sdr.Read() ? sdr[2].ToString() : "0";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MyDate.select_number():" + ex.Message);
            }
            MyConnection.Close();
            //Console.WriteLine(result);
            return result;
        }

        public static string select_next(string db, string sql_query)
        {//下一编号
            SqlConnection MyConnection = new SqlConnection(db);
            string result = string.Empty;
            try
            {
                MyConnection.Open();
                if (MyConnection.State == System.Data.ConnectionState.Open)
                {
                    //Console.WriteLine("select:" + sql_query);
                    SqlCommand MyCommand = new SqlCommand(sql_query, MyConnection);
                    SqlDataReader sdr = MyCommand.ExecuteReader();
                    result = sdr.Read() ? sdr[3].ToString() : "1";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MyDate.select_next():" + ex.Message);
            }
            MyConnection.Close();
            //Console.WriteLine(result);
            return result;
        }

        public static string select_name(string db, string sql_query)
        {//元器件种类
            SqlConnection MyConnection = new SqlConnection(db);
            string result = string.Empty;
            try
            {
                MyConnection.Open();
                if (MyConnection.State == System.Data.ConnectionState.Open)
                {
                    //Console.WriteLine("select:" + sql_query);
                    SqlCommand MyCommand = new SqlCommand(sql_query, MyConnection);
                    SqlDataReader sdr = MyCommand.ExecuteReader();
                    while (sdr.Read())
                    {
                        result += sdr[1].ToString() + ",";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MyDate.select_name():" + ex.Message);
            }
            MyConnection.Close();
            //Console.WriteLine(result);
            return result;
        }

        public static string select_user(string db, string sql_query)
        {//用户名，密码
            SqlConnection MyConnection = new SqlConnection(db);
            string result = string.Empty;
            try
            {
                MyConnection.Open();
                if (MyConnection.State == System.Data.ConnectionState.Open)
                {
                    //Console.WriteLine("select:" + sql_query);
                    SqlCommand MyCommand = new SqlCommand(sql_query, MyConnection);
                    SqlDataReader sdr = MyCommand.ExecuteReader();
                    string resul2 = "";
                    while (sdr.Read())
                    {
                        result += sdr[0].ToString() + ",";
                        resul2 += sdr[1].ToString() + ",";
                    }
                    result += ";" + resul2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MyDate.select_user():" + ex.Message);
            }
            MyConnection.Close();
            //Console.WriteLine(result);
            return result;
        }

        public static void update(string db, string sql_query)
        {
            SqlConnection MyConnection = new SqlConnection(db);
            try
            {
                MyConnection.Open();
                if (MyConnection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand MyCommand = new SqlCommand(sql_query, MyConnection);
                    MyCommand.ExecuteNonQuery();
                    //Console.WriteLine("commit:"+ sql_query);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("MyDate.update():"+ ex.Message);
            }
            MyConnection.Close();
        }
    }
}
