using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public static class CollectionHelper
    {
        //Convert a list of objects to a data table
        public static DataTable ConvertTo<T>(IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
            List<string> col = new List<string>();

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    try
                    {
                        if (!prop.Name.Contains("TimeStamp"))
                            row[prop.Name] = prop.GetValue(item);
                        else
                            col.Add(prop.Name);
                    }
                    catch(Exception e)
                    {
                        if (e.Message.Contains("DBNull"))
                            row[prop.Name] = DBNull.Value;
                        else
                            col.Add(prop.Name);
                    }
                }

                table.Rows.Add(row);
            }

            //Removing every columns which did not load data
            foreach(string s in col)
            {
                try
                {
                    table.Columns.Remove(s.ToString());
                }
                catch
                { }
            }

            return table;
        }

        //Convert a list of rows to another type of list
        public static IList<T> ConvertTo<T>(IList<DataRow> rows)
        {
            IList<T> list = null;

            if (rows != null)
            {
                list = new List<T>();

                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }

            return list;
        }

        //Converting a data table to a list of type T
        public static IList<T> ConvertTo<T>(DataTable table)
        {
            if (table == null)
            {
                return null;
            }

            List<DataRow> rows = new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            return ConvertTo<T>(rows);
        }

        //Creating an item of type t
        public static T CreateItem<T>(DataRow row)
        {
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                    try
                    {
                        object value = row[column.ColumnName];
                        prop.SetValue(obj, value, null);
                    }
                    catch
                    {
                        // You can log something here
                        throw;
                    }
                }
            }

            return obj;
        }

        //Creating a data table of type T
        public static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                if (!prop.Name.Contains("TimeStamp") && !prop.Name.Contains("tbl"))
                {
                        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(
                prop.PropertyType) ?? prop.PropertyType);

                }
            }

            return table;
        }

        //Joining two data tables and returning the joined table
        public static DataTable JoinDataTables(DataTable t1, DataTable t2, params Func<DataRow, DataRow, bool>[] joinOn)
        {
            DataTable result = new DataTable();

            //Adding columns from first table
            foreach (DataColumn col in t1.Columns)
            {
                if (result.Columns[col.ColumnName] == null)
                    result.Columns.Add(col.ColumnName, col.DataType);
            }

            //Adding columns from second table
            foreach (DataColumn col in t2.Columns)
            {
                if (result.Columns[col.ColumnName] == null)
                    result.Columns.Add(col.ColumnName, col.DataType);
            }

            //Joining the rows
            foreach (DataRow row1 in t1.Rows)
            {
                var joinRows = t2.AsEnumerable().Where(row2 =>
                {
                    foreach (var parameter in joinOn)
                    {
                        if (!parameter(row1, row2)) return false;
                    }
                    return true;
                });
                foreach (DataRow fromRow in joinRows)
                {
                    DataRow insertRow = result.NewRow();
                    foreach (DataColumn col1 in t1.Columns)
                    {
                        insertRow[col1.ColumnName] = row1[col1.ColumnName];
                    }
                    foreach (DataColumn col2 in t2.Columns)
                    {
                        insertRow[col2.ColumnName] = fromRow[col2.ColumnName];
                    }
                    result.Rows.Add(insertRow);
                }
            }
            return result;
        }

        /// <summary>
        /// Filter a data set based on a parameter
        /// </summary>
        /// <param name="models"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> models, string parameter)
        {

            var type = models.GetType().GetGenericArguments()[0];
            var properties = type.GetProperties();
            try
            {
                return models.Where(x => properties
                            .Any(p => p.GetValue(x) != null && p.GetValue(x).ToString().ToLower().Contains(parameter.ToLower())));
            }
            catch(Exception e)
            {
                return Enumerable.Empty<T>();
            }
        }
    }
}
