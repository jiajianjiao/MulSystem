using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysClient.EntityClasses
{
    public class TerminalEquipmentSerialCommand_EntityModel
    {
        public Nullable<int> status { get; set; }
        public string msg { get; set; }
        public Nullable<int> TerSerialCommandID { get; set; }
        public Nullable<int> TerminalEquipmentID { get; set; }
        public Nullable<int> TerminalStatus { get; set; }
        public string SessionId { get; set; }
        public string DeviceCode { get; set; }
        public string TerminalName { get; set; }

        public Nullable<int> CommandTypeId { get; set; }
        public string SendContent { get; set; }
        public Nullable<int> SendStatus { get; set; }
        public Nullable<DateTime> ReceiveBackTime { get; set; }
        public Nullable<DateTime> SendTime { get; set; }

        public string SendAdminName { get; set; }
    }
}
