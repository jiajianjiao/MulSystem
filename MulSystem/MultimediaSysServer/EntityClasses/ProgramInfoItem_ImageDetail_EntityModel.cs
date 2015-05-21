using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer.EntityClasses
{
    public class ProgramInfoItem_ImageDetail_EntityModel
    {
        public Nullable<int> status { get; set; }
        public string msg { get; set; }

        public Nullable<int> ID { get; set; }
        public Nullable<int> ProgramInfoItemID { get; set; }
        public Nullable<int> ImageLayer { get; set; }
        public Nullable<int> TimeLength { get; set; }
        public Nullable<int> CartoonType { get; set; }
        public string ImageUrl { get; set; }
    }
}
