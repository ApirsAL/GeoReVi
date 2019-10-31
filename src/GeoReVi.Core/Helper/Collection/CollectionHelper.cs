using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Linq.Dynamic;


namespace GeoReVi
{
    /// <summary>
    /// Static class to handle collections
    /// </summary>
    public static class CollectionHelper
    {
        //Convert a list of objects to a data table
        public static DataTable ConvertTo<T>(IList<T> list)
        {
            DataTable table = CreateTable<T>();
            table.TableName = "MyTable";
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
                    catch (Exception e)
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
            foreach (string s in col)
            {
                try
                {
                    table.Columns.Remove(s.ToString());
                }
                catch
                {

                }
            }

            return table;
        }

        public static DataTable RemoveNonNumericColumns(this DataTable data)
        {
            for (int i = 0; i < data.Columns.Count; i++)
            {
                if (data.Columns[i].DataType != typeof(Int32)
                    && data.Columns[i].DataType != typeof(Int16)
                    && data.Columns[i].DataType != typeof(Int64)
                    && data.Columns[i].DataType != typeof(double)
                    && data.Columns[i].DataType != typeof(float)
                    && data.Columns[i].DataType != typeof(decimal))
                {
                    try
                    {
                        data.Columns.Remove(data.Columns[i]);
                        i = i - 1;

                        try
                        {
                            data.Columns[i].DataType = typeof(double);
                        }
                        catch
                        {
                            data.Columns.Remove(data.Columns[i]);
                        }
                    }
                    catch
                    {

                    }

                }
                else
                {
                    try
                    {
                        data.Columns[i].DataType = typeof(double);
                    }
                    catch
                    {
                        data.Columns.Remove(data.Columns[i]);
                        i = i - 1;
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Removes all nan, 0 and default value rows and columns from a numeric data table
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable RemoveNanRowsAndColumns(this DataTable data)
        {
            for (int i = 1; i < data.Rows.Count; i++)
            {
                bool isnan = false;

                for (int j = 1; j < data.Columns.Count; j++)
                {
                    try
                    {
                        double value = Convert.ToDouble(data.Rows[i][j]);

                        if (Double.IsNaN(value) || value == -9999 || value == 9999999 || value == 9999999 || value == 0)
                        {
                            isnan = true;
                        }
                        else
                        {
                            isnan = false;
                            break;
                        }
                    }
                    catch
                    {

                    }
                }

                if (isnan)
                {
                    data.Rows.RemoveAt(i);
                    i -= 1;
                }

            }

            int length = data.Columns.Count;

            for (int i = 1; i < length; i++)
            {
                bool allZero = false;

                try
                {
                    allZero = data.AsEnumerable().Where(x => Double.IsNaN(x.Field<double>(i)) || x.Field<double>(i) == -9999 || x.Field<double>(i) == 9999999 || x.Field<double>(i) == 999999 || x.Field<double>(i) == 0).Count() == data.Rows.Count;
                }
                catch
                {
                    try
                    {
                        allZero = data.AsEnumerable().Where(x => Double.IsNaN(x.Field<int>(i)) || x.Field<int>(i) == -9999 || x.Field<int>(i) == 9999999 || x.Field<int>(i) == 999999 || x.Field<int>(i) == 0).Count() == data.Rows.Count;
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (allZero)
                {
                    data.Columns.RemoveAt(i);
                    i -= 1;
                    length = data.Columns.Count;
                }

            }

            return data;
        }

        /// <summary>
        /// Removing the -9999 values predefined by the database
        /// </summary>
        /// <param name="dt"></param>
        public static void ProcessNumericDataTable(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    try
                    {
                        var a = dt.Rows[i][j];

                        if (dt.Rows[i][j] == null)
                            dt.Rows[i][j] = 0;
                        else if(dt.Rows[i][j] == DBNull.Value)
                            dt.Rows[i][j] = 0;
                        else if ((double)dt.Rows[i][j] == -9999 || (double)dt.Rows[i][j] == 999999 || (double)dt.Rows[i][j] == 9999999)
                        {
                            dt.Rows[i][j] = 0;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Removing the -9999 values predefined by the database
        /// </summary>
        /// <param name="dt"></param>
        public static void TreatMissingValues(this DataTable dt, MissingDataTreatment dataTreatment)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    try
                    {
                        if (dt.Rows[i][j] == null || (double)dt.Rows[i][j] == 0 || DBNull.Value.Equals((double)dt.Rows[i][j]))
                        {
                            switch(dataTreatment)
                            {
                                case MissingDataTreatment.ArithmeticAverage:
                                    double av  = dt.AsEnumerable().Where(x => x.Field<double>(j) != 0).Average(x => x.Field<double>(j));
                                    dt.Rows[i][j] = (double?)dt.AsEnumerable().Where(x=> x.Field<double>(j) != 0).Average(x => x.Field<double>(j));
                                    break;
                                default:
                                    dt.Rows[i][j] = (double?)dt.AsEnumerable().Average(x => x.Field<double>(j));
                                    break;
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
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
        public static IEnumerable<T> FilterByCoordinates<T>(this IEnumerable<T> models,
                                                            double latitudeBegin,
                                                            double longitudeBegin,
                                                            double latitudeEnd,
                                                            double longitudeEnd)
        {

            try
            {
                //Instantiate local variables
                Type entityType = typeof(T);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

                //Getting the fields of the object
                var model = models.FirstOrDefault();
                Type type = model.GetType();
                var fields = type.GetFields();

                string latitudeName = "", longitudeName = "";

                if (fields.Count() > 2)
                    foreach (FieldInfo i in fields)
                    {
                        if (i.Name.ToLower().Contains("latitude"))
                            latitudeName = i.Name;

                        if (i.Name.ToLower().Contains("longitude"))
                            longitudeName = i.Name;
                    }
                else
                    foreach (PropertyDescriptor i in properties)
                    {
                        if (i.Name.ToLower().Contains("latitude"))
                            latitudeName = i.Name;

                        if (i.Name.ToLower().Contains("longitude"))
                            longitudeName = i.Name;
                    }


                if (latitudeName == "" || longitudeName == "")
                    return models;

                var latiprop = entityType.GetProperty(latitudeName);
                var longiprop = entityType.GetProperty(longitudeName, BindingFlags.Public | BindingFlags.Instance);

                return models.AsQueryable().Where(latitudeName + " < " + latitudeEnd.ToString()
                                            + " And " + latitudeName + " > " + latitudeBegin.ToString()
                                            + " And " + longitudeName + " < " + longitudeEnd.ToString()
                                            + " And " + longitudeName + " > " + longitudeBegin.ToString());
            }
            catch (Exception e)
            {
                return Enumerable.Empty<T>();
            }
        }

        /// <summary>
        /// Filter a data set based on WGS84 LAT LONG Coordinates
        /// </summary>
        /// <param name="models"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> models, string parameter, bool notContained = false)
        {

            var type = models.GetType().GetGenericArguments()[0];
            var properties = type.GetProperties();
            try
            {
                if(!notContained)
                    return models.Where(x => properties.Any(p => p.GetValue(x) != null && p.GetValue(x).ToString().ToLower().Contains(parameter.ToLower())));
                else
                    return models.Where(x => !properties.Any(p => (p.GetValue(x) != null && p.GetValue(x).ToString().ToLower().Contains(parameter.ToLower()))));
            }
            catch (Exception e)
            {
                return Enumerable.Empty<T>();
            }
        }


        /// <summary>
        /// Getting the display name of a property
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static string GetPropertyDisplayName(object descriptor)
        {
            var pd = descriptor as PropertyDescriptor;

            if (pd != null)
            {
                try
                {
                    var disp = new ApirsRepository<tblAlia>()
                         .GetModelByExpression(x => x.alColumnName == pd.Name
                                                    && x.alImport == true
                                                    && x.alAlias != "User name"
                                                    && x.alAlias != "Project name"
                                                    && x.alAlias != "Project ID"
                                                    && x.alAlias != "Uploader ID"
                                                    && x.alAlias != "Uploader"
                                                    && x.alAlias != "User ID")
                         .Select(x => x.alAlias)
                         .First();

                    //// Check for DisplayName attribute and set the column header accordingly
                    //var displayName = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;

                    if (disp != null && disp != DisplayNameAttribute.Default.DisplayName)
                    {
                        return disp;
                    }
                }
                catch (Exception e)
                {
                    return pd.Name;
                }

            }
            else
            {
                var pi = descriptor as PropertyInfo;

                if (pi != null)
                {
                    // Check for DisplayName attribute and set the column header accordingly
                    Object[] attributes = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    for (int i = 0; i < attributes.Length; ++i)
                    {
                        var displayName = attributes[i] as DisplayNameAttribute;
                        if (displayName != null && displayName != DisplayNameAttribute.Default)
                        {
                            return displayName.DisplayName;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Getting the display name of a property
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static string GetPropertyDisplayName(string parameter)
        {
            if (parameter != null)
            {
                try
                {
                    var disp = new ApirsRepository<tblAlia>()
                         .GetModelByExpression(x => x.alColumnName == parameter)
                         .Select(x => x.alAlias)
                         .First();

                    //// Check for DisplayName attribute and set the column header accordingly
                    //var displayName = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;

                    if (disp != null && disp != DisplayNameAttribute.Default.DisplayName)
                    {
                        return disp;
                    }
                }
                catch (Exception e)
                {
                    return parameter;
                }

            }

            return null;
        }

        /// <summary>
        /// Getting the display name of a property
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static string GetPropertyColumnName(string parameter)
        {
            if (parameter != null)
            {
                try
                {
                    var disp = new ApirsRepository<tblAlia>()
                         .GetModelByExpression(x => x.alAlias == parameter)
                         .Select(x => x.alColumnName)
                         .First();

                    //// Check for DisplayName attribute and set the column header accordingly
                    //var displayName = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;

                    if (disp != null && disp != DisplayNameAttribute.Default.DisplayName)
                    {
                        return disp;
                    }
                }
                catch (Exception e)
                {
                    return null;
                }

            }

            return null;
        }
        #region Helper

        /// <summary>
        /// Getting the property value of a generic class
        /// </summary>
        /// <param name="src"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static object GetPropValue(object src, string propName)
        {
            try
            {
                return src.GetType().GetProperty(propName).GetValue(src, null);
            }
            catch
            {
                return null;
            }
        }

        //Converts a null value to string
        public static string NullToString(object Value)
        {

            // Value.ToString() allows for Value being DBNull, but will also convert int, double, etc.
            return Value == null ? "" : Value.ToString();
        }

        #endregion
    }


}
