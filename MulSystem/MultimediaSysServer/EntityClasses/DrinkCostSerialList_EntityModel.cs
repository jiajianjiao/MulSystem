using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer.EntityClasses
{
    /// <summary>
    /// 用户饮水消费记录实体类
    /// </summary>
    public class DrinkCostSerialList_EntityModel
    {
        public Nullable<int> status { get; set; }
        public string msg { get; set; }

        public string SerialNumber { get; set; }
        public string IcCardNo { get; set; }
        public string DeviceCode { get; set; }

        public Nullable<decimal> CostFee { get; set; }
        public Nullable<decimal> RunningWaterLitre { get; set; }
        public Nullable<decimal> CurrentUserBalance { get; set; }

        public Nullable<DateTime> CostTime { get; set; }
    }
}
