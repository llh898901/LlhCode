using System;
using System.Collections.Generic;
using UMeng.Model;
using UMeng.UMengAPI;

namespace UMeng
{
    public class Program
    {
        //public static string StrConnMsg = "server=192.168.9.102;database=SD_DEV_AD;uid=sd_dev;pwd=sd.com.cn";
        //public static string appKey = "587547794ad1567b03001eec";
        //public static string userKey = "e3195d1988d8a72e21431743e703b106";
        //public static string secretkey = "a253ca99297b620d47aa2075f6fa4dfc";
        static void Main(string[] args)
        {
            Console.WriteLine("友盟数据回调开始！");
            Common com = new Common();
            com.CreateTable();
            ModuleConfig config = new ModuleConfig();
            List<TaskModule> Tasks = config.GetModules();
            
            foreach (var item in Tasks)
            {
                RunRule(item);
            }
            Console.Write("Done!");
        }
        public static void RunRule(TaskModule task)
        {
            Activation activation = new Activation();
            string appKey = task.AppKey;
            string userKey = task.UserKey;
            string secretkey = task.Secretkey;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            if (task.Manual_Run)
            {
                startDate = task.StartDate;
                endDate = task.EndDate;
            }else
            {
                string value = DB.Context.From<ETLStatus>().Select(o => o.Value).Where(o => o.Key == "UMeng_Activation").First().Value;
                startDate = Convert.ToDateTime(value).AddDays(1);
                endDate = DateTime.Now.Date;
            }
            //激活数据
            activation.ActivationAll(secretkey, appKey, userKey,startDate,endDate);
        }
        public static List<UMeng_Activation> Test()
        {
            var data = DB.Context.From<UMeng_Activation>().ToList<UMeng_Activation>();
            return data;
        }

        #region 测试代码
        //public static void ActivationAll()
        //{
        //    string value = DB.Context.From<ETLStatus>().Select(o => o.Value).Where(o => o.Key == "YMActivation").First().Value;
        //    DateTime startTime = Convert.ToDateTime(value).AddDays(1);
        //    while (startTime <DateTime.Now.Date)
        //    {
        //        string active_date = string.Format("{0:yyyyMMdd}", startTime);
        //        int runtimes = 1;
        //        var result = "";
        //        while (runtimes <= 3)
        //        {
        //            try
        //            {
        //                result = GetActivation(1, active_date);
        //                break;
        //            }
        //            catch (Exception)
        //            {
        //                runtimes += 1;
        //            }
        //        }
        //        JObject jo = (JObject)JsonConvert.DeserializeObject(result);
        //        var num = JsonConvert.DeserializeObject<int>(JsonConvert.SerializeObject(jo["ext"]["num"]));
        //        if (num > 0)
        //        {
        //            var delete = DB.Context.Delete<YM_Activation>(o=>o.Active_Date==active_date);
        //            InsertActivation(result, active_date);
        //            int pageCnt = num / 500 + 1;
        //            for (int i = 2; i <= pageCnt; i++)
        //            {
        //                int runtimes2 = 1;
        //                while (runtimes2 <= 3)
        //                {
        //                    try
        //                    {
        //                        result = GetActivation(i, active_date);
        //                        InsertActivation(result, active_date);
        //                        break;
        //                    }
        //                    catch (Exception)
        //                    {
        //                        runtimes += 1;
        //                    }
        //                }
        //            }
        //        }
        //        var updateStatus = DB.Context.Update<ETLStatus>(ETLStatus._.Value, string.Format("{0:yyyy-MM-dd}", startTime), ETLStatus._.Key == "YMActivation");
        //        startTime = startTime.AddDays(1);
        //    }
        //}
        //public static string GetActivation(int pagenum,string active_date)
        //{
            
        //    int timestamp = GetTimestamp(DateTime.Now);
        //    string auth = MD5Encrypt(secretkey + timestamp + active_date, 32);

        //    string activationUrl = "https://apptrackapi.umeng.com/?c=download&a=getdatadetail&app_key={0}&user_key={1}&timestamp={2}&auth={3}&type={4}&rpid={5}&page_num={6}&limit={7}&active_date={8}";
        //    activationUrl = string.Format(activationUrl, appKey, userKey, timestamp, auth, 1, 0, pagenum, 500, active_date);
        //    string result = GetHttpResponse(activationUrl, 30000);
        //    return result;
        //}
        //static void InsertActivation(string result, string active_date)
        //{
        //    JObject jo = (JObject)JsonConvert.DeserializeObject(result);
        //    string listStr = JsonConvert.SerializeObject(jo["ext"]["list"]);
        //    List<YM_Activation> list = JsonConvert.DeserializeObject<List<YM_Activation>>(listStr);
        //    var num = DB.Context.Insert<YM_Activation>(list);
        //    Console.WriteLine(string.Format("激活数据插入{0}条记录",list.Count));
        //    #region 链接串Bulk方式插入
        //    //Console.WriteLine("使用Bulk方式插入");
        //    //Stopwatch sw = new Stopwatch();
        //    //DataTable dts = ToDataTable(list);
        //    //int num = list.Count;//dt.Rows.Count;

