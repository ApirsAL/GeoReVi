using System.Collections.Generic;
using Caliburn.Micro;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System;
using Rh.DateRange.Picker.Common;
using System.IO;
using Microsoft.Win32;
using MoreLinq;
using System.Net.Http;
using System.Text;

namespace GeoReVi
{
    /// <summary>
    /// View model to load, display and transform data sets from the database
    /// </summary>
    public class LoadParameterDataViewModel : PropertyChangedBase
    {

        #region Public Properties

        /// <summary>
        /// Helper class for coordinate transformations
        /// </summary>
        private CoordinateTransformationHelper coordinateTransformationHelper = new CoordinateTransformationHelper();
        public CoordinateTransformationHelper CoordinateTransformationHelper
        {
            get => this.coordinateTransformationHelper;
            set
            {
                this.coordinateTransformationHelper = value;
                NotifyOfPropertyChange(() => CoordinateTransformationHelper);
            }
        }

        /// <summary>
        /// Measurement points of a property
        /// </summary>
        private BindableCollection<Mesh> measPoints = new BindableCollection<Mesh>();
        public BindableCollection<Mesh> MeasPoints
        {
            get => this.measPoints;
            set
            {
                this.measPoints = value;

                CreateBasicStatisticsHelper();
                NotifyOfPropertyChange(() => MeasPoints);
            }
        }

        /// <summary>
        /// A descriptive statistics view model
        /// </summary>
        private HeterogeneityStatisticsViewModel heterogeneityStatisticsViewModel = new HeterogeneityStatisticsViewModel();
        public HeterogeneityStatisticsViewModel HeterogeneityStatisticsViewModel
        {
            get => this.heterogeneityStatisticsViewModel;
            set
            {
                this.heterogeneityStatisticsViewModel = value;
                NotifyOfPropertyChange(() => HeterogeneityStatisticsViewModel);
            }
        }

        /// <summary>
        /// View model for the mesh statistics
        /// </summary>
        private MeshStatisticsViewModel meshStatisticsViewModel = new MeshStatisticsViewModel();
        public MeshStatisticsViewModel MeshStatisticsViewModel
        {
            get => this.meshStatisticsViewModel;
            set
            {
                this.meshStatisticsViewModel = value;
                NotifyOfPropertyChange(() => MeshStatisticsViewModel);
            }
        }

        /// <summary>
        /// Solver class for differential equations
        /// </summary>
        private NumericalSolver numericalSolverHelper = new NumericalSolver();
        public NumericalSolver NumericalSolverHelper
        {
            get => this.numericalSolverHelper;
            set
            {
                this.numericalSolverHelper = value;
                NotifyOfPropertyChange(() => NumericalSolverHelper);
            }
        }

        /// <summary>
        /// The selected Item
        /// </summary>
        private Mesh selectedMeasPoint = new Mesh();
        public Mesh SelectedMeasPoint
        {
            get => this.selectedMeasPoint;
            set
            {
                this.selectedMeasPoint = value;
                NotifyOfPropertyChange(() => SelectedMeasPoint);
            }
        }

        /// <summary>
        /// variable to check to show all objects
        /// </summary>
        private bool all;
        public bool All
        {
            get => this.all;
            set
            {
                this.all = value;
                NotifyOfPropertyChange(() => All);
            }
        }

        /// <summary>
        /// Variable the data set will be grouped by
        /// </summary>
        private string groupBy;
        public string GroupBy
        {
            get => this.groupBy;
            set
            {
                this.groupBy = value;
                NotifyOfPropertyChange(() => GroupBy);
            }
        }

        //Checks if values should be filtered by a date range
        private bool filterByDate = false;
        public bool FilterByDate
        {
            get => this.filterByDate;
            set
            {
                this.filterByDate = value;
                NotifyOfPropertyChange(() => FilterByDate);
            }
        }

        /// <summary>
        /// Lower time limit
        /// </summary>
        private DateTime? from = new DateTime(1900, 1, 1, 0, 0, 0);
        public DateTime? From
        {
            get => this.from;
            set
            {
                this.from = value;
                NotifyOfPropertyChange(() => From);
            }
        }

        /// <summary>
        /// Upper time limit
        /// </summary>
        private DateTime? to = DateTime.Now;
        public DateTime? To
        {
            get => this.to;
            set
            {
                this.to = value;
                NotifyOfPropertyChange(() => To);
            }
        }

