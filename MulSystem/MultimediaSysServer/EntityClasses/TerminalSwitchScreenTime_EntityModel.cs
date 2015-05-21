using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer.EntityClasses
{
    public class TerminalSwitchScreenTime_EntityModel
    {
        public Nullable<int> status { get; set; }
        public string msg { get; set; }
        public Nullable<int> TerminalSwitchScreenTimeID { get; set; }
        public Nullable<int> TerminalEquipmentID { get; set; }
        public string SessionId { get; set; }

        public Nullable<TimeSpan> TurnOnTime { get; set; }
        public Nullable<TimeSpan> TurnOffTime { get; set; }

        public Nullable<bool> IsMonday { get; set; }
        public Nullable<bool> IsTuesday { get; set; }
        public Nullable<bool> IsWednesday { get; set; }
        public Nullable<bool> IsThursday { get; set; }
        public Nullable<bool> IsFriday { get; set; }
        public Nullable<bool> IsSaturday { get; set; }
        public Nullable<bool> IsSunday { get; set; }

        public Nullable<bool> IsEnable { get; set; }
        public Nullable<DateTime> UpdateTime { get; set; }
        public Nullable<int> IsSend { get; set; }
    }
}
