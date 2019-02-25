using CM.Framework.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Collections.Concurrent;
using CM.Framework;

namespace Dos.ORM
{
    /// <summary>
    /// DBSession拓展
    /// </summary>
    public static class DBSessionExtensions
    {
        public static PageList<T> PageList<T>(this FromSection<T> table, int PageIndex, int PageSize, Where<T> where, OrderByClip order)
            where T : Entity
        {
            PageList<T> page = new PageList<T>();
            var Count = table.Where(where).Count();
            var List = table.Where(where).Page(PageIndex, PageSize).OrderBy(order).ToList();
            page.Cnt = Count;
            page.List = List;
            return page;
        }
        public static FromSection<T> Order<T>(this FromSection<T> from, QueryOrder orderField, string defaultFiled = "AddTime")
            where T : Entity
        {
            string field = string.IsNullOrEmpty(orderField.Field) ? defaultFiled : orderField.Field;
            from.OrderByClip = new OrderByClip(field, orderField.IsDesc ? OrderByOperater.DESC : OrderByOperater.ASC);
            return from;
        }

        //public static DbSession InitDatabase<T>(this DbSession Db, List<T> Entitys, string[] FileGroups = null, string _BuilderFiled = "[{0}]") where T : Entity
        //{
        //    foreach (var item in Entitys)
        //    {
        //        if (!Db.TableExists(item.GetTableName()))
        //        {
        //            var sql = Db.CreateTable<T>(FileGroups, _BuilderFiled);
        //        }

        //    }
        //    return Db;
        //}

        public static bool CreateTable<T>(this DbSession Db, string schm, string tableName, string Parttion, string[] FileGroups, string _BuilderFiled = "[{0}]") where T : Entity, new()
        {
            var table = new T();
            var builderTail = _BuilderFiled;
            var _type = table.GetType();
            var AllPropertis = _type.GetProperties().Where(o => o.GetCustomAttributes(typeof(FieldAttribute), false).Length > 0);
            #region TableInfo

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = table.GetTableName();
            }
            if (string.IsNullOrEmpty(schm))
            {
                schm = table.GetUserName() == null ? "dbo" : table.GetUserName();
            }

            #endregion

            if (string.IsNullOrEmpty(Parttion))
            {
                #region 文件组Info
                string file = "";

                if (FileGroups == null)
                    FileGroups = new string[] { "PRIMARY" };
                if (FileGroups.Length == 0)
                    FileGroups[0] = "PRIMARY";
                if (FileGroups.Length > 1)
                    file = FileGroups[new Random(Guid.NewGuid().GetHashCode()).Next(0, FileGroups.Length - 1)];
                else
                    file = FileGroups[0];
                Parttion = string.Format("[{0}]", file);
                #endregion
            }

