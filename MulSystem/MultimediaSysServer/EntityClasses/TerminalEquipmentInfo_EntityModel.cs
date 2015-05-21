using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer.EntityClasses
{
    public class TerminalEquipmentInfo_EntityModel
    {
        public Nullable<int> TerminalEquipmentID { get; set; }
        public Nullable<int> TerminalEquipmentCategoryId { get; set; }
        public Nullable<int> CurrentUserId { get; set; }
        public Nullable<int> TerminalStatus { get; set; }
        public Nullable<int> IsRegister { get; set; }
        public Nullable<int> Layer { get; set; }
        public Nullable<int> AdvanceDownNum { get; set; }
        public string SessionId { get; set; }
        public string TerminalVersion { get; set; }
        public string TerminalIP { get; set; }
        public string TerminalPlatform { get; set; }
        public string DeviceCode { get; set; }
        public string TerminalName { get; set; }
        public string MainServerIP { get; set; }
        public string MainServerPort { get; set; }
        public string TerminalEquipmentCategoryName { get; set; }

        public Nullable<TimeSpan> DownloadStartTime { get; set; }
        public Nullable<TimeSpan> DownloadEndTime { get; set; }
        public Nullable<DateTime> UpdateTime { get; set; }
        public Nullable<int> IsSend { get; set; }
    }

}
