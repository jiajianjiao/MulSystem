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
    public class UserDrinkCost : CommandBase<TelnetSession, StringRequestInfo>
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
                DrinkCostSerialList_EntityModel drinkCostSerialList = JsonMutualObject1.ScriptDeserialize<DrinkCostSerialList_EntityModel>(requestInfoTemp);  //Json字符串转换为类

                if (drinkCostSerialList.status == null || drinkCostSerialList.SerialNumber == null || drinkCostSerialList.CurrentUserBalance == null)
                {
                    if (sessionT != null)
                    {
                        sessionT.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"请检查提交的参数数量，参数传值。\",\"SerialNumber\":\"\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionT.SessionID + "\"}");
                    }
                    else if (sessionW != null)
                    {
                        sessionW.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"请检查提交的参数数量，参数传值。\",\"SerialNumber\":\"\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionW.SessionID + "\"}");
                    }
                }
                else
                {
                    //发送成功后,客户端返回成功消息之后，需更新流水记录状态 
                    int tmp1 = multimediaSysOperate.AddUser_DrinkCostSerialList(drinkCostSerialList.SerialNumber,drinkCostSerialList.IcCardNo,drinkCostSerialList.CostTime.ToString(),drinkCostSerialList.DeviceCode,drinkCostSerialList.CostFee.ToString(),drinkCostSerialList.RunningWaterLitre.ToString(),drinkCostSerialList.CurrentUserBalance.ToString());
                    if (sessionT != null)
                    {
                        sessionT.Send(this.GetType().Name + ":" + "{ \"status\":1, \"msg\": \"成功收到客户端提交的消息。\",\"SerialNumber\":\"" + drinkCostSerialList.SerialNumber + "\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionT.SessionID + "\"}");
                    }
                    else if (sessionW != null)
                    {
                        sessionW.Send(this.GetType().Name + ":" + "{ \"status\":1, \"msg\": \"成功收到客户端提交的消息，参数传值。\",\"SerialNumber\":\"" + drinkCostSerialList.SerialNumber + "\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionW.SessionID + "\"}");
                    }
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
    }
}
