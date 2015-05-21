using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Configuration;
using System.Net.Json;
using CommonLibrary;
using MultimediaSysClient.EntityClasses;
using System.Web.Security;
using System.Threading;
using System.IO;

namespace MultimediaSysClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        AsyncTcpClient client;
        private DispatcherTimer timer_current = new DispatcherTimer(); //定时器
        string DeviceCode = ConfigurationManager.AppSettings["Local_DeviceCode"]; //客户端，终端设备号，对应jjj_TerminalEquipmentList表中的DeviceCode
        int remotePort =Convert.ToInt32( ConfigurationManager.AppSettings["Server_remotePort"]);   //远程段服务器端口
        public byte[] m_BeginMark = new byte[] { (byte)'*' };
        public byte[] m_EndMark = new byte[] { (byte)'#' };

        public static string SessionID_Temp = "";  //每次连接服务器，服务器端返回给客户端的key值，即是服务器端生成的SessionID，用以唯一表示一条连接信息。
        private int count = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        void timer_current_Tick(object sender, EventArgs e)
        {
            try
            {
                //开始获取更新商品分类信息和商品信息_start
                Action action = new Action(
                    delegate()
                    {
                        count += 1;
                        this.lblCount.Dispatcher.Invoke(new Action(() => { lblCount.Content = count + "【" + client.Connected.ToString() + "】"; }));
                        timer_current.Stop(); //暂时先停止此timer，等获取json和下载图片完毕之后再启动此timer                      
                        if (!client.Connected)
                        {
                            this.btnExit.Dispatcher.Invoke(new Action(() => { btnExit.IsEnabled = false; }));
                            this.btnLogin.Dispatcher.Invoke(new Action(() => { btnLogin.IsEnabled = true; }));
                            this.lblStatus.Dispatcher.Invoke(new Action(() => { lblStatus.Content = "已断开和服务器端链接。"; }));
                        }
                    });
                action.BeginInvoke(new AsyncCallback(
                    delegate
                    {
                        timer_current.Start();
                    }),
                    null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer_current.Interval = TimeSpan.FromMilliseconds(1000);
            timer_current.Tick += timer_current_Tick;     
        }

        void client_PlaintextReceived(object sender, ReceivedEventArgs<string> e)
        {
            JsonMutualObject JsonMutualObject1 = new JsonMutualObject();
            this.txtReceive.Dispatcher.Invoke(new Action(() => { txtReceive.Text = e.Datagram;
            if (e.Datagram.IndexOf(ConfigurationManager.AppSettings["CustomTerminatorSign"])>0)  //判断当前服务器端返回的消息是否有终结符，如果没有，则表示信息丢失了，或传输不完整。
            {
                string BackInfo = e.Datagram.Replace(ConfigurationManager.AppSettings["CustomTerminatorSign"], "");
                if (BackInfo.IndexOf("Welcome:") == 0)  //判断是否是连接服务器之后，服务器返回的欢迎消息。
                {
                    string msgTmp = BackInfo.Replace("Welcome:", ""); //Welcome:为和服务器端约定的业务协议简述。然后再把终结符也取消掉。剩下的就是一个标准的json                    
                    BasicMsgEntity deviceLoginEntity = JsonMutualObject1.ScriptDeserialize<BasicMsgEntity>(msgTmp);  //Json字符串转换为类
                    if (deviceLoginEntity != null)
                    {
                        SessionID_Temp = deviceLoginEntity.key;
                    }
                }
                else if (BackInfo.IndexOf("DeviceLogin:") == 0)     //分号前面表示 大的业务类别简述,是一个和服务器端通讯的约定。此时表示用户设备登录的操作业务
                {
                    if (BackInfo.IndexOf("\"status\": 1")>0)  //status:1表示登录成功
                    {
                        btnUserLogin.IsEnabled = false;
                    }                    
                }
                else if (BackInfo.IndexOf("TerminalEquipment:") == 0)  //服务器端发来的“终端指令信息。”，CommandTypeId表示发送的指令类型，1表示开机，2表示关机，3表示重启，4表示播放，5表示停止，6表示暂停,7表示文字类时时公告信息
                {
                    #region//模拟已经成功执行指令，返回正确信息给服务器                    
                    string msgTmp = BackInfo.Replace("TerminalEquipment:", ""); 
                    TerminalEquipmentSerialCommand_EntityModel terminalEquipmentEntity = JsonMutualObject1.ScriptDeserialize<TerminalEquipmentSerialCommand_EntityModel>(msgTmp);  //Json字符串转换为类
                    if (terminalEquipmentEntity!=null)
                    {
                        if (terminalEquipmentEntity.status==0)
                        {
                            MessageBox.Show(terminalEquipmentEntity.msg);
                        }
                        else
                        {
                            string ReadySendMsg = "TerminalEquipment:";  //分号前面表示 大的业务类别简述,是一个和服务器端通讯的约定。此时表示服务器发送的 终端指令信息                    
                            JsonObjectCollection delivered = new JsonObjectCollection();
                            delivered.Add(new JsonStringValue("ReceiveBackTime", DateTime.Now.ToString()));
                            delivered.Add(new JsonStringValue("msg", "客户端已成功执行终端指令"));
                            delivered.Add(new JsonNumericValue("TerSerialCommandID", Convert.ToInt32(terminalEquipmentEntity.TerSerialCommandID)));
                            delivered.Add(new JsonNumericValue("status", 1));
                            ReadySendMsg = ReadySendMsg + delivered.ToString();
                            client.Send(ReadySendMsg);
                        }                        
                    }                    
                    #endregion
                }
                else if (BackInfo.IndexOf("TerminalProgram:") == 0)  //服务器端发来的“节目制作信息。”
                {
                    #region//模拟已经成功执行指令，返回正确信息给服务器
                    string msgTmp = BackInfo.Replace("TerminalProgram:", "");                    
                    try
                    {
                        ProgramSendSerialList_EntityModel programSendSerialList_EntityModel = JsonMutualObject1.ScriptDeserialize<ProgramSendSerialList_EntityModel>(msgTmp);  //Json字符串转换为类
                        if (programSendSerialList_EntityModel != null)
                        {
                            if (programSendSerialList_EntityModel.status == 0)
                            {
                                MessageBox.Show(programSendSerialList_EntityModel.msg);
                            }
                            else
                            {
                                string ReadySendMsg = "TerminalProgram:";  //分号前面表示 大的业务类别简述,是一个和服务器端通讯的约定。此时表示服务器发送的 终端指令信息                    
                                JsonObjectCollection delivered = new JsonObjectCollection();
                                delivered.Add(new JsonStringValue("ReceiveBackTime", DateTime.Now.ToString()));
                                delivered.Add(new JsonStringValue("msg", "客户端已成功执行终端指令"));
                                delivered.Add(new JsonNumericValue("ProgramSendSerialID", Convert.ToInt32(programSendSerialList_EntityModel.ProgramSendSerialID)));
                                delivered.Add(new JsonNumericValue("status", 1));
                                ReadySendMsg = ReadySendMsg + delivered.ToString();
                                client.Send(ReadySendMsg);
                            }
                        }
                    }
                    catch //(Exception)
                    {
                        string ReadySendMsg = "TerminalProgram:";  //分号前面表示 大的业务类别简述,是一个和服务器端通讯的约定。此时表示服务器发送的 终端指令信息                    
                        JsonObjectCollection delivered = new JsonObjectCollection();
                        delivered.Add(new JsonStringValue("ReceiveBackTime", DateTime.Now.ToString()));
                        delivered.Add(new JsonStringValue("msg", "客户端接收到的json格式不对，疑似数据包丢失"));
                        delivered.Add(new JsonNumericValue("ProgramSendSerialID", 0));
                        delivered.Add(new JsonNumericValue("status", 0));
                        ReadySendMsg = ReadySendMsg + delivered.ToString();
                        client.Send(ReadySendMsg);   
                    }                   
                    #endregion
                }
            }

            }));
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            client.Send(txtSend.Text);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            client = new AsyncTcpClient(txtIpAddress.Text, remotePort);
            client.Encoding = Encoding.UTF8;
            //当收到服务器端文本数据的时候
            client.PlaintextReceived += new EventHandler<ReceivedEventArgs<string>>(client_PlaintextReceived);
            client.Connect();
            System.Threading.Thread.Sleep(1000);
            if (client.Connected)
            {
                btnExit.IsEnabled = true;
                btnLogin.IsEnabled = false;
                timer_current.Start();
                lblStatus.Content = "已成功链接到服务器。";
                btnUserLogin.IsEnabled = true;
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            client.Close();
            if (!client.Connected)
            {
                btnExit.IsEnabled = false;
                btnLogin.IsEnabled = true;
            }
        }

        private void UserLogin_Click(object sender, RoutedEventArgs e)
        {
            if (SessionID_Temp!="")
            {
                string ReadySendMsg = "DeviceLogin:";  //分号前面表示 大的业务类别简述,是一个和服务器端通讯的约定。此时表示用户设备登录的操作业务
                JsonObjectCollection delivered = new JsonObjectCollection();
                delivered.Add(new JsonStringValue("DeviceCode", ConfigurationManager.AppSettings["Local_DeviceCode"]));
                delivered.Add(new JsonStringValue("UserName", txtUsername.Text));
                string strPwdTmp = FormsAuthentication.HashPasswordForStoringInConfigFile(SessionID_Temp.ToUpper() + FormsAuthentication.HashPasswordForStoringInConfigFile(txtPwd.Text,"md5"), "md5");
                delivered.Add(new JsonStringValue("Password",strPwdTmp));
                ReadySendMsg = ReadySendMsg + delivered.ToString();
                client.Send(ReadySendMsg);                
            }
            
        }

        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            //以下仅为测试发送图片流的代码
            Byte[] imgbyteTmp = File.ReadAllBytes(@"E:\696cProjects\HengYuSite\GoShop\MultimediaSYS\images\48x48\user48x48.png"); //获取byte[]
            byte[] byteArray1 = m_BeginMark.Concat<byte>(imgbyteTmp).ToArray<byte>();
            byte[] byteArray2 = byteArray1.Concat<byte>(m_EndMark).ToArray<byte>();
            client.Send(byteArray2);
        }

    }
}
