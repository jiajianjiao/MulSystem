using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer.EntityClasses
{
    public class ProgramInfoItem_TimeDetail_EntityModel
    {
        public Nullable<int> status { get; set; }
        public string msg { get; set; }
        public Nullable<int> ID { get; set; }
        public Nullable<int> ProgramInfoItemID { get; set; }
        public Nullable<int> FontSize { get; set; }
        public Nullable<int> IsBackgroundTransparent { get; set; }
        public Nullable<int> IsBold { get; set; }
        public string FontFamily { get; set; }
        public string FontFormat { get; set; }
        public string BackgroundColor { get; set; }
        public string foreColor { get; set; }
    }
}
