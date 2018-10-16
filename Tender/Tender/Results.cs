using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tender
{
    public class Results
    {
        TenderHelper tenderHelper = new TenderHelper();
        /// <summary>
        /// 获取招标表格信息
        /// </summary>
        /// <param name="url"></param>
        public void GetTenderTable(string keyWprd,string name ,string url)
        {
            //url = "http://www.ccgp.gov.cn/cggg/dfgg/gkzb/201810/t20181010_10858059.htm";
            string result = tenderHelper.GetHtml(url);
            Regex r1 = new Regex("(?i)<table\\swidth=\'600\'\\sborder=\'0\'\\scellspacing=\'1\'\\sbgcolor=\'#bfbfbf\'\\sstyle=\'text-align:left;\'>(?:(?!<\\/table>)[\\s\\S])+<\\/table>");
            Match tableStr = r1.Match(result);
            if (tableStr.Success)
            {
                Regex r2 = new Regex("(?i)<td[^>]+>(?:<p>)*([^<]*)(?:<\\/p>)*<\\/td>");
                //Match td = r2.Match(tableStr.Groups[0].Value);
                MatchCollection td = r2.Matches(tableStr.Groups[0].Value);
                Tender tender = new Tender();
                #region
                tender.StatDate = Convert.ToDateTime(td[9].Groups[1].Value.Substring(0, 11));
                tender.BusinessName = name;
                tender.KeyWord = keyWprd;
                tender.Area = td[7].Groups[1].Value;
                tender.BulletinTime = td[9].Groups[1].Value;
                tender.Name = td[1].Groups[1].Value; 
                tender.Catalogue = td[3].Groups[1].Value;
                tender.Budget = td[21].Groups[1].Value;
                tender.BidTime = td[17].Groups[1].Value;
                tender.BidAddress = td[19].Groups[1].Value; 
                tender.FileAddress = td[15].Groups[1].Value;
                tender.FileTime = td[11].Groups[1].Value.Replace("&nbsp;", " ");
                tender.PurchaseUnit = td[27].Groups[1].Value;
                tender.ContactPeople = td[23].Groups[1].Value;
                tender.ContactPhone = td[25].Groups[1].Value;
                tender.AgentName = td[33].Groups[1].Value;
                tender.AgentContact = td[37].Groups[1].Value;
                #endregion
                if (DB.Context.Exists<Tender>(o => o.Name == tender.Name && o.StatDate < tender.StatDate))
                {
                    DB.Context.Delete<Tender>(o => o.Name == tender.Name);
                    DB.Context.Insert<Tender>(tender);
                }
                else if(!DB.Context.Exists<Tender>(o => o.Name == tender.Name))
                {
                    DB.Context.Insert<Tender>(tender);
                }
            }
        }
        /// <summary>
        /// 获取中标表格信息
        /// </summary>
        /// <param name="keyWprd"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        public void GetBidTable(string keyWprd, string name, string url)
        {
            //url = "http://www.ccgp.gov.cn/cggg/dfgg/zbgg/201810/t20181012_10879565.htm";
            string result = tenderHelper.GetHtml(url);
            Regex r1 = new Regex("(?i)<table\\swidth=\'600\'\\sborder=\'0\'\\scellspacing=\'1\'\\sbgcolor=\'#bfbfbf\'\\sstyle=\'text-align:left;\'>(?:(?!<\\/table>)[\\s\\S])+<\\/table>");
            Match tableStr = r1.Match(result);
            if (tableStr.Success)
            {
                Regex r2 = new Regex("(?i)<td[^>]+>(?:<p>)*([^<]*)(?:<\\/p>)*<\\/td>");
                //Match td = r2.Match(tableStr.Groups[0].Value);
                MatchCollection td = r2.Matches(tableStr.Groups[0].Value);
                Bid bid = new Bid();
                #region
                bid.StatDate = Convert.ToDateTime(td[9].Groups[1].Value.Substring(0, 11));
                bid.BusinessName = name;
                bid.KeyWord = keyWprd;
                bid.Area = td[7].Groups[1].Value;
                bid.BulletinTime = td[9].Groups[1].Value;
                bid.Name = td[1].Groups[1].Value;
                bid.Catalogue = td[3].Groups[1].Value;
                bid.Money = td[17].Groups[1].Value;
                bid.PurchaseUnit = td[23].Groups[1].Value;
                bid.ContactPeople = td[19].Groups[1].Value;
                bid.ContactPhone = td[21].Groups[1].Value;
                #endregion
                if (DB.Context.Exists<Bid>(o => o.Name == bid.Name && o.StatDate < bid.StatDate))
                {
                    DB.Context.Delete<Bid>(o => o.Name == bid.Name);
                    DB.Context.Insert<Bid>(bid);
                }
                else if (!DB.Context.Exists<Bid>(o => o.Name == bid.Name))
                {
                    DB.Context.Insert<Bid>(bid);
                }
            }
        }


    }
}