            var IdentityFiled = "";
            var PrimaryField = "";
            var _pkey = table.GetPrimaryKeyFields();
            if (_pkey != null && _pkey.Length == 1)
            {
                PrimaryField = table.GetPrimaryKeyFields()[0].Name;
            }
            if (!string.IsNullOrEmpty(table.GetIdentityField().Name))
            {
                IdentityFiled = table.GetIdentityField().Name;
            }
            var Fields = table.GetFields();
            string sql = "";
            //sql += (" SET ANSI_NULLS ON");
            //sql += (" SET QUOTED_IDENTIFIER ON");
            //sql += (" SET ANSI_PADDING ON");
            sql += (string.Format(" CREATE TABLE {0}.{1}(", string.Format(builderTail, schm), string.Format(builderTail, tableName)));
            string PrimaryKey = "";
            foreach (var o in AllPropertis)
            {
                var attrs = o.GetCustomAttributes(typeof(Attribute), true);
                var identity = false;
                bool IsPrimary = false;
                string fieldName = "";
                int Count = 255;
                string dateType = "";
                bool NotNull = false;

                foreach (var item in attrs)
                {
                    if (item is ColumnAttribute)
                    {
                        var cAttr = (ColumnAttribute)item;
                        fieldName = string.IsNullOrEmpty(cAttr.Name) ? o.Name : cAttr.Name;
                        if (!string.IsNullOrEmpty(cAttr.TypeName))
                        {
                            dateType = cAttr.TypeName;
                        }
                    }
                    if (item is FieldAttribute)
                    {
                        var cAttr = (FieldAttribute)item;
                        fieldName = string.IsNullOrEmpty(cAttr.Field) ? o.Name : cAttr.Field;
                    }
                    if (item is KeyAttribute)
                    {
                        IsPrimary = true;
                    }
                    if (item is DatabaseGeneratedAttribute)
                    {
                        if (((DatabaseGeneratedAttribute)item).DatabaseGeneratedOption != DatabaseGeneratedOption.None)
                            identity = true;
                    }
                    if (item is MaxLengthAttribute)
                    {
                        Count = ((MaxLengthAttribute)item).Length;
                    }
                }
                if (string.IsNullOrEmpty(fieldName))
                    fieldName = o.Name;

                if (IdentityFiled.Equals(fieldName))
                {
                    identity = true;
                    NotNull = true;
                }
                if (fieldName == PrimaryField)
                {
                    IsPrimary = true;
                }
                if (fieldName == IdentityFiled)
                {
                    identity = true;
                }
                if (string.IsNullOrEmpty(dateType))
                {
                    if (o.PropertyType.Name == "Nullable`1")
                    {
                        NotNull = false;
                        if (o.PropertyType.GenericTypeArguments.Length == 1)
                        {
                            dateType = o.PropertyType.GenericTypeArguments[0].Name;
                        }
                    }
                    else
                        dateType = o.PropertyType.Name;
                    dateType = GetSqlTypeFormType(dateType, Count, builderTail);
                }
                if (IsPrimary)
                {
                    PrimaryKey = fieldName;
                    NotNull = true;
                }

                sql += (string.Format("    {0} {1} {2} {3} {4},", string.Format(builderTail, fieldName), dateType, identity ? "IDENTITY(1,1) " : "", NotNull || identity ? "NOT NULL" : "NULL", IsPrimary ? "PRIMARY KEY" : ""));
            }
            sql = sql.TrimEnd(',');
            sql += (" )");
            //if (!string.IsNullOrEmpty(PrimaryKey))
            //{
            //    //sql += (string.Format(" CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ", tableName));
            //    //sql += (" (");
            //    //sql += (string.Format(" 	{0} ASC", string.Format(builderTail, PrimaryKey)));
            //    //sql += (string.Format(" )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON {0}", Parttion));

            //}
            if (!string.IsNullOrEmpty(Parttion))
            {
                sql += (string.Format(" ON {0}", Parttion));

            }
            //sql += (string.Format(" ) ON {0}", Parttion));
            //sql += (" ");
            //sql += ("");
            //sql += (" SET ANSI_PADDING OFF");
            Db.FromSql(sql).ToList<int>();
            return true;
        }
        public static bool CreateTable<T>(this SqlConnection conn, string schm, string tableName, string Parttion, string[] FileGroups, string _BuilderFiled = "[{0}]") where T : Entity, new()
        {
            var table = new T();
            var builderTail = _BuilderFiled;
            var _type = table.GetType();
            var AllPropertis = _type.GetProperties().Where(o => o.GetCustomAttributes(typeof(FieldAttribute), false).Length > 0);
            #region TableInfo

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = table.GetTableName();
            }
            if (string.IsNullOrEmpty(schm))
            {
                schm = table.GetUserName() == null ? "dbo" : table.GetUserName();
            }

            #endregion

            if (string.IsNullOrEmpty(Parttion))
            {
                #region 文件组Info
                string file = "";

                if (FileGroups == null)
                    FileGroups = new string[] { "PRIMARY" };
                if (FileGroups.Length == 0)
                    FileGroups[0] = "PRIMARY";
                if (FileGroups.Length > 1)
                    file = FileGroups[new Random(Guid.NewGuid().GetHashCode()).Next(0, FileGroups.Length - 1)];
                else
                    file = FileGroups[0];
                Parttion = string.Format("[{0}]", file);
                #endregion
            }

