using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using RestSharp;

namespace ConsoleApp6
{
    internal class Program
    {
        public static string GenerateMD5(string txt)
        {
            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.Default.GetBytes(txt);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        public static string GetCode(string username, string userwlanip, string wlanacip, string mac)
        {
            //var username = usernameK;
            //var userwlanip = userwlanipK;
            //var wlanacip = wlanacipK;
            //var mac = macK;
            var time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            var secret = "Eshore!@#";
		    var version = "214";
            var authenticatorToMd5 = version + userwlanip + wlanacip + mac + time + secret;
            var authenticator = (GenerateMD5(authenticatorToMd5)).ToUpper();
            var body = @"{""version"":""214"",""username"": """ + username + @""",""clientip"":"""+ userwlanip + @""",""nasip"":""" + wlanacip + @""",""mac"":""" + mac + @""",""timestamp"":" + time + @",""authenticator"":""" + authenticator + @""",""iswifi"":""4060""}";
            var client = new RestClient("http://enet.10000.gd.cn:10001/client/vchallenge");
            client.Timeout = -1;
            client.FollowRedirects = false;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            // var body = @"{""version"":""214"",""username"":""1111"",""clientip"":""10.170.48.215"",""nasip"":""61.146.140.73"",""mac"":""84-A9-38-30-DB-B4"",""timestamp"":1646574938,""authenticator"":""CC1B0FD55B8C896D3D9B190DCEF229A7"",""iswifi"":""4060""}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string resString = (response.Content).ToString();
            string[] codeOne = resString.Split('"');
            var code = codeOne[3];
            return code;
        }
        public static string Login(string username, string userwlanip, string wlanacip, string mac, string code ,string password)
        {
            var time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            var secret = "Eshore!@#";
            var authenticatorToMd5 = userwlanip + wlanacip + mac + time + code +secret;
            var authenticator = (GenerateMD5(authenticatorToMd5)).ToUpper();
            var client = new RestClient("http://125.88.59.131:10001/client/login");
            client.Timeout = -1;
            client.FollowRedirects = false;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{""username"":""" + username + @""",""password"":""" + password + @""",""clientip"":""" + userwlanip + @""",""nasip"":""" + wlanacip + @""",""mac"":"""+ mac +@""",""timestamp"":"+time+@",""authenticator"":"""+authenticator+@""",""iswifi"":""1050"",""verificationcode"":"""+code+@"""}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string resString = (response.Content).ToString();
            string[] codeOne = resString.Split('"');
            var loginRes = codeOne[3];
            return loginRes;
        }
        public static string Logout(string username, string userwlanip, string wlanacip, string mac)
        {
            var time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            var secret = "Eshore!@#";
            var authenticatorToMd5 = userwlanip + wlanacip + mac + time + secret;
            var authenticator = (GenerateMD5(authenticatorToMd5)).ToUpper();
            var client = new RestClient("http://125.88.59.131:10001/client/logout");
            client.Timeout = -1;
            client.FollowRedirects = false;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{""username"":""" + username + @""",""clientip"":""" + userwlanip + @""",""nasip"":""" + wlanacip + @""",""mac"":""" + mac + @""",""timestamp"":" + time + @",""authenticator"":""" + authenticator + @"""}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string resString = (response.Content).ToString();
            string[] codeOne = resString.Split('"');
            var loginRes = codeOne[7];
            return loginRes;
        }
        //public static void ResWriteFile()
        //{
        //    StreamWriter a = new StreamWriter("参考.txt");
        //    a.WriteLine("账号");
        //    //a.Flush();
        //    //a.Close();
        //}
        public static void WriteFile()
        {
            //"账号>填在这里面>"
            string strH = "\n\n\n---------------------------------------------------------------------------\n\n\n";
            StreamWriter streamWriter = new StreamWriter("parameter.txt");
            Console.WriteLine("请输入账号：");
            streamWriter.WriteLine("账号>" + Console.ReadLine() + ">");
            Console.WriteLine("请输入密码：");
            streamWriter.WriteLine("密码>" + Console.ReadLine() + ">");
            Console.WriteLine("请输入拨号设备IP：");
            streamWriter.WriteLine("拨号设备IP>" + Console.ReadLine() + ">");
            Console.WriteLine("请输入服务器IP：");
            streamWriter.WriteLine("服务器IP>" + Console.ReadLine() + ">");
            Console.WriteLine("请输入拨号设备物理地址(Mac)：");
            streamWriter.WriteLine("拨号设备物理地址(Mac)>" + Console.ReadLine() + ">");
            streamWriter.WriteLine("不要删除“ > ” 符号！>改这里面的参数>");
            Console.WriteLine(strH + "请重新打开软件！任意键退出！" + strH);
            Console.ReadKey();
            streamWriter.Flush();
            streamWriter.Close();
        }
        public static void ParameterFile()
        {

        }
        static void Main()
        {
            string strH = "\n\n---------------------------------------------------------------------------\n\n";
            FileInfo file = new FileInfo("parameter.txt");
            if (file.Exists != true)
            {
                Console.WriteLine("正在生成参数文件夹");
                WriteFile();
            }
            else
            {
                StreamReader parameter = new StreamReader("parameter.txt");               
                string paramet = parameter.ReadToEnd();
                parameter.Close();
                if (paramet.Length == 0)
                {
                    Console.WriteLine(strH + "你的配置有误！已为你初始化parameter.txt文件\n您可以在paramer.txt文件内修改\n也可以在下方重新输入配置" + strH);
                    WriteFile();
                    Console.ReadLine();
                }
                else {
                    string[] strarr = paramet.Split(new char[] { '>' });
                    string username = strarr[1];
                    string password = strarr[3];
                    string userwlanip = strarr[5];
                    string wlanacip = strarr[7];
                    string mac = strarr[9];
                    //Console.WriteLine(paramet);
                    Console.WriteLine(strH);
                    Console.WriteLine("请关闭VPN后再运行此软件！否则无法连接学校服务器！");
                    Console.WriteLine(strH);
                    Console.WriteLine("你配置的参数如下：如有错误请在paramer.txt文件内修改");
                    Console.WriteLine("账号:" + username + "\n" + "密码:" + password + "\n" + "拨号设备IP:" + userwlanip + "\n" + "服务器IP:" + wlanacip + "\n" + "拨号设备Mac:" + mac);
                    Console.WriteLine(strH);
                    Console.WriteLine("回车执行登录程序！输入0回车退出登录");
                    if (Console.ReadLine() == "0")
                    {
                        Console.WriteLine("正在退出登录校园网");
                        var res = Logout(username, userwlanip, wlanacip, mac);
                        if (res == "logout success")
                        {
                            Console.WriteLine(strH + "退出成功！" + strH);
                        }
                        else
                        {
                            Console.WriteLine(strH + "您未登录过校园网或其他原因\n服务器返回值为：\n" + res + strH);
                        };
                    }
                    else
                    {
                        var code = GetCode(username, userwlanip, wlanacip, mac);
                        if (code == "-1")
                        {
                            Console.WriteLine(strH + "验证码获取失败!" + strH);
                        }
                        else
                            Console.WriteLine(strH + "正在获取验证码！");
                        Console.WriteLine("获取验证码成功！\n验证码为：" + code);
                        Console.WriteLine("正在登录天翼校园" + strH);
                        var res = Login(username, userwlanip, wlanacip, mac, code, password);
                        if (res == "0")
                        {
                            Console.WriteLine(strH + "登录成功！" + strH);
                        }
                        else
                        {
                            Console.WriteLine(strH + "登录失败！" + strH);
                        };
                    };
                    Console.WriteLine("任意键退出！");
                    Console.ReadKey();
                }
            }
        }
    };
}
