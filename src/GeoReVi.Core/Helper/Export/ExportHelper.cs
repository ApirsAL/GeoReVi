using Caliburn.Micro;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// Helper class for file export
    /// </summary>
    public static class ExportHelper
    {
        /// <summary>
        /// Exporting a data table (dt) to a csv file path
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="filePath"></param>
        public static void ExportDataTableToCsv(DataTable dt, string filePath, bool append = false, bool convert = true)
        {
            if (dt != null && filePath != "")
            {
                StringBuilder sb = new StringBuilder();
                AliasDbColumnConverter adb = new AliasDbColumnConverter();

                //Reading the column names
                List<string> columnNames = dt.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName).ToList();

                //Converting the database column headers to local headers
                if(convert)
                for (int i = 0; i < columnNames.Count(); i++)
                {
                    columnNames[i] = adb.Convert(columnNames[i]);
                }

                //Appending the column names
                sb.AppendLine(string.Join(",", columnNames));

                //Exporting each row
                foreach (DataRow row in dt.Rows)
                {

                    List<string> fields = row.ItemArray.Select(field => field.ToString()).ToList();

                    //Eliminating all commas and semicolons from the texts
                    for (int i = 0; i < fields.Count(); i++)
                    {
                        if (fields[i].Contains(","))
                            fields[i] = fields[i].Replace(',', '|');
                        else if (fields[i].Contains(";"))
                            fields[i] = fields[i].Replace(';', '|');
                        else if (fields[i].Contains("-9999"))
                            fields[i] = fields[i].Replace("-9999", "");

                        fields[i] = fields[i].Replace("\r\n", "").Replace("\n", "").Replace("\r", "");

                    }
                    sb.AppendLine(string.Join(",", fields));
                }

                try
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    if (!append)
                        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                    else
                        File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("another process"))
                        ((ShellViewModel)IoC.Get<IShell>()).ShowError("The selected file is beeing used by another process.");
                    else
                        ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occured.");
                }
            }
        }

        //Exporting a generic list based on its type and the additional parameter provided
        public static async void ExportList<T>(IList<T> List, string path, string AdditionalParameter = "")
        {
            //Getting the type of the generic list
            Type typeParameterType = typeof(T);

            //Switching the type
            switch (typeParameterType.ToString())
            {
                case ("GeoReVi.UnivariateHeterogeneityMeasuresHelper"):
                    //Casing the generic list to an object list
                    List<UnivariateHeterogeneityMeasuresHelper> aList = List.Cast<UnivariateHeterogeneityMeasuresHelper>().ToList();
                    await Task.Run(() =>
                    {
                        try
                        {
                            DataTable statTable = CollectionHelper.ConvertTo<UnivariateHeterogeneityMeasuresHelper>(aList);
                            //Exporting data to a csv
                            ExportDataTableToCsv(statTable, path, false, false);
                        }
                        catch
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occured.");
                        }
                    });
                    break;
                case ("GeoReVi.tblRockSample"):

                    //Casing the generic list to an object list
                    List<tblRockSample> myList = List.Cast<tblRockSample>().ToList();

                    switch (AdditionalParameter)
                    {
                        case ("Plug"):
                            await Task.Run(() =>
                            {
                                try
                                {
                                    tblPlug query;
                                    List<tblPlug> plugs = new List<tblPlug>();

                                    using (var db = new ApirsDatabase())
                                    {
                                        //The rock sample name
                                        string label = "";

                                        //Iterating through all participating projects and select all related rock samples and facies types
                                        foreach (var samp in List)
                                        {
                                            try
                                            {
                                                label = samp.GetType().GetProperty("sampLabel").GetValue(samp).ToString();

                                                query = (from plug in db.tblPlugs
                                                         where plug.plugLabel == label
                                                         select plug).First();

                                                plugs.Add(query);
                                            }
                                            catch
                                            {
                                                plugs.Add(new tblPlug() { plugLabel = label });
                                            }
                                        }
                                    }


                                    //Converting data to data tables
                                    DataTable rockSampleTable = CollectionHelper.ConvertTo<tblRockSample>(myList);
                                    DataTable plugTable = CollectionHelper.ConvertTo<tblPlug>(plugs);
                                    DataTable joinPlug = CollectionHelper.JoinDataTables(rockSampleTable, plugTable, (row1, row2) => row1.Field<string>("sampLabel") == row2.Field<string>("plugLabel"));

                                    //Exporting data to a csv
                                    ExportDataTableToCsv(joinPlug, path, false);
                                }
                                catch
                                {
                                    throw new Exception("An unexpected error occured.");
                                }
                            });
                            break;
                        case ("Cuboid"):
                            await Task.Run(() =>
                            {
                                try
                                {
                                    tblCuboid query;
                                    List<tblCuboid> cubs = new List<tblCuboid>();

                                    using (var db = new ApirsDatabase())
                                    {
                                        //The rock sample name
                                        string label = "";

                                        //Iterating through all participating projects and select all related rock samples and facies types
                                        foreach (var samp in List)
                                        {
                                            try
                                            {
                                                label = samp.GetType().GetProperty("sampLabel").GetValue(samp).ToString();

                                                query = (from cub in db.tblCuboids
                                                         where cub.cubLabel == label
                                                         select cub).First();

                                                cubs.Add(query);
                                            }
                                            catch
                                            {
                                                cubs.Add(new tblCuboid() { cubLabel = label });
                                            }
                                        }
                                    }

                                    //Converting data to data tables
                                    DataTable rockSampleTable = CollectionHelper.ConvertTo<tblRockSample>(myList);
                                    DataTable cubTable = CollectionHelper.ConvertTo<tblCuboid>(cubs);
                                    DataTable joinCub = CollectionHelper.JoinDataTables(rockSampleTable, cubTable, (row1, row2) => row1.Field<string>("sampLabel") == row2.Field<string>("cubLabel"));

                                    //Exporting data to a csv
                                    ExportHelper.ExportDataTableToCsv(joinCub, path, false);
                                }
                                catch
                                {
                                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occured.");
                                }
                            });
                            break;
                        case ("Handpiece"):
                            await Task.Run(() =>
                            {
                                try
                                {
                                    tblHandpiece query;
                                    List<tblHandpiece> hps = new List<tblHandpiece>();

                                    using (var db = new ApirsDatabase())
                                    {
                                        //The rock sample name
                                        string label = "";

                                        //Iterating through all participating projects and select all related rock samples and facies types
                                        foreach (var samp in List)
                                        {
                                            try
                                            {
                                                label = samp.GetType().GetProperty("sampLabel").GetValue(samp).ToString();

                                                query = (from hp in db.tblHandpieces
                                                         where hp.hpLabelFk == label
                                                         select hp).First();

                                                hps.Add(query);
                                            }
                                            catch
                                            {
                                                hps.Add(new tblHandpiece() { hpLabelFk = label });
                                            }
                                        }
                                    }

                                    //Converting data to data tables
                                    DataTable rockSampleTable = CollectionHelper.ConvertTo<tblRockSample>(myList);
                                    DataTable cubTable = CollectionHelper.ConvertTo<tblHandpiece>(hps);
                                    DataTable joinCub = CollectionHelper.JoinDataTables(rockSampleTable, cubTable, (row1, row2) => row1.Field<string>("sampLabel") == row2.Field<string>("hpLabelFk"));

                                    //Exporting data to a csv
                                    ExportHelper.ExportDataTableToCsv(joinCub, path, false);
                                }
                                catch
                                {
                                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occured.");
                                }
                            });
                            break;
                        case ("All"):
                        case ("Sediment/Soil"):
                            await Task.Run(() =>
                            {
                                try
                                {
                                    //Converting data to data tables
                                    DataTable rockSampleTable = CollectionHelper.ConvertTo<tblRockSample>(myList);

                                    //Exporting data to a csv
                                    ExportHelper.ExportDataTableToCsv(rockSampleTable, path, false);
                                }
                                catch
                                {
                                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occured.");
                                }
                            });
                            break;
                        case ("LabMeasurement"):
                            await Task.Run(() =>
                            {
                                try
                                {
                                    v_PetrophysicsRockSamples query;
                                    List<v_PetrophysicsRockSamples> petrophysics = new List<v_PetrophysicsRockSamples>();

                                    using (var db = new ApirsDatabase())
                                    {
                                        //The rock sample name
                                        string label = "";

                                        //Iterating through all participating projects and select all related rock samples and facies types
                                        foreach (var samp in List)
                                        {
                                            try
                                            {
                                                label = samp.GetType().GetProperty("sampLabel").GetValue(samp).ToString();

                                                query = (from pet in db.v_PetrophysicsRockSamples
                                                         where pet.labmeSampleName == label
                                                         select pet).First();

                                                petrophysics.Add(query);
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }
                                    }


                                    //Converting data to data tables
                                    DataTable petrophysicsTable = CollectionHelper.ConvertTo<v_PetrophysicsRockSamples>(petrophysics);

                                    //Exporting data to a csv
                                    ExportHelper.ExportDataTableToCsv(petrophysicsTable, path, false);
                                }
                                catch
                                {
                                    throw new Exception("An unexpected error occured.");
                                }
                            });
                            break;
                    }
                    break;
                case "GeoReVi.tblObjectOfInvestigation":
                    //Casing the generic list to an object list
                    List<tblObjectOfInvestigation> myOoiList = List.Cast<tblObjectOfInvestigation>().ToList();

                    switch (AdditionalParameter)
                    {
                        case ("Outcrop"):
                            await Task.Run(() =>
                            {
                                try
                                {
                                    tblOutcrop query;
                                    List<tblOutcrop> outcrops = new List<tblOutcrop>();

                                    using (var db = new ApirsDatabase())
                                    {
                                        //The rock sample name
                                        string label = "";

                                        //Iterating through all participating projects and select all related rock samples and facies types
                                        foreach (var samp in List)
                                        {
                                            try
                                            {
                                                label = samp.GetType().GetProperty("ooiName").GetValue(samp).ToString();

                                                query = (from outc in db.tblOutcrops
                                                         where outc.outLocalName == label
                                                         select outc).First();

                                                outcrops.Add(query);
                                            }
                                            catch
                                            {
                                                outcrops.Add(new tblOutcrop() { outLocalName = label });
                                            }
                                        }
                                    }

                                    //Converting data to data tables
                                    DataTable ooiTable = CollectionHelper.ConvertTo<tblObjectOfInvestigation>(myOoiList);
                                    DataTable outTable = CollectionHelper.ConvertTo<tblOutcrop>(outcrops);
                                    DataTable joinOut = CollectionHelper.JoinDataTables(ooiTable, outTable, (row1, row2) => row1.Field<string>("ooiName") == row2.Field<string>("outLocalName"));

                                    //Exporting data to a csv
                                    ExportHelper.ExportDataTableToCsv(joinOut, path, false);
                                }
                                catch
                                {
                                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occured.");
                                }
                            });
                            break;
                        case ("Drilling"):
                            await Task.Run(() =>
                            {
                                try
                                {
                                    tblDrilling query;
                                    List<tblDrilling> drillings = new List<tblDrilling>();

                                    using (var db = new ApirsDatabase())
                                    {
                                        //The rock sample name
                                        string label = "";

                                        //Iterating through all participating projects and select all related rock samples and facies types
                                        foreach (var samp in List)
                                        {
                                            try
                                            {
                                                label = samp.GetType().GetProperty("ooiName").GetValue(samp).ToString();

                                                query = (from drill in db.tblDrillings
                                                         where drill.drillName == label
                                                         select drill).First();

                                                drillings.Add(query);
                                            }
                                            catch
                                            {
                                                drillings.Add(new tblDrilling() { drillName = label });
                                            }
                                        }
                                    }

                                    //Converting data to data tables
                                    DataTable ooiTable = CollectionHelper.ConvertTo<tblObjectOfInvestigation>(myOoiList);
                                    DataTable drillTable = CollectionHelper.ConvertTo<tblDrilling>(drillings);
                                    DataTable joindrill = CollectionHelper.JoinDataTables(ooiTable, drillTable, (row1, row2) => row1.Field<string>("ooiName") == row2.Field<string>("drillName"));

                                    //Exporting data to a csv
                                    ExportHelper.ExportDataTableToCsv(joindrill, path, false);
                                }
                                catch
                                {
                                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occured.");
                                }
                            });
                            break;
                        case ("Transect"):
                            await Task.Run(() =>
                            {
                                try
                                {
                                    tblTransect query;
                                    List<tblTransect> transects = new List<tblTransect>();

                                    using (var db = new ApirsDatabase())
                                    {
                                        //The rock sample name
                                        string label = "";

                                        //Iterating through all participating projects and select all related rock samples and facies types
                                        foreach (var samp in List)
                                        {
                                            try
                                            {
                                                label = samp.GetType().GetProperty("ooiName").GetValue(samp).ToString();

                                                query = (from tra in db.tblTransects
                                                         where tra.traName == label
                                                         select tra).First();

                                                transects.Add(query);
                                            }
                                            catch
                                            {
                                                transects.Add(new tblTransect() { traName = label });
                                            }
                                        }
                                    }

                                    //Converting data to data tables
                                    DataTable ooiTable = CollectionHelper.ConvertTo<tblObjectOfInvestigation>(myOoiList);
                                    DataTable traTable = CollectionHelper.ConvertTo<tblTransect>(transects);
                                    DataTable joinTra = CollectionHelper.JoinDataTables(ooiTable, traTable, (row1, row2) => row1.Field<string>("ooiName") == row2.Field<string>("traName"));

                                    //Exporting data to a csv
                                    ExportHelper.ExportDataTableToCsv(joinTra, path, false);
                                }
                                catch
                                {
                                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occured.");
                                }
                            });
                            break;
                        case ("All"):
                            await Task.Run(() =>
                            {
                                try
                                {
                                    //Converting data to data tables
                                    DataTable ooiTable = CollectionHelper.ConvertTo<tblObjectOfInvestigation>(myOoiList);

                                    //Exporting data to a csv
                                    ExportHelper.ExportDataTableToCsv(ooiTable, path, false);
                                }
                                catch
                                {
                                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occured.");
                                }
                            });
                            break;
                    }

                    break;
                case ("GeoReVi.tblFieldMeasurement"):
                    await Task.Run(() =>
                    {
                        try
                        {

                            List<v_PetrophysicsFieldMeasurements> query;
                            List<v_PetrophysicsFieldMeasurements> petrophysics = new List<v_PetrophysicsFieldMeasurements>();
                            string label = "";
                            label = List.FirstOrDefault().GetType().GetProperty("fimeObjectOfInvestigation").GetValue(List.FirstOrDefault()).ToString();

                            List<int> b = ((ShellViewModel)IoC.Get<IShell>()).Projects.Select(x => x.prjIdPk).ToList();

                            using (var db = new ApirsRepository<v_PetrophysicsFieldMeasurements>())
                            {

                                //Iterating through all participating projects and select all related rock samples and facies types
                                foreach (var samp in List)
                                {
                                    try
                                    {
                                        //Local coordinates and OOI
                                        double Xcoordinate = 0;
                                        double Ycoordinate = 0;
                                        double Zcoordinate = 0;
                                        int ProjectID = -1;


                                        Xcoordinate = (double)samp.GetType().GetProperty("fimeLocalCoordinateX").GetValue(samp);
                                        Ycoordinate = (double)samp.GetType().GetProperty("fimeLocalCoordinateY").GetValue(samp);
                                        Zcoordinate = (double)samp.GetType().GetProperty("fimeLocalCoordinateZ").GetValue(samp);
                                        ProjectID = (int)samp.GetType().GetProperty("fimeprjIdFk").GetValue(samp);

                                        query = db.GetModelByExpression(x => x.Local_x == Xcoordinate
                                                                            && x.Local_y == Ycoordinate
                                                                            && x.Local_z == Zcoordinate
                                                                            && x.Object_of_investigation == label
                                                                            && x.Project_ID == ProjectID).ToList();

                                        foreach (v_PetrophysicsFieldMeasurements pet in query)
                                        {
                                            petrophysics.Add(pet);
                                        }
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            }


                            //Converting data to data tables
                            DataTable petrophysicsTable = CollectionHelper.ConvertTo<v_PetrophysicsFieldMeasurements>(petrophysics);

                            //Exporting data to a csv
                            ExportHelper.ExportDataTableToCsv(petrophysicsTable, path, false);
                        }
                        catch
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occured.");
                        }
                    });
                    break;
                default:

                    break;
            }
        }

        //Exporting a list of images to a pdf document
        public static void ExportImageToPdf(List<Bitmap> ImageList, string path)
        {
            PdfDocument doc = new PdfDocument();

            foreach(Bitmap image in ImageList)
            {
                //Initializing new pdf document and pages
                PdfPage oPage = new PdfPage();

                //Adding the page to the document
                doc.Pages.Add(oPage);

                //Creating a graphics element form the pdf page
                XGraphics xgr = XGraphics.FromPdfPage(oPage);

                if (image.Width > image.Height)
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);

                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //Creating an XImage from the image
                    XImage img = XImage.FromStream(ms);

                    double ratioHeight = image.Height / oPage.Height;
                    double ratioWidth = image.Width / oPage.Width;
                    double newWidth = 0;
                    double newHeight = 0;

                    if(ratioHeight>1&&ratioHeight>ratioWidth)
                    {
                        newWidth = image.Width/(image.Height/(oPage.Height - 40));
                        xgr.DrawImage(img, 0, 0, newWidth, oPage.Height -40);
                    }
                    else if(ratioWidth>1&&ratioWidth>ratioHeight)
                    {
                        newHeight = image.Height / (image.Width / (oPage.Width - 40));
                        xgr.DrawImage(img, 0,0, oPage.Width - 40, newHeight);
                    }
                    else
                    {
                        xgr.DrawImage(img, 0, 0);
                    }
                    img = null;
                }

                xgr = null;
            }

            try
            {
                doc.Save(path);
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError("File in use.");
            }

            doc.Close();

        }
    }
}