            var IdentityFiled = "";
            var PrimaryField = "";
            var _pkey = table.GetPrimaryKeyFields();
            if (_pkey != null && _pkey.Length == 1)
            {
                PrimaryField = table.GetPrimaryKeyFields()[0].Name;
            }
            if (!string.IsNullOrEmpty(table.GetIdentityField().Name))
            {
                IdentityFiled = table.GetIdentityField().Name;
            }
            var Fields = table.GetFields();
            string sql = "";
            //sql += (" SET ANSI_NULLS ON");
            //sql += (" SET QUOTED_IDENTIFIER ON");
            //sql += (" SET ANSI_PADDING ON");
            sql += (string.Format(" CREATE TABLE {0}.{1}(", string.Format(builderTail, schm), string.Format(builderTail, tableName)));
            string PrimaryKey = "";
            foreach (var o in AllPropertis)
            {
                var attrs = o.GetCustomAttributes(typeof(Attribute), true);
                var identity = false;
                bool IsPrimary = false;
                string fieldName = "";
                int Count = 255;
                string dateType = "";
                bool NotNull = false;

                foreach (var item in attrs)
                {
                    if (item is ColumnAttribute)
                    {
                        var cAttr = (ColumnAttribute)item;
                        fieldName = string.IsNullOrEmpty(cAttr.Name) ? o.Name : cAttr.Name;
                        if (!string.IsNullOrEmpty(cAttr.TypeName))
                        {
                            dateType = cAttr.TypeName;
                        }
                    }
                    if (item is FieldAttribute)
                    {
                        var cAttr = (FieldAttribute)item;
                        fieldName = string.IsNullOrEmpty(cAttr.Field) ? o.Name : cAttr.Field;
                    }
                    if (item is KeyAttribute)
                    {
                        IsPrimary = true;
                    }
                    if (item is DatabaseGeneratedAttribute)
                    {
                        if (((DatabaseGeneratedAttribute)item).DatabaseGeneratedOption != DatabaseGeneratedOption.None)
                            identity = true;
                    }
                    if (item is MaxLengthAttribute)
                    {
                        Count = ((MaxLengthAttribute)item).Length;
                    }
                }
                if (string.IsNullOrEmpty(fieldName))
                    fieldName = o.Name;

                if (IdentityFiled.Equals(fieldName))
                {
                    identity = true;
                    NotNull = true;
                }
                if (fieldName == PrimaryField)
                {
                    IsPrimary = true;
                }
                if (fieldName == IdentityFiled)
                {
                    identity = true;
                }
                if (string.IsNullOrEmpty(dateType))
                {
                    if (o.PropertyType.Name == "Nullable`1")
                    {
                        NotNull = false;
                        if (o.PropertyType.GenericTypeArguments.Length == 1)
                        {
                            dateType = o.PropertyType.GenericTypeArguments[0].Name;
                        }
                    }
                    else
                        dateType = o.PropertyType.Name;
                    dateType = GetSqlTypeFormType(dateType, Count, builderTail);
                }
                if (IsPrimary)
                {
                    PrimaryKey = fieldName;
                    NotNull = true;
                }

                sql += (string.Format("    {0} {1} {2} {3} {4},", string.Format(builderTail, fieldName), dateType, identity ? "IDENTITY(1,1) " : "", NotNull || identity ? "NOT NULL" : "NULL", IsPrimary ? "PRIMARY KEY" : ""));
            }
            sql = sql.TrimEnd(',');
            sql += (" )");
            //if (!string.IsNullOrEmpty(PrimaryKey))
            //{
            //    //sql += (string.Format(" CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ", tableName));
            //    //sql += (" (");
            //    //sql += (string.Format(" 	{0} ASC", string.Format(builderTail, PrimaryKey)));
            //    //sql += (string.Format(" )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON {0}", Parttion));

            //}
            if (!string.IsNullOrEmpty(Parttion))
            {
                sql += (string.Format(" ON {0}", Parttion));

            }

