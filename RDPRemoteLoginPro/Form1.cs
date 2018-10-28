using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RDPRemoteLoginPro;

namespace RDPRemoteLoginPro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Connect();
            SaveConfig();


        }
        /// <summary>
        /// 执行命令行命令
        /// </summary> 
        /// <param name="cmd"></param>
        /// <returns></returns>
        void ProcCmd(String cmd)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine(cmd);
            p.StandardInput.WriteLine("exit");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AddressComboBox.Select();
            LoadConfig();
            Connect();
        }

        private void LoadConfig()
        {
          

            address = ConfigHelper.GetAppConfig("serverIP");
            linkName = ConfigHelper.GetAppConfig("linkName");
            username = ConfigHelper.GetAppConfig("user");
            password = ConfigHelper.GetAppConfig("password");
            filename = linkName + "0.rdp";
            AddressComboBox.Text = address;
            UsernameTextBox.Text = username;
            PasswordTextBox.Text = password;
            NameTextBox.Text = linkName;
        }
        private void SaveConfig()
        {
            ConfigHelper.UpdateAppConfig("serverIP", AddressComboBox.Text);
            ConfigHelper.UpdateAppConfig("linkName", "NameTextBox.Text");
            ConfigHelper.UpdateAppConfig("user", UsernameTextBox.Text);
            ConfigHelper.UpdateAppConfig("password", PasswordTextBox.Text);
            ConfigHelper.UpdateAppConfig("linkName", NameTextBox.Text);
        }

        public string filename ;

        public string address ;
        public string linkName ;
        public string username ;
        public string password;

        private void Connect()
        {
            if (string.IsNullOrEmpty(address) ||
                string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(NameTextBox.Text.Trim())
                )
            {

                MessageBox.Show("请检查配置");
                return;
            }
            AddressComboBox.Items.Add(address);

            var TemplateStr = RDPRemoteLoginPro.Properties.Resources.TemplateRDP;//获取RDP模板字符串
            //用DataProtection加密密码,并转化成二进制字符串
            var pwstr = BitConverter.ToString(DataProtection.ProtectData(Encoding.Unicode.GetBytes(password), ""));
            pwstr = pwstr.Replace("-", "");
            //替换模板里面的关键字符串,生成当前的drp字符串
            var NewStr = TemplateStr.Replace("{#address}", address).Replace("{#username}", username).Replace("{#password}", pwstr);
            //将drp保存到文件，并放在程序目录下，等待使用
            StreamWriter sw = new StreamWriter(filename);
            sw.Write(NewStr);
            sw.Close();
            //利用CMD命令调用MSTSC
            ProcCmd("mstsc -admin " + filename);

            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Minimized;//使当前窗体最小化
        }
    }
}
