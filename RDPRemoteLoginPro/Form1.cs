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
        private string _filename;

        private string _address;
        private string _linkName;
        private string _username;
        private string _password;
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
            _address = ConfigHelper.GetAppConfig("serverIP");
            _linkName = ConfigHelper.GetAppConfig("linkName");
            _username = ConfigHelper.GetAppConfig("user");
            _password = ConfigHelper.GetAppConfig("password");
            AddressComboBox.Text = _address;
            UsernameTextBox.Text = _username;
            PasswordTextBox.Text = _password;
            NameTextBox.Text = _linkName;
        }
        private void SaveConfig()
        {
            ConfigHelper.UpdateAppConfig("serverIP", AddressComboBox.Text);
            ConfigHelper.UpdateAppConfig("linkName", NameTextBox.Text);
            ConfigHelper.UpdateAppConfig("user", UsernameTextBox.Text);
            ConfigHelper.UpdateAppConfig("password", PasswordTextBox.Text);
            ConfigHelper.UpdateAppConfig("linkName", NameTextBox.Text);
        }



        private void Connect()
        {
            if (string.IsNullOrEmpty(AddressComboBox.Text) ||
                string.IsNullOrEmpty(UsernameTextBox.Text) ||
            string.IsNullOrEmpty(PasswordTextBox.Text) ||
            string.IsNullOrEmpty(NameTextBox.Text.Trim())
                )
            {

                MessageBox.Show(@"请检查配置！");
                return;
            }

            if (!AddressComboBox.Items.Contains(_address))
            {
                AddressComboBox.Items.Add(_address);
            }


            var TemplateStr = RDPRemoteLoginPro.Properties.Resources.TemplateRDP;//获取RDP模板字符串
            //用DataProtection加密密码,并转化成二进制字符串
            var pwstr = BitConverter.ToString(DataProtection.ProtectData(Encoding.Unicode.GetBytes(PasswordTextBox.Text), ""));
            pwstr = pwstr.Replace("-", "");
            //替换模板里面的关键字符串,生成当前的drp字符串
            var NewStr = TemplateStr.Replace("{#address}", AddressComboBox.Text).Replace("{#username}", UsernameTextBox.Text).Replace("{#password}", pwstr);
            //将drp保存到文件，并放在程序目录下，等待使用
            _filename = NameTextBox.Text + "0" + ".rdp";
            StreamWriter sw = new StreamWriter(_filename);
            sw.Write(NewStr);
            sw.Close();
            //利用CMD命令调用MSTSC
            ProcCmd("mstsc -admin " + _filename);

            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Minimized;//使当前窗体最小化
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProcCmd(@"D:\soft\cmder\Cmder.exe " + AddressComboBox.Text);
        }
    }
}
