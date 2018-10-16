using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tender
{
    public class Search
    {
        TenderHelper tenderHelper = new TenderHelper();
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
    }
}
