//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MultimediaSysServer.EntityModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class jjj_ProgramInfoItem_TimeDetail
    {
        public int ID { get; set; }
        public int ProgramInfoItemID { get; set; }
        public string FontFamily { get; set; }
        public Nullable<int> FontSize { get; set; }
        public string FontFormat { get; set; }
        public Nullable<int> IsBackgroundTransparent { get; set; }
        public Nullable<int> IsBold { get; set; }
        public string BackgroundColor { get; set; }
        public string foreColor { get; set; }
        public string ProgramInfoItemTimeClientId { get; set; }
        public string Second_ProgramInfoItemTime_ServerBack { get; set; }
    
        public virtual jjj_ProgramInfoListItem jjj_ProgramInfoListItem { get; set; }
    }
}
