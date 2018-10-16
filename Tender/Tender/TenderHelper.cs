using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tender
{
    public class TenderHelper
    {
        public List<string> results = new List<string>();
        /// <summary>
        /// 抓取页面html
        /// </summary>
        /// <returns></returns>
        public string GetHtml(string url)
        {
            try
            {
                WebClient MyWebClient = new WebClient();
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据

                //string search = "http://search.ccgp.gov.cn/bxsearch?searchtype=1&page_index=1&bidSort=0&buyerName=&projectId=&pinMu=0&bidType=1&dbselect=bidx&kw=消防&start_time=2018%3A10%3A01&end_time=2018%3A10%3A10&timeType=6&displayZone=&zoneId=&pppStatus=0&agentName=";
                //string result = "http://www.ccgp.gov.cn/cggg/dfgg/zbgg/201810/t20181012_10879565.htm";
                //Byte[] pageData = MyWebClient.DownloadData(search); //下载招标搜索结果
                Byte[] pageData = MyWebClient.DownloadData(url); //下载招标公告

                //string pageHtml = Encoding.Default.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句            
                string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句
                //using (StreamWriter sw = new StreamWriter("E:\\2018\\Tender\\result2.html"))//将获取的内容写入文本
                //{
                //    sw.Write(pageHtml);
                //}
                return pageHtml;
                //Console.ReadLine(); //让控制台暂停,否则一闪而过了             
            }
            catch (WebException webEx)
            {
                Console.WriteLine(webEx.Message.ToString());
                return "false";
            }
        }
        /// <summary>
        /// 获取页面中的链接 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="rs"></param>
        /// <param name="index"></param>
        public  void GetSearchRegex(string str, Regex[] rs, int index)
        {
            string vv = "";
            if (index > rs.Length - 1)
            {
                return ;
            }
            Regex r = rs[index];
            Match m = r.Match(str);
            while (m.Success)
            {
                if (index == rs.Length - 1)
                {
                    Console.WriteLine(m.Groups[1]);
                }
                for (int i = 0; i < m.Groups.Count; i++)
                {
                    Group group = m.Groups[i];
                    vv = group.Value;
                    GetSearchRegex(vv, rs, index + 1);
                }
                m = m.NextMatch();
            }
            if (index == 2)
            {
                results.Add(vv);
            }
        }
    }
}
