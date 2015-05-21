using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer.EntityClasses
{
    public class ProgramSendSerialList_EntityModel
    {
        public Nullable<int> status { get; set; }
        public string msg { get; set; }

        public Nullable<int> ProgramSendSerialID { get; set; }
        public Nullable<int> TerminalEquipmentID { get; set; }
        public Nullable<int> ProgramInfoID { get; set; }
        /// <summary>
        /// 节目发送状态：0为未发送，1为已发送，2为客户端已成功接收到推送完整消息。
        /// </summary>
        public Nullable<int> SendStatus { get; set; }
        public Nullable<DateTime> EfficaciousDateStart { get; set; }
        public Nullable<DateTime> EfficaciousDateEnd { get; set; }
        public Nullable<TimeSpan> EfficaciousTimeStart { get; set; }
        public Nullable<TimeSpan> EfficaciousTimeEnd { get; set; }
        public string SessionId { get; set; }
        public string DeviceCode { get; set; }
        public string TerminalName { get; set; }
        public Nullable<DateTime> ReceiveBackTime { get; set; }
    }

}
