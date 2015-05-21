using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer.EntityClasses
{
    public class ProgramInfoList_EntityModel
    {
        public Nullable<int> status { get; set; }
        public string msg { get; set; }

        public Nullable<int> ProgramInfoID { get; set; }
        public Nullable<int> ProgramWidth { get; set; }
        public Nullable<int> ProgramHeight { get; set; }
        public Nullable<int> ProgramStatus { get; set; }
        public Nullable<int> CreateUserId { get; set; }
        public string ProgramTitle { get; set; }
        public Nullable<DateTime> CreateTime { get; set; }
        public Nullable<DateTime> UpdateTime { get; set; }
        
    }
}
