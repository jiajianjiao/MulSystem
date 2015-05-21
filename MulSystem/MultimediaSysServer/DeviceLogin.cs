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
using log4net.Repository.Hierarchy;
using MultimediaSysServer.commonClasses;

namespace MultimediaSysServer
{
    public class DeviceLogin : CommandBase<TelnetSession, StringRequestInfo>
    {
        MultimediaSysOperate multimediaSysOperate = new MultimediaSysOperate();
        
        public void ExecuteCommand_WebSocket(WebSocketSession session,string msg)
        {
            ExecuteCommand_Basic(null, session, msg);
        }

        public override void ExecuteCommand(TelnetSession session, StringRequestInfo requestInfo)
        {
            //LogManager.WriteLog("socket日志", "DeviceLogin_Session:" + session.SessionID);
            //LogManager.WriteLog("socket日志", "DeviceLogin_Key:" + requestInfo.Key);
            //LogManager.WriteLog("socket日志", "DeviceLogin_Body:" + requestInfo.Body);
            LogHelper.WriteLog(this.GetType(), "DeviceLogin_Session:" + session.SessionID);
            LogHelper.WriteLog(this.GetType(), "DeviceLogin_Key:" + requestInfo.Key);
            LogHelper.WriteLog(this.GetType(), "DeviceLogin_Body:" + requestInfo.Body);
            ExecuteCommand_Basic(session, null, requestInfo.Body);           
        }

        public void ExecuteCommand_Basic(TelnetSession sessionT, WebSocketSession sessionW, string msg)
        {
            string requestInfoTemp = msg.Replace("\"", "'");          //处理json中可能出现的双引号问题
            string requestInfoTemp2 = "[" + msg + "]"; //要再次回返给客户端的信息
            try
            {                
                JsonMutualObject JsonMutualObject1 = new JsonMutualObject();
                DeviceLoginEntity deviceLoginEntity = JsonMutualObject1.ScriptDeserialize<DeviceLoginEntity>(requestInfoTemp);  //Json字符串转换为类

                //开始进行2.用户名密码的验证操作,1.查看是否当前的设备号存在
                if (deviceLoginEntity.UserName == null || deviceLoginEntity.DeviceCode == null || deviceLoginEntity.Password == null)
                {
                    if (sessionT!=null)
                    {
                        sessionT.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"请检查提交的参数数量，参数传值。\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionT.SessionID + "\"}");
                    }
                    else if (sessionW!=null)
                    {
                        sessionW.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"请检查提交的参数数量，参数传值。\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionW.SessionID + "\"}");
                    }
                }
                else
                {
                    //开始判断当前设备是否已注册过
                    List<TerminalEquipmentInfo_EntityModel> terEntity = multimediaSysOperate.GetTerminalEquipmentInfo1(deviceLoginEntity.DeviceCode);
                    if (terEntity.Count == 0)  //当前设备号不存在
                    {
                        if (sessionT != null)
                        {
                            sessionT.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"当前设备号不存在。\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionT.SessionID + "\"}");                            
                        }
                        else if (sessionW != null)
                        {
                            sessionW.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"当前设备号不存在。\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionW.SessionID + "\"}");                            
                        }                        
                    }
                    else   //判断当前设备号是否已注册。
                    {
                        if (terEntity[0].IsRegister != 1)
                        {
                            if (sessionT != null)
                            {
                                sessionT.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"当前设备还未注册。\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionT.SessionID + "\"}");                                                            
                            }
                            else if (sessionW != null)
                            {
                                sessionW.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"当前设备还未注册。\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionW.SessionID + "\"}");                                                                                            
                            }                              
                        }
                        else  //开始判断当前登陆的用户名密码是否正确
                        {
                            string userPwd = "";
                            List<UserInfo_EntityModel> userEntity = multimediaSysOperate.GetUserInfo1(deviceLoginEntity.UserName);
                            if (userEntity.Count == 0)  //当前登陆用户并不存在
                            {
                                if (sessionT != null)
                                {
                                    sessionT.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"当前登陆用户并不存在。\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionT.SessionID + "\"}");                                                                                                
                                }
                                else if (sessionW != null)
                                {
                                    sessionW.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"当前登陆用户并不存在。\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionW.SessionID + "\"}");
                                }                                      
                            }
                            else
                            {
                                userPwd = userEntity[0].password;
                                string sessionId_Tmp = "";
                                if (sessionT != null)
                                {
                                    sessionId_Tmp = sessionT.SessionID;
                                }
                                else if (sessionW != null)
                                {
                                    sessionId_Tmp = sessionW.SessionID;
                                }
                                string strPwdTmp = FormsAuthentication.HashPasswordForStoringInConfigFile(sessionId_Tmp.ToUpper() + userPwd, "md5");
                                if (strPwdTmp.ToUpper() == deviceLoginEntity.Password.ToUpper())
                                {
                                    //开始更新硬件设备的状态为“在线”,更新SessionID
                                    int tmp1 = multimediaSysOperate.UpdateTerminalEquipmentInfo1(deviceLoginEntity.DeviceCode, sessionId_Tmp);
                                    string msg2 = GetRightLoginInfo(sessionId_Tmp);
                                    if (sessionT != null)
                                    {
                                        sessionT.Send(this.GetType().Name + ":" + msg2);
                                    }
                                    else if (sessionW != null)
                                    {
                                        sessionW.Send(this.GetType().Name + ":" + msg2);
                                    }                                    
                                }
                                else
                                {
                                    if (sessionT != null)
                                    {
                                        sessionT.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"密码错误。\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionT.SessionID + "\"}");                                                                                                                                        
                                    }
                                    else if (sessionW != null)
                                    {
                                        sessionW.Send(this.GetType().Name + ":" + "{ \"status\":0, \"msg\": \"密码错误。\",\"receiveMsg\":" + requestInfoTemp2 + ",\"key\":\"" + sessionW.SessionID + "\"}");                                                                                                                                        
                                    }                                      
                                }
                            }
                        }
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

        /// <summary>
        /// 登陆成功之后返回的信息
        /// </summary>
        /// <param name="SessionID"></param>
        /// <returns></returns>
        public string GetRightLoginInfo(string SessionID)
        {
            JsonObjectCollection delivered = new JsonObjectCollection();
            delivered.Add(new JsonNumericValue("status",1));
            delivered.Add(new JsonStringValue("msg", "登陆成功。"));
            delivered.Add(new JsonStringValue("key", SessionID));

            JsonArrayCollection collection1 = new JsonArrayCollection();
            collection1.Name = "NeedUpdateInfo";
            DataSet dsTmp = new DataSet();
            if (dsTmp.Tables.Count>0)
            {
                for (int i = 0; i < dsTmp.Tables[0].Rows.Count; i++)
                {
                    JsonObjectCollection clt = new JsonObjectCollection();
                    clt.Add(new JsonStringValue("待定", "待定"));
                    collection1.Add(clt);
                }
            }            
            delivered.Add(collection1);

            return delivered.ToString();
        }

    }
}
