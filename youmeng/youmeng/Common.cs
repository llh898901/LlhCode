using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UMeng
{
    public class Common
    {
        public static string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["UMeng"].ToString();
        public string HmacSha1(string secret, string strOrgData)
        {
            var encoding = new System.Text.UTF8Encoding();
            HMACSHA1 hmacsha1 = new HMACSHA1(encoding.GetBytes(secret));
            var hashBytes = hmacsha1.ComputeHash(encoding.GetBytes(strOrgData));
            StringBuilder EnText = new StringBuilder();
            foreach (byte Byte in hashBytes)
            {
                EnText.AppendFormat("{0:x2}", Byte);
            }
            return EnText.ToString();
            //return Convert.ToBase64String(hashBytes);
        }
        public string GetHttpResponse(string url, int Timeout)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.UserAgent = null;
            request.Timeout = Timeout;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        /// <summary>
        /// 用MD5加密字符串，可选择生成16位或者32位的加密字符串
        /// </summary>
        /// <param name="password">待加密的字符串</param>
        /// <param name="bit">位数，一般取值16 或 32</param>
        /// <returns>返回的加密后的字符串</returns>
        public string MD5Encrypt(string password, int bit)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes;
            hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(password));
            StringBuilder tmp = new StringBuilder();
            foreach (byte i in hashedDataBytes)
            {
                tmp.Append(i.ToString("x2"));
            }
            if (bit == 16)
                return tmp.ToString().Substring(8, 16);
            else
            if (bit == 32) return tmp.ToString();//默认情况
            else return string.Empty;
        }

        public int GetTimestamp(DateTime d)
        {
            TimeSpan ts = d.ToUniversalTime() - new DateTime(1970, 1, 1);
            return (int)ts.TotalSeconds;     //精确到秒
        }
        public DataTable GetTableSchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
            new DataColumn("Id",typeof(Guid)),
            new DataColumn("Name",typeof(string)),
            new DataColumn("Price",typeof(decimal))});
            return dt;
        }
        public DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }
        public void CreateTable()
        {
            #region 建表语句
            string sql = @"if OBJECT_ID('dbo.ETLStatus') is null
                        begin
	                        CREATE TABLE [dbo].[ETLStatus](
		                        [AutoID] [bigint] IDENTITY(1,1) NOT NULL,
		                        [Key] [varchar](255) NULL,
		                        [Value] [varchar](255) NULL,
	                         CONSTRAINT [pk_ETLStatus] PRIMARY KEY CLUSTERED 
	                        (
		                        [AutoID] ASC
	                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	                        ) ON [PRIMARY]
                            insert into [dbo].[ETLStatus]([Key],[Value])
                            values('UMeng_Activation','2018-09-04');
                        end
  
                        if OBJECT_ID('dbo.UMeng_Activation') is null
                        begin  
	                        CREATE TABLE [dbo].[UMeng_Activation](
	                        [AutoID] [bigint] IDENTITY(1,1) NOT NULL,
	                        [Mid] [varchar](255) NULL,
	                        [Active_Id] [varchar](255) NULL,
	                        [Type] [varchar](255) NULL,
	                        [Active_Date] [varchar](255) NULL,
	                        [Click_Date] [varchar](255) NULL,
	                        [Rp_Name] [varchar](255) NULL,
	                        [M_Name] [varchar](255) NULL,
	                        [Chan_Name] [varchar](255) NULL,
                         CONSTRAINT [pk_UMeng_Activation] PRIMARY KEY CLUSTERED 
                        (
	                        [AutoID] ASC
                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                        ) ON [PRIMARY]
                        end";
            #endregion
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                connection.Open();
                var data = SqlMapper.Execute(connection, sql);
            }
        }
    }
}
