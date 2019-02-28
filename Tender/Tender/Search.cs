using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tender.Model;

namespace Tender
{
    public class Search
    {
        TenderHelper tenderHelper = new TenderHelper();
        static Results results = new Results();
        /// <summary>
        /// 开始入口
        /// </summary>
        public void SartAll()
        {
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
                    List<string> urlList = Start(keyWord, startDate, endDate, "1");
                    //避免重复加载
                    var delete = DB.Context.Delete<Tender>(o => o.StatDate >= startDate && o.StatDate <= endDate && o.KeyWord == keyWord);
                    foreach (string url in urlList)
                    {
                        //根据链接获取表格并入库
                        results.GetTenderTable(keyWord, item.Name, url);
                    }

                    Console.WriteLine("---开始抓取---中-标-数-据---关键词:{0}", keyWord);
                    //获取中标搜索结果列表
                    List<string> urlList2 = Start(keyWord, startDate, endDate, "7");
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
        /// <summary>
        /// 获取关键词搜索结果所有分页下的链接
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public List<string> Start(string keyWord,DateTime startDate,DateTime endDate,string bidType)
        {
            List<string> searchList = new List<string>();
            int pageCount = 1;
            string startTime = startDate.Year + "%3A" + startDate.Month + "%3A" + startDate.Day;
            string endTime = endDate.Year + "%3A" + endDate.Month + "%3A" + endDate.Day;
            string url = "http://search.ccgp.gov.cn/bxsearch?searchtype=1&page_index="+pageCount+"&bidSort=0&buyerName=&projectId=&pinMu=0&bidType="+bidType+"&dbselect=bidx&kw="+keyWord+"&start_time="+ startTime +"&end_time="+ endTime +"&timeType=6&displayZone=&zoneId=&pppStatus=0&agentName=";
            string str = tenderHelper.GetHtml(url);
            Regex r4 = new Regex("(?i)共找到[\\s\\S]*<span\\sstyle=\"color:#c00000\">([\\d]*)<\\/span>");

            Match m = r4.Match(str);
            int Count = int.Parse(m.Groups[1].Value);
            if (Count % 20 > 0)
            {
                pageCount = Count / 20 + 1;
            }
            else
            {
                pageCount = Count / 20;
            }
            for (int i = 1; i <= pageCount; i++)
            {
                url = "http://search.ccgp.gov.cn/bxsearch?searchtype=1&page_index=" + i + "&bidSort=0&buyerName=&projectId=&pinMu=0&bidType=" + bidType + "&dbselect=bidx&kw=" + keyWord + "&start_time=" + startTime + "&end_time=" + endTime + "&timeType=6&displayZone=&zoneId=&pppStatus=0&agentName=";
                searchList.AddRange(SearchTender(url));
            }
            return searchList;
        }
        /// <summary>
        /// 获取关键词搜索结果某页下的链接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List<string> SearchTender(string url)
        {
            //string url = "http://search.ccgp.gov.cn/bxsearch?searchtype=1&page_index=1&bidSort=0&buyerName=&projectId=&pinMu=0&bidType=1&dbselect=bidx&kw=消防&start_time=2018%3A10%3A01&end_time=2018%3A10%3A10&timeType=6&displayZone=&zoneId=&pppStatus=0&agentName=";
            string str = tenderHelper.GetHtml(url);
            //"<ul class='lists3'>ss</ul><ul>\r\n<li>\r\n<a href=abc.htm>abc</a>\r\n</li>\r\n</ul>\r\n<UL class=\"lists3\">\r\n<li><a href=\"/dcac.htm\">dcac</a></li>\r\n\r\n<li><a href=\"bbac.htm\">dcac</a></li>\r\n\r\n</ul>";
            //Regex r1 = new Regex("(?i)<ul[^>]+class[=\"\'\\s]+lists3[\"\']?[^>]*>(?:(?!<\\/ul>)[\\s\\S])+<\\/ul>");
            //Regex r2 = new Regex("(?i)<li[^>]*>(?:(?!<\\/li>)[\\s\\S])+<\\/li>");
            //Regex r3 = new Regex("(?i)<a[^>]+href[=\"\'\\s]+([^\"\'>]*)[\"\']?[^>]*>");
            Regex r1 = new Regex("(?i)<ul[^>]+class[=\"\'\\s]+vT-srch-result-list-bid[\"\']?[^>]*>(?:(?!<\\/ul>)[\\s\\S])+<\\/ul>");
            Regex r2 = new Regex("(?i)<li[^>]*>(?:(?!<\\/li>)[\\s\\S])+<\\/li>");
            Regex r3 = new Regex("(?i)<a[^>]+href[=\"\'\\s]+([^\"\'>]*)[\"\']?(?:\\sstyle=\"line-height:18px\"\\starget=\"_blank\")[^>]*>");
            
            Regex[] rs = { r1, r2, r3 };
            tenderHelper.results.Clear();
            tenderHelper.GetSearchRegex(str, rs, 0);
            return tenderHelper.results;
        }

        /// <summary>
        /// 使用 HtmlAgilityPack
        /// </summary>
        public void Test()
        {
            HtmlDocument doc = new HtmlDocument();
            string html = tenderHelper.GetHtml("http://www.ccgp.gov.cn/cggg/dfgg/zbgg/201810/t20181012_10879565.htm");
            //string html = tenderHelper.GetHtml("http://search.ccgp.gov.cn/bxsearch?searchtype=1&page_index=1&bidSort=0&buyerName=&projectId=&pinMu=0&bidType=1&dbselect=bidx&kw=vr&start_time=2018%3A12%3A28&end_time=2019%3A01%3A28&timeType=3&displayZone=&zoneId=&pppStatus=0&agentName=");
            doc.LoadHtml(html);
            #region 搜索结果
            //HtmlNodeCollection hrefs = doc.DocumentNode.SelectNodes("//ul[@class='vT-srch-result-list-bid']");
            //var lis = hrefs[0].ChildNodes.Where(s => s.Name == "li");
            //foreach (var tr in lis)
            //{
            //    var a = tr.ChildNodes.Where(s => s.Name == "a");
            //    if (a.Count()>0)
            //    {
            //        var a_href = a.First().Attributes["href"].Value;
            //        var a_text = a.First().InnerText;
            //    }
            //    var span = tr.ChildNodes.Where(s => s.Name == "span");
            //    if (span.Count() > 0)
            //    {
            //        var span_time = span.First().InnerText.Substring(0, 19);
            //        var span_area = span.First().ChildNodes.Where(s => s.Name == "a").First().InnerText;
            //    }
            //}
            #endregion
            #region table
            HtmlNodeCollection hrefs = doc.DocumentNode.SelectNodes("//table");
            string name = "";
            foreach (var tr in hrefs[0].ChildNodes)
            {
                foreach (var td in tr.ChildNodes)
                {
                    name = td.InnerText;
                }
            }
            #endregion
        }
    }
}
