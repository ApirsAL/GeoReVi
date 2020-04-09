using Caliburn.Micro;
using Kml2Sql.Mapping;
using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GeoReVi
{
    /// <summary>
    /// Static class to handle file interaction
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Downloads images related to an ID of a certain selected object
        /// </summary>
        /// <param name="id">ID of the object</param>
        /// <param name="tok">Token to cancel the method</param>
        /// <returns></returns>
        public static async Task<BindableCollection<v_PictureStore>> LoadImagesAsync(int id, string type)
        {
            using (var db = new ApirsRepository<v_PictureStore>())
            {
                try
                {
                    switch (type)
                    {
                        case "Object":
                            return new BindableCollection<v_PictureStore>(db.GetObjectImagesAsync(id).Result);
                        case "RockSample":
                            return new BindableCollection<v_PictureStore>(db.GetRockSampleImagesAsync(id).Result);
                        case "Facies":
                            return new BindableCollection<v_PictureStore>(db.GetFaciesImagesAsync(id).Result);
                        case "ArchitecturalElement":
                            return new BindableCollection<v_PictureStore>(db.GetArchitecturalElementImagesAsync(id).Result);
                        default:
                            return new BindableCollection<v_PictureStore>();
                    }
                }
                catch
                {
                    return new BindableCollection<v_PictureStore>();
                }

            }
        }

        /// <summary>
        /// Downloads images related to an ID of a certain selected object
        /// </summary>
        /// <param name="id">ID of the object</param>
        /// <param name="tok">Token to cancel the method</param>
        /// <returns></returns>
        public static async Task<BindableCollection<v_FileStore>> LoadFilesAsync(int id, string type)
        {
            using (var db = new ApirsRepository<v_PictureStore>())
            {
                try
                {
                    switch (type)
                    {
                        case "LabMeasurement":
                            return new BindableCollection<v_FileStore>(db.GetLabMeasurementFileAsync(id).Result);
                        case "FieldMeasurement":
                            return new BindableCollection<v_FileStore>(db.GetFieldMeasurementFileAsync(id).Result);
                        case "Section":
                            return new BindableCollection<v_FileStore>(db.GetSectionFileAsync(id).Result);
                        default:
                            return new BindableCollection<v_FileStore>();
                    }
                }
                catch
                {
                    return new BindableCollection<v_FileStore>();
                }

            }
        }

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
                image.Freeze();
            }

            return image;
        }

        /// <summary>
        /// Static method to load an image
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static BitmapImage LoadImageAsync(byte[] fileStreamByte)
        {

            var image = new BitmapImage();
            if (fileStreamByte == null)
                return null;

            using (MemoryStream memoryStream = new MemoryStream(fileStreamByte, 0, fileStreamByte.Length))
            {
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = memoryStream;
                image.EndInit();
                image.Freeze();
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
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("File is being used by another process.");
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
                    {
                        //Retrieving file meta data
                        string fileName = fi.Name;
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                        string extension = fi.Extension;
                        char charac = 'a';

                        ApirsDatabase ap = new ApirsDatabase();

                        //Changing File name based on the count of occurences
                        while (ap.v_FileStore.Where(x => x.name == fileName).Count() > 0)
                        {
                            fileName = fileNameWithoutExtension + charac + extension;
                            charac++;
                        }

                        ap = new ApirsDatabase();
                        //Establishing a sql connection
                        using (SqlConnection SqlConn = new SqlConnection(ap.Database.Connection.ConnectionString.ToString()))
                        {
                            SqlCommand spAddFile = new SqlCommand("dbo.spAddFile", SqlConn);
                            //Testing if a connection is established
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

                                return result;
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(1));
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
        public static DataSet LoadWorksheetsInDataSheets(string fileName, bool old, string sheetName = "", string type = "")
        {
            DataTable sheetData = new DataTable();
            DataSet ds = new DataSet();

            try
            {
                if(type==".xlsx")
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
                            dt.Locale = CultureInfo.CurrentCulture;
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
                else if (type==".xls")
                {

                }

            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(1));
                return null;
            }

            return ds;
        }

        /// <summary>
        /// Reading a CSV file into a data table
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isFirstRowHeader"></param>
        /// <returns></returns>
        public static DataTable CsvToDataTable(string filePath, bool isFirstRowHeader)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);

                string sql = @"SELECT * FROM [" + fileName + "]";

                string CultureName = Thread.CurrentThread.CurrentCulture.Name;
                CultureInfo ci = new CultureInfo(CultureName);
                if (ci.NumberFormat.NumberDecimalSeparator != ".")
                {
                    // Forcing use of decimal separator for numerical values
                    ci.NumberFormat.NumberDecimalSeparator = ".";
                    ci.NumberFormat.NumberGroupSeparator = ",";
                    Thread.CurrentThread.CurrentCulture = ci;
                }

                using (OleDbConnection conn = returnCsvConnection(filePath, isFirstRowHeader))
                {
                    using (OleDbCommand command = new OleDbCommand(sql, conn))
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Locale = Thread.CurrentThread.CurrentCulture;
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch(Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(1));
                return null;
            }
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

        /// <summary>
        /// Implementing a db connection to a csv file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isFirstRowHeader"></param>
        /// <returns></returns>
        private static OleDbConnection returnCsvConnection(string filePath, bool isFirstRowHeader)
        {
            string header = isFirstRowHeader ? "Yes" : "No";
            string pathOnly = Path.GetDirectoryName(filePath);

            return new OleDbConnection(
              @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
              ";Extended Properties=\"Text;HDR=" + header + "\"");
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

        public static FileInfo GetFileInfo(DragEventArgs e)
        {
            //Reading the data out
            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            //Getting file information
            return new FileInfo(FileList[0]);
        }

        /// <summary>
        /// Checks drag event args if it is of a valid type
        /// </summary>
        /// <param name="e"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CheckFileFormat(FileInfo fi, FileImportTypes type)
        {
            //Retrieving file meta data
            string fileName = fi.Name;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = fi.Extension;

            switch(type)
            {
                case FileImportTypes.ExcelAndCSV:
                if (extension != ".XLSX" && extension != ".XLS" && extension != ".CSV" && extension != ".xlsx" && extension != ".xls" && extension != ".csv")
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please comply to the file formats .xlsx, .xls or .csv");
                    return false;
                }
                else
                    return true;
                default:
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please comply to the file formats .xlsx, .xls or .csv");
                    return false;
            }
        }

        /// <summary>
        /// Converts a csv file to a data table
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            StreamReader sr = new StreamReader(strFilePath);
            string[] headers = sr.ReadLine().Split(',');
            DataTable dt = new DataTable();
            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }
            while (!sr.EndOfStream)
            {
                string[] rows = Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = rows[i];
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }

    /// <summary>
    /// Import type for files
    /// </summary>
    public enum FileImportTypes
    {
        ExcelAndCSV = 0
    }
}
