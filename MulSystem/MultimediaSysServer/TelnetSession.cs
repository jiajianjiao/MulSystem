using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Json;
using MultimediaSysServer.EntityModels;
using MultimediaSysServer.commonClasses;

namespace MultimediaSysServer
{
    public class TelnetSession:AppSession<TelnetSession>
    {
        MultimediaSysOperate multimediaSysOperate = new MultimediaSysOperate();
        public override void Send(string message)
        {
            base.Send(message + ConfigurationManager.AppSettings["CustomTerminatorSign"]);
        }
        protected override void OnSessionStarted()
        {
            //LogManager.WriteLog("socket日志", "Welcome_Session:" + this.SessionID);
            LogHelper.WriteLog(this.GetType(), "Welcome_Session:" + this.SessionID);
            
            this.Send("Welcome:{\"msg\":\"Welcome to MultimediaSysServer\",\"status\":1,\"key\":\"" + this.SessionID + "\"}");
        }
        
        protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
        {
            //LogManager.WriteLog("socket日志", "UnknownRequest_Session:" + this.SessionID);
            LogHelper.WriteLog(this.GetType(), "UnknownRequest_Session:" + this.SessionID);
            LogHelper.WriteLog(this.GetType(), "UnknownRequest_Session:Key:" + requestInfo.Key);
            LogHelper.WriteLog(this.GetType(), "UnknownRequest_Session:Body" + requestInfo.Body);

            string requestInfoTemp2 = "[" + requestInfo.Body + "]"; //要再次回返给客户端的信息
            JsonObjectCollection delivered = new JsonObjectCollection();
            delivered.Add(new JsonNumericValue("status", 0));
            delivered.Add(new JsonStringValue("msg", "你所输入的格式为非法格式"));
            delivered.Add(new JsonStringValue("key", this.SessionID));
            delivered.Add(new JsonStringValue("receiveMsg", requestInfoTemp2));
            delivered.Add(new JsonStringValue("requestInfoKey", requestInfo.Key));
            delivered.Add(new JsonStringValue("receiveAll", requestInfo.Key + ":" + requestInfo.Body + ConfigurationManager.AppSettings["CustomTerminatorSign"]));
            //this.Send("{ \"status\":0, \"msg\": \"你所输入的格式不合法。\",\"key\":\"" + this.SessionID + "\"}" + ConfigurationManager.AppSettings["CustomTerminatorSign"]);
            this.Send("Unknown:"+delivered.ToString());
        }

        protected override void HandleException(Exception e)
        {
            //LogManager.WriteLog("socket日志", "异常:" + e.Message);
            LogHelper.WriteLog(this.GetType(), "异常:" + e.Message);
            this.Send("Application error: {0}", e.Message);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            //LogManager.WriteLog("socket日志", "关闭session:" + this.SessionID);
            LogHelper.WriteLog(this.GetType(), "关闭session:" + this.SessionID);
            //开始进行终端硬件设备状态更新的操作，更新为下线状态
            int tmp1 = multimediaSysOperate.UpdateTerminalEquipmentInfo2(this.SessionID);
            base.OnSessionClosed(reason);
        }
    }
}
