using Dos.ORM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tender.Model;
using Tender.Tool;

namespace Tender
{
    public class Program
    {
       
        //static TenderHelper t = new TenderHelper();
        static void Main(string[] args)
        {
            //招投标爬虫
            //Search search = new Search();
            //search.Test();

            //hive数据导入sql
            //HiveImport hiveImport = new HiveImport();
            //hiveImport.Import_tenant_extend_data();

            //正则表达式解析
            //RegexFiles regexFiles = new RegexFiles();
            //regexFiles.Test();

            //H5log解析
            H5Import h5Import = new H5Import();
            h5Import.ImportSpend();
        }
    }
}
