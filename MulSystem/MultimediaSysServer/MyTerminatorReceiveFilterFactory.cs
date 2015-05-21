using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer
{
    public class MyTerminatorReceiveFilterFactory:TerminatorReceiveFilterFactory
    {
        public MyTerminatorReceiveFilterFactory()
            :this(Encoding.UTF8)
        { }

        public MyTerminatorReceiveFilterFactory(Encoding encoding):this(encoding,new BasicRequestInfoParser())
        { }

        public MyTerminatorReceiveFilterFactory(Encoding encoding,IRequestInfoParser<StringRequestInfo> requestInfoParaser)
            : base(ConfigurationManager.AppSettings["CustomTerminatorSign"], encoding, requestInfoParaser)
        { }

    }
}
