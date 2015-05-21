using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer.EntityClasses
{
    public class ProgramInfoItem_DocumentDetail_EntityModel
    {
        public Nullable<int> status { get; set; }
        public string msg { get; set; }
        public Nullable<int> ID { get; set; }
        public Nullable<int> ProgramInfoItemID { get; set; }
        public Nullable<int> DocumentLayer { get; set; }
        public Nullable<int> TimeLength { get; set; }
        public Nullable<int> CartoonType { get; set; }
        public string DocumentUrl { get; set; }
    }
}
