using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Json;
using System.Net;
using MultimediaSysServer.EntityClasses;
using CommonLibrary;
using MultimediaSysServer.EntityModels;
using System.Web.Security;
using System.Data;
using SuperWebSocket;

namespace MultimediaSysServer
{
    public class TerminalEquipment : CommandBase<TelnetSession, StringRequestInfo>
    {
        MultimediaSysOperate multimediaSysOperate = new MultimediaSysOperate();

        public void ExecuteCommand_WebSocket(WebSocketSession session, string msg)
        {
            ExecuteCommand_Basic(null, session, msg);
        }

        public override void ExecuteCommand(TelnetSession session, StringRequestInfo requestInfo)
        {
            ExecuteCommand_Basic(session, null, requestInfo.Body);
        }

        public void ExecuteCommand_Basic(TelnetSession sessionT, WebSocketSession sessionW, string msg)
        {
            string requestInfoTemp = msg.Replace("\"", "'");  //处理json中可能出现的双引号问题
            string requestInfoTemp2 = "[" + msg + "]"; //要再次回返给客户端的信息
            try
            {
                JsonMutualObject JsonMutualObject1 = new JsonMutualObject();
                TerminalEquipmentSerialCommand_EntityModel terminalEquipmentSerialCommand = JsonMutualObject1.ScriptDeserialize<TerminalEquipmentSerialCommand_EntityModel>(requestInfoTemp);  //Json字符串转换为类

                if (terminalEquipmentSerialCommand.status == null || terminalEquipmentSerialCommand.TerSerialCommandID == null || terminalEquipmentSerialCommand.ReceiveBackTime == null)
                {
                    if (sessionT != null)
                    {
                        sessionT.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"请检查提交的参数数量，参数传值。\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionT.SessionID + "\"}");
                    }
                    else if (sessionW != null)
                    {
                        sessionW.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"请检查提交的参数数量，参数传值。\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionW.SessionID + "\"}");
                    }
                }
                else
                {
                    //发送成功后,客户端返回成功消息之后，需更新流水记录状态 
                    int tmp1 = multimediaSysOperate.UpdateTerminalEquipmentSerialCommand1(Convert.ToInt32(terminalEquipmentSerialCommand.TerSerialCommandID), 2,terminalEquipmentSerialCommand.ReceiveBackTime);
                }
            }
            catch (Exception ex)
            {
                if (sessionT != null)
                {
                    sessionT.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"" + ex.Message + "\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionT.SessionID + "\"}");
                }
                else if (sessionW != null)
                {
                    sessionW.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"" + ex.Message + "\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionW.SessionID + "\"}");
                }    
            }
        }

        //public override void ExecuteCommand(TelnetSession session, StringRequestInfo requestInfo)
        //{
        //    string requestInfoTemp = requestInfo.Body.Replace("\"", "'");  //处理json中可能出现的双引号问题
        //    try
        //    {
        //        JsonMutualObject JsonMutualObject1 = new JsonMutualObject();
        //        TerminalEquipmentSerialCommand_EntityModel terminalEquipmentSerialCommand = JsonMutualObject1.ScriptDeserialize<TerminalEquipmentSerialCommand_EntityModel>(requestInfoTemp);  //Json字符串转换为类

        //        if (terminalEquipmentSerialCommand.status == null || terminalEquipmentSerialCommand.TerSerialCommandID == null || terminalEquipmentSerialCommand.ReceiveBackTime == null)
        //        {
        //            session.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"请检查提交的参数数量，参数传值。\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionT.SessionID + "\"}");
        //            session.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"请检查提交的参数数量，参数传值。接收到信息:" + requestInfoTemp + "\",\"key\":\"" + session.SessionID + "\"}");
        //        }
        //        else
        //        {
        //            //发送成功后,客户端返回成功消息之后，需更新流水记录状态 
        //            int tmp1 = multimediaSysOperate.UpdateTerminalEquipmentSerialCommand1(Convert.ToInt32(terminalEquipmentSerialCommand.TerSerialCommandID), 2);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        session.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"" + ex.Message + "。接收到信息:" + requestInfoTemp + "\",\"key\":\"" + session.SessionID + "\"}");
        //    }
        //}
    }
}
