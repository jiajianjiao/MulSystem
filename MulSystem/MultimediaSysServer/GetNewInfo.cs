using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace MultimediaSysServer
{
    public class GetNewInfo : CommandBase<TelnetSession, StringRequestInfo>
    {
        public override void ExecuteCommand(TelnetSession session, StringRequestInfo requestInfo)
        {
            session.Send(this.GetType().Name+":"+"{\"msg\":\"key是:" + requestInfo.Key + ",body是：" + requestInfo.Body+"\"}" + ConfigurationManager.AppSettings["CustomTerminatorSign"]);            
        }
    }

}
