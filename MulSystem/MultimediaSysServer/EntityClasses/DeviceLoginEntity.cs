using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer.EntityClasses
{
    [Serializable()]
    public class DeviceLoginEntity
    {
        public string DeviceCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
