using Dos.ORM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tender
{
    /// <summary>
    /// DOS.ORM 批量导入
    /// </summary>
    public static class DosHelper
    {
        public static void SqlBuckCopy<T>(DbSession Db, List<T> list, string tableName, string[] propertyName = null, Action<Exception, List<T>> exHandle = null) where T : Entity
        {

            try
            {
                if (list == null || list.Count == 0)
                    return;
                if (propertyName==null)
                {
                    propertyName = typeof(T).GetProperties().ToList().Select(o=>o.Name).ToArray();
                }
                if (string.IsNullOrEmpty(tableName))
                {
                    tableName = list[0].GetTableName();
                }
                DataTable dts = ToDataTable(list, propertyName);
                using (var connection = (SqlConnection)Db.Db.GetConnection())
                {
                    connection.Open();
                    using (SqlBulkCopy SqlBulk = new SqlBulkCopy(connection, SqlBulkCopyOptions.UseInternalTransaction, null)
                    {
                        DestinationTableName = tableName,
                        BatchSize = dts.Rows.Count,
                    })
                    {
                        for (int i = 0; i < dts.Columns.Count; i++)
                        {
                            SqlBulk.ColumnMappings.Add(dts.Columns[i].ColumnName, dts.Columns[i].ColumnName);
                        }
                        SqlBulk.WriteToServer(dts);
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                exHandle(ex, list);
            }

        }
        public static void SqlBuckCopy(this DbSession Db, DataTable dts, string tableName)
        {
            if (dts != null)
            {
                using (SqlConnection connection = (SqlConnection)Db.Db.GetConnection())
                {

                    using (SqlBulkCopy SqlBulk = new SqlBulkCopy(connection, SqlBulkCopyOptions.UseInternalTransaction, null)
                    {
                        DestinationTableName = tableName,
                        BatchSize = dts.Rows.Count,

                    })
                    {
                        for (int i = 0; i < dts.Columns.Count; i++)
                        {
                            SqlBulk.ColumnMappings.Add(dts.Columns[i].ColumnName, dts.Columns[i].ColumnName);
                        }
                        SqlBulk.WriteToServer(dts);
                    }
                    connection.Close();
                }
            }
        }
        private static DataTable ToDataTable<TEntity>(IList<TEntity> list, params string[] propertyName) where TEntity : class
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[0], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
    }
}
