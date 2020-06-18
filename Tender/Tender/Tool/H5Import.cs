using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Tender.Model;

namespace Tender.Tool
{
    public class H5Import
    {
        public void ImportCharge()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            StreamReader sr = new StreamReader("E:\\log\\201903\\H5_Charge\\H5_Charge-20190306150000.log");
            string s = sr.ReadLine();
            List<H5_Charge_201903> list = new List<H5_Charge_201903>();
            Mapper.Initialize(x =>
                    x.CreateMap<ChargeJsonModel, H5_Charge_201903>()
                    .ForMember(a => a.AppID, o => o.MapFrom(t => t.Data.AppID))
                    .ForMember(a => a.AccountID, o => o.MapFrom(t => t.Data.AccountID))
                    .ForMember(a => a.ItemType, o => o.MapFrom(t => t.Data.ItemType))
                    .ForMember(a => a.IP, o => o.MapFrom(t => t.Data.IP))
                    .ForMember(a => a.OrderID, o => o.MapFrom(t => t.Data.OrderID))
                    .ForMember(a => a.ItemCnt, o => o.MapFrom(t => t.Data.ItemCnt))
                    .ForMember(a => a.ChargeNum, o => o.MapFrom(t => t.Data.ChargeNum))
                    .ForMember(a => a.ChargeCode, o => o.MapFrom(t => t.Data.ChargeCode))
                    .ForMember(a => a.ServerID, o => o.MapFrom(t => t.Data.ServerID))
                    .ForMember(a => a.Price, o => o.MapFrom(t => t.Data.Price))
                    .ForMember(a => a.UserID, o => o.MapFrom(t => t.Data.UserID))
                    .ForMember(a => a.ChargeType, o => o.MapFrom(t => t.Data.ChargeType))
                    .ForMember(a => a.AccountName, o => o.MapFrom(t => t.Data.AccountName))
                    .ForMember(a => a.StatTime, o => o.MapFrom(t => t.Data.StatTime))

                );
            while (s != null)
            {
                string chargeJson = s.Substring(0, s.LastIndexOf("}}") + 2);
                DateTime _AddTime_ = Convert.ToDateTime(s.Substring(s.LastIndexOf("}}") + 5));
                var chargeModel = js.Deserialize<ChargeJsonModel>(chargeJson);
                DateTime _StatTime_ = startTime.AddSeconds(chargeModel.Data.StatTime);
                int _PartDay_ = _StatTime_.Day;
                
                var charge_201903 = Mapper.Map<H5_Charge_201903>(chargeModel);
                charge_201903._AddTime_ = _AddTime_;
                charge_201903._StatTime_ = _StatTime_;
                charge_201903._PartDay_ = _PartDay_;
                list.Add(charge_201903);
                //1万条入库
                if (list.Count >= 10000)
                {
                    DosHelper.SqlBuckCopy<H5_Charge_201903>(DB.Context, list, "dbo.H5_Charge_201903");
                    list.Clear();
                }
                s = sr.ReadLine();//读取每行
                // DB.Context.Insert<H5_Charge_201903>(charge_201903);
            }
            if (list.Count > 0 && list.Count < 10000)
            {
                DosHelper.SqlBuckCopy<H5_Charge_201903>(DB.Context, list, "dbo.H5_Charge_201903");
                list.Clear();
            }
        }
        public void ImportConsume()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            StreamReader sr = new StreamReader("E:\\log\\201903\\H5_Consume\\H5_Consume-20190306150000.log");
            string s = sr.ReadLine();
            List<H5_Consume_201903> list = new List<H5_Consume_201903>();
            Mapper.Initialize(x =>
                    x.CreateMap<ConsumeJsonModel, H5_Consume_201903>()
                    .ForMember(a => a.AppID, o => o.MapFrom(t => t.Data.AppID))
                    .ForMember(a => a.AccountID, o => o.MapFrom(t => t.Data.AccountID))
                    .ForMember(a => a.ItemType, o => o.MapFrom(t => t.Data.ItemType))
                    .ForMember(a => a.IP, o => o.MapFrom(t => t.Data.IP))
                    .ForMember(a => a.ItemCnt, o => o.MapFrom(t => t.Data.ItemCnt))
                    .ForMember(a => a.ServerID, o => o.MapFrom(t => t.Data.ServerID))
                    .ForMember(a => a.Price, o => o.MapFrom(t => t.Data.Price))
                    .ForMember(a => a.UserID, o => o.MapFrom(t => t.Data.UserID))
                    .ForMember(a => a.StatTime, o => o.MapFrom(t => t.Data.StatTime))
                    .ForMember(a => a.Type, o => o.MapFrom(t => t.Data.Type))
                    .ForMember(a => a.Number, o => o.MapFrom(t => t.Data.Number))
                    .ForMember(a => a.Price, o => o.MapFrom(t => t.Data.Price))
                    .ForMember(a => a.MoneyType, o => o.MapFrom(t => t.Data.MoneyType))

                );
            while (s != null)
            {
                string consumeJson = s.Substring(0, s.LastIndexOf("}}") + 2);
                DateTime _AddTime_ = Convert.ToDateTime(s.Substring(s.LastIndexOf("}}") + 5));
                var consumeModel = js.Deserialize<ConsumeJsonModel>(consumeJson);
                DateTime _StatTime_ = startTime.AddSeconds(consumeModel.Data.StatTime);
                int _PartDay_ = _StatTime_.Day;

                var consume_201903 = Mapper.Map<H5_Consume_201903>(consumeModel);
                consume_201903._AddTime_ = _AddTime_;
                consume_201903._StatTime_ = _StatTime_;
                consume_201903._PartDay_ = _PartDay_;
                list.Add(consume_201903);
                //1万条入库
                if (list.Count >= 10000)
                {
                    DosHelper.SqlBuckCopy<H5_Consume_201903>(DB.Context, list, "dbo.H5_Consume_201903");
                    list.Clear();
                }
                s = sr.ReadLine();//读取每行
            }
            if (list.Count > 0 && list.Count < 10000)
            {
                DosHelper.SqlBuckCopy<H5_Consume_201903>(DB.Context, list, "dbo.H5_Consume_201903");
                list.Clear();
            }
        }
        public void ImportOnline()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            StreamReader sr = new StreamReader("E:\\log\\201903\\H5_Online\\H5_Online-20190306140000.log");
            string s = sr.ReadLine();
            List<H5_Online_201903> list = new List<H5_Online_201903>();
            Mapper.Initialize(x =>
                    x.CreateMap<OnlineJsonModel, H5_Online_201903>()
                    .ForMember(a => a.AppID, o => o.MapFrom(t => t.Data.AppID))
                    .ForMember(a => a.ServerID, o => o.MapFrom(t => t.Data.ServerID))
                    .ForMember(a => a.StatTime, o => o.MapFrom(t => t.Data.StatTime))
                    .ForMember(a => a.AccountCnt, o => o.MapFrom(t => t.Data.AccountCnt))
                    .ForMember(a => a.Platform, o => o.MapFrom(t => t.Data.Platform))
                    .ForMember(a => a.ServerName, o => o.MapFrom(t => t.Data.ServerName))
                );
            while (s != null)
            {
                string onlineJson = s.Substring(0, s.LastIndexOf("}}") + 2);
                DateTime _AddTime_ = Convert.ToDateTime(s.Substring(s.LastIndexOf("}}") + 5));
                var onlineModel = js.Deserialize<OnlineJsonModel>(onlineJson);
                DateTime _StatTime_ = startTime.AddSeconds(onlineModel.Data.StatTime);
                int _PartDay_ = _StatTime_.Day;

                var online_201903 = Mapper.Map<H5_Online_201903>(onlineModel);
                online_201903._AddTime_ = _AddTime_;
                online_201903._StatTime_ = _StatTime_;
                online_201903._PartDay_ = _PartDay_;
                list.Add(online_201903);
                //1万条入库
                if (list.Count >= 10000)
                {
                    DosHelper.SqlBuckCopy<H5_Online_201903>(DB.Context, list, "dbo.H5_Online_201903");
                    list.Clear();
                }
                s = sr.ReadLine();//读取每行
            }
            if (list.Count > 0 && list.Count < 10000)
            {
                DosHelper.SqlBuckCopy<H5_Online_201903>(DB.Context, list, "dbo.H5_Online_201903");
                list.Clear();
            }
        }
        public void ImportLogin()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            StreamReader sr = new StreamReader("E:\\log\\201903\\H5_login\\H5_Login-20190306140000.log");
            string s = sr.ReadLine();
            List<H5_Login_201903> list = new List<H5_Login_201903>();
            Mapper.Initialize(x =>
                    x.CreateMap<LoginJsonModel, H5_Login_201903>()
                    .ForMember(a => a.AppID, o => o.MapFrom(t => t.Data.AppID))
                    .ForMember(a => a.ServerID, o => o.MapFrom(t => t.Data.ServerID))
                    .ForMember(a => a.CreateTime, o => o.MapFrom(t => t.Data.CreateTime))
                    .ForMember(a => a.Platform, o => o.MapFrom(t => t.Data.Platform))
                    .ForMember(a => a.AccountID, o => o.MapFrom(t => t.Data.AccountID))
                    .ForMember(a => a.ActType, o => o.MapFrom(t => t.Data.ActType))
                    .ForMember(a => a.DeviceID, o => o.MapFrom(t => t.Data.DeviceID))
                    .ForMember(a => a.ScreenX, o => o.MapFrom(t => t.Data.ScreenX))
                    .ForMember(a => a.NetMode, o => o.MapFrom(t => t.Data.NetMode))
                    .ForMember(a => a.ScreenY, o => o.MapFrom(t => t.Data.ScreenY))
                    .ForMember(a => a.IP, o => o.MapFrom(t => t.Data.IP))
                    .ForMember(a => a.ChannelCode, o => o.MapFrom(t => t.Data.ChannelCode))
                    .ForMember(a => a.AppVer, o => o.MapFrom(t => t.Data.AppVer))
                    .ForMember(a => a.DeviceModel, o => o.MapFrom(t => t.Data.DeviceModelodel))
                    .ForMember(a => a.UserID, o => o.MapFrom(t => t.Data.UserID))
                    .ForMember(a => a.SessionID, o => o.MapFrom(t => t.Data.SessionID))
                    .ForMember(a => a.DeviceVer, o => o.MapFrom(t => t.Data.DeviceVer))

                );
            while (s != null)
            {
                string loginJson = s.Substring(0, s.LastIndexOf("}}") + 2);
                DateTime _AddTime_ = Convert.ToDateTime(s.Substring(s.LastIndexOf("}}") + 5));
                var loginModel = js.Deserialize<LoginJsonModel>(loginJson);
                DateTime _StatTime_ = startTime.AddSeconds(loginModel.Data.CreateTime);
                int _PartDay_ = _StatTime_.Day;

                var login_201903 = Mapper.Map<H5_Login_201903>(loginModel);
                login_201903._AddTime_ = _AddTime_;
                login_201903._StatTime_ = _StatTime_;
                login_201903._PartDay_ = _PartDay_;
                list.Add(login_201903);
                //1万条入库
                if (list.Count >= 10000)
                {
                    DosHelper.SqlBuckCopy<H5_Login_201903>(DB.Context, list, "dbo.H5_Login_201903");
                    list.Clear();
                }
                s = sr.ReadLine();//读取每行
            }
            if (list.Count > 0 && list.Count < 10000)
            {
                DosHelper.SqlBuckCopy<H5_Login_201903>(DB.Context, list, "dbo.H5_Login_201903");
                list.Clear();
            }
        }
        public void ImportEvent()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            StreamReader sr = new StreamReader("E:\\log\\201903\\H5_event\\H5_Event-20190306130000.log");
            string s = sr.ReadLine();
            List<H5_Event_201903> list = new List<H5_Event_201903>();
            Mapper.Initialize(x =>
                    x.CreateMap<EventJsonModel, H5_Event_201903>()
                    .ForMember(a => a.AppID, o => o.MapFrom(t => t.Data.AppID))
                    .ForMember(a => a.ServerID, o => o.MapFrom(t => t.Data.ServerID))
                    .ForMember(a => a.CreateTime, o => o.MapFrom(t => t.Data.CreateTime))
                    .ForMember(a => a.Platform, o => o.MapFrom(t => t.Data.Platform))
                    .ForMember(a => a.AccountID, o => o.MapFrom(t => t.Data.AccountID))
                    .ForMember(a => a.DeviceID, o => o.MapFrom(t => t.Data.DeviceID))
                    .ForMember(a => a.ChannelCode, o => o.MapFrom(t => t.Data.ChannelCode))
                    .ForMember(a => a.DeviceModel, o => o.MapFrom(t => t.Data.DeviceModel))
                    .ForMember(a => a.UserID, o => o.MapFrom(t => t.Data.UserID))
                    .ForMember(a => a.EventID, o => o.MapFrom(t => t.Data.EventID))
                    .ForMember(a => a.EventValue, o => o.MapFrom(t => t.Data.EventValue))
                );
            while (s != null)
            {
                string eventJson = s.Substring(0, s.LastIndexOf("}}") + 2);
                DateTime _AddTime_ = Convert.ToDateTime(s.Substring(s.LastIndexOf("}}") + 5));
                var eventModel = js.Deserialize<EventJsonModel>(eventJson);
                DateTime _StatTime_ = startTime.AddSeconds(eventModel.Data.CreateTime);
                int _PartDay_ = _StatTime_.Day;

                var event_201903 = Mapper.Map<H5_Event_201903>(eventModel);
                event_201903._AddTime_ = _AddTime_;
                event_201903._StatTime_ = _StatTime_;
                event_201903._PartDay_ = _PartDay_;
                list.Add(event_201903);
                //1万条入库
                if (list.Count >= 10000)
                {
                    DosHelper.SqlBuckCopy<H5_Event_201903>(DB.Context, list, "dbo.H5_Event_201903");
                    list.Clear();
                }
                s = sr.ReadLine();//读取每行
            }
            if (list.Count > 0 && list.Count < 10000)
            {
                DosHelper.SqlBuckCopy<H5_Event_201903>(DB.Context, list, "dbo.H5_Event_201903");
                list.Clear();
            }
        }

        public void ImportSpend()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            StreamReader sr = new StreamReader("E:\\log\\spend_log\\spend-20181220.log");
            string s = sr.ReadLine();
            List<H5_spend> list = new List<H5_spend>();
           
            while (s != null)
            {
                var a = s.Split(new[] {','}, StringSplitOptions.None);

                H5_spend spend =  new H5_spend()
                {
                    _LogDate_ = Convert.ToDateTime("2018-12-20"),
                    CreateTime  = long.Parse(a[0]),
                    ChannelCode = int.Parse(a[1]),
                    AccountName = int.Parse(a[2]),
                    ServerID = int.Parse(a[3]),
                    UserID = long.Parse(a[4]),
                    VipExp = int.Parse(a[5]),
                    Fight = int.Parse(a[6]),
                    Costtype = int.Parse(a[7]),
                    Subtype = int.Parse(a[8]),
                    Item_num = int.Parse(a[9]),
                    MoneyType = int.Parse(a[10]),
                    MoneyNum = int.Parse(a[11]),
                };
                DB.Context.Insert<H5_spend>(spend);
                //1万条入库
                //if (list.Count >= 10000)
                //{
                //    DosHelper.SqlBuckCopy<H5_spend>(DB.Context, list, "dbo.H5_spend");
                //    list.Clear();
                //}
                s = sr.ReadLine();//读取每行
            }
            //if (list.Count > 0 && list.Count < 10000)
            //{
            //    DosHelper.SqlBuckCopy<H5_spend>(DB.Context, list, "dbo.H5_spend");
            //    list.Clear();
            //}
        }

    }
}
