using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer.EntityClasses
{
    public class ProgramInfoItem_VideoDetail_EntityModel
    {
        public Nullable<int> status { get; set; }
        public string msg { get; set; }

        public Nullable<int> ID { get; set; }
        public Nullable<int> ProgramInfoItemID { get; set; }
        public Nullable<int> VideoLayer { get; set; }
        public Nullable<int> VideoVoice { get; set; }
        public string VideoUrl { get; set; }
    }
}
