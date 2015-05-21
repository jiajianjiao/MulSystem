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
    public class TerminalProgram : CommandBase<TelnetSession, StringRequestInfo>
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
                ProgramSendSerialList_EntityModel programSendSerialList_EntityModel = JsonMutualObject1.ScriptDeserialize<ProgramSendSerialList_EntityModel>(requestInfoTemp);  //Json字符串转换为类

                if (programSendSerialList_EntityModel.status == null || programSendSerialList_EntityModel.ProgramSendSerialID == null || programSendSerialList_EntityModel.ReceiveBackTime == null)
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
                    if (programSendSerialList_EntityModel.status==0)  //此时可能是客户端接收到的数据包有丢失，所以返回了status为0，这个时候，按理服务器应再推送之前的消息到客户端。
                    {
                        //再次推送消息的代码代写。
                    }
                    else if (programSendSerialList_EntityModel.status == 1)  //status为1表示该条节目json字符串已被客户端完成接收到。
                    {
                        //发送成功后,客户端返回成功消息之后，需更新流水记录状态 
                        int tmp1 = multimediaSysOperate.Updatejjj_ProgramSendSerialList2(Convert.ToInt32(programSendSerialList_EntityModel.ProgramSendSerialID), 2, programSendSerialList_EntityModel.ReceiveBackTime);
                    }
                    else if (programSendSerialList_EntityModel.status == 2) //status为2表示该条节目里面的图片和视频已全部被下载到客户端。
                    {
                        //为了方便客户端。此时客户端发送过来的ReceiveBackTime实际上是表示“DownloadOverTime”。
                        int tmp1 = multimediaSysOperate.Updatejjj_ProgramSendSerialList3(Convert.ToInt32(programSendSerialList_EntityModel.ProgramSendSerialID), 3, programSendSerialList_EntityModel.ReceiveBackTime);
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