            //sql += (string.Format(" ) ON {0}", Parttion));
            //sql += (" ");
            //sql += ("");
            //sql += (" SET ANSI_PADDING OFF");
            lock (objLock)
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                int i = 0;
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    i = cmd.ExecuteNonQuery();
                }
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                return i > 0;

            }
        }
        public static bool CreateTable<T>(this DbSession Db, string[] FileGroups = null, string __BuilderFiled = "[{0}]") where T : Entity, new()
        {
            return Db.CreateTable<T>("", "", "", FileGroups, __BuilderFiled);
        }
        public static bool CreateTable<T>(this SqlConnection Db, string[] FileGroups = null, string __BuilderFiled = "[{0}]") where T : Entity, new()
        {
            return Db.CreateTable<T>("", "", "", FileGroups, __BuilderFiled);
        }
        public static bool CreateTable<T>(this DbSession Db, string schm, string tableName, string Parttion) where T : Entity, new()
        {
            return Db.CreateTable<T>(schm, tableName, Parttion, null);
        }
        public static bool CreateTable<T>(this SqlConnection Db, string schm, string tableName, string Parttion) where T : Entity, new()
        {
            return Db.CreateTable<T>(schm, tableName, Parttion, null);
        }


        private static string GetSqlTypeFormType(string fieldType, int length = 255, string builderTail = "[{0}]")
        {
            switch (fieldType)
            {
                case "Int64":
                    return string.Format(builderTail, "bigint");
                case "Single":
                    return string.Format(builderTail, "float");
                case "Decimal":
                    return string.Format(builderTail, "decimal") + "(18, 2)";
                case "Int32":
                    return string.Format(builderTail, "int");
                case "DateTime":
                    return string.Format(builderTail, "DateTime");
                case "String":
                    return string.Format(builderTail, "nvarchar") + "(" + (length == 99999 ? "max" : length.ToString()) + ")";
            }
            return "";
        }
        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Db"></param>
        /// <returns></returns>
        public static bool TableExists<T>(this DbSession Db) where T : Entity
        {
            return Db.FromSql(string.Format("select  case ISNULL(object_id(N'{0}',N'U'),0) when 0 then 0 else 1 end ", EntityCache.GetTableName<T>())).ToList<int>()[0] > 0 ? true : false;
        }
        public static bool TableExists(this DbSession Db, string TableName)
        {
            return Db.FromSql(string.Format("select  case ISNULL(object_id(N'{0}',N'U'),0) when 0 then 0 else 1 end ", TableName)).ToList<int>()[0] > 0 ? true : false;
        }
        public static bool TableExists(this SqlConnection conn, string TableName)
        {
            lock (objLock)
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("select  case ISNULL(object_id(N'{0}',N'U'),0) when 0 then 0 else 1 end ", TableName);
                    var res = cmd.ExecuteScalar();
                    if (res != null && (int)res == 1)
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                        return true;
                    }
                    return false;
                }
            }
        }
        //用于缓存DataTable结构
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
        public static void SqlBuckCopy<T>(this DbSession Db, List<T> list, string[] propertyName = null, Action<Exception, List<T>> exHandle = null) where T : Entity
        {
            SqlBuckCopy(Db, list, "", propertyName, exHandle);
        }
        public static void SqlBuckCopy<T>(this DbSession Db, List<T> list, string tableName, string[] propertyName = null, Action<Exception, List<T>> exHandle = null) where T : Entity
        {

            try
            {
                if (list == null || list.Count == 0)
                    return;
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
        public static object objLock = new object();
        public static void SqlBuckCopy<T>(this SqlConnection conn, List<T> list, string tableName, string[] propertyName = null, Action<Exception, List<T>> exHandle = null) where T : Entity
        {

            try
            {
                if (list == null || list.Count == 0)
                    return;
                if (string.IsNullOrEmpty(tableName))
                {
                    tableName = list[0].GetTableName();
                }
                DataTable dts = ToDataTable(list, propertyName);

                lock (objLock)
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (SqlBulkCopy SqlBulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.UseInternalTransaction, null)
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
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                exHandle(ex, list);
            }
            finally
            {
                lock (objLock)
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
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


        public static void SqlBuckCopyGroupByKeyOld<T>(this DbSession Db, List<T> list, Func<T, string> GroupBy, string sch = "", string parttion = "", string[] propertyName = null, Action<Exception, List<T>> exHandle = null) where T : Entity, new()
        {
            try
            {
                if (list == null || list.Count == 0)
                    return;
                var groupList = list.GroupBy(GroupBy);
                foreach (var item in groupList)
                {
                    var diffList = item.ToList();
                    string tableName = diffList[0].GetTableName() + "_" + item.Key;

                    //if (!Db.TableExists(tableName))
                    //{
                    //    Db.CreateTable<T>(sch, tableName, parttion);
                    //}
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
            }
            catch (Exception ex)
            {
                exHandle(ex, list);
            }
        }
        public static void SqlBuckCopyGroupByKey<T>(this DbSession Db, List<T> list, Func<T, string> GroupBy, string sch = "", string parttion = "", string[] propertyName = null, Action<Exception, List<T>> exHandle = null) where T : Entity, new()
        {
            try
            {

                if (list == null || list.Count == 0)
                    return;
                var groupList = list.GroupBy(GroupBy);
                foreach (var item in groupList)
                {
                    var diffList = item.ToList();
                    string tableName = diffList[0].GetTableName() + "_" + item.Key;

                    //if (!Db.TableExists(tableName))
                    //{
                    //    Db.CreateTable<T>(sch, tableName, parttion);
                    //}
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
            }
            catch (Exception ex)
            {
                exHandle(ex, list);
            }
        }
    }
}
