using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tender.Model
{
    public class ChargeJsonModel
    {
        public string AppID { get; set; }
        public string _IP_ { get; set; }
        public string _RequestTime_ { get; set; }
        public string _Language_ { get; set; }
        public string _UserAgent_ { get; set; }

        public string _Route_ { get; set; }

        public ChargeData Data { get; set; }

    }
    public class ChargeData
    {
        public string AccountID { get; set; }
        public double StatTime { get; set; }
        public string ItemType { get; set; }
        public string IP { get; set; }
        public string OrderID { get; set; }
        public string ItemCnt { get; set; }
        public string ChargeNum { get; set; }
        public string ChargeCode { get; set; }
        public string ServerID { get; set; }
        public string AppID { get; set; }
        public string Price { get; set; }
        public string UserID { get; set; }
        public string ChargeType { get; set; }
        public string UserAgent { get; set; }
        public string AccountName { get; set; }

    }
}
