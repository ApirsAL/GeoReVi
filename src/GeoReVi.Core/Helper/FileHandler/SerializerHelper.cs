using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// A serializer to serialize CLR objects into binary files
    /// </summary>
    public static class BinarySerializer<T>
    {
        public static void Serialize(Stream stream, T value)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, value);
        }

        public static void Serialize(FileInfo file, T value)
        {
            try
            {
                using (FileStream stream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    Serialize(stream, value);
                    stream.Close();
                }
            }
            finally
            {

            }
        }
        public static T Deserialize(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return (T)formatter.Deserialize(stream);
        }

        public static T Deserialize(FileInfo file)
        {
            T obj = (T)Activator.CreateInstance(typeof(T));

            try
            {
                using (var s = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    s.Position = 0;
                    obj = (T)formatter.Deserialize(s);
                }
            }
            catch
            {
                
            }

            return obj;
        }
    }
}
