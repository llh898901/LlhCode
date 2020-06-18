using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tender.Model
{
    public class OnlineJsonModel
    {
        public string AppID { get; set; }
        public string _IP_ { get; set; }
        public string _RequestTime_ { get; set; }
        public string _Language_ { get; set; }
        public string _UserAgent_ { get; set; }

        public string _Route_ { get; set; }
        public OnlineData Data {get;set;}
    }
    public class OnlineData
    {
        public string ServerID { get; set; }
        public string AccountCnt { get; set; }
        public string AppID { get; set; }
        public string Platform { get; set; }
        public double StatTime { get; set; }
        public string ServerName { get; set; }

    }
}