        //    //using (SqlConnection conn = new SqlConnection(StrConnMsg))
        //    //{
        //    //    SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);
        //    //    bulkCopy.DestinationTableName = "YM_Activation";
        //    //    bulkCopy.BatchSize = num;
        //    //    conn.Open();
        //    //    sw.Start();
        //    //    if (dts != null && dts.Rows.Count != 0)
        //    //    {
        //    //        bulkCopy.WriteToServer(dts);
        //    //        sw.Stop();
        //    //    }
        //    //    Console.WriteLine(string.Format("激活数据插入{0}条记录共花费{1}毫秒", num, sw.ElapsedMilliseconds));
        //    //}
        //    #endregion
        //}

        //public static void PlanList()
        //{
        //    string url1 = "https://gateway.open.umeng.com/openapi/";
        //    int pageNum = 1;
        //    int pageSize = 500;
        //    int _aop_timestamp = GetTimestamp(DateTime.Now);
        //    string url2 = "param2/1/com.umeng.adplus/umeng.adplus.apptrack.getPlanList/" + appKey + "_aop_timestamp" + _aop_timestamp + "pageNum" + pageNum + "pageSize" + pageSize;//"param2/1/com.umeng.adplus/umeng.adplus.apptrack.getClickActiveData/2340629_aop_timestamp1523243036070planId58421queryDate2018-03-11unitId138689";
        //    string _aop_signature = HmacSha1(secretkey, url2);
        //    string url = url1+ "param2/1/com.umeng.adplus/umeng.adplus.apptrack.getPlanList/" + appKey + "?_aop_signature="+ _aop_signature +"&_aop_timestamp=" + _aop_timestamp + "&pageNum=" + pageNum + "&pageSize=" + pageSize;
        //    Console.WriteLine(url);
        //}
        ///// <summary>
        ///// HmacSha1加密
        ///// </summary>
        ///// <param name="secret">秘钥</param>
        ///// <param name="strOrgData">原文</param>
        ///// <returns></returns>
        //public static string HmacSha1(string secret, string strOrgData)
        //{
        //    var encoding = new System.Text.UTF8Encoding();
        //    HMACSHA1 hmacsha1 = new HMACSHA1(encoding.GetBytes(secret));
        //    var hashBytes = hmacsha1.ComputeHash(encoding.GetBytes(strOrgData));
        //    StringBuilder EnText = new StringBuilder();
        //    foreach (byte Byte in hashBytes)
        //    {
        //        EnText.AppendFormat("{0:x2}", Byte);
        //    }
        //    return EnText.ToString();
        //    //return Convert.ToBase64String(hashBytes);
        //}
        //public static string GetHttpResponse(string url, int Timeout)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    request.Method = "GET";
        //    request.ContentType = "text/html;charset=UTF-8";
        //    request.UserAgent = null;
        //    request.Timeout = Timeout;

        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    Stream myResponseStream = response.GetResponseStream();
        //    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
        //    string retString = myStreamReader.ReadToEnd();
        //    myStreamReader.Close();
        //    myResponseStream.Close();

        //    return retString;
        //}
    
        ///// <summary>
        ///// 用MD5加密字符串，可选择生成16位或者32位的加密字符串
        ///// </summary>
        ///// <param name="password">待加密的字符串</param>
        ///// <param name="bit">位数，一般取值16 或 32</param>
        ///// <returns>返回的加密后的字符串</returns>
        //public  static string MD5Encrypt(string password, int bit)
        //{
        //    MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
        //    byte[] hashedDataBytes;
        //    hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(password));
        //    StringBuilder tmp = new StringBuilder();
        //    foreach (byte i in hashedDataBytes)
        //    {
        //        tmp.Append(i.ToString("x2"));
        //    }
        //    if (bit == 16)
        //        return tmp.ToString().Substring(8, 16);
        //    else
        //    if (bit == 32) return tmp.ToString();//默认情况
        //    else return string.Empty;
        //}

        //public static int GetTimestamp(DateTime d)
        //{
        //    TimeSpan ts = d.ToUniversalTime() - new DateTime(1970, 1, 1);
        //    return (int)ts.TotalSeconds;     //精确到秒
        //}
        #endregion
       
    }
}
