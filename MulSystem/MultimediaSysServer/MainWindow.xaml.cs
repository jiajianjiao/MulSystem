using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using MultimediaSysServer.EntityModels;
using System.Windows.Threading;
using System.Data;
using MultimediaSysServer.EntityClasses;
using System.Net.Json;
using System.Threading;
using log4net.Repository.Hierarchy;
using MultimediaSysServer.commonClasses;

namespace MultimediaSysServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        TelnetServer appServer = new TelnetServer();
        IBootstrap bootstrap = BootstrapFactory.CreateBootstrap();
        private const string ip = "Any";
        private int port = Convert.ToInt32(ConfigurationManager.AppSettings["SuperWebSocketServerPort"]);
        public WebSocketServer ws = null;//SuperWebSocket中的WebSocketServer对象
        private DispatcherTimer timer_current = new DispatcherTimer(); //定时器,用来实现顶部 的图片滚动效果
        MultimediaSysOperate multimediaSysOperate = new MultimediaSysOperate();
        MyLog4NetClass1 mylog = new MyLog4NetClass1();
        public MainWindow()
        {
            mylog.LogFactoryBase("log4net.config");  //加载启动log4net相关配置信息。只需要加载一次即可全部代码中使用日志功能。
            mylog.LoadLogConfig();

            InitializeComponent();
            btnStop.IsEnabled = false;
            //下面为传统的设置启动方式
            //var serverConfig = new ServerConfig
            //{
            //    Port = 1616 //设置监听端口
            //};
            ////Setup the appServer
            //if (!appServer.Setup(serverConfig))
            //{
            //    MessageBox.Show("Failed to setup!");
            //    return;
            //}

            //下面为按照配置文件的方式来安装appServer。
            if (!bootstrap.Initialize())
            {
                MessageBox.Show("Failed to initialize bootstrap!");
                return;
            }
            //以下为superwebsocket的相关代码
            ws = new WebSocketServer();//实例化WebSocketServer
            //添加事件侦听
            ws.NewSessionConnected += ws_NewSessionConnected;//有新会话握手并连接成功
            ws.SessionClosed += ws_SessionClosed;//有会话被关闭 可能是服务端关闭 也可能是客户端关闭
            ws.NewMessageReceived += ws_NewMessageReceived;//有客户端发送新的消息
            if (!ws.Setup(ip, port))
            {
                Console.WriteLine("ChatWebSocket 设置WebSocket服务侦听地址失败");
                return;
            }

            timer_current.Interval = TimeSpan.FromMilliseconds(100);
            timer_current.Tick += timer_current_Tick;
            timer_current.Start();
        }
        void timer_current_Tick(object sender, EventArgs e)
        {
            try
            {
                Action action = new Action(
                    delegate()
                    {
                        timer_current.Stop(); //暂时先停止此timer，
                        TelnetServer tsTmpServer = bootstrap.GetServerByName("TelnetServer") as TelnetServer;
                        #region//开始扫描，看是否有未发送的终端管理指令或普通公告文字信息
                        List<TerminalEquipmentSerialCommand_EntityModel> TerInfos = multimediaSysOperate.GetTerminalEquipmentSerialCommand1(1, 0);  //TerminalStatus为1表示查询在线的设备。SendStatus为0表示正在等待发送。
                        foreach (TerminalEquipmentSerialCommand_EntityModel item in TerInfos)
                        {
                            foreach (var session in tsTmpServer.GetAllSessions())
                            {
                                if (session.SessionID == item.SessionId)
                                {
                                    string CommandMsgTitle = "";//表示发送的指令类型，1表示开机，2表示关机，3表示重启，4表示播放，5表示停止，6表示暂停,7表示文字类时时公告信息
                                    switch (Convert.ToInt32(item.CommandTypeId))
                                    {
                                        case 1:
                                            CommandMsgTitle = "开机";
                                            break;
                                        case 2:
                                            CommandMsgTitle = "关机";
                                            break;
                                        case 3:
                                            CommandMsgTitle = "重启";
                                            break;
                                        case 4:
                                            CommandMsgTitle = "播放";
                                            break;
                                        case 5:
                                            CommandMsgTitle = "停止";
                                            break;
                                        case 6:
                                            CommandMsgTitle = "暂停";
                                            break;
                                        case 7:
                                            CommandMsgTitle = "时时公告信息";
                                            break;
                                        default:
                                            break;
                                    }
                                    JsonObjectCollection delivered = new JsonObjectCollection();
                                    delivered.Add(new JsonNumericValue("status", 1));
                                    delivered.Add(new JsonStringValue("msg", "服务器发送了[" + CommandMsgTitle + "]的指令操作。"));
                                    delivered.Add(new JsonStringValue("key", session.SessionID));
                                    delivered.Add(new JsonNumericValue("CommandTypeId", Convert.ToInt32(item.CommandTypeId)));
                                    delivered.Add(new JsonNumericValue("TerSerialCommandID", Convert.ToInt32(item.TerSerialCommandID)));
                                    delivered.Add(new JsonStringValue("SendTime", Convert.ToString(item.SendTime)));
                                    delivered.Add(new JsonStringValue("SendContent", item.SendContent));
                                    session.Send("TerminalEquipment:" + delivered.ToString());
                                    //发送成功后，要修改发送流水记录的状态，修改为1，已发送。
                                    int tmp1 = multimediaSysOperate.UpdateTerminalEquipmentSerialCommand2(Convert.ToInt32(item.TerSerialCommandID), 1);
                                    Thread.Sleep(50);
                                }
                            }
                        }
                        #endregion
                        #region//开始扫描，看看哪个硬件终端有未发送的 节目信息。然后发送
                        List<ProgramSendSerialList_EntityModel> psslEnt = multimediaSysOperate.GetProgramSendSerialList1(1, 0);  //TerminalStatus为1表示查询在线的设备。SendStatus为0表示正在等待发送。
                        foreach (ProgramSendSerialList_EntityModel item in psslEnt)
                        {
                            foreach (var session in tsTmpServer.GetAllSessions())
                            {
                                if (session.SessionID == item.SessionId)
                                {
                                    //开始拼装要发送的json节目信息,ProgramInfoID表示节目id，ProgramSendSerialID发送节目流水号
                                    string ReadySendMsg = GetReadySendProgramInfo(item.SessionId, Convert.ToInt32(item.ProgramInfoID), Convert.ToInt32(item.ProgramSendSerialID));
                                    session.Send("TerminalProgram:" + ReadySendMsg);
                                    //发送成功后，要修改发送流水记录的状态，修改为1，已发送。
                                    int tmp6 = multimediaSysOperate.Updatejjj_ProgramSendSerialList1(Convert.ToInt32(item.ProgramSendSerialID), 1);
                                }
                            }
                        }
                        #endregion
                        #region  //开始扫描，看看哪些硬件终端的【开关屏时间】 设置记录信息有更新，即查询jjj_TerminalSwitchScreenTime表
                        List<IGrouping<int?, TerminalSwitchScreenTime_EntityModel>> tsstEnt = multimediaSysOperate.GetTerminalSwitchScreenTime2(true, 0, 1);
                        foreach (IGrouping<int?, TerminalSwitchScreenTime_EntityModel> item_layer in tsstEnt)
                        {
                            List<TerminalEquipmentInfo_EntityModel> terEquiEntity = multimediaSysOperate.GetTerminalEquipmentInfo2(Convert.ToInt32(item_layer.Key));
                            foreach (var session in tsTmpServer.GetAllSessions())
                            {
                                if (session.SessionID == terEquiEntity[0].SessionId)
                                {
                                    //开始拼装要发送的json信息
                                    string ReadySendMsg = GetReadySendTerminalSwitchScreenTime(session.SessionID, Convert.ToInt32(item_layer.Key));
                                    session.Send("TerminalSwitchScreenTime:" + ReadySendMsg);
                                    //发送成功后，要修改发送流水记录的状态，修改为1，已发送。
                                    int tmp6 = multimediaSysOperate.Updatejjj_TerminalSwitchScreenTime1(Convert.ToInt32(item_layer.Key), 1);
                                }
                            }
                        }
                        #endregion
                        #region//开始扫描，看是否有未发送的终端管理指令或普通公告文字信息
                        List<TerminalEquipmentInfo_EntityModel> TerInfo2 = multimediaSysOperate.GetTerminalEquipmentInfo3(0,1);  //TerminalStatus为1表示查询在线的设备。SendStatus为0表示正在等待发送。
                        foreach (TerminalEquipmentInfo_EntityModel item in TerInfo2)
                        {
                            foreach (var session in tsTmpServer.GetAllSessions())
                            {
                                if (session.SessionID == item.SessionId)
                                {
                                    JsonObjectCollection delivered = new JsonObjectCollection();
                                    delivered.Add(new JsonNumericValue("status", 1));
                                    delivered.Add(new JsonStringValue("msg", "服务器发送了一条终端设备基础信息数据。"));
                                    delivered.Add(new JsonStringValue("key", session.SessionID));
                                    delivered.Add(new JsonStringValue("DeviceCode", item.DeviceCode));                                    
                                    delivered.Add(new JsonStringValue("TerminalVersion", item.TerminalVersion));
                                    delivered.Add(new JsonNumericValue("IsRegister",Convert.ToInt32(item.IsRegister)));
                                    delivered.Add(new JsonStringValue("DownloadStartTime", item.DownloadStartTime.ToString()));
                                    delivered.Add(new JsonStringValue("DownloadEndTime", item.DownloadEndTime.ToString()));
                                    session.Send("TerminalEquipmentBasicInfo:" + delivered.ToString());
                                    //发送成功后，要修改记录的状态
                                    int tmp1 = multimediaSysOperate.UpdateTerminalEquipmentInfo3(Convert.ToInt32(item.TerminalEquipmentID), 1);
                                    Thread.Sleep(50);
                                }
                            }
                        }
                        #endregion
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

        /// <summary>
        /// 获取准备发送的“硬件终端的开关屏时间 设置”信息
        /// </summary>        
        /// <returns></returns>
        public string GetReadySendTerminalSwitchScreenTime(string SessionID, int TerminalEquipmentID)
        {
            JsonObjectCollection delivered = new JsonObjectCollection();
            delivered.Add(new JsonNumericValue("status", 1));
            delivered.Add(new JsonNumericValue("TerminalEquipmentID", TerminalEquipmentID));
            delivered.Add(new JsonStringValue("msg", "成功发送硬件终端的开关屏时间设置信息。"));
            delivered.Add(new JsonStringValue("key", SessionID));
            JsonArrayCollection collection1 = new JsonArrayCollection();
            collection1.Name = "data";
            List<TerminalSwitchScreenTime_EntityModel> psslEnt = multimediaSysOperate.GetTerminalSwitchScreenTime3(true, 0, 1, TerminalEquipmentID);  //TerminalStatus为1表示查询在线的设备。SendStatus为0表示正在等待发送。
            foreach (TerminalSwitchScreenTime_EntityModel item in psslEnt)
            {
                JsonObjectCollection clt = new JsonObjectCollection();
                clt.Add(new JsonNumericValue("TerminalSwitchScreenTimeID", Convert.ToInt32(item.TerminalSwitchScreenTimeID)));
                clt.Add(new JsonStringValue("TurnOnTime", item.TurnOnTime.ToString()));
                clt.Add(new JsonStringValue("TurnOffTime", item.TurnOffTime.ToString()));
                clt.Add(new JsonStringValue("IsMonday", item.IsMonday.ToString()));
                clt.Add(new JsonStringValue("IsTuesday", item.IsTuesday.ToString()));
                clt.Add(new JsonStringValue("IsWednesday", item.IsWednesday.ToString()));
                clt.Add(new JsonStringValue("IsThursday", item.IsThursday.ToString()));
                clt.Add(new JsonStringValue("IsFriday", item.IsFriday.ToString()));
                clt.Add(new JsonStringValue("IsSaturday", item.IsSaturday.ToString()));
                clt.Add(new JsonStringValue("IsSunday", item.IsSunday.ToString()));
                clt.Add(new JsonStringValue("UpdateTime", item.UpdateTime.ToString()));
                collection1.Add(clt);
            }
            delivered.Add(collection1);
            return delivered.ToString();
        }
        //

        /// <summary>
        /// 获取准备发送的广告信息
        /// </summary>
        /// <param name="SessionID"></param>
        /// <returns></returns>
        public string GetReadySendProgramInfo(string SessionID, int ProgramInfoID, int ProgramSendSerialID)
        {
            JsonObjectCollection delivered = new JsonObjectCollection();
            delivered.Add(new JsonNumericValue("status", 1));
            delivered.Add(new JsonNumericValue("ProgramSendSerialID", ProgramSendSerialID));
            delivered.Add(new JsonStringValue("msg", "成功发送节目信息。"));
            delivered.Add(new JsonStringValue("key", SessionID));

            JsonArrayCollection collection1 = new JsonArrayCollection();
            collection1.Name = "data";
            List<ProgramInfoList_EntityModel> psslEnt = multimediaSysOperate.GetProgramInfoList1(ProgramInfoID);  //TerminalStatus为1表示查询在线的设备。SendStatus为0表示正在等待发送。
            foreach (ProgramInfoList_EntityModel item in psslEnt)
            {
                JsonObjectCollection clt = new JsonObjectCollection();
                clt.Add(new JsonNumericValue("ProgramInfoID", Convert.ToInt32(item.ProgramInfoID)));
                clt.Add(new JsonNumericValue("ProgramWidth", Convert.ToInt32(item.ProgramWidth)));
                clt.Add(new JsonNumericValue("ProgramHeight", Convert.ToInt32(item.ProgramHeight)));
                clt.Add(new JsonNumericValue("ProgramStatus", Convert.ToInt32(item.ProgramStatus)));
                clt.Add(new JsonNumericValue("CreateUserId", Convert.ToInt32(item.CreateUserId)));
                clt.Add(new JsonStringValue("ProgramTitle", item.ProgramTitle));
                clt.Add(new JsonStringValue("CreateTime", item.CreateTime.ToString()));
                clt.Add(new JsonStringValue("UpdateTime", item.UpdateTime.ToString()));
                //开始
                JsonArrayCollection collectionLayer = new JsonArrayCollection();
                collectionLayer.Name = "ProgramInfoLists";
                List<IGrouping<int?, ProgramInfoListItem_EntityModel>> psslEntLayer = multimediaSysOperate.GetProgramInfoListItem3(ProgramInfoID);  //TerminalStatus为1表示查询在线的设备。SendStatus为0表示正在等待发送。
                foreach (IGrouping<int?, ProgramInfoListItem_EntityModel> item_layer in psslEntLayer)
                {
                    JsonObjectCollection clt_layer = new JsonObjectCollection();
                    clt_layer.Add(new JsonNumericValue("ProgramInfoItemLayer", Convert.ToInt32(item_layer.Key)));
                    //666666
                    #region
                    JsonArrayCollection collection2 = new JsonArrayCollection();
                    collection2.Name = "ProgramInfoListItems";
                    List<ProgramInfoListItem_EntityModel> psslEnt2 = multimediaSysOperate.GetProgramInfoListItem2(ProgramInfoID, Convert.ToInt32(item_layer.Key));  //TerminalStatus为1表示查询在线的设备。SendStatus为0表示正在等待发送。
                    foreach (ProgramInfoListItem_EntityModel item2 in psslEnt2)
                    {
                        JsonObjectCollection clt2 = new JsonObjectCollection();
                        clt2.Add(new JsonNumericValue("ProgramInfoItemID", Convert.ToInt32(item2.ProgramInfoItemID)));
                        clt2.Add(new JsonStringValue("ProgramInfoItemTitle", item2.ProgramInfoItemTitle));
                        clt2.Add(new JsonNumericValue("ProgramInfoItemLeft", Convert.ToDouble(item2.ProgramInfoItemLeft)));
                        clt2.Add(new JsonNumericValue("ProgramInfoItemTop", Convert.ToDouble(item2.ProgramInfoItemTop)));
                        clt2.Add(new JsonNumericValue("ProgramInfoItemLayer", Convert.ToInt32(item2.ProgramInfoItemLayer)));
                        clt2.Add(new JsonNumericValue("ProgramInfoItemHeight", Convert.ToInt32(item2.ProgramInfoItemHeight)));
                        clt2.Add(new JsonNumericValue("ProgramInfoItemWidth", Convert.ToInt32(item2.ProgramInfoItemWidth)));
                        clt2.Add(new JsonNumericValue("ProgramInfoItemType", Convert.ToInt32(item2.ProgramInfoItemType)));
                        clt2.Add(new JsonNumericValue("IsShowWater", Convert.ToInt32(item2.IsShowWater)));
                        if (item2.ProgramInfoItemType == 1)//节目子元素的类型。1为图片，2为音乐，3为视频，4为文档元素(WORD、EXCEL、PPT、PDF)，5为网页元素，6为文字元素，7为日期元素，8为时间元素，9为天气元素,0为其它类型
                        {
                            JsonArrayCollection collection_image = new JsonArrayCollection();
                            collection_image.Name = "ProgramInfoListItems_Image";
                            List<ProgramInfoItem_ImageDetail_EntityModel> psslEnt_image = multimediaSysOperate.GetProgramInfoItem_ImageDetail1(Convert.ToInt32(item2.ProgramInfoItemID));
                            foreach (ProgramInfoItem_ImageDetail_EntityModel item_image in psslEnt_image)
                            {
                                JsonObjectCollection clt_image = new JsonObjectCollection();
                                clt_image.Add(new JsonNumericValue("ID", Convert.ToInt32(item_image.ID)));
                                clt_image.Add(new JsonNumericValue("ProgramInfoItemID", Convert.ToInt32(item_image.ProgramInfoItemID)));
                                clt_image.Add(new JsonNumericValue("ImageLayer", Convert.ToInt32(item_image.ImageLayer)));
                                clt_image.Add(new JsonNumericValue("TimeLength", Convert.ToInt32(item_image.TimeLength)));
                                clt_image.Add(new JsonNumericValue("CartoonType", Convert.ToInt32(item_image.CartoonType)));
                                clt_image.Add(new JsonStringValue("ImageUrl", item_image.ImageUrl));
                                collection_image.Add(clt_image);
                            }
                            clt2.Add(collection_image);
                        }
                        else if (item2.ProgramInfoItemType == 3)
                        {
                            JsonArrayCollection collection_video = new JsonArrayCollection();
                            collection_video.Name = "ProgramInfoListItems_Video";
                            List<ProgramInfoItem_VideoDetail_EntityModel> psslEnt_video = multimediaSysOperate.GetProgramInfoItem_VideoDetail1(Convert.ToInt32(item2.ProgramInfoItemID));
                            foreach (ProgramInfoItem_VideoDetail_EntityModel item_video in psslEnt_video)
                            {
                                JsonObjectCollection clt_image = new JsonObjectCollection();
                                clt_image.Add(new JsonNumericValue("ID", Convert.ToInt32(item_video.ID)));
                                clt_image.Add(new JsonNumericValue("ProgramInfoItemID", Convert.ToInt32(item_video.ProgramInfoItemID)));
                                clt_image.Add(new JsonNumericValue("VideoLayer", Convert.ToInt32(item_video.VideoLayer)));
                                clt_image.Add(new JsonNumericValue("VideoVoice", Convert.ToInt32(item_video.VideoVoice)));
                                List<MaterialManagementDetail> jjj_materialManagementDetail = multimediaSysOperate.Getjjj_MaterialManagementDetail1(item_video.VideoUrl);
                                if (jjj_materialManagementDetail.Count>0)
                                {
                                    clt_image.Add(new JsonStringValue("VideoUrl", jjj_materialManagementDetail[0].TransformFilePath));
                                }
                                else
                                {
                                    clt_image.Add(new JsonStringValue("VideoUrl", item_video.VideoUrl));
                                }
                                collection_video.Add(clt_image);
                            }
                            clt2.Add(collection_video);
                        }
                        else if (item2.ProgramInfoItemType == 4)
                        {
                            JsonArrayCollection collection_video = new JsonArrayCollection();
                            collection_video.Name = "ProgramInfoListItems_Document";
                            List<ProgramInfoItem_DocumentDetail_EntityModel> psslEnt_document = multimediaSysOperate.GetProgramInfoItem_DocumentDetail1(Convert.ToInt32(item2.ProgramInfoItemID));
                            foreach (ProgramInfoItem_DocumentDetail_EntityModel item_document in psslEnt_document)
                            {
                                JsonObjectCollection clt_image = new JsonObjectCollection();
                                clt_image.Add(new JsonNumericValue("ID", Convert.ToInt32(item_document.ID)));
                                clt_image.Add(new JsonNumericValue("ProgramInfoItemID", Convert.ToInt32(item_document.ProgramInfoItemID)));
                                clt_image.Add(new JsonNumericValue("DocumentLayer", Convert.ToInt32(item_document.DocumentLayer)));
                                clt_image.Add(new JsonNumericValue("TimeLength", Convert.ToInt32(item_document.TimeLength)));
                                clt_image.Add(new JsonNumericValue("CartoonType", Convert.ToInt32(item_document.CartoonType)));
                                clt_image.Add(new JsonStringValue("DocumentUrl", item_document.DocumentUrl));
                                collection_video.Add(clt_image);
                            }
                            clt2.Add(collection_video);
                        }
                        else if (item2.ProgramInfoItemType == 6)
                        {
                            JsonArrayCollection collection_video = new JsonArrayCollection();
                            collection_video.Name = "ProgramInfoListItems_Text";
                            List<ProgramInfoItem_TextDetail_EntityModel> psslEnt_text = multimediaSysOperate.GetProgramInfoItem_TextDetail1(Convert.ToInt32(item2.ProgramInfoItemID));
                            foreach (ProgramInfoItem_TextDetail_EntityModel item_text in psslEnt_text)
                            {
                                JsonObjectCollection clt_image = new JsonObjectCollection();
                                clt_image.Add(new JsonNumericValue("ID", Convert.ToInt32(item_text.ID)));
                                clt_image.Add(new JsonNumericValue("ProgramInfoItemID", Convert.ToInt32(item_text.ProgramInfoItemID)));
                                clt_image.Add(new JsonNumericValue("TextLayer", Convert.ToInt32(item_text.TextLayer)));
                                clt_image.Add(new JsonNumericValue("TextDirectionType", Convert.ToInt32(item_text.TextDirectionType)));
                                clt_image.Add(new JsonNumericValue("IsBackgroundTransparent", Convert.ToInt32(item_text.IsBackgroundTransparent)));
                                clt_image.Add(new JsonNumericValue("IsBold", Convert.ToInt32(item_text.IsBold)));
                                clt_image.Add(new JsonNumericValue("FontSize", Convert.ToInt32(item_text.FontSize)));
                                clt_image.Add(new JsonNumericValue("PlayCount", Convert.ToInt32(item_text.PlayCount)));
                                clt_image.Add(new JsonNumericValue("PlaySpeed", Convert.ToInt32(item_text.PlaySpeed)));
                                clt_image.Add(new JsonStringValue("TextContent", item_text.TextContent));
                                clt_image.Add(new JsonStringValue("BackgroundColor", item_text.BackgroundColor));
                                clt_image.Add(new JsonStringValue("foreColor", item_text.foreColor));
                                clt_image.Add(new JsonStringValue("FontFamily", item_text.FontFamily));
                                collection_video.Add(clt_image);
                            }
                            clt2.Add(collection_video);
                        }
                        else if (item2.ProgramInfoItemType == 8)
                        {
                            JsonArrayCollection collection_video = new JsonArrayCollection();
                            collection_video.Name = "ProgramInfoListItems_Time";
                            List<ProgramInfoItem_TimeDetail_EntityModel> psslEnt_time = multimediaSysOperate.GetProgramInfoItem_TimeDetail1(Convert.ToInt32(item2.ProgramInfoItemID));
                            foreach (ProgramInfoItem_TimeDetail_EntityModel item_time in psslEnt_time)
                            {
                                JsonObjectCollection clt_image = new JsonObjectCollection();
                                clt_image.Add(new JsonNumericValue("ID", Convert.ToInt32(item_time.ID)));
                                clt_image.Add(new JsonNumericValue("ProgramInfoItemID", Convert.ToInt32(item_time.ProgramInfoItemID)));
                                clt_image.Add(new JsonNumericValue("FontSize", Convert.ToInt32(item_time.FontSize)));
                                clt_image.Add(new JsonNumericValue("IsBackgroundTransparent", Convert.ToInt32(item_time.IsBackgroundTransparent)));
                                clt_image.Add(new JsonNumericValue("IsBold", Convert.ToInt32(item_time.IsBold)));
                                clt_image.Add(new JsonStringValue("FontFamily", item_time.FontFamily));
                                clt_image.Add(new JsonStringValue("FontFormat", item_time.FontFormat));
                                clt_image.Add(new JsonStringValue("BackgroundColor", item_time.BackgroundColor));
                                clt_image.Add(new JsonStringValue("foreColor", item_time.foreColor));
                                collection_video.Add(clt_image);
                            }
                            clt2.Add(collection_video);
                        }
                        collection2.Add(clt2);
                    }
                    clt_layer.Add(collection2);
                    #endregion
                    //666666
                    collectionLayer.Add(clt_layer);
                }
                clt.Add(collectionLayer);
                //结束            
                collection1.Add(clt);
            }

            delivered.Add(collection1);
            return delivered.ToString();
        }
        //
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //下面为传统启动方式
            //if (!appServer.Start())
            //{
            //    btnStop.IsEnabled = true;
            //    MessageBox.Show("Failed to start!");
            //    return;
            //}
            //else
            //{
            //    btnStart.IsEnabled = false;
            //}

            //下面为配置文件的方式
            var result = bootstrap.Start();
            if (result == StartResult.Failed)
            {
                MessageBox.Show("Failed to start!");
                return;
            }
            else
            {
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = true;
            }
            //以下为SuperWebSocket相关代码
            Start_SuperWebSocket();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            //下面为传统停止方式
            //appServer.Stop();

            //下面为配置文件的方式来停止
            bootstrap.Stop();

            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            //以下为SuperWebSocket相关代码
            Stop_SuperWebSocket();
        }

        void ws_NewSessionConnected(WebSocketSession session)
        {
            Console.WriteLine("{0:HH:MM:ss}  与客户端:{1}创建新会话{2}", DateTime.Now, GetSessionName(session), session.SessionID);
            //var msg = string.Format("{0:HH:MM:ss} {1} 进入聊天,SessionID:{2}", DateTime.Now, GetSessionName(session), session.SessionID);
            var msg = "Welcome:{\"msg\":\"Welcome to MultimediaSysServer\",\"status\":1,\"key\":\"" + session.SessionID + "\"}" + ConfigurationManager.AppSettings["CustomTerminatorSign"];
            SendToOneUser(session, msg);//只对当前登陆人发送消息
        }

        void ws_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            Console.WriteLine("{0:HH:MM:ss}  与客户端:{1}的会话被关闭 原因：{2}", DateTime.Now, GetSessionName(session), value);
            var msg = string.Format("{0:HH:MM:ss} {1} 离开聊天", DateTime.Now, GetSessionName(session));
            if (session.Path != null)
            {
                SendToAll(session, msg);
            }
        }
        void ws_NewMessageReceived(WebSocketSession session, string value)
        {
            var msg = string.Format("{0:HH:MM:ss} {1}说: {2}", DateTime.Now, GetSessionName(session), value);
            //开始进行业务逻辑的判断，不同的业务，给它指定到不同的类中处理。首先要验证发送消息数据的完整性，看是否终结符存在
            if (value.IndexOf(ConfigurationManager.AppSettings["CustomTerminatorSign"]) >= 0)
            {
                value = value.Replace(ConfigurationManager.AppSettings["CustomTerminatorSign"], "");
                if (value.IndexOf("DeviceLogin:") == 0) //是用户登陆的业务请求
                {
                    DeviceLogin dl = new DeviceLogin();
                    dl.ExecuteCommand_WebSocket(session, value.Replace("DeviceLogin:", ""));
                }
            }
            else
            {
                msg = string.Format("Unknown:{ \"status\":0, \"msg\": \"你发送的数据格式不符合约定。\""); ;
                SendToOneUser(session, msg + ConfigurationManager.AppSettings["CustomTerminatorSign"]);//只对当前登陆人发送消息
            }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        public void Start_SuperWebSocket()
        {
            if (!ws.Start())
            {
                Console.WriteLine("ChatWebSocket 启动WebSocket服务侦听失败");
                return;
            }
            Console.WriteLine("ChatWebSocket 启动服务成功");
        }

        /// <summary>
        /// 停止侦听服务
        /// </summary>
        public void Stop_SuperWebSocket()
        {
            if (ws != null)
            {
                ws.Stop();
            }
        }

        private string GetSessionName(WebSocketSession session)
        {
            if (session.Path != null)
            {
                //这里用Path来取Name 不太科学…… 
                return HttpUtility.UrlDecode(session.Path.TrimStart('/'));
            }
            else
            {
                return "";
            }
        }

        private void SendToAll(WebSocketSession session, string msg)
        {
            //广播
            foreach (var sendSession in session.AppServer.GetAllSessions())
            {
                sendSession.Send(msg);
            }
        }

        private void SendToOneUser(WebSocketSession session, string msg)
        {
            session.Send(msg);
        }

        private void btnGuangBo_Click(object sender, RoutedEventArgs e)
        {
            foreach (var session in ws.GetAllSessions())
            {
                session.Send(txtGuangBoInfo.Text + "###");
            }
            //foreach (var session in appServer.GetAllSessions())  //这种广播方法，可以使用在普通实例化一个 TelnetServer appServer = new TelnetServer();的时候使用。
            //{
            //    session.Send(txtGuangBoInfo.Text + "###");
            //}
            TelnetServer tsTmpServer = bootstrap.GetServerByName("TelnetServer") as TelnetServer;
            foreach (var session in tsTmpServer.GetAllSessions())
            {
                //session.Send(txtGuangBoInfo.Text + "###");
                session.Send(txtGuangBoInfo.Text);
            }
        }
    }
}
