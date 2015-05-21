using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer.EntityClasses
{
    public class MaterialManagementDetail
    {
        public Nullable<int> status { get; set; }
        public string msg { get; set; }

        public int ID { get; set; }
        public int MaterialID { get; set; }
        public string FileName { get; set; }
        public Nullable<decimal> FileSize { get; set; }
        public Nullable<int> FileType { get; set; }
        public string FilePath { get; set; }
        public string FileSuffixName { get; set; }
        public Nullable<DateTime> UploadTime { get; set; }
        public Nullable<int> UploadUserId { get; set; }
        public string TransformFilePath { get; set; }

        public string TransformFileSuffixName { get; set; }
        public string ThumbnailImgPath { get; set; }
        public string OriginalFileName { get; set; }

    }
}
