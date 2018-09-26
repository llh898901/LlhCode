using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMeng.UMengAPI
{
    public class Activation
    {
        Common com = new Common();
        
        public  void ActivationAll(string secretkey,string appKey,string userKey,DateTime startDate,DateTime endDate)
        {
            while (startDate <= endDate && startDate < DateTime.Now.Date)
            {
                string active_date = string.Format("{0:yyyyMMdd}", startDate);
                int runtimes = 1;
                var result = "";
                while (runtimes <= 3)
                {
                    try
                    {
                        result = GetActivation(secretkey,appKey,userKey,1, active_date);
                        break;
                    }
                    catch (Exception)
                    {
                        runtimes += 1;
                    }
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                var num = JsonConvert.DeserializeObject<int>(JsonConvert.SerializeObject(jo["ext"]["num"]));
                if (num > 0)
                {
                    var delete = DB.Context.Delete<UMeng_Activation>(o => o.Active_Date == active_date);
                    InsertActivation(result, active_date);
                    int pageCnt = num / 500 + 1;
                    for (int i = 2; i <= pageCnt; i++)
                    {
                        int runtimes2 = 1;
                        while (runtimes2 <= 3)
                        {
                            try
                            {
                                result = GetActivation(secretkey, appKey, userKey, i, active_date);
                                InsertActivation(result, active_date);
                                break;
                            }
                            catch (Exception)
                            {
                                runtimes += 1;
                            }
                        }
                    }
                }
                var updateStatus = DB.Context.Update<ETLStatus>(ETLStatus._.Value, string.Format("{0:yyyy-MM-dd}", startDate), ETLStatus._.Key == "UMeng_Activation");
                startDate = startDate.AddDays(1);
            }
        }
        public  string GetActivation(string secretkey, string appKey, string userKey,int pagenum, string active_date)
        {

            int timestamp = com.GetTimestamp(DateTime.Now);
            string auth = com.MD5Encrypt(secretkey + timestamp + active_date, 32);

            string activationUrl = "https://apptrackapi.umeng.com/?c=download&a=getdatadetail&app_key={0}&user_key={1}&timestamp={2}&auth={3}&type={4}&rpid={5}&page_num={6}&limit={7}&active_date={8}";
            activationUrl = string.Format(activationUrl, appKey, userKey, timestamp, auth, 1, 0, pagenum, 500, active_date);
            string result = com.GetHttpResponse(activationUrl, 30000);
            return result;
        }
        static void InsertActivation(string result, string active_date)
        {
            JObject jo = (JObject)JsonConvert.DeserializeObject(result);
            string listStr = JsonConvert.SerializeObject(jo["ext"]["list"]);
            List<UMeng_Activation> list = JsonConvert.DeserializeObject<List<UMeng_Activation>>(listStr);
            var num = DB.Context.Insert<UMeng_Activation>(list);
            Console.WriteLine(string.Format("{0} 激活数据插入 {1} 条记录", active_date,list.Count));
            #region 链接串Bulk方式插入
            //Console.WriteLine("使用Bulk方式插入");
            //Stopwatch sw = new Stopwatch();
            //DataTable dts = ToDataTable(list);
            //int num = list.Count;//dt.Rows.Count;

            //using (SqlConnection conn = new SqlConnection(StrConnMsg))
            //{
            //    SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);
            //    bulkCopy.DestinationTableName = "YM_Activation";
            //    bulkCopy.BatchSize = num;
            //    conn.Open();
            //    sw.Start();
            //    if (dts != null && dts.Rows.Count != 0)
            //    {
            //        bulkCopy.WriteToServer(dts);
            //        sw.Stop();
            //    }
            //    Console.WriteLine(string.Format("激活数据插入{0}条记录共花费{1}毫秒", num, sw.ElapsedMilliseconds));
            //}
            #endregion
        }

    }
}
