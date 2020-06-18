using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tender.Tool
{
    public  class RegexFiles
    {
        public void Test()
        {
            string log = "Checkout card1 userid:7694917 user_name:lovemoyu idRef:1902234147 type:780001 price:1380 client:2 -- 00:29:29-19/02/23";
            string result = "Checkout card1 userid:? user_name:? idRef:? type:? price:? client:? -- ";
            string[] results = result.Split('?');
            string name = "userid;username;udRef;type;price;client;statdate";
            string[] names = name.Split(';');

            string pattern = "";
            //string pattern = @".*userid:(?<userid>\d+) user_name:(?<username>.*) idRef:(?<udRef>\d+) type:(?<type>\d+) price:(?<price>\d+) client:(?<client>\d+) -- (?<statdate>.*)";
            for (int i = 0; i < results.Length; i++)
            {
                pattern += results[i] + "(?<" + names[i] + ">.*)";
            } 
            foreach (Match match in Regex.Matches(log, pattern))
            {
                foreach (var item in names)
                {
                    Console.Write(item + ":" + match.Groups[item].Value);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
                
            Console.ReadKey();
        }

    }
}
