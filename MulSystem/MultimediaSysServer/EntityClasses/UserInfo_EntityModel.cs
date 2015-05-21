using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer.EntityClasses
{
    public class UserInfo_EntityModel
    {
        public int uid { get; set; }
        public int groupid { get; set; }
        public string username { get; set; }
        public string grouptitle { get; set; }
        public string actualname { get; set; }
        public string phone { get; set; }
        public string HengYuCode { get; set; }
        public string password { get; set; }
        public Nullable<int> isEnable { get; set; }
    }

}
