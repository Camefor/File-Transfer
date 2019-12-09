using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Transfer
{
    public partial class Form1 : Form
    {
        FolderBrowserDialog fbd;
        bool IsOpen = false; //指示是否选择路径
        string Path = "";
        string Url = "";
        int Seconds;
        string File = @".\Data\data.ini";

        //public static StringBuilder sb = new StringBuilder();
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            Path = Win32API.INIGetStringValue(File, "txt", "path", @".\Data\data.ini");
            lblPath.Text = "文件要发送的服务端地址: " + "(上次保存的数据) " + Path;


            textBox1.Text = Win32API.INIGetStringValue(File, "txt", "url", "http://47.105.203.240:85/api/File/PostFile");
            Url = Win32API.INIGetStringValue(File, "txt", "url", "http://47.105.203.240:85/api/File/PostFile");
            lblUrl.Text = "文件要发送的服务端地址: " + "(上次保存的数据)" + Url;


            txtTimer.Text = (Convert.ToInt32(Win32API.INIGetStringValue(File, "txt", "time", "5")) / 1000).ToString();

            int.TryParse(txtTimer.Text, out Seconds);
            Seconds = Seconds * 1000;
            if (Path != null && Url != null)
            {
                button2_Click(null, null);
            }
            else
            {
                MessageBox.Show("请设置参数并点击确定开始监控~");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //openFileDialog1 = new OpenFileDialog();
            folderBrowserDialog1 = new FolderBrowserDialog();
            fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                Path = fbd?.SelectedPath;
                if (Path != null)
                {
                    IsOpen = true;
                }

                //打开目录对话框所选择的路径是  SelectedPath
            }
            else
            {
                IsOpen = false;
            }

            textBox1.Enabled = true;
            txtTimer.Enabled = true;
        }



        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsOpen &&
              string.IsNullOrEmpty(textBox1.Text) &&
              string.IsNullOrEmpty(txtTimer.Text))
                {
                    MessageBox.Show("请检查您的输入~", "出错!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Application.Exit();
                    System.Environment.Exit(0);
                }




                string _input = txtTimer.Text.Trim(); //秒数 1 , 2 ,3 , 4 ,5 
                if (_input == "0")
                {
                    _input = "1";
                }
                Seconds = 1; //默认值一秒
                if (int.TryParse(_input, out Seconds))
                {
                    Seconds = Seconds * 1000;
                }
                else
                {
                    Seconds = 5000;
                }




                //MessageBox.Show("保存正常~");
                textBox1.Enabled = false;
                txtTimer.Enabled = false;
                File = @".\Data\data.ini";
                Win32API.INIWriteValue(File, "txt", "path", Path);
                Win32API.INIWriteValue(File, "txt", "url", Url);
                Win32API.INIWriteValue(File, "txt", "time", Seconds.ToString());

                //sb.AppendFormat("已设置要监控的文件夹: {0} \n", Path);
                //sb.AppendFormat("已设置接收端服务器地址: {0}\n", Url);
                //sb.AppendLine("开始执行监控文件夹传输~");
                //RichTextBox.Text = sb?.ToString();

                //timer1 = new Timer();
                timer1.Interval = Seconds;
                timer1.Enabled = true;
                timer1.Start();
                //timer1_Tick(null, null); //定时监控文件夹
                if (sender != null && e != null)
                {

                    MessageBox.Show("保存正常~", "确认", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //this.Visible = false;
                }
                else
                {
                    //this.Visible = false;
                }


            }



            catch (Exception ex)
            {
                MessageBox.Show("出错了，请重试!\n" + ex.Message);
            }
        }

        private void txtTimer_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar != '\b')//这是允许输入退格键
                {
                    if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                    {
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            FileDTTool.WatcherStrat(Path, Url);

            //sb.Clear();
        }

        private void button1_Leave(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Path))
                {
                    lblPath.Text = "选择的路径: " + Path;
                }
                else
                {
                    lblPath.Text = "选择的路径: " + "暂无";
                }

                //sb.AppendFormat("已设置要监控的文件夹: {0} \n", Path);
                //RichTextBox.Text = sb?.ToString();
            }
            catch (Exception ex)
            {
                //RichTextBox.Text = ex.Message;
                //MessageBox.Show(ex.Message);
            }

        }

        private void textBox1_Leave(object sender, EventArgs e)
        {

            try
            {
                var _url = textBox1.Text.Trim();
                if (!string.IsNullOrEmpty(_url) && _url.Contains("/api/"))
                {
                    Url = _url;
                }

                if (!string.IsNullOrEmpty(Url))
                {

                    lblUrl.Text = "文件要发送的服务端地址: " + Url;

                }
                else
                {
                    lblUrl.Text = "文件要发送的服务端地址: " + "暂无";
                }

                //sb.AppendFormat("已设置接收端服务器地址: {0}\n", Url);
                //RichTextBox.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                //RichTextBox.Text = ex.Message;
            }

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            this.Visible = true;
        }
    }
}
