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
    public class TelnetServer:AppServer<TelnetSession>
    {
        public TelnetServer()
            : base(new MyTerminatorReceiveFilterFactory(Encoding.UTF8, new BasicRequestInfoParser(":", ",")))
            //:base(new DefaultReceiveFilterFactory<SwitchReceiveFilter, StringRequestInfo>())                     //请求的 key 和 body 通过字符 ':' 分隔, 而且多个参数被字符 ',' 分隔。 支持这种类型的请求非常简单, 你只需要用下面的代码扩展命令行协议:
            //: base(new CommandLineReceiveFilterFactory(Encoding.UTF8, new BasicRequestInfoParser(":", ",")))  //请求的 key 和 body 通过字符 ':' 分隔, 而且多个参数被字符 ',' 分隔。 支持这种类型的请求非常简单, 你只需要用下面的代码扩展命令行协议:
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

}
