using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer.EntityClasses
{
    public class ProgramInfoListItem_EntityModel
    {
        public Nullable<int> status { get; set; }
        public string msg { get; set; }
        public Nullable<int> ProgramInfoItemID { get; set; }
        public Nullable<int> ProgramInfoID { get; set; }
        public Nullable<int> ProgramInfoItemType { get; set; }
        public Nullable<int> ProgramInfoItemWidth { get; set; }
        public Nullable<int> ProgramInfoItemHeight { get; set; }
        public Nullable<int> ProgramInfoItemLayer { get; set; }
        public string ProgramInfoItemTitle { get; set; }
        public Nullable<decimal> ProgramInfoItemLeft { get; set; }
        public Nullable<decimal> ProgramInfoItemTop { get; set; }
        public Nullable<int> IsShowWater { get; set; }
    }
}
