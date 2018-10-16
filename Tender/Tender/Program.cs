using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tender.Model;

namespace Tender
{
    public class Program
    {
        static Search search = new Search();
        static Results results = new Results();
        //static TenderHelper t = new TenderHelper();
        static void Main(string[] args)
        {
            //var a = t.GetHtml("");
            Console.WriteLine("开始抓取数据");
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            ConfigHelper config = new ConfigHelper();
            //获取跑数配置
            List<TaskModule> Tasks = config.GetModules();
            List<string> keyWords = new List<string>();
            foreach (TaskModule item in Tasks)
            {
                Console.WriteLine("---开始抓取---业务名称:{0}", item.Name);
                //判断是补数还是正常跑数
                if (item.Manual_Run)
                {
                    startDate = item.StartDate;
                    endDate = item.EndDate;
                }
                else
                {
                    var ETLStatus = DB.Context.From<ETLStatus>().Select(o => o.Value).Where(o => o.Key == item.Name).First();
                    if (ETLStatus == null)
                    {
                        startDate = DateTime.Now.Date.AddDays(-1);
                        endDate = startDate;
                    }
                    else
                    {
                        if (ETLStatus.Value == DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"))
                        {
                            continue;
                        }
                        startDate = Convert.ToDateTime(ETLStatus.Value).AddDays(1);
                        endDate = DateTime.Now.Date.AddDays(-1);
                    }
                }
                keyWords = DB.Context.From<Tender_Setting>().Where(o => o.Name == item.Name).Select(o => o.KeyWord).ToList<string>(); ;
                foreach (string keyWord in keyWords)
                {
                    Console.WriteLine("---开始抓取---招-标-数-据---关键词:{0}", keyWord);
                    //获取招标搜索结果列表
                    List<string> urlList = search.Start(keyWord, startDate, endDate, "1");
                    //避免重复加载
                    var delete = DB.Context.Delete<Tender>(o => o.StatDate >= startDate && o.StatDate <= endDate && o.KeyWord == keyWord);
                    foreach (string url in urlList)
                    {
                        //根据链接获取表格并入库
                        results.GetTenderTable(keyWord, item.Name, url);
                    }

                    Console.WriteLine("---开始抓取---中-标-数-据---关键词:{0}", keyWord);
                    //获取中标搜索结果列表
                    List<string> urlList2 = search.Start(keyWord, startDate, endDate, "7");
                    //避免重复加载
                    var delete2 = DB.Context.Delete<Bid>(o => o.StatDate >= startDate && o.StatDate <= endDate && o.KeyWord == keyWord);
                    foreach (string url in urlList2)
                    {
                        //根据链接获取表格并入库
                        results.GetBidTable(keyWord, item.Name, url);
                    }
                }
                if (!item.Manual_Run)
                {
                    var key = DB.Context.Exists<ETLStatus>(o => o.Key == item.Name);
                    if (key)
                    {
                        //更新跑数状态，补数不更新
                        var updateStatus = DB.Context.Update<ETLStatus>(ETLStatus._.Value, endDate.ToString("yyyy-MM-dd"), o => o.Key == item.Name);
                    }
                    else
                    {
                        //插入新业务
                        var insertStatus = DB.Context.Insert<ETLStatus>(new ETLStatus { Key = item.Name, Value = endDate.ToString("yyyy-MM-dd") });
                    }
                    
                }
            }
            Console.WriteLine("---抓取成功---");
        }
    }
}
