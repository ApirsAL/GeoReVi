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
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Threading;

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
        /// View model for univariate statistical tests
        /// </summary>
        private UnivariateStatisticalTestViewModel univariateStatisticalTestViewModel = new UnivariateStatisticalTestViewModel();
        public UnivariateStatisticalTestViewModel UnivariateStatisticalTestViewModel
        {
            get => this.univariateStatisticalTestViewModel;
            set
            {
                this.univariateStatisticalTestViewModel = value;
                NotifyOfPropertyChange(() => UnivariateStatisticalTestViewModel);
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

        /// <summary>
        /// The type of transformation that should be applied
        /// </summary>
        private TransformationType transformationType = TransformationType.ZScore;
        public TransformationType TransformationType
        {
            get => this.transformationType;
            set
            {
                this.transformationType = value;
                NotifyOfPropertyChange(() => TransformationType);
            }
        }


        /// <summary>
        /// The type of join that should be applied
        /// </summary>
        private JoinMethod joinMethod= JoinMethod.Exact;
        public JoinMethod JoinMethod
        {
            get => this.joinMethod;
            set
            {
                this.joinMethod = value;
                NotifyOfPropertyChange(() => JoinMethod);
            }
        }

        /// <summary>
        /// The threshold for the joining algorithm
        /// </summary>
        private double threshold = 0.2;
        public double Threshold
        {
            get => this.threshold;
            set
            {
                this.threshold = value;
                NotifyOfPropertyChange(() => Threshold);
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
        /// Updates the dataSet
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="newKey"></param>
        public void UpdateProperty(string newKey)
        {
            try
            {
                int index = SelectedMeasPoint.Properties.IndexOf(SelectedMeasPoint.Properties.First(x => x.Equals(SelectedMeasPoint.SelectedProperty)));

                var a = SelectedMeasPoint.Properties.First(x => x.Equals(SelectedMeasPoint.SelectedProperty));

                a = new KeyValuePair<int, string>(SelectedMeasPoint.SelectedProperty.Key, newKey);

                SelectedMeasPoint.Properties.RemoveAt(index);

                SelectedMeasPoint.Properties.Insert(index, a);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Brings up the actually selected property
        /// </summary>
        /// <returns></returns>
        public async Task BringUp()
        {
            try
            {
                await SelectedMeasPoint.BringToFront();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Updates the dataSet
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="newKey"></param>
        public void UpdateHeader(string newKey, ActionExecutionContext context)
        {
            try
            {
                var keyArgs = context.EventArgs as KeyEventArgs;

                if (keyArgs != null && keyArgs.Key == Key.Enter)
                {
                    SelectedMeasPoint.Name = newKey;
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Importing a dropped object of investigation data file
        /// </summary>
        /// <param name="e"></param>
        public async Task ImportMesh()
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


                foreach (var file in openFileDlg.FileNames)
                {
                    //Getting file information
                    FileInfo fi = new FileInfo(file);

                    DataTable table = new DataTable() { TableName = "MyTableName" };

                    if (fi.Extension == ".XLSX" || fi.Extension == ".xlsx")
                    {

                        DataSet tables = FileHelper.LoadWorksheetsInDataSheets(fi.FullName, false, "", fi.Extension);
                        table = tables.Tables[0];

                        ((ShellViewModel)IoC.Get<IShell>()).ShowLocationValueImport(ref table);

                        DataTable tableCloned = table.Clone();
                        tableCloned.ConvertColumnType("Value1", typeof(double));
                        tableCloned.ConvertColumnType("X", typeof(double));
                        tableCloned.ConvertColumnType("Y", typeof(double));
                        tableCloned.ConvertColumnType("Z", typeof(double));
                        tableCloned.ConvertColumnType("DateTime", typeof(DateTime));
                        tableCloned.Columns["DateTime"].DefaultValue = DateTime.Now;

                        foreach (DataRow row in table.Rows)
                        {
                            try
                            {
                                tableCloned.ImportRow(row);
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        ObservableCollection<LocationTimeValue> vertices = new ObservableCollection<LocationTimeValue>(tableCloned.AsEnumerable()
                        .Select(x => new LocationTimeValue()
                        {
                            Value = new List<double>() { (x.Field<double?>("Value1") == -9999 || x.Field<double?>("Value1") == -999999 || x.Field<double?>("Value1") == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>("Value1")) },
                            X = (x.Field<double?>("X") == -9999 || x.Field<double?>("X") == -999999 || x.Field<double?>("X") == 9999999 || Double.IsNaN(Convert.ToDouble(x.Field<double?>("X")))) ? 0 : Convert.ToDouble(x.Field<double?>("X")),
                            Y = (x.Field<double?>("Y") == -9999 || x.Field<double?>("Y") == -999999 || x.Field<double?>("Y") == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>("Y")),
                            Z = (x.Field<double?>("Z") == -9999 || x.Field<double?>("Z") == -999999 || x.Field<double?>("Z") == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>("Z")),
                            Name = x.Field<string>("Name"),
                            Date = x.Field<DateTime?>("DateTime").HasValue ? x.Field<DateTime>("DateTime") : DateTime.Now
                        }).ToList());

                        MeasPoints.Add(new Mesh() { Name = "New data set", Vertices = vertices});
                    }
                    else if (fi.Extension == ".CSV" || fi.Extension == ".csv")
                    {
                        if (Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                            table = FileHelper.ConvertCSVtoDataTable(fi.FullName);
                        else
                            table = FileHelper.CsvToDataTable(fi.FullName, true);

                        ((ShellViewModel)IoC.Get<IShell>()).ShowLocationValueImport(ref table);

                        DataTable tableCloned = table.Clone();
                        tableCloned.ConvertColumnType("Value1", typeof(double));
                        tableCloned.ConvertColumnType("Value2", typeof(double));
                        tableCloned.ConvertColumnType("Value3", typeof(double));
                        tableCloned.ConvertColumnType("Value4", typeof(double));
                        tableCloned.ConvertColumnType("Value5", typeof(double));
                        tableCloned.ConvertColumnType("Value6", typeof(double));
                        tableCloned.ConvertColumnType("Value7", typeof(double));
                        tableCloned.ConvertColumnType("Value8", typeof(double));
                        tableCloned.ConvertColumnType("Value9", typeof(double));
                        tableCloned.ConvertColumnType("Value10", typeof(double));
                        tableCloned.ConvertColumnType("X", typeof(double));
                        tableCloned.ConvertColumnType("Y", typeof(double));
                        tableCloned.ConvertColumnType("Z", typeof(double));
                        tableCloned.ConvertColumnType("DateTime", typeof(DateTime));
                        tableCloned.Columns["DateTime"].DefaultValue = DateTime.Now;

                        table.Locale = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                        tableCloned.Locale = System.Globalization.CultureInfo.GetCultureInfo("en-US");

                        foreach (DataRow row in table.Rows)
                        {
                            try
                            {
                                tableCloned.ImportRow(row);
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        ObservableCollection<LocationTimeValue> vertices = new ObservableCollection<LocationTimeValue>(tableCloned.AsEnumerable()
                        .Select(x => new LocationTimeValue()
                        {
                            Value = new List<double>()
                            {
                                Convert.ToDouble(x.Field<double?>("Value1")),
                                Convert.ToDouble(x.Field<double?>("Value2")),
                                Convert.ToDouble(x.Field<double?>("Value3")),
                                Convert.ToDouble(x.Field<double?>("Value4")),
                                Convert.ToDouble(x.Field<double?>("Value5")),
                                Convert.ToDouble(x.Field<double?>("Value6")),
                                Convert.ToDouble(x.Field<double?>("Value7")),
                                Convert.ToDouble(x.Field<double?>("Value8")),
                                Convert.ToDouble(x.Field<double?>("Value9")),
                                Convert.ToDouble(x.Field<double?>("Value10")),
                            },
                            X = (x.Field<double?>("X") == -9999 || x.Field<double?>("X") == -999999 || x.Field<double?>("X") == 9999999 || Double.IsNaN(Convert.ToDouble(x.Field<double?>("X")))) ? 0 : Convert.ToDouble(x.Field<double?>("X")),
                            Y = (x.Field<double?>("Y") == -9999 || x.Field<double?>("Y") == -999999 || x.Field<double?>("Y") == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>("Y")),
                            Z = (x.Field<double?>("Z") == -9999 || x.Field<double?>("Z") == -999999 || x.Field<double?>("Z") == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>("Z")),
                            Name = x.Field<string>("Name"),
                            Date = x.Field<DateTime?>("DateTime").HasValue ? x.Field<DateTime>("DateTime") : DateTime.Now
                        }).ToList());

                        Mesh importMesh = new Mesh() { Name = "New data set", Vertices = vertices, Dimensionality = Dimensionality.ThreeD };
                        importMesh.GetProperties();

                        MeasPoints.Add(importMesh);
                    }
                    else if (fi.Extension == ".gmsh" || fi.Extension == ".gmsh")
                    {
                        await Task.Run(() =>
                        {
                            Mesh importMesh = (Mesh)fi.FullName.FromXml<Mesh>();

                            for (int i = 0; i < importMesh.Vertices.Count(); i++)
                                if (importMesh.Vertices[i].Value.Count() > 1)
                                {
                                    for (int j = 0; j < importMesh.Vertices[i].Value.Count(); j++)
                                    {
                                        if (j == 0)
                                            continue;
                                        else
                                            if (importMesh.Vertices[i].Value[j] > importMesh.Vertices[i].Value[0])
                                            importMesh.Vertices[i].Value[0] = importMesh.Vertices[i].Value[j];
                                    }
                                }

                            importMesh.GetProperties();
                            importMesh.Cells.Clear();
                            importMesh.Faces.Clear();

                            switch (importMesh.Dimensionality)
                            {
                                case Dimensionality.TwoD:
                                    importMesh.FacesFromPointCloud();
                                    break;
                                case Dimensionality.ThreeD:
                                    importMesh.CellsFromPointCloud();
                                    break;
                            }

                            MeasPoints.Add(importMesh);
                        });
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
        /// </summary>+		$exception	{"DBNull.Value kann nicht in den Typ 'System.DateTime' umgewandelt werden. Verwenden Sie einen Typ, der NULL-Werte zulässt."}	System.InvalidCastException

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
                    Mesh export = new Mesh(SelectedMeasPoint);

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
        /// Merges the selected meshes
        /// </summary>
        public void MergeMeshes()
        {
            try
            {
                var a = MeshJoiner.JoinMeshes(SpatialInterpolationHelper.SelectedMeasPoints.ToList(), JoinMethod, Threshold);
                MeasPoints.Add(a);
            }
            catch
            {
                throw new Exception("Cannot merge meshes.");
            }
        }

        /// <summary>
        /// Removes the actually selected property
        /// </summary>
        /// <returns></returns>
        public async Task RemoveProperty()
        {
            try
            {
                await SelectedMeasPoint.RemoveProperty();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Applying a mean normalization on the dataset
        /// </summary>
        public void Transform()
        {
            try
            {
                var a = new Mesh(SelectedMeasPoint);

                switch(TransformationType)
                {
                    case TransformationType.ZScore:
                        DistributionHelper.ZScoreTransformation(ref a);
                        break;
                    case TransformationType.MeanNormalization:
                        DistributionHelper.MeanTransformation(ref a);
                        break;
                    case TransformationType.Elevation:
                        DistributionHelper.MakeZValue(ref a);
                        break;
                    case TransformationType.Exponential:
                        DistributionHelper.ExponentialTransformation(ref a);
                        break;
                    case TransformationType.SubtractMean:
                        DistributionHelper.SubtractMeanTransformation(ref a);
                        break;
                    case TransformationType.Rescaling:
                        DistributionHelper.RescalingTransformation(ref a);
                        break;
                    case TransformationType.Logarithmic:
                        DistributionHelper.LogarithmicTransformation(ref a);
                        break;
                    case TransformationType.NormalSpace:
                        DistributionHelper.QuantileQuantileNormalScoreTransformation(ref a);
                        break;
                    case TransformationType.QuantileQuantileTransform:
                        var b = new Mesh(SpatialInterpolationHelper.SelectedMeasPoints[0]);
                        DistributionHelper.QuantileQuantileBackTransformation(ref a, b);
                        break;
                    case TransformationType.RoundValues:
                        DistributionHelper.RoundingTransformation(ref a);
                        break;
                }

                a.Name += " transformed";
                MeasPoints.Add(a);
            }
            catch
            {
                throw new Exception("Please select at least one mesh before");
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

                List<string> names =  SelectedMeasPoint.Vertices.Select(x => x.Name).Distinct().ToList();

                names.ForEach(x =>
                {
                    Mesh mesh = new Mesh();
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
                    double[] dataSet = MeasPoints[i].Vertices.Select(x => x.Value[0]).ToArray();

                    HeterogeneityStatisticsViewModel.UnivariateHeterogeneityMeasuresHelper.Add(new KeyValuePair<string, UnivariateHeterogeneityMeasuresHelper>(MeasPoints[i].Name, new UnivariateHeterogeneityMeasuresHelper(dataSet)));
                }
            }
            catch
            {
                throw new Exception("Cannot build basic statistics helper");
            }
        }

        /// <summary>
        /// Computing the univariate tests for the sample meshes
        /// </summary>
        public void CreateBasicUnivariateTestsHelper()
        {
            try
            {
                UnivariateStatisticalTestViewModel.UnivariateTestHelper = new BindableCollection<KeyValuePair<string, UnivariateDistributionTestHelper>>();

                for (int i = 0; i < MeasPoints.Count; i++)
                {
                    double[] dataSet = MeasPoints[i].Vertices.Select(x => x.Value[0]).ToArray();

                    UnivariateStatisticalTestViewModel.UnivariateTestHelper.Add(new KeyValuePair<string, UnivariateDistributionTestHelper>(MeasPoints[i].Name, new UnivariateDistributionTestHelper(dataSet)));
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
