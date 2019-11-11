
using Caliburn.Micro;
using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;

namespace GeoReVi
{
    /// <summary>
    /// An extension of the BackgroundWorker to await it's tasks
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Running the BackgroundWorker as a Task
        /// </summary>
        /// <param name="backgroundWorker"></param>
        /// <returns></returns>
        public static Task<object> RunWorkerTaskAsync(this BackgroundWorker backgroundWorker)
        {
            var tcs = new TaskCompletionSource<object>();

            RunWorkerCompletedEventHandler handler = null;
            handler = (sender, args) =>
            {
                if (args.Cancelled)
                    tcs.TrySetCanceled();
                else if (args.Error != null)
                    tcs.TrySetException(args.Error);
                else
                    tcs.TrySetResult(args.Result);
            };

            backgroundWorker.RunWorkerCompleted += handler;
            try
            {
                backgroundWorker.RunWorkerAsync();
            }
            catch
            {
                backgroundWorker.RunWorkerCompleted -= handler;
                throw;
            }

            return tcs.Task;
        }

        /// <summary>
        /// Updating a line chart
        /// </summary>
        public async static void UpdateChart<T>(this BindableCollection<T> dataCollection) where T : class
        {
            try
            {
                var i = default(T);
                dataCollection.Add(i);
                dataCollection.Remove(i);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Enabling to add a range of values to an observable collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observableCollection"></param>
        /// <param name="rangeList"></param>
        public static void AddRange<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> rangeList)
        {
            foreach (T item in rangeList)
            {
                observableCollection.Add(item);
            }
        }

        /// <summary>
        /// Cloning an object without reference
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            //A formatter
            System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            try
            {
                using (Stream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(stream);
                }
            }
            catch
            {
                return default(T);
            }


        }

        /// <summary>
        /// Shifting elements in an array
        /// FROM https://stackoverflow.com/questions/7242909/moving-elements-in-array-c-sharp
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="array">Array</param>
        /// <param name="oldIndex">Old index</param>
        /// <param name="newIndex">New index</param>
        public static void ShiftElement<T>(this T[] array, int oldIndex, int newIndex)
        {
            // TODO: Argument validation
            if (oldIndex == newIndex)
            {
                return; // No-op
            }
            T tmp = array[oldIndex];
            if (newIndex < oldIndex)
            {
                // Need to move part of the array "up" to make room
                Array.Copy(array, newIndex, array, newIndex + 1, oldIndex - newIndex);
            }
            else
            {
                // Need to move part of the array "down" to fill the gap
                Array.Copy(array, oldIndex + 1, array, oldIndex, newIndex - oldIndex);
            }
            array[newIndex] = tmp;
        }

        /// <summary>
        /// Transposes a data table
        ///FROM https://stackoverflow.com/questions/14817061/transpose-a-datatable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataTable Transpose(this DataTable dt)
        {
            DataTable dtNew = new DataTable();

            try
            {
                //adding columns    
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    dtNew.Columns.Add(i.ToString());
                }

                //Changing Column Captions: 
                dtNew.Columns[0].ColumnName = " ";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //For dateTime columns use like below
                    dtNew.Columns[i + 1].ColumnName = Convert.ToDateTime(dt.Rows[i].ItemArray[0].ToString()).ToString("MM/dd/yyyy");
                    //Else just assign the ItermArry[0] to the columnName property
                }

                //Adding Row Data
                for (int k = 1; k < dt.Columns.Count; k++)
                {
                    DataRow r = dtNew.NewRow();
                    r[0] = dt.Columns[k].ToString();
                    for (int j = 1; j <= dt.Rows.Count; j++)
                        r[j] = dt.Rows[j - 1][k];
                    dtNew.Rows.Add(r);
                }
            }
            catch
            {

            }

            return dtNew;
        }

        /// <summary>
        /// Multiplying vector3d objects
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Multiply(this Vector3D a, Vector3D b)
        => (a.X * b.X + a.Y * b.Y + a.Z * b.Z);

        /// <summary>
        /// Multiplying vector3d objects
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3D Multiply(this Vector3D a, double b)
        => new Vector3D(a.X * b, a.Y * b, a.Z * b);

        /// <summary>
        /// Dividing vector3d objects
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3D Divide(this Vector3D a, double b)
        => Vector3D.Divide(a, b);

        /// <summary>
        /// Dividing vector3d objects
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static LocationTimeValue ToLocationTimeValue(this Vector3D a)
        => new LocationTimeValue(a.X, a.Y, a.Z);

        /// <summary>
        /// Converting a list of points to a point collection
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static PointCollection ToPointCollection(this List<LocationTimeValue> points)
        {
            PointCollection ret = new PointCollection();
            try
            {
                for(int i = 0; i < points.Count(); i++)
                {
                    Point pt = new Point(points[i].X, points[i].Y);
                    ret.Add(pt);
                }
            }
            catch
            {

            }

            return ret;
        }

        /// <summary>
        /// Calculating cross product of vector3d objects
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3D CrossProduct(this Vector3D a, Vector3D b)
        => Vector3D.CrossProduct(a, b);

        /// <summary>
        /// Converts a list of locationtimevalues to a sql geometry (MULTIPOINT)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SqlGeometry ToSqlGeometry(this List<LocationTimeValue> a)
        => GeometryBuilder.BuildMultiPoint(a);

        /// <summary>
        /// Converts a list of locationtimevalues to a sql geometry (MULTIPOINT)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static void SortXYClockWise(this List<LocationTimeValue> a)
        => a.OrderBy(x => Math.Atan2(x.X, x.Y)).ToList();

        /// <summary>
        /// Converts a list of locationtimevalues to a sql geometry (MULTIPOINT)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static void SortXYZClockWise(this List<LocationTimeValue> a)
        {
            try
            {
                a.OrderBy(x => x.X).OrderBy(x => x.Y).OrderBy(x => x.Z);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Converts a point3d to a locationtimevalue
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static LocationTimeValue ToLocationTimeValue(this Point3D point)
            => new LocationTimeValue(point.X, point.Y, point.Z);

        /// <summary>
        /// Converts location values to Point collection
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Point3DCollection ToPointCollection(this ObservableCollection<LocationTimeValue> points)
            => new Point3DCollection(points.Select(x => new Point3D(x.X, x.Y, x.Z)));


        /// <summary>
        /// Converts any serializable object to an XML file
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void ToXml(this object obj, string path)
        {
            XmlSerializer s = new XmlSerializer(obj.GetType());
            using (TextWriter tw = new StreamWriter(@path))
            {
                s.Serialize(tw,obj);
            }
        }

        /// <summary>
        /// Converts an XML file back to an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T FromXml<T>(this string path)
        {
            XmlSerializer s = new XmlSerializer(typeof(T));
            using (TextReader reader = new StreamReader(@path))
            {
                object obj = s.Deserialize(reader);
                return (T)obj;
            }
        }

        private static Random rng = new Random();

        /// <summary>
        /// Shuffle a list based on the Fisher-Yates shuffle
        /// FROM https://stackoverflow.com/questions/273313/randomize-a-listt
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
