using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tender.Model
{
    public class LoginJsonModel
    {
        public string AppID { get; set; }
        public string _IP_ { get; set; }
        public string _RequestTime_ { get; set; }
        public string _Language_ { get; set; }
        public string _UserAgent_ { get; set; }

        public string _Route_ { get; set; }
        public LoginData Data { get; set; }
    }
    public class LoginData
    {
        public string AccountID { get; set; }
        public string ActType { get; set; }
        public string Platform { get; set; }
        public string DeviceID { get; set; }
        public string ScreenX { get; set; }
        public string NetMode { get; set; }
        public string ScreenY { get; set; }
        public string IP { get; set; }
        public double CreateTime { get; set; }
        public string ChannelCode { get; set; }
        public string AppVer { get; set; }
        public string ServerID { get; set; }
        public string AppID { get; set; }
        public string DeviceModelodel { get; set; }
        public string UserID { get; set; }
        public string UserAgent { get; set; }
        public string SessionID { get; set; }
        public string DeviceVer { get; set; }

    }
}
