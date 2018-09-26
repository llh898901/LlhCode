using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToMysql
{
    public class Program
    {
         static void Main(string[] args)
        {
            NameValueCollection sall = ConfigurationManager.AppSettings;
            string path = sall.Get("path");//"E:\\2018\\product_mysql_ppt_data_colletc\\test";
            DirectoryInfo folder = new DirectoryInfo(path);
            FileInfo[] files = folder.GetFiles("*.sql");
            WriteBat(files);
        }
        static void WriteBat(FileInfo[] files)
        {
            NameValueCollection sall = ConfigurationManager.AppSettings;
            string mysqlconnect = sall.Get("mysqlconnect");//"mysql -uroot -plh,348144 -hlocalhost -P3306 --default-character-set=utf8 test<";//mysql链接串
            string mysqlBin = sall.Get("mysqlBin");//"cd D:\\Program Files (x86)\\MySQL\\MySQL Server 5.0\\bin\\";//mysql 安装路径
            string d = sall.Get("d");//"d:";
            string batPath = sall.Get("batPath");//"E:\\2018\\toMysql\\";//bat脚本存放路径
            int batCnt = int.Parse(sall.Get("batCnt"));//6;//bat脚本并发数量

            int fileCnt = files.Count();
            int count = fileCnt / batCnt + 1;
            int j = 1;//bat文件序号
            int i = 0;//第i+1个sql文件名
            FileStream fs = new FileStream(batPath + j + ".bat", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            foreach (FileInfo file in files)
            {
                if (i == 0){
                    //开始写入
                    sw.WriteLine(mysqlBin);
                    sw.WriteLine(d);
                    sw.WriteLine(mysqlconnect + file.FullName);
                    i++;
                }
                else if (i < count)
                {
                    sw.WriteLine(mysqlconnect + file.FullName);
                    i++;
                }
                else
                {
                    i = 0;
                    j++;
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();
                    fs = new FileStream(batPath + j + ".bat", FileMode.Create);
                    sw = new StreamWriter(fs);
                    sw.WriteLine(mysqlBin);
                    sw.WriteLine(d);
                    sw.WriteLine(mysqlconnect + file.FullName);
                    i++;
                }
            }
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }
    }
}
