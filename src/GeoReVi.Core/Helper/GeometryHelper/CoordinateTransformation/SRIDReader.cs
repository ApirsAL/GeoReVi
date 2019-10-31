namespace GeoReVi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using ProjNet.CoordinateSystems;

    public class SRIDReader
    {
        private static readonly Lazy<CoordinateSystemFactory> CoordinateSystemFactory =
            new Lazy<CoordinateSystemFactory>(() => new CoordinateSystemFactory());

        public struct WktString
        {
            /// <summary>
            /// Well-known ID
            /// </summary>
            public int WktId;
            /// <summary>
            /// Well-known Text
            /// </summary>
            public string Wkt;
        }

        /// <summary>
        /// List of strings
        /// </summary>
        public List<WktString> WktStrings
        {
            get;
            set;
        }

        /// <summary>
        /// Enumerates all SRID's in the SRID.csv file.
        /// </summary>
        /// <returns>Enumerator</returns>
        private string GetSrids(string filename = null, int id = 0)
        {
            if (WktStrings != null)
                return WktStrings.Where(x => x.WktId == id).Select(x=>x.Wkt).First();
            else
            {
                WktStrings = new List<WktString>();
                string dir = System.IO.Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().Location);

                string file = dir + @"\Media\CoordinateSystems\SRID.csv";

                var stream = string.IsNullOrWhiteSpace(filename)
                    ? File.OpenRead(file)
                    : File.OpenRead(filename);

                foreach (string line in File.ReadLines(string.IsNullOrWhiteSpace(filename) ? file : filename, Encoding.ASCII))
                 {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string line1 = line.Substring(1, line.Length - 1).Replace("\"\"", "\"");

                    int split = line1.IndexOf(';');
                    if (split <= -1)
                        continue;

                    var wkt = new WktString
                    {
                        WktId = int.Parse(line1.Substring(0, split)),
                        Wkt = line1.Substring(split + 1)
                    };

                    WktStrings.Add(wkt);
                }
            }

            return WktStrings.Where(x => x.WktId == id).Select(x => x.Wkt).First();
        }

        /// <summary>
        /// Gets a coordinate system from the SRID.csv file
        /// </summary>
        /// <param name="id">EPSG ID</param>
        /// <param name="file">(optional) path to CSV File with WKT definitions.</param>
        /// <returns>Coordinate system, or <value>null</value> if no entry with <paramref name="id"/> was not found.</returns>
        public CoordinateSystem GetCSbyID(int id, string file = null)
        {
            //ICoordinateSystemFactory factory = new CoordinateSystemFactory();
             return CoordinateSystemFactory.Value.CreateFromWkt(GetSrids(file, id));
        }
    }
}
