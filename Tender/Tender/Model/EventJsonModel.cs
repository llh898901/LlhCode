using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tender.Model
{
    public class EventJsonModel
    {
        public string AppID { get; set; }
        public string _IP_ { get; set; }
        public string _RequestTime_ { get; set; }
        public string _Language_ { get; set; }
        public string _UserAgent_ { get; set; }

        public string _Route_ { get; set; }
        public EventData Data { get; set; }
    }
    public class EventData
    {
        public string AccountID { get; set; }
        public string AppID { get; set; }
        public string ChannelCode { get; set; }
        public double CreateTime { get; set; }
        public string DeviceID { get; set; }
        public string DeviceModel { get; set; }
        public string EventID { get; set; }
        public string EventValue { get; set; }
        public string Platform { get; set; }
        public string ServerID { get; set; }
        public string UserID { get; set; }

    }
}
