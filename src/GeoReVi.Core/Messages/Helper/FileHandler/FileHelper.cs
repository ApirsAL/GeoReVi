using Kml2Sql.Mapping;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GeoReVi
{
    /// <summary>
    /// Static class to handle file interaction
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Static method to laod an image
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static BitmapImage LoadImage(string fileName)
        {
            var image = new BitmapImage();

            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
            }

            return image;
        }

        /// <summary>
        /// Static method to load an image
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static BitmapImage LoadImage(byte[] fileStreamByte)
        {
            var image = new BitmapImage();

            using (MemoryStream memoryStream = new MemoryStream(fileStreamByte, 0, fileStreamByte.Length))
            {
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = memoryStream;
                image.EndInit();
            }

            return image;
        }

        /// <summary>
        /// Static method to laod a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static void LoadFile(byte[] fileStreamByte, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
            fs.Write(fileStreamByte, 0, System.Convert.ToInt32(fileStreamByte.Length));
            fs.Seek(0, SeekOrigin.Begin);
            fs.Close();
        }

        /// <summary>
        /// Uploading a filestream to the server
        /// </summary>
        /// <param name="fileStreamByte"></param>
        /// <returns></returns>
        public static Guid UploadFile(string filePath)
        {

            if (filePath != "")
            {
                //Getting file information
                FileInfo fi = new FileInfo(filePath);
                FileStream fs;

                try
                {
                    //Implementing a new filestream
                    fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read);
                }
                catch
                {
                    MessageBox.ShowError("File is being used by another process.");
                    return new Guid();
                }

                //Transfering filestream into binary format
                BinaryReader rdr = new BinaryReader(fs);
                byte[] fileData = rdr.ReadBytes((int)fs.Length);

                //Closing filestream
                rdr.Close();
                fs.Close();

                try
                {
                    //Instantiate database
                    using (ApirsDatabase db = new ApirsDatabase())
                    {
                        //Retrieving file meta data
                        string fileName = fi.Name;
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                        string extension = fi.Extension;
                        char charac = 'a';

                        //Changing File name based on the count of occurences
                        while (db.v_FileStore.Where(x => x.name == fileName).Count() > 0)
                        {
                            fileName = fileNameWithoutExtension + charac + extension;
                            charac++;
                        }

                        //Establishing a sql connection
                        using (SqlConnection SqlConn = new SqlConnection(db.Database.Connection.ConnectionString.ToString()))
                        {
                            SqlCommand spAddFile = new SqlCommand("dbo.spAddFile", SqlConn);
                            //Testing if a connection is established
                            //Normally: if (sv.IsNetworkAvailable() && sv.IsServerConnected("130.83.96.87"))
                            if (ServerInteractionHelper.IsNetworkAvailable())
                            {
                                //Preparing the stored procedure
                                spAddFile.CommandType = CommandType.StoredProcedure;

                                //Adding the parameters
                                spAddFile.Parameters.Add("@pName", SqlDbType.NVarChar, 255).Value = fileName;
                                spAddFile.Parameters.Add("@pFile_Stream", SqlDbType.Image, fileData.Length).Value = fileData;

                                //Opening the connection
                                SqlConn.Open();

                                Guid result = (Guid)spAddFile.ExecuteScalar();

                                SqlConn.Close();

                                db.SaveChanges();

                                return result;
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.ShowError(UserMessageValueConverter.ConvertBack(1));
                    return new Guid();
                }

            }

            return new Guid();
        }

        /// <summary>
        /// Adding a country files kml to sql server
        /// </summary>
        public static void AddCountryKml(string kmlFile)
        {
            var fileStream = File.Open(kmlFile, FileMode.Open);
            var mapper = new Kml2SqlMapper(fileStream);

            var ap = new ApirsDatabase();

            using (var connection = new SqlConnection(ap.Database.Connection.ConnectionString))
            {
                connection.Open();
                var createTableCommand = mapper.GetCreateTableCommand(connection);
                createTableCommand.ExecuteNonQuery();

                foreach (var mapFeature in mapper.GetMapFeatures())
                {
                    try
                    {
                        var command = mapFeature.GetInsertCommand();
                        command.Connection = connection;
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

        }

        /// <summary>
        /// Reads an excel sheet dependent on wheter it is an old or a new version
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="sheetName"></param>
        /// <param name="old"></param>
        /// <returns></returns>
        public static DataSet LoadWorksheetsInDataSheets(string fileName, bool old, string sheetName = "")
        {
            DataTable sheetData = new DataTable();
            DataSet ds = new DataSet();

            try
            {
                using (OleDbConnection conn = returnExcelConnection(fileName, old))
                {
                    conn.Open();

                    if (sheetName == "")
                    {
                        // Get the data table containg the schema guid.
                        sheetData = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                        if (sheetData == null)
                        {
                            return null;
                        }

                        int i = 0;
                        String[] excelSheets = new String[sheetData.Rows.Count];

                        foreach (DataRow row in sheetData.Rows)
                        {
                            excelSheets[i] = sheetData.Rows[i]["TABLE_NAME"].ToString();
                            // retrieve the data using data adapter
                            OleDbDataAdapter sheetAdapter = new OleDbDataAdapter("SELECT * FROM [" + excelSheets[i] + "]", conn);

                            DataTable dt = new DataTable();
                            dt.TableName = excelSheets[i];

                            //Filling a data sheet
                            sheetAdapter.Fill(dt);
                            //Adding the data sheet to the collection
                            ds.Tables.Add(dt);
                            i++;
                        }
                    }
                    else
                    {
                        // retrieve the data using data adapter
                        OleDbDataAdapter sheetAdapter = new OleDbDataAdapter("select * from [" + sheetName + "]", conn);
                        //Filling a data sheet
                        sheetAdapter.Fill(sheetData);
                        //Adding the data sheet to the collection
                        ds.Tables.Add(sheetData);
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.ShowError(UserMessageValueConverter.ConvertBack(1));

                return null;
            }

            return ds;
        }

        /// <summary>
        /// Implementing an db connection to an excel file dependent on wheter it is an old or new excel sheet
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static OleDbConnection returnExcelConnection(string fileName, bool old = false)
        {

            return (old) ?
                new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + "; Jet OLEDB:Engine Type=5;Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1;")
                : new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + String.Format(@";Extended Properties=""Excel 12.0;HDR=Yes;IMEX=1;"""));
        }

        //Reading the table columns out of a data table
        public static string[] ReadColumnNamesFromDataTable(DataTable dt)
        {
            string[] columns = new String[dt.Columns.Count];

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                columns[i] = dt.Columns[i].ToString();
            }

            return columns;
        }


        //Reading the table columns out of a data table
        public static string[] ReadColumnNamesFromDataView(DataView dv)
        {
            string[] columns = new String[dv.Table.Columns.Count];

            for (int i = 0; i < dv.Table.Columns.Count; i++)
            {
                columns[i] = dv.Table.Columns[i].ToString();
            }

            return columns;
        }
        
        //Writing all properties of an object to a csv
        public static string WriteObjectToCsv(object obj)
        {
            StringBuilder sb = new StringBuilder();

            PropertyInfo[] properties = obj.GetType().GetProperties();

            foreach (PropertyInfo pi in properties)
            {
                sb.Append(
                    string.Format("{0};",
                            pi.GetValue(obj, null)
                        )
                );
            }

            return sb.ToString();
        }
    }
}
