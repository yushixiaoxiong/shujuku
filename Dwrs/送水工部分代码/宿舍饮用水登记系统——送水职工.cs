using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;

namespace 宿舍饮用水登记系统
{
    public partial class 宿舍饮用水登记系统__送水职工 : Form
    {
        public const int BufferSize = 1024;    //缓存大小
        int t = 0;
        更改信息 ChangeInformation = null;
        /*private void CloseProcess(string name)
        {
            try
            {
                System.Diagnostics.Process[] myProcesses = System.Diagnostics.Process.GetProcesses();
                foreach (System.Diagnostics.Process myProcess in myProcesses)
                {
                    if (name == myProcess.ProcessName)
                        myProcess.Kill();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }*/

        //以ASCII编码的发送信息
        public void AsciiGetBytesSend(NetworkStream ns, string str)
        {
            byte[] byteTime = Encoding.ASCII.GetBytes(str);
            try
            {
                ns.Write(byteTime, 0, byteTime.Length);
            }
            catch (Exception)
            {
                MessageBox.Show("向服务器发送数据时出错！");
            }
        }

        //以ASCII编码的读取数据
        public string AsciiGetstringRead(NetworkStream ns)
        {
            byte[] bytes = new byte[BufferSize];
            int bytesRead = ns.Read(bytes, 0, bytes.Length);
            return Encoding.ASCII.GetString(bytes, 0, bytesRead);
        }

        宿舍饮用水登记系统.登录 land;
        NetworkStream ns = null;
        Workers worker;
        string account, name, ipAPort,balance,totalRecharge;
        public 宿舍饮用水登记系统__送水职工(Workers worker,NetworkStream ns, 宿舍饮用水登记系统.登录 land, string account)
        {
            InitializeComponent();
            this.land = land;
            this.worker = worker;
            this.ns = ns;
            this.account = account;

            //向服务器发送操作号
            //0表示验证---用户---用户名和密码是否正确
            //1表示---用户---返回窗口初始化信息(用户名和密码)
            //2表示---送水职工---返回窗口初始化信息（用户名，姓名，ip以及端口）
            //3表示验证---送水职工---用户名和密码是否正确
            //4表示---用户---注册
            //5表示---送水职工---注册
            //6表示---用户---查询余额
            //7表示---送水职工---查询账户信息
            //8表示---用户---查询当前订单
            //9表示---用户---查询送水记录
            //10表示---送水职工---查询当前订单
            //11表示---送水职工---查询送水记录
            //12表示---用户---订水
            //13表示查询单价
            //14表示查询---用户---默认寝室号
            //15表示---用户---查询当前订单号
            //16表示---用户---取消订单
            //17表示---用户---更改送水时间段
            //18表示充值
            //19表示---用户---更改密码
            //20表示---送水职工---更改密码
            //21表示---送水职工---更改单价
            //22表示---送水职工---根据订单号查询订水寝室号和定水量，以及更改订单的送水账户、送水人及送水状态信息
            //23表示---送水职工---查询当前未开始送水的订单号
            //24表示---用户---查询当前正在送水的订单号
            //25表示---用户---确认已送水
            //26表示---送水职工---查询职工密钥
            //27表示---送水职工---更改职工密钥
            AsciiGetBytesSend(ns, "2");

            //向服务器发送用户名
            AsciiGetBytesSend(ns, account);

            byte[] bytes2 = new byte[BufferSize];
            int bytesRead2 = ns.Read(bytes2, 0, bytes2.Length);
            name = Encoding.Unicode.GetString(bytes2, 0, bytesRead2);
            name = name.Substring(0, name.Length - 1);
            //byte[] bytes3 = new byte[BufferSize];
            //int bytesRead3 = ns.Read(bytes3, 0, bytes3.Length);
            //ipAPort = Encoding.ASCII.GetString(bytes3, 0, bytesRead3);
            ipAPort = AsciiGetstringRead(ns);

            label1.Text = "用户名：" + account + " 姓名：" + name + " 服务器IP：" + ipAPort;
        }

        private void 宿舍饮用水登记系统__送水职工_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //更改账户
            DialogResult a = MessageBox.Show("您确定要退出，更换其他账号登陆吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            if (a == System.Windows.Forms.DialogResult.OK)
            {
                //CloseProcess("宿舍饮用水登记系统");
                t=1;
                this.Close();
                land.Show();
            }
        }

        private void 宿舍饮用水登记系统__送水职工_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (t != 1 && ChangeInformation != null)
                t = ChangeInformation.Ti;
            if (t == 0)
            {
                DialogResult a = MessageBox.Show("您确定要退出吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                if (a == System.Windows.Forms.DialogResult.OK)
                {
                    t++;
                    Application.Exit();
                }
                else
                    e.Cancel = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //向服务器发送操作号
            //0表示验证---用户---用户名和密码是否正确
            //1表示---用户---返回窗口初始化信息(用户名和密码)
            //2表示---送水职工---返回窗口初始化信息（用户名，姓名，ip以及端口）
            //3表示验证---送水职工---用户名和密码是否正确
            //4表示---用户---注册
            //5表示---送水职工---注册
            //6表示---用户---查询余额
            //7表示---送水职工---查询账户信息
            //8表示---用户---查询当前订单
            //9表示---用户---查询送水记录
            //10表示---送水职工---查询当前订单
            //11表示---送水职工---查询送水记录
            //12表示---用户---订水
            //13表示查询单价
            //14表示查询---用户---默认寝室号
            //15表示---用户---查询当前订单号
            //16表示---用户---取消订单
            //17表示---用户---更改送水时间段
            //18表示充值
            //19表示---用户---更改密码
            //20表示---送水职工---更改密码
            //21表示---送水职工---更改单价
            //22表示---送水职工---根据订单号查询订水寝室号和定水量，以及更改订单的送水账户、送水人及送水状态信息
            //23表示---送水职工---查询当前未开始送水的订单号
            //24表示---用户---查询当前正在送水的订单号
            //25表示---用户---确认已送水
            //26表示---送水职工---查询职工密钥
            //27表示---送水职工---更改职工密钥
            AsciiGetBytesSend(ns, "7");

            AsciiGetBytesSend(ns, account);

            //接收账户信息
            balance = AsciiGetstringRead(ns);

            totalRecharge = AsciiGetstringRead(ns);
            MessageBox.Show("您的账户（" + account + "）的账户金额为：" + balance + "元！\r充值总额为：" + totalRecharge + "元！");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = worker.QrecordsNow(ns).Tables["Orders"];
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = worker.Qrecords(ns, account).Tables["Orders"];
        }

        private void button7_Click(object sender, EventArgs e)
        {
            充值 Recharge = new 充值(ns, account);
            Recharge.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ChangeInformation = new 更改信息(ns, account,land, this);
            ChangeInformation.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            确认送水 PreparedBottled = new 确认送水(ns, account);
            PreparedBottled.Show();
        }
    }
}