        /// <summary>
        /// The kind of date time range
        /// </summary>
        private DateRangeKind? kind = DateRangeKind.Custom;
        public DateRangeKind? Kind
        {
            get => this.kind;
            set
            {
                this.kind = value;
                NotifyOfPropertyChange(() => Kind);
            }
        }

        /// <summary>
        /// Variable to select the global or local reference system
        /// </summary>
        private bool global;
        public bool Global
        {
            get => this.global;
            set
            {
                this.global = value;
                NotifyOfPropertyChange(() => Global);
            }
        }

        /// <summary>
        /// An object to conduct spatial interpolations
        /// </summary>
        private SpatialInterpolationHelper spatialInterpolationHelper = new SpatialInterpolationHelper();
        public SpatialInterpolationHelper SpatialInterpolationHelper
        {
            get => this.spatialInterpolationHelper;
            set
            {
                this.spatialInterpolationHelper = value;
                NotifyOfPropertyChange(() => SpatialInterpolationHelper);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Lab measurement constructor
        /// </summary>
        /// <param name="_parameterClass"></param>
        public LoadParameterDataViewModel()
        {

        }

        #endregion

        #region Public methods

        /// <summary>
        /// Converts all coordinates in a mesh based on SRID
        /// </summary>
        public async Task ConvertCoordinates()
        {
            try
            {
                CoordinateTransformationHelper.ConvertCoordinates(SelectedMeasPoint);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Removing a data set from the data collection based on the key
        /// </summary>
        /// <param name="key">Key of the data set</param>
        public void RemoveDataSet()
        {
            try
            {
                MeasPoints.Remove(SelectedMeasPoint);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Updates the dataSet
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="newKey"></param>
        public void UpdateHeader(string newKey)
        {
            try
            {
                SelectedMeasPoint.Name = newKey;

            }
            catch
            {

            }
        }

        /// <summary>
        /// Importing a dropped object of investigation data file
        /// </summary>
        /// <param name="e"></param>
        public void ImportMesh()
        {
            try
            {
                //File dialog for opening a jpeg, png or bmp file
                OpenFileDialog openFileDlg = new OpenFileDialog();
                openFileDlg.Filter = @"Excel (*.xlsx;*.xls)|*.xlsx;*.xls|CSV (*.csv)|*.csv|GeoReVi Mesh (*.gmsh)|*.gmsh";
                openFileDlg.RestoreDirectory = true;
                openFileDlg.Multiselect = true;
                openFileDlg.ShowDialog();

                if (openFileDlg.FileName == "")
                {
                    return;
                }

                foreach(var file in openFileDlg.FileNames)
                {
                    //Getting file information
                    FileInfo fi = new FileInfo(file);

                    DataTable table = new DataTable() { TableName = "MyTableName" };
                    DataTable tableCloned = new DataTable();

                    if (fi.Extension == ".XLSX" || fi.Extension == ".xlsx")
                    {
                        DataSet tables = FileHelper.LoadWorksheetsInDataSheets(fi.FullName, false, "", fi.Extension);
                        table = tables.Tables[0];

                        ((ShellViewModel)IoC.Get<IShell>()).ShowLocationValueImport(ref table);

                        tableCloned = table.Clone();
                        tableCloned.Columns["Value1"].DataType = typeof(double);
                        tableCloned.Columns["X"].DataType = typeof(double);
                        tableCloned.Columns["Y"].DataType = typeof(double);
                        tableCloned.Columns["Z"].DataType = typeof(double);

                        foreach (DataRow row1 in table.Rows)
                        {
                            tableCloned.ImportRow(row1);
                        }

                        MeasPoints.Add(new Mesh() { Name = "New data set", Data = tableCloned });
                    }
                    else if (fi.Extension == ".CSV" || fi.Extension == ".csv")
                    {
                        table = FileHelper.CsvToDataTable(fi.FullName, true);

                        ((ShellViewModel)IoC.Get<IShell>()).ShowLocationValueImport(ref table);

                        tableCloned = table.Clone();
                        tableCloned.Columns["Value1"].DataType = typeof(double);
                        tableCloned.Columns["X"].DataType = typeof(double);
                        tableCloned.Columns["Y"].DataType = typeof(double);
                        tableCloned.Columns["Z"].DataType = typeof(double);

                        foreach (DataRow row1 in table.Rows)
                        {
                            tableCloned.ImportRow(row1);
                        }

                        MeasPoints.Add(new Mesh() { Name = "New data set", Data = tableCloned });
                    }
                    else if (fi.Extension == ".gmsh" || fi.Extension == ".gmsh")
                    {
                        Mesh importMesh = (Mesh)fi.FullName.FromXml<Mesh>();

                        for(int i = 0; i<importMesh.Vertices.Count();i++)
                            if(importMesh.Vertices[i].Value.Count() > 1)
                            {
                                for(int j = 0; j< importMesh.Vertices[i].Value.Count(); j++)
                                {
                                    if (j == 0)
                                        continue;
                                    else
                                        if (importMesh.Vertices[i].Value[j] > importMesh.Vertices[i].Value[0])
                                            importMesh.Vertices[i].Value[0] = importMesh.Vertices[i].Value[j];
                                }
                            }

                        importMesh.Cells.Clear();
                        importMesh.Faces.Clear();

                        switch(importMesh.Dimensionality)
                        {
                            case Dimensionality.TwoD:
                                importMesh.FacesFromPointCloud();
                                break;
                            case Dimensionality.ThreeD:
                                importMesh.CellsFromPointCloud();
                                break;
                        }

                        MeasPoints.Add(importMesh);
                    }
                }
            }
            catch (Exception ex)
            {
                ((ShellViewModel)IoC.Get<IShell>()).LogError(ex);
            }
        }

        /// <summary>
        /// Saving the chart object
        /// </summary>
        public void ExportMesh()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "GeoReVi Mesh (*.gmsh)|*.gmsh";
            saveFileDialog1.RestoreDirectory = true;

            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                try
                {
                    //Getting file information
                    FileInfo fi = new FileInfo(saveFileDialog1.FileName);

                    switch (fi.Extension)
                    {
                        case ".gmsh":
                            SelectedMeasPoint.ToXml(fi.FullName);
                            break;
                    }
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
                }
            }
        }

        /// <summary>
        /// Applies a logarithmic transformation on the selected column
        /// </summary>
        /// <param name="columnName"></param>
        public void LogarithmicTransformation()
        {
            if (SelectedMeasPoint.Data.AsEnumerable().Where(y => y.Field<double>(0) <= 0).Count() > 0)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError("Cannot log-transform negative values");
                return;
            }

            //Transforming data set
            for (int i = 0; i < SelectedMeasPoint.Data.Rows.Count; i++)
            {
                SelectedMeasPoint.Data.Rows[i][0] = Math.Log10((double)SelectedMeasPoint.Data.Rows[i][0]);
                SelectedMeasPoint.Vertices[i].Value[0] = Math.Log10((double)SelectedMeasPoint.Vertices[i].Value[0]);
            }
        }

        /// <summary>
        /// Applies an exponential transformation on the selected column
        /// </summary>
        /// <param name="columnName"></param>
        public void ExponentialTransformation()
        {
            //Transforming data set
            for (int i = 0; i < SelectedMeasPoint.Data.Rows.Count; i++)
            {
                SelectedMeasPoint.Data.Rows[i][0] = Math.Pow(10, (double)SelectedMeasPoint.Data.Rows[i][0]);
                SelectedMeasPoint.Vertices[i].Value[0] = Math.Pow(10, (double)SelectedMeasPoint.Vertices[i].Value[0]);
            }
        }

        /// <summary>
        /// Makes z coordinate the value of the point
        /// </summary>
        /// <param name="columnName"></param>
        public void MakeZValue()
        {
            //Transforming data set
            for (int i = 0; i < SelectedMeasPoint.Data.Rows.Count; i++)
            {
                SelectedMeasPoint.Data.Rows[i][0] = (double)SelectedMeasPoint.Data.Rows[i][3];
                SelectedMeasPoint.Vertices[i].Value[0] = (double)SelectedMeasPoint.Vertices[i].Z;
                
            }
        }

        /// <summary>
        /// Applying a z transformation on the dataset
        /// </summary>
        public void ZScoreTransformation()
        {
            int count = SelectedMeasPoint.Data.Rows.Count;

            if (count >= 0)
            {
                double avg = SelectedMeasPoint.Data.AsEnumerable().Average(r => r.Field<double>(0));
                double standardDeviation = SelectedMeasPoint.Data.AsEnumerable().Select(r => r.Field<double>(0)).StdDev();

                for (int i = 0; i < SelectedMeasPoint.Data.Rows.Count; i++)
                {
                    SelectedMeasPoint.Data.Rows[i][0] = ((double)SelectedMeasPoint.Data.Rows[i][0] - avg)/standardDeviation;
                    SelectedMeasPoint.Vertices[i].Value[0] = (SelectedMeasPoint.Vertices[i].Value[0] - avg)/standardDeviation;
                }
            }
        }

        /// <summary>
        /// Applying a rescaling on the dataset
        /// </summary>
        public void Rescaling()
        {
            int count = SelectedMeasPoint.Data.Rows.Count;

            if (count >= 0)
            {
                double avg = SelectedMeasPoint.Data.AsEnumerable().Average(r => r.Field<double>(0));
                double max = SelectedMeasPoint.Data.AsEnumerable().Select(r => r.Field<double>(0)).Max();
                double min = SelectedMeasPoint.Data.AsEnumerable().Select(r => r.Field<double>(0)).Min();

                for (int i = 0; i < SelectedMeasPoint.Data.Rows.Count; i++)
                {
                    SelectedMeasPoint.Data.Rows[i][0] = ((double)SelectedMeasPoint.Data.Rows[i][0] - min) / (max-min);
                    SelectedMeasPoint.Vertices[i].Value[0] = ((double)SelectedMeasPoint.Vertices[i].Value[0] - min) / (max-min);
                }
            }
        }

        /// <summary>
        /// Applying a mean normalization on the dataset
        /// </summary>
        public void MeanNormalization()
        {
            int count = SelectedMeasPoint.Data.Rows.Count;

            if (count >= 0)
            {
                double avg = SelectedMeasPoint.Data.AsEnumerable().Average(r => r.Field<double>(0));
                double max = SelectedMeasPoint.Data.AsEnumerable().Select(r => r.Field<double>(0)).Max();
                double min = SelectedMeasPoint.Data.AsEnumerable().Select(r => r.Field<double>(0)).Min();

                for (int i = 0; i < SelectedMeasPoint.Data.Rows.Count; i++)
                {
                    SelectedMeasPoint.Data.Rows[i][0] = ((double)SelectedMeasPoint.Data.Rows[i][0] - avg) / (max - min);
                    SelectedMeasPoint.Vertices[i].Value[0] = ((SelectedMeasPoint.Vertices[i].Value[0] - avg) / (max - min));
                }
            }
        }

        /// <summary>
        /// Applying a mean normalization on the dataset
        /// </summary>
        public void SubtractMean()
        {
            int count = SelectedMeasPoint.Data.Rows.Count;

            if (count >= 0)
            {
                double avg = SelectedMeasPoint.Data.AsEnumerable().Average(r => r.Field<double>(0));

                for (int i = 0; i < SelectedMeasPoint.Data.Rows.Count; i++)
                {
                    SelectedMeasPoint.Data.Rows[i][0] = ((double)SelectedMeasPoint.Data.Rows[i][0] - avg);
                    SelectedMeasPoint.Vertices[i].Value[0] = ((double)SelectedMeasPoint.Vertices[i].Value[0] - avg);
                }
            }
        }

        /// <summary>
        /// Group the vertices by name
        /// </summary>
        public async Task GroupByName()
        {
            try
            {
                List<Mesh> groupedMeshes = new List<Mesh>();

                List<string> names =  SelectedMeasPoint.Data.AsEnumerable().Select(x => x.Field<string>("Name")).Distinct().ToList();

                names.ForEach(x =>
                {
                    Mesh mesh = new Mesh();
                    mesh.Data = SelectedMeasPoint.Data.AsEnumerable().Where(y => y.Field<string>("Name") == x).CopyToDataTable();
                    mesh.Vertices = new System.Collections.ObjectModel.ObservableCollection<LocationTimeValue>(SelectedMeasPoint.Vertices.Where(y => y.Name == x).ToList());
                    mesh.Name = x;
                    groupedMeshes.Add(mesh);
                });

                MeasPoints.AddRange(groupedMeshes);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Creating the basic statistics objects
        /// </summary>
        public void CreateBasicStatisticsHelper()
        {
            try
            {
                HeterogeneityStatisticsViewModel.UnivariateHeterogeneityMeasuresHelper = new BindableCollection<KeyValuePair<string, UnivariateHeterogeneityMeasuresHelper>>();

                for (int i = 0; i < MeasPoints.Count; i++)
                {
                    double[] dataSet = MeasPoints[i].Data.AsEnumerable().Select(x => x.Field<double>(0)).ToArray();

                    HeterogeneityStatisticsViewModel.UnivariateHeterogeneityMeasuresHelper.Add(new KeyValuePair<string, UnivariateHeterogeneityMeasuresHelper>(MeasPoints[i].Name, new UnivariateHeterogeneityMeasuresHelper(dataSet, MeasPoints[i].Name)));
                }
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Creating the mesh statistics objects
        /// </summary>
        public void CreateMeshStatisticsHelper()
        {
            try
            {
                MeshStatisticsViewModel.MeshStatisticsHelper = new BindableCollection<MeshStatisticsHelper>();

                for (int i = 0; i < MeasPoints.Count; i++)
                {
                    MeshStatisticsViewModel.MeshStatisticsHelper.Add(new MeshStatisticsHelper(MeasPoints[i]));
                }
            }
            catch
            {
                return;
            }

        }

        /// <summary>
        /// Assigns an API-based elevation value to a set of points
        /// </summary>
        public async Task GetElevation()
        {
            try
            {
                List<double> dList = new List<double>();

                //Data set has to be two dimensional
                if (SpatialInterpolationHelper.SelectedMeasPoints.First().Dimensionality != Dimensionality.TwoD)
                    return;

                string url = FormatURI(SpatialInterpolationHelper.SelectedMeasPoints.First().Vertices.ToList());

                if (url.Length > 2000)
                {
                    for (int i = 0; i < SpatialInterpolationHelper.SelectedMeasPoints.First().Vertices.Count(); i += 100)
                    {
                        try
                        {
                            url = FormatURI(SpatialInterpolationHelper.SelectedMeasPoints.First().Vertices.Skip(i).Take(100).ToList());

                            dList.AddRange(await GetElevationsFromHTTP(url));
                        }
                        catch
                        {

                        }
                    }
                }
                else
                {
                    dList.AddRange(await GetElevationsFromHTTP(url));
                }

                for ( int i = 0; i<SelectedMeasPoint.Vertices.Count(); i++)
                {
                    LocationTimeValue loc = SelectedMeasPoint.Vertices[i];

                    SelectedMeasPoint.Vertices[i].Z = dList[i];

                    var a = SelectedMeasPoint.Data.AsEnumerable().Where(x => x.Field<double>(1) == SelectedMeasPoint.Vertices[i].X && x.Field<double>(2) == SelectedMeasPoint.Vertices[i].Y).First();

                    a[3] = dList[i];
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Computes the interpolation of the selected data sets
        /// </summary>
        /// <returns></returns>
        public async Task ComputeDiscretization()
        {
            try
            {
                MeasPoints.Add(await SpatialInterpolationHelper.DiscretizeDataSet());
            }
            catch
            {

            }

        }

        /// <summary>
        /// Computes the interpolation of the selected data sets
        /// </summary>
        /// <returns></returns>
        public async Task ComputeOperation()
        {
            try
            {
                MeasPoints.Add(await SpatialInterpolationHelper.ComputeSpatialOperation());
            }
            catch
            {

            }

        }

        /// <summary>
        /// Computes the interpolation of the selected data sets
        /// </summary>
        /// <returns></returns>
        public async Task ComputeNumericalSolution()
        {
            try
            {
                MeasPoints.Add(await NumericalSolverHelper.ComputeNumericalSolution());
            }
            catch
            {

            }

        }

        /// <summary>
        /// Computes the interpolation of the selected data sets
        /// </summary>
        /// <returns></returns>
        public async Task ComputeInterpolation()
        {
            try
            {
                var a = await SpatialInterpolationHelper.ComputeInterpolation();
                MeasPoints.Add(a);
                if(SpatialInterpolationHelper.ExportResiduals)
                     MeasPoints.Add(new Mesh(SpatialInterpolationHelper.Residuals));
            }
            catch
            {

            }
        }

        /// <summary>
        /// Building a slice of the selected model
        /// </summary>
        /// <returns></returns>
        public async Task GetSlice()
        {
            try
            {

            }
            catch
            {

            }

        }

        /// <summary>
        /// Computes the interpolation of the selected data sets
        /// </summary>
        /// <returns></returns>
        public async Task ComputeCategorization()
        {
            try
            {
                await Task.WhenAll(SpatialInterpolationHelper.ComputeCategorization());
            }
            catch
            {

            }
        }

        /// <summary>
        /// Computes the interpolation of the selected data sets
        /// </summary>
        /// <returns></returns>
        public async Task ComputeResiduals()
        {
            try
            {
                MeasPoints.Add(await SpatialInterpolationHelper.ComputeResiduals());
            }
            catch
            {

            }

        }

        /// <summary>
        /// Opens the coordinate systems table
        /// </summary>
        /// <returns></returns>
        public async Task ShowCoordinateTable()
        {
            try
            {
                System.Diagnostics.Process.Start(@"Media\CoordinateSystems\SRID.csv");
            }
            catch
            {

            }
        }

        /// <summary>
        /// Making a shallow copy of the selected mesh
        /// </summary>
        /// <returns></returns>
        public async Task CopyMesh()
        {
            try
            {
                MeasPoints.Add(new Mesh(SelectedMeasPoint));

            }
            catch
            {

            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the bing key from the associated file
        /// </summary>
        /// <returns></returns>
        private string GetBingKey()
        {
            string ret = "";

            try
            {
                string dir = System.IO.Path.GetDirectoryName(
    System.Reflection.Assembly.GetExecutingAssembly().Location);

                string file = "";

                file = dir + @"\Media\Data\K.csv";

                var stream = File.OpenRead(file);

                foreach (string line in File.ReadLines(file, Encoding.ASCII))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    else
                        ret = line;
                }
            }
            catch
            {

            }

            return ret;
        }

        // Format the URI from a list of locations.
        protected string FormatURI(List<LocationTimeValue> locList)
        {
            // The base URI string. Fill in: 
            // {0}: The lat/lon list, comma separated. 
            // {1}: The heights: datum string, lower case. 
            // {2}: The key. 
            const string BASE_URI_STRING =
              "http://dev.virtualearth.net/REST/v1/Elevation/List?points={0}&key={1}";
            string retVal = string.Empty;
            string locString = string.Empty;
            for (int ndx = 0; ndx < locList.Count; ++ndx)
            {
                if (ndx != 0)
                {
                    locString += ",";
                }
                locString += locList[ndx].Y.ToString() + "," + locList[ndx].X.ToString();
            }
            retVal = string.Format(BASE_URI_STRING, locString,GetBingKey());
            return retVal;
        }

        /// Format the URI from a list of locations.
        protected async Task<List<double>> GetElevationsFromHTTP(string url)
        {
            List<double> retVal = new List<double>();
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage msg = await httpClient.GetAsync(url);

                //Check the status code. Throw an exception on error. 
                if (!msg.IsSuccessStatusCode)
                {
                    string errMsg = "HTTP Response Error: [" + msg + "]";
                    throw new Exception(errMsg);
                }

                Stream inStream = await msg.Content.ReadAsStreamAsync();

                using (StreamReader reader = new StreamReader(inStream))
                {
                    //Get the string from the HTTP stream, find the index of the
                    //substring where the altitudes are enumerated.
                    string readString = reader.ReadToEnd();
                    int ndx = readString.IndexOf("elevations\":[");

                    //Check to see if the substring has been found.
                    if (ndx >= 0)
                    {
                        string elevationListStr =
                           readString.Substring(ndx + "elevations\":[".Length);
                        ndx = elevationListStr.IndexOf(']');

                        if (ndx > 0)
                        {
                            elevationListStr = elevationListStr.Substring(0, ndx);

                            //Split the comma delimited list into doubles.
                            char[] parm = { ',' };
                            string[] result = elevationListStr.Split(parm);

                            //Add the strings to the list.
                            foreach (string dbl in result)
                            {
                                retVal.Add(double.Parse(dbl));
                            }
                        }
                        else
                        {
                            string errMsg = "Format Error: [" + readString + "]";
                            throw new Exception(errMsg);
                        }
                    }
                    else
                    {
                        string errMsg = "No elevations found in the return string: [" +
                            readString + "]";
                        throw new Exception(errMsg);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
            return retVal;
        }

        #endregion
    }
}
