using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using MultimediaSysServer.commonClasses;

namespace MultimediaSysServer
{
    public class TelnetServerByte : AppServer<TelnetSession>
    {
        public TelnetServerByte()
            : base(new DefaultReceiveFilterFactory<SwitchReceiveFilter, StringRequestInfo>())
        {

        }
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            return base.Setup(rootConfig, config);
        }

        protected override void OnStarted()
        {
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }
    }

    //public class DefaultReceiveFilterFactory_My : DefaultReceiveFilterFactory<SwitchReceiveFilter, StringRequestInfo>
    //{
    //    public DefaultReceiveFilterFactory_My()
    //        : base(new MyTerminatorReceiveFilterFactory(Encoding.UTF8, new BasicRequestInfoParser(":", ",")))
    //    {

    //    }
    //}
}
