using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;

namespace 宿舍饮用水登记系统
{
    public class Workers
    {
        public const int BufferSize = 1024;    //缓存大小

        //private string password;    //密码
        //private string no;          //职工号
        public string account;      //用户名
        public string name;         //姓名
        public float balance;       //账户金额
        public float totalRecharge; //充值总额

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

        public int Initialization(NetworkStream ns, string account, string password)    //对象的初始化,1表示成功，0表示失败
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
            AsciiGetBytesSend(ns, "3");

            //向服务器发送用户名
            AsciiGetBytesSend(ns, account);
            if (AsciiGetstringRead(ns) == "1")
            {
                //向服务器发送密码
                AsciiGetBytesSend(ns, password);
            }

            //从服务器接收信息
            if (AsciiGetstringRead(ns) == "1")
                return 1;
            else
                return 0;
        }

        public int Recharge(string Uaccount, float money)  //充值
        {
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            conn.ConnectionString = "Data Source=dell-pc;Initial catalog=Dwrs;Integrated Security=SSPI";
            string sql0 = "select  账户余额 from Users where 用户名='" + Uaccount + "'";
            SqlCommand command0 = new SqlCommand(sql0, conn);
            conn.Open();
            if (command0.ExecuteScalar() == null)
            {
                conn.Close();
                return 0; 
            }
            else
            {
                totalRecharge = totalRecharge + money;
                string sql = "update Users set 账户余额=账户余额+" + money + " where 用户名='" + Uaccount + "'";
                string sql1 = "update Workers set 充值总额=充值总额+" + money + " where 用户名='" + account + "'";
                SqlCommand command = new SqlCommand(sql, conn);
                SqlCommand command1 = new SqlCommand(sql1, conn);
                command.ExecuteScalar();
                command1.ExecuteScalar();
                conn.Close();
                return 1;
            }
        }

        public DataSet QrecordsNow(NetworkStream ns)   //查询当前订单
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
            AsciiGetBytesSend(ns, "10");

            DataSet myst;
            myst = new DataSet();

            byte[] bytes = new byte[BufferSize * 64];
            int bytesRead = ns.Read(bytes, 0, bytes.Length);
            myst = DataSetDeserialize(bytes);
            return myst;

        }

        static DataSet DataSetDeserialize(byte[] bytes)
        {
            System.IO.MemoryStream memStream = new MemoryStream(bytes);

            memStream.Position = 0;

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter deserializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            DataSet ds;
            try
            {
                ds = (DataSet)deserializer.Deserialize(memStream); //反序列化 
                memStream.Close();
                return ds;
            }
            catch (Exception)
            {
                memStream.Close();
                MessageBox.Show("反序列化数据时出错！");
                return null;
            }
        }

        public DataSet Qrecords(NetworkStream ns, string account)   //查询送水记录
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
            AsciiGetBytesSend(ns, "11");

            DataSet myst;
            myst = new DataSet();

            //向服务器发送用户名
            
            AsciiGetBytesSend(ns, account);

            byte[] bytes = new byte[BufferSize * 64];
            int bytesRead = ns.Read(bytes, 0, bytes.Length);
            myst = DataSetDeserialize(bytes);
            return myst;
        }
    }
}
