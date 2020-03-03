using Accord.Math;
using Caliburn.Micro;
using MoreLinq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class SpatialInterpolationHelper : SpatialDiscretizationHelper
    {
        #region Private members

        //Random member
        private static Random rnd = new Random();
        //Lock object for synchronization
        private readonly object sync = new object();

        #endregion

        #region Public properties

        /// <summary>
        /// Getting the distance type
        /// </summary>
        private DistanceType distanceType = DistanceType.Euclidean;
        public DistanceType DistanceType
        {
            get => this.distanceType;
            set
            {
                this.distanceType = value;
                NotifyOfPropertyChange(() => DistanceType);
            }
        }

        /// <summary>
        /// The wanted interpolation method
        /// </summary>
        private GeostatisticalInterpolationMethod interpolationMethod = GeostatisticalInterpolationMethod.IDW;
        public GeostatisticalInterpolationMethod InterpolationMethod
        {
            get => this.interpolationMethod;
            set
            {
                this.interpolationMethod = value;
                NotifyOfPropertyChange(() => this.InterpolationMethod);
                NotifyOfPropertyChange(() => this.IsIDW);
                NotifyOfPropertyChange(() => this.IsUniversalKriging);
            }
        }

        /// <summary>
        /// The wanted categorization method
        /// </summary>
        private CategorizationMethod categorizationMethod = CategorizationMethod.NearestNeighbor;
        public CategorizationMethod CategorizationMethod
        {
            get => this.categorizationMethod;
            set
            {
                this.categorizationMethod = value;
                NotifyOfPropertyChange(() => this.CategorizationMethod);
            }
        }

        /// <summary>
        /// The wanted interpolation method
        /// </summary>
        private GeostatisticalInterpolationMethod estimationVariance;
        public GeostatisticalInterpolationMethod EstimationVariance
        {
            get => this.estimationVariance;
            set
            {
                this.estimationVariance = value;
                NotifyOfPropertyChange(() => this.EstimationVariance);
            }
        }

        /// <summary>
        /// Check if the error variance of the measurement should be included
        /// </summary>
        private bool transformToOriginalDistribution = false;
        public bool TransformToOriginalDistribution
        {
            get => this.transformToOriginalDistribution;
            set
            {
                this.transformToOriginalDistribution = value;
                NotifyOfPropertyChange(() => TransformToOriginalDistribution);
            }
        }

        /// <summary>
        /// Check if the error variance of the measurement should be included
        /// </summary>
        private bool includeErrorVariance = false;
        public bool IncludeErrorVariance
        {
            get => this.includeErrorVariance;
            set
            {
                this.includeErrorVariance = value;
                NotifyOfPropertyChange(() => IncludeErrorVariance);
            }
        }

        /// <summary>
        /// Check if the local variance of the field should be included
        /// </summary>
        private bool includeLocalVariance = false;
        public bool IncludeLocalVariance
        {
            get => this.includeLocalVariance;
            set
            {
                this.includeLocalVariance = value;
                NotifyOfPropertyChange(() => IncludeLocalVariance);
            }
        }

        /// <summary>
        /// Check if the residuals should be exported
        /// </summary>
        private bool exportResiduals = false;
        public bool ExportResiduals
        {
            get => this.exportResiduals;
            set
            {
                this.exportResiduals = value;
                NotifyOfPropertyChange(() => ExportResiduals);
            }
        }

        /// <summary>
        /// Defines whether all values of the selected source grids should be 
        /// used for interpolation or only those where the target vertex name fits the source grid name
        /// </summary>
        private bool shouldTargetVertexFitSourceGridName = false;
        public bool ShouldTargetVertexFitSourceGridName
        {
            get => this.shouldTargetVertexFitSourceGridName;
            set
            {
                this.shouldTargetVertexFitSourceGridName = value;
                NotifyOfPropertyChange(() => ShouldTargetVertexFitSourceGridName);
            }
        }



        /// <summary>
        /// The error variance of the measurement
        /// </summary>
        private double errorVariance = 0;
        public double ErrorVariance
        {
            get => this.errorVariance;
            set
            {
                this.errorVariance = value;
                NotifyOfPropertyChange(() => ErrorVariance);
            }
        }

        /// <summary>
        /// The power parameter needed in the IDW interpolation
        /// </summary>
        private double power = 2;
        public double Power
        {
            get => this.power;
            set
            {
                if (value >= -10 && value <= 10)
                    this.power = value;
                else
                    this.power = 2;

                NotifyOfPropertyChange(() => Power);
            }
        }

        /// <summary>
        /// A variogram helper object
        /// </summary>
        private VariogramHelper vh = new VariogramHelper();
        public VariogramHelper Vh
        {
            get => this.vh;
            set
            {
                this.vh = value;
                NotifyOfPropertyChange(() => Vh);
            }
        }

        /// <summary>
        /// Selected data sets for interpolation
        /// </summary>
        private BindableCollection<Mesh> selectedInterpolationMeasPoints = new BindableCollection<Mesh>();
        public BindableCollection<Mesh> SelectedInterpolationMeasPoints
        {
            get => this.selectedInterpolationMeasPoints;
            set
            {
                this.selectedInterpolationMeasPoints = value;
                NotifyOfPropertyChange(() => SelectedInterpolationMeasPoints);
            }
        }

        /// <summary>
        /// Selected data sets for providing a global variance for conditional simulation
        /// </summary>
        private BindableCollection<Mesh> selectedGlobalVariance = new BindableCollection<Mesh>();
        public BindableCollection<Mesh> SelectedGlobalVariance
        {
            get => this.selectedGlobalVariance;
            set
            {
                this.selectedGlobalVariance = value;
                NotifyOfPropertyChange(() => SelectedGlobalVariance);
            }
        }

        /// <summarZ>
        /// The maximum count of neighbors included into an interpolation analysis
        /// </summarZ>
        private int maximumNeighborCount = 9999;
        public int MaximumNeighborCount
        {
            get => this.maximumNeighborCount;
            set
            {
                this.maximumNeighborCount = value;
                NotifyOfPropertyChange(() => MaximumNeighborCount);
            }
        }

        /// <summary>
        /// Mean-Absolute-Error of the algorithm
        /// </summary>
        private double mae = 0;
        public double MAE
        {
            get => this.mae;
            set
            {
                this.mae = value;
                NotifyOfPropertyChange(() => MAE);
            }
        }

        /// <summary>
        /// The Root-Mean-Square-Error of the algorithm
        /// </summary>
        private double rmse = 0;
        public double RMSE
        {
            get => this.rmse;
            set
            {
                this.rmse = value;
                NotifyOfPropertyChange(() => RMSE);
            }
        }

        /// <summary>
        /// The Frobenius norm of a simulation algorithm
        /// </summary>
        private double frobeniusNorm = 0;
        public double FrobeniusNorm
        {
            get => this.frobeniusNorm;
            set
            {
                this.frobeniusNorm = value;
                NotifyOfPropertyChange(() => FrobeniusNorm);
            }
        }

        /// <summary>
        /// Number of points that should be removed from the original data set before cross validation
        /// </summary>
        private int crossValidationRemovePointCount = 1;
        public int CrossValidationRemovePointCount
        {
            get => this.crossValidationRemovePointCount;
            set
            {
                this.crossValidationRemovePointCount = value;
                NotifyOfPropertyChange(() => CrossValidationRemovePointCount);
            }
        }

        /// <summary>
        /// Number of points that should be removed from the original data set before cross validation
        /// </summary>
        private int numerOfSwaps = 1000;
        public int NumberOfSwaps
        {
            get => this.numerOfSwaps;
            set
            {
                this.numerOfSwaps = value;
                NotifyOfPropertyChange(() => NumberOfSwaps);
            }
        }

        /// <summary>
        /// The feature to be interpolated
        /// </summary>
        private InterpolationFeature interpolationFeature = InterpolationFeature.Value;
        public InterpolationFeature InterpolationFeature
        {
            get => this.interpolationFeature;
            set
            {
                this.interpolationFeature = value;
                NotifyOfPropertyChange(() => InterpolationFeature);
            }
        }

        #region Visibility properties

        public bool IsIDW => this.InterpolationMethod == GeostatisticalInterpolationMethod.IDW;
        public bool IsUniversalKriging => this.InterpolationMethod == GeostatisticalInterpolationMethod.UniversalKriging;

        #endregion

        /// <summary>
        /// Selected polynomial
        /// </summary>
        private Polynomial selectedPolynomial = new Polynomial();
        public Polynomial SelectedPolynomial
        {
            get => this.selectedPolynomial;
            set
            {
                this.selectedPolynomial = value;
                NotifyOfPropertyChange(() => SelectedPolynomial);
            }
        }

        /// <summary>
        /// Spatial function used for universal kriging
        /// </summary>
        private BindableCollection<Polynomial> spatialFunction = new BindableCollection<Polynomial>();
        public BindableCollection<Polynomial> SpatialFunction
        {
            get => this.spatialFunction;
            set
            {
                this.spatialFunction = value;
                NotifyOfPropertyChange(() => SpatialFunction);
            }
        }

        /// <summary>
        /// The type of drift that is used for universal kriging
        /// </summary>
        private Drift driftType = Drift.SpatialFunction;
        public Drift DriftType
        {
            get => this.driftType;
            set
            {
                this.driftType = value;
                NotifyOfPropertyChange(() => DriftType);
            }
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with data set
        /// </summary>
        /// <param name="locationValues"></param>
        public SpatialInterpolationHelper(BindableCollection<KeyValuePair<string, DataTable>> locationValues,
            VariogramHelper _vh,
            int binsX = 20,
            int binsY = 20,
            int binsZ = 20,
            DiscretizationMethod _discretizationMethod = DiscretizationMethod.Hexahedral)
        {
            BinsX = binsX;
            BinsY = binsY;
            BinsZ = binsZ;
            DiscretizationMethod = _discretizationMethod;

            Vh = _vh;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="locationValues"></param>
        public SpatialInterpolationHelper() : base()
        {

        }

        #endregion

        #region Public methods

        /// <summary>
        /// Computing the interpolation
        /// </summary>
        public async Task<Mesh> ComputeInterpolation()
        {
            DiscretizedLocationValues = new Mesh();
            Residuals = new Mesh();
            OriginalLocationValues.Clear();

            //Adding selected source meshes to a data collection
            foreach (var d in SelectedMeasPoints)
            {
                try
                {
                    OriginalLocationValues.AddRange(new BindableCollection<LocationTimeValue>(d.Data.AsEnumerable()
                        .Select(x => new LocationTimeValue()
                        {
                            Value = new List<double>() { (x.Field<double?>(0) == -9999 || x.Field<double?>(0) == -999999 || x.Field<double?>(0) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(0)), 0 },
                            X = (x.Field<double?>(1) == -9999 || x.Field<double?>(1) == -999999 || x.Field<double?>(1) == 9999999 || Double.IsNaN(Convert.ToDouble(x.Field<double?>(1)))) ? 0 : Convert.ToDouble(x.Field<double?>(1)),
                            Y = (x.Field<double?>(2) == -9999 || x.Field<double?>(2) == -999999 || x.Field<double?>(2) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(2)),
                            Z = (x.Field<double?>(3) == -9999 || x.Field<double?>(3) == -999999 || x.Field<double?>(3) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(3)),
                            Name = d.Name
                        }).ToList()));
                }
                catch
                {
                    continue;
                }
            }

            //Creating a new mesh for the interpolation
            try
            {
                if (SelectedInterpolationMeasPoints[0].Vertices.Count() == 0)
                    DiscretizedLocationValues.Vertices.AddRange(new BindableCollection<LocationTimeValue>(SelectedInterpolationMeasPoints[0].Data.AsEnumerable()
                        .Select(x => new LocationTimeValue()
                        {
                            Value = new List<double> { 0, 0 },
                            X = (x.Field<double?>(1) == -9999 || x.Field<double?>(1) == -999999 || x.Field<double?>(1) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(1)),
                            Y = (x.Field<double?>(2) == -9999 || x.Field<double?>(2) == -999999 || x.Field<double?>(2) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(2)),
                            Z = (x.Field<double?>(3) == -9999 || x.Field<double?>(3) == -999999 || x.Field<double?>(3) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(3))
                        }).ToList()));
                else
                {
                    DiscretizedLocationValues.Vertices = new System.Collections.ObjectModel.ObservableCollection<LocationTimeValue>(SelectedInterpolationMeasPoints[0].Vertices.Select(x =>
                    new LocationTimeValue()
                    {
                        X = x.X,
                        Y = x.Y,
                        Z = x.Z,
                        Value = new List<double>() { 0, 0 },
                        MeshIndex = x.MeshIndex,
                        IsActive = x.IsActive,
                        IsDiscretized = x.IsDiscretized,
                        Brush = x.Brush,
                        Date = x.Date,
                        Name = x.Name,
                        Neighbors = x.Neighbors,
                        Geographic = x.Geographic,
                        IsExterior = x.IsExterior
                    }).ToList());

                }



            }
            catch
            {

            }

            //Performing the interpolation
            CommandHelper ch = new CommandHelper();
            await Task.WhenAll(ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsComputing, async () =>
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                try
                {
                    string type = "";
                    Status = 0;

                    switch (InterpolationMethod)
                    {
                        case GeostatisticalInterpolationMethod.IDW:
                            Task.WaitAll(ComputeIDW());
                            type = "IDW interpolation";
                            break;
                        case GeostatisticalInterpolationMethod.OrdinaryKriging:
                            Task.WaitAll(ComputeOrdinaryKriging());
                            type = "OK interpolation";
                            break;
                        case GeostatisticalInterpolationMethod.SimpleKriging:
                            Task.WaitAll(ComputeSimpleKriging());
                            type = "SK interpolation";
                            break;
                        case GeostatisticalInterpolationMethod.UniversalKriging:
                            Task.WaitAll(ComputeUniversalKriging());
                            type = "UK interpolation";
                            break;
                        case GeostatisticalInterpolationMethod.KrigingWithExternalDrift:
                            break;
                        case GeostatisticalInterpolationMethod.SimulatedAnnealing:
                            Task.WaitAll(ComputeSimulatedAnnealing());
                            break;
                        case GeostatisticalInterpolationMethod.SequentialGaussianSimulation:
                            Task.WaitAll(ComputeSequentialGaussianSimulation());
                            Task.WaitAll(ComputeSequentialGaussianSimulationCrossValidation());
                            type = "Sequential Gaussian Simulation";
                            break;
                    }
                }
                catch
                {

                }
                finally
                {
                    IsCancelled = false;
                    stopWatch.Stop();
                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = stopWatch.Elapsed;

                    ComputationTime = ts.TotalSeconds;
                    Status = 0;
                }
            }));

            try
            {
                //Adding interpolated values and variances to the original data set
                DiscretizedLocationValues.Name = "Interpolated mesh";
                DiscretizedLocationValues.Data =
                    CollectionHelper.ConvertTo<Tuple<double, double, double, double, DateTime, string>>(
                        new List<Tuple<double, double, double, double, DateTime, string>>(DiscretizedLocationValues.Vertices.Select(a =>
                           new Tuple<double, double, double, double, DateTime, string>(
                               a.Value[0],
                               a.X,
                               a.Y,
                               a.Z,
                               a.Date,
                               a.Name
                               )).ToList()));

                DiscretizedLocationValues.Data.TableName = "Interpolated mesh";

                //Creating the mesh cells
                if (SelectedInterpolationMeasPoints[0].MeshCellType == MeshCellType.Hexahedral || SelectedInterpolationMeasPoints[0].MeshCellType == MeshCellType.Tetrahedal)
                    DiscretizedLocationValues.CellsFromPointCloud();

                //Defining mesh cell type
                DiscretizedLocationValues.MeshCellType = SelectedInterpolationMeasPoints[0].MeshCellType;
                DiscretizedLocationValues.Dimensionality = SelectedInterpolationMeasPoints[0].Dimensionality;
                DiscretizedLocationValues.Faces = SelectedInterpolationMeasPoints[0].Faces;

                //Adding interpolated values and variances to the original data set
                Residuals.Name = "Residuals";
                Residuals.Data =
                    CollectionHelper.ConvertTo<Tuple<double, double, double, double, DateTime, string>>(
                        new List<Tuple<double, double, double, double, DateTime, string>>(Residuals.Vertices.Select(a =>
                           new Tuple<double, double, double, double, DateTime, string>(
                               a.Value[0],
                               a.X,
                               a.Y,
                               a.Z,
                               a.Date,
                               a.Name
                               )).ToList()));

                return DiscretizedLocationValues;
            }
            catch
            {
                return new Mesh();
            }
        }

        /// <summary>
        /// Spatially interpolating the data set
        /// </summary>
        public async Task<Mesh> ComputeSpatialOperation()
        {
            CommandHelper ch = new CommandHelper();

            OriginalLocationValues.Clear();

            await Task.WhenAll(ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsComputing, async () =>
            {
                ComputeOperation();
            }));

            //Adding interpolated values and variances to the original data set
            DiscretizedLocationValues.Name = "Joined mesh";

            return DiscretizedLocationValues;
        }

        /// <summary>
        /// Spatially interpolating the data set
        /// </summary>
        public async Task<Mesh> DiscretizeDataSet()
        {
            CommandHelper ch = new CommandHelper();

            OriginalLocationValues.Clear();

            await Task.WhenAll(ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsComputing, async () =>
            {
                foreach (var d in SelectedMeasPoints)
                {
                    try
                    {
                        if (d.Vertices.Count() == 0)
                            OriginalLocationValues.AddRange(new BindableCollection<LocationTimeValue>(d.Data.AsEnumerable()
                                .Select(x => new LocationTimeValue()
                                {
                                    Value = new List<double>() { (x.Field<double?>(0) == -9999 || x.Field<double?>(0) == -999999 || x.Field<double?>(0) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(0)), 0 },
                                    X = (x.Field<double?>(1) == -9999 || x.Field<double?>(1) == -999999 || x.Field<double?>(1) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(1)),
                                    Y = (x.Field<double?>(2) == -9999 || x.Field<double?>(2) == -999999 || x.Field<double?>(2) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(2)),
                                    Z = (x.Field<double?>(3) == -9999 || x.Field<double?>(3) == -999999 || x.Field<double?>(3) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(3))
                                }).ToList()));
                        else
                            OriginalLocationValues.AddRange(d.Vertices);
                    }
                    catch
                    {
                        continue;
                    }
                }

                try
                {
                    if (OriginalLocationValues.Count < 1)
                    {
                        DiscretizedLocationValues = new Mesh();
                    }

                    DiscretizedLocationValues = new Mesh();

                    ComputeDiscretization();
                }
                catch
                {

                }
            }));

            //Adding interpolated values and variances to the original data set
            DiscretizedLocationValues.Name = "Discretized mesh";
            DiscretizedLocationValues.Data =
                CollectionHelper.ConvertTo<Tuple<double, double, double, double, DateTime, string>>(
                    new List<Tuple<double, double, double, double, DateTime, string>>(DiscretizedLocationValues.Vertices.Select(a =>
                       new Tuple<double, double, double, double, DateTime, string>(
                           a.Value[0],
                           a.X,
                           a.Y,
                           a.Z,
                           a.Date,
                           a.Name
                           )).ToList()));

            DiscretizedLocationValues.Data.TableName = "DiscretizedMesh";

            return DiscretizedLocationValues;
        }

        /// <summary>
        /// Computing the interpolation
        /// </summary>
        public async Task ComputeCategorization()
        {
            DiscretizedLocationValues = new Mesh();
            OriginalLocationValues.Clear();

            //If we have an indicator variogram
            List<int> indicatorValues = new List<int>();
            string[] classes = new string[] { };


            if (Vh.IsIndicator)
            {
                try
                {
                    if (SelectedInterpolationMeasPoints[0].Vertices.Count() == 0)
                        DiscretizedLocationValues.Vertices.AddRange(new BindableCollection<LocationTimeValue>(SelectedInterpolationMeasPoints[0].Data.AsEnumerable()
                            .Select(x => new LocationTimeValue()
                            {
                                Value = new List<double> { 0, 0 },
                                X = (x.Field<double?>(1) == -9999 || x.Field<double?>(1) == -999999 || x.Field<double?>(1) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(1)),
                                Y = (x.Field<double?>(2) == -9999 || x.Field<double?>(2) == -999999 || x.Field<double?>(2) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(2)),
                                Z = (x.Field<double?>(3) == -9999 || x.Field<double?>(3) == -999999 || x.Field<double?>(3) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(3))
                            }).ToList()));
                    else
                    {
                        DiscretizedLocationValues.Vertices = new System.Collections.ObjectModel.ObservableCollection<LocationTimeValue>(SelectedInterpolationMeasPoints[0].Vertices.Select(x =>
                        new LocationTimeValue()
                        {
                            X = x.X,
                            Y = x.Y,
                            Z = x.Z,
                            Value = new List<double>() { 0, 0 },
                            MeshIndex = x.MeshIndex,
                            IsActive = x.IsActive,
                            IsDiscretized = x.IsDiscretized,
                            Brush = x.Brush,
                            Date = x.Date,
                            Name = x.Name,
                            Neighbors = x.Neighbors,
                            Geographic = x.Geographic,
                            IsExterior = x.IsExterior
                        }).ToList());

                    }

                    classes = SelectedMeasPoints.Select(x => x.Name).GroupBy(g => g).Select(f => f.Key).ToArray();

                    for (int i = 0; i < classes.Length; i++)
                    {
                        indicatorValues.Add(i);
                    }

                }
                catch
                {

                }
            }

            for (int i = 0; i < SelectedMeasPoints.Count(); i++)
            {
                try
                {
                    OriginalLocationValues.AddRange(new BindableCollection<LocationTimeValue>(SelectedMeasPoints[i].Data.AsEnumerable()
                        .Select(x => new LocationTimeValue()
                        {
                            Value = new List<double>() { (x.Field<double?>(0) == -9999 || x.Field<double?>(0) == -999999 || x.Field<double?>(0) == 9999999) ? 0 : CategorizationMethod != CategorizationMethod.IndicatorKriging ? Convert.ToDouble(x.Field<double?>(0)) : Convert.ToDouble(i), 0 },
                            X = (x.Field<double?>(1) == -9999 || x.Field<double?>(1) == -999999 || x.Field<double?>(1) == 9999999 || Double.IsNaN(Convert.ToDouble(x.Field<double?>(1)))) ? 0 : Convert.ToDouble(x.Field<double?>(1)),
                            Y = (x.Field<double?>(2) == -9999 || x.Field<double?>(2) == -999999 || x.Field<double?>(2) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(2)),
                            Z = (x.Field<double?>(3) == -9999 || x.Field<double?>(3) == -999999 || x.Field<double?>(3) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(3)),
                            Name = SelectedMeasPoints[i].Name
                        }).ToList()));
                }
                catch
                {
                    continue;
                }
            }

            //Running the categorization
            CommandHelper ch = new CommandHelper();
            await Task.WhenAll(ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsComputing, async () =>
            {
                try
                {
                    switch (CategorizationMethod)
                    {
                        case CategorizationMethod.NearestNeighbor:
                            Task.WaitAll(ComputeNearestNeighborClassification());
                            break;
                        case CategorizationMethod.IndicatorKriging:
                            Task.WaitAll(ComputeSimpleKriging());
                            DiscretizedLocationValues.Vertices.ForEach(x =>
                            {
                                x.Value[0] = Math.Round(x.Value[0], MidpointRounding.AwayFromZero);
                                if (x.Value[0] < indicatorValues[0])
                                    x.Value[0] = indicatorValues[0];
                                if (x.Value[0] > indicatorValues[indicatorValues.Count()])
                                    x.Value[0] = indicatorValues[indicatorValues.Count()];

                                x.Name = classes[Convert.ToInt32(x.Value[0])];

                            });

                            //Creating a new data set
                            DiscretizedLocationValues.Vertices = new System.Collections.ObjectModel.ObservableCollection<LocationTimeValue>(SelectedInterpolationMeasPoints[0].Vertices.Select(x =>
                               new LocationTimeValue()
                               {
                                   X = x.X,
                                   Y = x.Y,
                                   Z = x.Z,
                                   Value = new List<double>() { 0, 0 },
                                   MeshIndex = x.MeshIndex,
                                   IsActive = x.IsActive,
                                   IsDiscretized = x.IsDiscretized,
                                   Brush = x.Brush,
                                   Date = x.Date,
                                   Name = DiscretizedLocationValues.Vertices.Where(y => y.X == x.X && y.Y == x.Y && y.Z == x.Z).First().Name,
                                   Neighbors = x.Neighbors,
                                   Geographic = x.Geographic,
                                   IsExterior = x.IsExterior
                               }).ToList());

                            break;
                    }

                    SelectedInterpolationMeasPoints[0].Data =
                CollectionHelper.ConvertTo<Tuple<double, double, double, double, DateTime, string>>(
                    new List<Tuple<double, double, double, double, DateTime, string>>(SelectedInterpolationMeasPoints[0].Vertices.Select(a =>
                       new Tuple<double, double, double, double, DateTime, string>(
                           a.Value[0],
                           a.X,
                           a.Y,
                           a.Z,
                           a.Date,
                           a.Name
                           )).ToList()));
                }
                catch
                {

                }
            }));
        }

        /// <summary>
        /// Categorization based on the nearest neighbor algorithm
        /// </summary>
        /// <returns></returns>
        private async Task ComputeNearestNeighborClassification()
        {
            try
            {
                //for (int i = 0; i < DiscretizedLocationValues.Vertices.Count(); i++)
                Parallel.For((int)0, (int)SelectedInterpolationMeasPoints[0].Vertices.Count(), k =>
                {
                    string key = "";

                    LocationTimeValue loc = OriginalLocationValues.OrderBy(x => GeographyHelper.EuclideanDistance(x, SelectedInterpolationMeasPoints[0].Vertices[k])).First();

                    key = loc.Name;

                    lock (sync)
                    {
                        SelectedInterpolationMeasPoints[0].Vertices[k].Name = key;
                    }

                });
            }
            catch
            {
                return;
            }
        }


        /// <summary>
        /// Computing the IDW data matrix
        /// </summary>
        public async Task ComputeIDW()
        {
            try
            {
                double globalMean = 0;

                //Calculating global mean for error treatment
                switch (InterpolationFeature)
                {
                    case InterpolationFeature.Value:
                        globalMean = OriginalLocationValues.ToArray().Average(x => x.Value[0]);
                        break;
                    case InterpolationFeature.Longitude:
                        globalMean = OriginalLocationValues.ToArray().Average(x => x.X);
                        break;
                    case InterpolationFeature.Latitutde:
                        globalMean = OriginalLocationValues.ToArray().Average(x => x.Y);
                        break;
                    case InterpolationFeature.Elevation:
                        globalMean = OriginalLocationValues.ToArray().Average(x => x.Z);
                        break;
                }

                for (int i = 0; i < DiscretizedLocationValues.Vertices.Count(); i++)
                //Parallel.For((int)0, (int)DiscretizedLocationValues.Vertices.Count(), k =>
                {
                    if (i != 0 && i % 100 == 0)
                        Status = (Convert.ToDouble(i) / Convert.ToDouble(DiscretizedLocationValues.Vertices.Count())) * 100;
                    else if (IsCancelled == true)
                        break;

                    try
                    {
                        //int i = Convert.ToInt32(k);
                        double value = 0;
                        double weightSum = 0;

                        List<LocationTimeValue> includedPoints = new List<LocationTimeValue>();
                        includedPoints.AddRange(await SpatialNeighborhoodHelper.SearchByDistance(OriginalLocationValues, DiscretizedLocationValues.Vertices[i], Vh.RangeX, Vh.RangeY, Vh.RangeZ, Vh.Azimuth, Vh.Dip, Vh.Plunge));

                        if (includedPoints == null || includedPoints.Count() == 0)
                            includedPoints = OriginalLocationValues.Where(x => ShouldTargetVertexFitSourceGridName ? DiscretizedLocationValues.Vertices[i].Name == x.Name : 0 == 0).Take(MaximumNeighborCount).ToList();

                        if (includedPoints.Count > MaximumNeighborCount)
                            includedPoints = includedPoints.OrderBy(x => DistanceType == DistanceType.Euclidean ? DiscretizedLocationValues.Vertices[i].GetEuclideanDistance(x) : DiscretizedLocationValues.Vertices[i].GetDijkstraDistance(DiscretizedLocationValues, x)).Take(MaximumNeighborCount).ToList();

                        //Weights for the interpolation
                        double[] weights = new double[includedPoints.Count()];

                        //Calculating IDW values
                        for (int j = 0; j < includedPoints.Count(); j++)
                        {
                            double dist = DistanceType == DistanceType.Euclidean ? DiscretizedLocationValues.Vertices[i].GetEuclideanDistance(includedPoints[j]) : DiscretizedLocationValues.Vertices[i].GetDijkstraDistance(DiscretizedLocationValues, includedPoints[j]);

                            if (dist != 0)
                            {
                                //Summing up the weights
                                weightSum += 1 / Math.Pow(dist, Power);

                                //Calculating the value
                                switch (InterpolationFeature)
                                {
                                    case InterpolationFeature.Value:
                                        //Increasing the value by the constrain value and it's weight
                                        value += Convert.ToDouble(includedPoints[j].Value[0]) / Math.Pow(dist, Power);
                                        break;
                                    case InterpolationFeature.Longitude:
                                        value += Convert.ToDouble(includedPoints[j].X) / Math.Pow(dist, Power);
                                        break;
                                    case InterpolationFeature.Latitutde:
                                        value += Convert.ToDouble(includedPoints[j].Y) / Math.Pow(dist, Power);
                                        break;
                                    case InterpolationFeature.Elevation:
                                        value += Convert.ToDouble(includedPoints[j].Z) / Math.Pow(dist, Power);
                                        break;
                                }

                                weights[j] = 1 / Math.Pow(dist, Power);
                            }
                            else
                            {
                                //Calculating global mean for error treatment
                                switch (InterpolationFeature)
                                {
                                    case InterpolationFeature.Value:
                                        //Increasing the value by the constrain value and it's weight
                                        value = includedPoints[j].Value[0];
                                        break;
                                    case InterpolationFeature.Longitude:
                                        value = includedPoints[j].X;
                                        break;
                                    case InterpolationFeature.Latitutde:
                                        value = includedPoints[j].Y;
                                        break;
                                    case InterpolationFeature.Elevation:
                                        value = includedPoints[j].Z;
                                        break;
                                }
                                break;
                            }
                        }

                        if (Double.IsNaN(value / weightSum) || Double.IsInfinity(value / weightSum))
                        {
                            value = globalMean;
                            weightSum = 1;
                        }

                        lock (sync)
                        {
                            switch (InterpolationFeature)
                            {
                                case InterpolationFeature.Value:
                                    //Increasing the value by the constrain value and it's weight
                                    DiscretizedLocationValues.Vertices[i].Value[0] = value / weightSum;
                                    break;
                                case InterpolationFeature.Longitude:
                                    DiscretizedLocationValues.Vertices[i].X = value / weightSum;
                                    break;
                                case InterpolationFeature.Latitutde:
                                    DiscretizedLocationValues.Vertices[i].Y = value / weightSum;
                                    break;
                                case InterpolationFeature.Elevation:
                                    DiscretizedLocationValues.Vertices[i].Z = value / weightSum;
                                    break;
                            };
                        }
                    }
                    catch
                    {

                    }
                }

                //If wanted, transforming the interpolated values to the original distribution range
                if (TransformToOriginalDistribution)
                {
                    double zMax = DiscretizedLocationValues.Vertices.Select(x => x.Value[0]).Max();
                    double zMin = DiscretizedLocationValues.Vertices.Select(x => x.Value[0]).Min();
                    double zAverage = DiscretizedLocationValues.Vertices.Select(x => x.Value[0]).Average();

                    double tMax = OriginalLocationValues.Select(x => x.Value[0]).Max();
                    double tMin = OriginalLocationValues.Select(x => x.Value[0]).Min();

                    for (int i = 0; i < DiscretizedLocationValues.Vertices.Count(); i++)
                    {
                        DiscretizedLocationValues.Vertices[i].Value[0] = ((DiscretizedLocationValues.Vertices[i].Value[0] - zMin) / (zMax - zMin)) * (tMax - tMin) + tMin;
                    }
                }

                if ((MaximumNeighborCount >= CrossValidationRemovePointCount))
                    CalculateIDWCrossValidation();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Calculate the Root-Mean-Squared-Error of the interpolation
        /// </summary>
        private async Task CalculateIDWCrossValidation()
        {
            double mae = 0;
            double rmseSum = 0;
            List<List<double>> pointPairs = new List<List<double>>();

            try
            {
                //Calculating the RMSE
                for (int i = 0; i < OriginalLocationValues.Count(); i++)
                //Parallel.For(0, OriginalLocationValues.Count(), i =>
                {
                    double weightSum = 0;
                    double value = 0;

                    List<LocationTimeValue> validationPoints = new List<LocationTimeValue>();
                    validationPoints.AddRange((await SpatialNeighborhoodHelper.SearchByDistance(OriginalLocationValues, OriginalLocationValues[i], Vh.RangeX, Vh.RangeY, Vh.RangeZ, Vh.Azimuth, Vh.Dip, Vh.Plunge)).ToList());

                    if (validationPoints == null || validationPoints.Count() == 0 || validationPoints.Count() > MaximumNeighborCount)
                        validationPoints = validationPoints.Where(x => ShouldTargetVertexFitSourceGridName ? DiscretizedLocationValues.Vertices[i].Name == x.Name : 0 == 0).Take(MaximumNeighborCount).ToList();

                    while (validationPoints.Contains(OriginalLocationValues[i]))
                    {
                        validationPoints.Remove(OriginalLocationValues[i]);

                        int l = rnd.Next(0, OriginalLocationValues.Count());

                        validationPoints.Add(OriginalLocationValues[l]);
                    }

                    for (int k = 0; k < CrossValidationRemovePointCount; k++)
                    {
                        int l = rnd.Next(0, validationPoints.Count());
                        validationPoints.RemoveAt(l);
                    }

                    for (int j = 0; j < validationPoints.Count(); j++)
                    {
                        double dist = OriginalLocationValues[i].GetEuclideanDistance(validationPoints[j]);

                        if (dist != 0)
                        {
                            //Summing up the weights
                            weightSum += 1 / Math.Pow(dist, Power);

                            //Calculating global mean for error treatment
                            switch (InterpolationFeature)
                            {
                                case InterpolationFeature.Value:
                                    //Increasing the value by the constrain value and it's weight
                                    value += Convert.ToDouble(validationPoints[j].Value[0]) / Math.Pow(dist, Power);
                                    break;
                                case InterpolationFeature.Longitude:
                                    value += Convert.ToDouble(validationPoints[j].X) / Math.Pow(dist, Power);
                                    break;
                                case InterpolationFeature.Latitutde:
                                    value += Convert.ToDouble(validationPoints[j].Y) / Math.Pow(dist, Power);
                                    break;
                                case InterpolationFeature.Elevation:
                                    value += Convert.ToDouble(validationPoints[j].Z) / Math.Pow(dist, Power);
                                    break;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (weightSum != 0)
                    {
                        double interpolatedValue = (value / weightSum);
                        pointPairs.Add(new List<double>() { OriginalLocationValues[i].Value[0], interpolatedValue });
                        Residuals.Vertices.Add(new LocationTimeValue(OriginalLocationValues[i]));
                        Residuals.Vertices[i].Value[0] = OriginalLocationValues[i].Value[0] - interpolatedValue;
                    }

                    validationPoints = OriginalLocationValues.ToList();
                };

                if (TransformToOriginalDistribution)
                {
                    double zMax = pointPairs.Select(x => x[1]).Max();
                    double zMin = pointPairs.Select(x => x[1]).Min();
                    double zAverage = pointPairs.Select(x => x[1]).Average();

                    double tMax = pointPairs.Select(x => x[0]).Max();
                    double tMin = pointPairs.Select(x => x[0]).Min();

                    for (int i = 0; i < pointPairs.Count(); i++)
                    {
                        pointPairs[i][1] = ((pointPairs[i][1] - zMin) / (zMax - zMin)) * (tMax - tMin) + tMin;
                        Residuals.Vertices[i].Value[0] = Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                    }
                }

                for(int i = 0; i<pointPairs.Count();i++)
                {
                    switch (InterpolationFeature)
                    {
                        case InterpolationFeature.Value:
                            //Increasing the value by the constrain value and it's weight
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Longitude:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Latitutde:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Elevation:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                    }
                }

                MAE = mae / OriginalLocationValues.Count();
                RMSE = Math.Sqrt(rmseSum / OriginalLocationValues.Count());
            }
            catch
            {
                RMSE = Double.NaN;
            }
        }

        /// <summary>
        /// Kriging a dataset of location values
        /// </summary>
        /// <param name="locVal">List of location values</param>
        /// <param name="steps">discretization size</param>
        /// <returns></returns>
        public async Task ComputeOrdinaryKriging()
        {
            try
            {

                //Performing the kriging calculation for each point
                //Parallel.For(0, DiscretizedLocationValues.Vertices.Count(), j =>
                for (int j = 0; j < DiscretizedLocationValues.Vertices.Count(); j++)
                {
                    if (j != 0 && j % 100 == 0)
                        Status = (Convert.ToDouble(j) / Convert.ToDouble(DiscretizedLocationValues.Vertices.Count())) * 100;
                    else if (IsCancelled == true)
                        break;

                    DiscretizedLocationValues.Vertices[j].Value[0] = 0;

                    List<LocationTimeValue> includedPoints = new List<LocationTimeValue>();
                    includedPoints.AddRange(await SpatialNeighborhoodHelper.SearchByDistance(OriginalLocationValues, DiscretizedLocationValues.Vertices[j], Vh.RangeX, Vh.RangeY, Vh.RangeZ, Vh.Azimuth, Vh.Dip, Vh.Plunge));

                    if (includedPoints == null || includedPoints.Count() == 0 || includedPoints.Count() > MaximumNeighborCount)
                        includedPoints = OriginalLocationValues.Where(x => ShouldTargetVertexFitSourceGridName ? DiscretizedLocationValues.Vertices[j].Name == x.Name : 0 == 0).OrderBy(x => x.GetEuclideanDistance(DiscretizedLocationValues.Vertices[j])).Take(MaximumNeighborCount).ToList();

                    double[] semivarianceVector = new double[includedPoints.Count() + 1];

                    //Semivariance matrix for the ordinary kriging system
                    double[,] semivarianceMatrix = new double[includedPoints.Count() + 1, includedPoints.Count() + 1];

                    //Calculating the semivariance matrix based on the variogram model
                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        for (int k = 0; k < includedPoints.Count(); k++)
                        {
                            semivarianceMatrix[i, k] = Vh.CalculateSemivariance(includedPoints[i], includedPoints[k]);
                            if (i == k && IncludeErrorVariance)
                                semivarianceMatrix[i, k] += includedPoints[i].Value[0]*ErrorVariance;

                        }
                    }

                    //Calculating the semivariance matrix based on the variogram model
                    for (int i = 0; i < includedPoints.Count() + 1; i++)
                    {

                        if (i != includedPoints.Count())
                        {
                            semivarianceMatrix[i, includedPoints.Count()] = 1;
                            semivarianceMatrix[includedPoints.Count(), i] = 1;
                        }
                        else
                        {
                            semivarianceMatrix[i, i] = 0;
                        }
                    }

                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        semivarianceVector[i] = Vh.CalculateSemivariance(DiscretizedLocationValues.Vertices[j], includedPoints[i]);
                    }

                    semivarianceVector[includedPoints.Count()] = 1;

                    //Calculating the weights of the original data values on the interpolated value
                    double[] weights = Matrix.Solve(semivarianceMatrix, semivarianceVector, true);

                    double weightSum = weights.Sum();

                    //Calculating the value at the point which is the sum of the weights times values in the original data table
                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        //lock (sync)
                        //{
                        switch (InterpolationFeature)
                        {
                            case InterpolationFeature.Value:
                                DiscretizedLocationValues.Vertices[j].Value[0] += weights[i] * includedPoints[i].Value[0];
                                break;
                            case InterpolationFeature.Longitude:
                                DiscretizedLocationValues.Vertices[j].Value[0] += weights[i] * includedPoints[i].X;
                                break;
                            case InterpolationFeature.Latitutde:
                                DiscretizedLocationValues.Vertices[j].Value[0] += weights[i] * includedPoints[i].Y;
                                break;
                            case InterpolationFeature.Elevation:
                                DiscretizedLocationValues.Vertices[j].Value[0] += weights[i] * includedPoints[i].Z;
                                break;
                        }

                        DiscretizedLocationValues.Vertices[j].Value[1] += weights[i] * Vh.CalculateSemivariance(includedPoints[i], DiscretizedLocationValues.Vertices[j]);
                    }

                    DiscretizedLocationValues.Vertices[j].Value[1] = weights[includedPoints.Count()] - Vh.CalculateSemivariance(DiscretizedLocationValues.Vertices[j], DiscretizedLocationValues.Vertices[j]) + DiscretizedLocationValues.Vertices[j].Value[1];
                };

                //If wanted, transforming the interpolated values to the original distribution range
                if (TransformToOriginalDistribution)
                {
                    double zMax = DiscretizedLocationValues.Vertices.Select(x => x.Value[0]).Max();
                    double zMin = DiscretizedLocationValues.Vertices.Select(x => x.Value[0]).Min();
                    double zAverage = DiscretizedLocationValues.Vertices.Select(x => x.Value[0]).Average();

                    double tMax = OriginalLocationValues.Select(x => x.Value[0]).Max();
                    double tMin = OriginalLocationValues.Select(x => x.Value[0]).Min();

                    for (int i = 0; i < DiscretizedLocationValues.Vertices.Count(); i++)
                    {
                        DiscretizedLocationValues.Vertices[i].Value[0] = ((DiscretizedLocationValues.Vertices[i].Value[0] - zMin) / (zMax - zMin)) * (tMax - tMin) + tMin;
                    }
                }

            }
            catch
            {
                return;
            }
            try
            {
                //Computing the cross validation
                if (MaximumNeighborCount > CrossValidationRemovePointCount)
                    ComputeOrdinaryKrigingCrossValidation().AsResult();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Computing the p value cross validation for ordinary kriging
        /// </summary>
        /// <returns></returns>
        public async Task ComputeOrdinaryKrigingCrossValidation()
        {
            double rmseSum = 0;
            double mae = 0;
            List<List<double>> pointPairs = new List<List<double>>();

            //Semivariance matrix for RMSE
            double[,] semivarianceMatrixRMSE = new double[,] { };

            //Semivariance vector for RMSE
            double[] semivarianceVector2 = new double[] { };

            try
            {
                //Performing the kriging calculation for each point
                //Parallel.For(0, OriginalLocationValues.Count(), k =>
                for (int k = 0; k < OriginalLocationValues.Count(); k++)
                {

                    List<LocationTimeValue> includedPoints = new List<LocationTimeValue>();
                    includedPoints.AddRange(await SpatialNeighborhoodHelper.SearchByDistance(OriginalLocationValues, DiscretizedLocationValues.Vertices[k], Vh.RangeX, Vh.RangeY, Vh.RangeZ, Vh.Azimuth, Vh.Dip, Vh.Plunge));

                    if (includedPoints.Count > MaximumNeighborCount)
                        includedPoints = includedPoints.OrderBy(x => x.GetEuclideanDistance(OriginalLocationValues[k])).Take(MaximumNeighborCount).ToList();


                    //Semivariance matrix for the ordinary kriging system
                    double[,] semivarianceMatrix = new double[includedPoints.Count() + 1, includedPoints.Count() + 1];

                    //Buffer of the original data values where the number of points defined for validation will be removed before kriging 
                    BindableCollection<LocationTimeValue> validationPoints = new BindableCollection<LocationTimeValue>(includedPoints.ToList());

                    //Calculating the semivariance matrix based on the variogram model
                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        for (int j = 0; j < includedPoints.Count(); j++)
                        {
                            semivarianceMatrix[i, j] = Vh.CalculateSemivariance(includedPoints[i], includedPoints[j]);
                            if (i == j && IncludeErrorVariance)
                                semivarianceMatrix[i, k] += includedPoints[i].Value[0] * ErrorVariance;
                        }
                    }

                    //Calculating the semivariance matrix based on the variogram model
                    for (int i = 0; i < includedPoints.Count() + 1; i++)
                    {

                        if (i != includedPoints.Count())
                        {
                            semivarianceMatrix[i, includedPoints.Count()] = 1;
                            semivarianceMatrix[includedPoints.Count(), i] = 1;
                        }
                        else
                        {
                            semivarianceMatrix[i, i] = 0;
                        }
                    }

                    semivarianceMatrixRMSE = semivarianceMatrix.Copy();

                    semivarianceVector2 = new double[includedPoints.Count() + 1];

                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        semivarianceVector2[i] = Vh.CalculateSemivariance(OriginalLocationValues[k], includedPoints[i]);
                    }

                    //Semivariance vector
                    semivarianceVector2[includedPoints.Count()] = 1;

                    //Removing the defined number of points from the validation data set
                    if (validationPoints.Count() > CrossValidationRemovePointCount)
                        for (int l = 0; l < CrossValidationRemovePointCount; l++)
                        {
                            int m = rnd.Next(1, validationPoints.Count() - 1);
                            validationPoints.RemoveAt(m);

                            semivarianceMatrixRMSE = ArrayHelper.TrimArray(m, m, semivarianceMatrixRMSE);
                            semivarianceVector2 = semivarianceVector2.Where((val, idx) => idx != m).ToArray();
                        }

                    //Calculating the weights of the original data values on the interpolated value
                    double[] weights = Matrix.Solve(semivarianceMatrixRMSE, semivarianceVector2, true);

                    double weightSum = weights.Sum();

                    double value = 0;

                    //Calculating the value at the point which is the sum of the weights times values in the original data table
                    for (int i = 0; i < validationPoints.Count(); i++)
                    {
                        //Calculating global mean for error treatment
                        switch (InterpolationFeature)
                        {
                            case InterpolationFeature.Value:
                                value += weights[i] * validationPoints[i].Value[0];
                                break;
                            case InterpolationFeature.Longitude:
                                value += weights[i] * validationPoints[i].X;
                                break;
                            case InterpolationFeature.Latitutde:
                                value += weights[i] * validationPoints[i].Y;
                                break;
                            case InterpolationFeature.Elevation:
                                value += weights[i] * validationPoints[i].Z;
                                break;
                        }
                    }

                    pointPairs.Add(new List<double>() { OriginalLocationValues[k].Value[0], value });
                    Residuals.Vertices.Add(new LocationTimeValue(OriginalLocationValues[k]));
                    Residuals.Vertices[k].Value[0] = OriginalLocationValues[k].Value[0] - pointPairs[k][1];
                }

                if (TransformToOriginalDistribution)
                {
                    double zMax = pointPairs.Select(x => x[1]).Max();
                    double zMin = pointPairs.Select(x => x[1]).Min();
                    double zAverage = pointPairs.Select(x => x[1]).Average();

                    double tMax = pointPairs.Select(x => x[0]).Max();
                    double tMin = pointPairs.Select(x => x[0]).Min();

                    for (int i = 0; i < pointPairs.Count(); i++)
                    {
                        pointPairs[i][1] = ((pointPairs[i][1] - zMin) / (zMax - zMin)) * (tMax - tMin) + tMin;
                        Residuals.Vertices[i].Value[0] = Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                    }
                }

                for (int i = 0; i < pointPairs.Count(); i++)
                {
                    switch (InterpolationFeature)
                    {
                        case InterpolationFeature.Value:
                            //Increasing the value by the constrain value and it's weight
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Longitude:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Latitutde:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Elevation:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                    }
                }

                MAE = mae / OriginalLocationValues.Count();
                RMSE = Math.Sqrt(rmseSum / OriginalLocationValues.Count());
            }
            catch
            {

            }
        }

        /// <summary>
        /// Simple Kriging a dataset of location values
        /// </summary>
        /// <param name="locVal">List of location values</param>
        /// <param name="steps">discretization size</param>
        /// <returns></returns>
        public async Task ComputeSimpleKriging()
        {
            if (OriginalLocationValues.Count >= 1)

                try
                {
                    double globalMean = 0;

                    //Calculating global mean for error treatment
                    switch (InterpolationFeature)
                    {
                        case InterpolationFeature.Value:
                            globalMean = OriginalLocationValues.ToArray().Average(x => x.Value[0]);
                            break;
                        case InterpolationFeature.Longitude:
                            globalMean = OriginalLocationValues.ToArray().Average(x => x.X);
                            break;
                        case InterpolationFeature.Latitutde:
                            globalMean = OriginalLocationValues.ToArray().Average(x => x.Y);
                            break;
                        case InterpolationFeature.Elevation:
                            globalMean = OriginalLocationValues.ToArray().Average(x => x.Z);
                            break;
                    }


                    //Performing the kriging calculation for each point
                    //Parallel.For(0, DiscretizedLocationValues.Count(), j =>
                    for (int j = 0; j < DiscretizedLocationValues.Vertices.Count(); j++)
                    {
                        if (j != 0 && j % 100 == 0)
                            Status = (Convert.ToDouble(j) / Convert.ToDouble(DiscretizedLocationValues.Vertices.Count())) * 100;
                        else if (IsCancelled == true)
                            break;

                        DiscretizedLocationValues.Vertices[j].Value[0] = 0;

                        List<LocationTimeValue> includedPoints = new List<LocationTimeValue>();
                        includedPoints.AddRange(await SpatialNeighborhoodHelper.SearchByDistance(OriginalLocationValues, DiscretizedLocationValues.Vertices[j], Vh.RangeX, Vh.RangeY, Vh.RangeZ, Vh.Azimuth, Vh.Dip, Vh.Plunge));

                        if (includedPoints == null || includedPoints.Count() == 0 || includedPoints.Count() != MaximumNeighborCount)
                            includedPoints = OriginalLocationValues.Where(x => ShouldTargetVertexFitSourceGridName ? DiscretizedLocationValues.Vertices[j].Name == x.Name : 0 == 0).OrderBy(x => x.GetEuclideanDistance(DiscretizedLocationValues.Vertices[j])).Take(MaximumNeighborCount).ToList();

                        double mean = includedPoints.Average(x => x.Value[0]);

                        //Covariance vector
                        double[] semivarianceVector = new double[includedPoints.Count()];
                        //Semivariance matrix for the ordinary kriging system
                        double[,] semivarianceMatrix = new double[includedPoints.Count(), includedPoints.Count()];

                        //Calculating the semivariance matrix based on the variogram model
                        for (int i = 0; i < includedPoints.Count(); i++)
                        {
                            for (int k = 0; k < includedPoints.Count(); k++)
                            {
                                semivarianceMatrix[i, k] = Vh.CalculateCovariance(includedPoints[i], includedPoints[k]);

                                if (i == k && IncludeErrorVariance)
                                    semivarianceMatrix[i, k] += includedPoints[i].Value[0] * ErrorVariance;
                            }
                        }

                        for (int i = 0; i < includedPoints.Count(); i++)
                        {
                            semivarianceVector[i] = Vh.CalculateCovariance(DiscretizedLocationValues.Vertices[j], includedPoints[i]);
                        }

                        //Calculating the weights of the original data values on the interpolated value
                        double[] weights = Matrix.Solve(semivarianceMatrix, semivarianceVector, true);
                        
                        double weightSum = weights.Sum();

                        //Calculating the value at the point which is the sum of the weights times values in the original data table
                        for (int i = 0; i < includedPoints.Count(); i++)
                        {

                            //Calculating global mean for error treatment
                            switch (InterpolationFeature)
                            {
                                case InterpolationFeature.Value:
                                    DiscretizedLocationValues.Vertices[j].Value[0] += weights[i] * (includedPoints[i].Value[0] - mean);
                                    break;
                                case InterpolationFeature.Longitude:
                                    DiscretizedLocationValues.Vertices[j].X += weights[i] * (includedPoints[i].X - mean);
                                    break;
                                case InterpolationFeature.Latitutde:
                                    DiscretizedLocationValues.Vertices[j].Y += weights[i] * (includedPoints[i].Y - mean);
                                    break;
                                case InterpolationFeature.Elevation:
                                    DiscretizedLocationValues.Vertices[j].Z += weights[i] * (includedPoints[i].Z - mean);
                                    break;
                            }

                            //Calculating variance
                            DiscretizedLocationValues.Vertices[j].Value[1] += weights[i] * Vh.CalculateCovariance(includedPoints[i], DiscretizedLocationValues.Vertices[j]);

                        }

                        //Calculating global mean for error treatment
                        switch (InterpolationFeature)
                        {
                            case InterpolationFeature.Value:
                                DiscretizedLocationValues.Vertices[j].Value[0] += mean;
                                break;
                            case InterpolationFeature.Longitude:
                                DiscretizedLocationValues.Vertices[j].X += mean;
                                break;
                            case InterpolationFeature.Latitutde:
                                DiscretizedLocationValues.Vertices[j].Y += mean;
                                break;
                            case InterpolationFeature.Elevation:
                                DiscretizedLocationValues.Vertices[j].Z += mean;
                                break;
                        }

                        //Calculating variance
                        DiscretizedLocationValues.Vertices[j].Value[1] = Vh.CalculateCovariance(DiscretizedLocationValues.Vertices[j], DiscretizedLocationValues.Vertices[j]) - DiscretizedLocationValues.Vertices[j].Value[1];

                    }

                    //If wanted, transforming the interpolated values to the original distribution range
                    if (TransformToOriginalDistribution)
                    {
                        double zMax = DiscretizedLocationValues.Vertices.Select(x => x.Value[0]).Max();
                        double zMin = DiscretizedLocationValues.Vertices.Select(x => x.Value[0]).Min();
                        double zAverage = DiscretizedLocationValues.Vertices.Select(x => x.Value[0]).Average();

                        double tMax = OriginalLocationValues.Select(x => x.Value[0]).Max();
                        double tMin = OriginalLocationValues.Select(x => x.Value[0]).Min();

                        for (int i = 0; i < DiscretizedLocationValues.Vertices.Count(); i++)
                        {
                            DiscretizedLocationValues.Vertices[i].Value[0] = ((DiscretizedLocationValues.Vertices[i].Value[0] - zMin) / (zMax - zMin)) * (tMax - tMin) + tMin;
                        }
                    }
                }
                catch
                {

                }
            try
            {
                if (MaximumNeighborCount > CrossValidationRemovePointCount)
                    ComputeSimpleKrigingCrossValidation().AsResult();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Calculating the p value cross validation for the simple kriging method
        /// </summary>
        /// <returns></returns>
        private async Task ComputeSimpleKrigingCrossValidation()
        {
            double mae = 0;
            double rmseSum = 0;
            double globalMean = 0;
            List<List<double>> pointPairs = new List<List<double>>();

            try
            {
                //Calculating global mean
                switch (InterpolationFeature)
                {
                    case InterpolationFeature.Value:
                        globalMean = OriginalLocationValues.ToArray().Average(x => x.Value[0]);
                        break;
                    case InterpolationFeature.Longitude:
                        globalMean = OriginalLocationValues.ToArray().Average(x => x.X);
                        break;
                    case InterpolationFeature.Latitutde:
                        globalMean = OriginalLocationValues.ToArray().Average(x => x.Y);
                        break;
                    case InterpolationFeature.Elevation:
                        globalMean = OriginalLocationValues.ToArray().Average(x => x.Z);
                        break;
                }

                //Calculating the RMSE
                //Parallel.For(0, OriginalLocationValues.Count(), k =>
                for (int k = 0; k < OriginalLocationValues.Count(); k++)
                {
                    await Task.Delay(0);

                    List<LocationTimeValue> includedPoints = new List<LocationTimeValue>();
                    includedPoints.AddRange(await SpatialNeighborhoodHelper.SearchByDistance(OriginalLocationValues, OriginalLocationValues[k], Vh.RangeX, Vh.RangeY, Vh.RangeZ, Vh.Azimuth, Vh.Dip, Vh.Plunge));

                    if (includedPoints.Count > MaximumNeighborCount)
                        includedPoints = includedPoints.OrderBy(x => x.GetEuclideanDistance(OriginalLocationValues[k])).Take(MaximumNeighborCount).ToList();

                    //Semivariance matrix for the ordinary kriging system
                    double[,] semivarianceMatrix = new double[includedPoints.Count(), includedPoints.Count()];

                    //Buffer of the original data values where the number of points defined for validation will be removed before kriging 
                    BindableCollection<LocationTimeValue> validationPoints = new BindableCollection<LocationTimeValue>(includedPoints.ToList());

                    //Semivariance matrix for RMSE
                    double[,] semivarianceMatrixRMSE = new double[,] { };

                    //Semivariance vector for RMSE
                    double[] semivarianceVector2 = new double[] { };

                    //Calculating the semivariance matrix based on the variogram model
                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        for (int j = 0; j < includedPoints.Count(); j++)
                        {
                            semivarianceMatrix[i, j] = Vh.CalculateCovariance(includedPoints[i], includedPoints[j]);
                            if (i == j && IncludeErrorVariance)
                                semivarianceMatrix[i, j] += includedPoints[i].Value[0] * ErrorVariance;
                        }
                    }

                    semivarianceMatrixRMSE = semivarianceMatrix.Copy();

                    semivarianceVector2 = new double[includedPoints.Count()];

                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        semivarianceVector2[i] = Vh.CalculateCovariance(OriginalLocationValues[k], includedPoints[i]);
                    }

                    //Removing the defined number of points from the validation data set
                    for (int l = 0; l < CrossValidationRemovePointCount; l++)
                    {
                        int m = rnd.Next(0, validationPoints.Count() - 1);
                        validationPoints.RemoveAt(m);
                        semivarianceMatrixRMSE = ArrayHelper.TrimArray(m, m, semivarianceMatrixRMSE);
                        semivarianceVector2 = semivarianceVector2.Where((val, idx) => idx != m).ToArray();
                    }

                    //Calculating the weights of the original data values on the interpolated value
                    double[] weights = Matrix.Solve(semivarianceMatrixRMSE, semivarianceVector2, true);

                    double weightSum = weights.Sum();

                    //Initializing the calculated value
                    double value = 0;

                    //Calculating the value at the point which is the sum of the weights times values in the original data table
                    for (int i = 0; i < validationPoints.Count(); i++)
                    {
                        //Calculating global mean for error treatment
                        switch (InterpolationFeature)
                        {
                            case InterpolationFeature.Value:
                                value += weights[i] * (validationPoints[i].Value[0] - globalMean);
                                break;
                            case InterpolationFeature.Longitude:
                                value += weights[i] * (validationPoints[i].X - globalMean);
                                break;
                            case InterpolationFeature.Latitutde:
                                value += weights[i] * (validationPoints[i].Y - globalMean);
                                break;
                            case InterpolationFeature.Elevation:
                                value += weights[i] * (validationPoints[i].Z - globalMean);
                                break;
                        }
                    }

                    value += globalMean;

                    pointPairs.Add(new List<double>() { OriginalLocationValues[k].Value[0], value });
                    Residuals.Vertices.Add(new LocationTimeValue(OriginalLocationValues[k]));
                    Residuals.Vertices[k].Value[0] = OriginalLocationValues[k].Value[0] - pointPairs[k][1];
                }

                //Transforming to the range of the original distribution
                if (TransformToOriginalDistribution)
                {
                    double zMax = pointPairs.Select(x => x[1]).Max();
                    double zMin = pointPairs.Select(x => x[1]).Min();
                    double zAverage = pointPairs.Select(x => x[1]).Average();

                    double tMax = pointPairs.Select(x => x[0]).Max();
                    double tMin = pointPairs.Select(x => x[0]).Min();

                    for (int i = 0; i < pointPairs.Count(); i++)
                    {
                        pointPairs[i][1] = ((pointPairs[i][1] - zMin) / (zMax - zMin)) * (tMax - tMin) + tMin;
                        Residuals.Vertices[i].Value[0] = Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                    }
                }

                for (int i = 0; i < pointPairs.Count(); i++)
                {

                    switch (InterpolationFeature)
                    {
                        case InterpolationFeature.Value:
                            //Increasing the value by the constrain value and it's weight
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Longitude:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Latitutde:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Elevation:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                    }
                }

                MAE = mae / OriginalLocationValues.Count();
                RMSE = Math.Sqrt(rmseSum / OriginalLocationValues.Count());
            }
            catch
            {
                MAE = double.NaN;
                RMSE = double.NaN;
            }
        }

        /// <summary>
        /// Universal Kriging a dataset of location values
        /// </summary>
        /// <param name="locVal">List of location values</param>
        /// <param name="steps">discretization size</param>
        /// <returns></returns>
        public async Task ComputeUniversalKriging()
        {
            try
            {
                //Performing the kriging calculation for each point
                //Parallel.For(0, DiscretizedLocationValues.Vertices.Count(), j =>
                for (int j = 0; j < DiscretizedLocationValues.Vertices.Count(); j++)
                {
                    if (j != 0 && j % 100 == 0)
                        Status = (Convert.ToDouble(j) / Convert.ToDouble(DiscretizedLocationValues.Vertices.Count())) * 100;
                    else if (IsCancelled == true)
                        break;

                    await Task.Delay(0);

                    //Buffer of the original data values where the number of points defined for validation will be removed before kriging
                    List<LocationTimeValue> includedPoints = new List<LocationTimeValue>();
                    includedPoints.AddRange(await SpatialNeighborhoodHelper.SearchByDistance(OriginalLocationValues, DiscretizedLocationValues.Vertices[j], Vh.RangeX, Vh.RangeY, Vh.RangeZ, Vh.Azimuth, Vh.Dip, Vh.Plunge));

                    if (includedPoints.Count > MaximumNeighborCount)
                        includedPoints = new List<LocationTimeValue>(includedPoints.OrderBy(x => x.GetEuclideanDistance(DiscretizedLocationValues.Vertices[j])).Take(MaximumNeighborCount).ToList());

                    //Estimating the drift variable
                    BindableCollection<LocationTimeValue> driftPoints = new BindableCollection<LocationTimeValue>();
                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        double drift = 0;
                        for (int k = 0; k < SpatialFunction.Count(); k++)
                            drift += SpatialFunction[k].Solve(includedPoints[i]);

                        driftPoints.Add(new LocationTimeValue() { X = includedPoints[i].X, Y = includedPoints[i].Y, Z = includedPoints[i].Z, Value = new List<double> { drift }, Date = includedPoints[i].Date, Name = includedPoints[i].Name });
                    }

                    //Calculating the drift mean
                    double driftMean = driftPoints.Select(x => x.Value[0]).Average();

                    double[] semivarianceVector = new double[includedPoints.Count() + SpatialFunction.Count() + 1];

                    //Semivariance matrix for the ordinary kriging system
                    double[,] semivarianceMatrix = new double[includedPoints.Count() + SpatialFunction.Count() + 1, includedPoints.Count() + SpatialFunction.Count() + 1];

                    //Calculating the semivariance matrix based on the variogram model
                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        for (int k = 0; k < includedPoints.Count(); k++)
                        {
                            //for detailed explanations see Wackernagel 2003
                            semivarianceMatrix[i, k] = Vh.CalculateSemivariance(includedPoints[i], includedPoints[k]);

                            if (i == k && IncludeErrorVariance)
                                semivarianceMatrix[i, k] += includedPoints[i].Value[0] * ErrorVariance;
                        }
                    }

                    //Calculating the semivariance matrix based on the variogram model
                    for (int i = 0; i < includedPoints.Count(); i++)
                    {

                        if (i != includedPoints.Count())
                        {
                            semivarianceMatrix[i, includedPoints.Count()] = 1;
                            semivarianceMatrix[includedPoints.Count(), i] = 1;
                        }
                        else
                        {
                            semivarianceMatrix[i, i] = 0;
                        }
                    }

                    for (int i = 0; i < SpatialFunction.Count(); i++)
                        for (int k = 0; k < includedPoints.Count(); k++)
                        {
                            if (i == 0)
                                semivarianceMatrix[includedPoints.Count() + i, k] = 1;
                            else
                                semivarianceMatrix[includedPoints.Count() + i, k] = SpatialFunction[i].Solve(includedPoints[k]);

                            if (i == 0)
                                semivarianceMatrix[k, includedPoints.Count() + i] = 1;
                            else
                                semivarianceMatrix[k, includedPoints.Count() + i] = SpatialFunction[i].Solve(includedPoints[k]);

                        }

                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        semivarianceVector[i] = Vh.CalculateSemivariance(DiscretizedLocationValues.Vertices[j], includedPoints[i]);
                    }

                    semivarianceVector[includedPoints.Count()] = 1;

                    for (int i = 0; i < SpatialFunction.Count(); i++)
                    {
                        if (i == 0)
                            semivarianceVector[includedPoints.Count() + i] = 1;
                        else
                        {
                            semivarianceVector[includedPoints.Count() + i] = SpatialFunction[i].Solve(DiscretizedLocationValues.Vertices[j]);
                        }
                    }

                    //Calculating the weights of the original data values on the interpolated value
                    double[] weights = Matrix.Solve(semivarianceMatrix, semivarianceVector, true);

                    double weightSum = weights.Sum();

                    //Calculating the value at the point which is the sum of the weights times values in the original data table
                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        //Calculating global mean for error treatment
                        switch (InterpolationFeature)
                        {
                            case InterpolationFeature.Value:
                                DiscretizedLocationValues.Vertices[j].Value[0] += weights[i] * includedPoints[i].Value[0];
                                break;
                            case InterpolationFeature.Longitude:
                                DiscretizedLocationValues.Vertices[j].X += weights[i] * includedPoints[i].X;
                                break;
                            case InterpolationFeature.Latitutde:
                                DiscretizedLocationValues.Vertices[j].Y += weights[i] * includedPoints[i].Y;
                                break;
                            case InterpolationFeature.Elevation:
                                DiscretizedLocationValues.Vertices[j].Z += weights[i] * includedPoints[i].Z;
                                break;
                        }
                    }
                }

            }
            catch
            {
                return;
            }
            try
            {
                //Computing the cross validation
                if (MaximumNeighborCount > CrossValidationRemovePointCount)
                    ComputeUniversalKrigingCrossValidation().AsResult();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Computing the p value cross validation for ordinary kriging
        /// </summary>
        /// <returns></returns>
        public async Task ComputeUniversalKrigingCrossValidation()
        {
            double mae = 0;
            double rmseSum = 0;
            List<List<double>> pointPairs = new List<List<double>>();

            try
            {
                //Calculating the RMSE
                //Parallel.For(0, OriginalLocationValues.Count(), k =>
                for (int k = 0; k < OriginalLocationValues.Count(); k++)
                {
                    await Task.Delay(0);

                    List<LocationTimeValue> includedPoints = new List<LocationTimeValue>();
                    includedPoints.AddRange(await SpatialNeighborhoodHelper.SearchByDistance(OriginalLocationValues, OriginalLocationValues[k], Vh.RangeX, Vh.RangeY, Vh.RangeZ, Vh.Azimuth, Vh.Dip, Vh.Plunge));

                    if (includedPoints.Count > MaximumNeighborCount)
                        includedPoints = new List<LocationTimeValue>(includedPoints.OrderBy(x => x.GetEuclideanDistance(OriginalLocationValues[k])).Take(MaximumNeighborCount).ToList());

                    //The drift points 
                    BindableCollection<LocationTimeValue> driftPoints = new BindableCollection<LocationTimeValue>();
                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        double drift = 0;
                        for (int j = 0; j < SpatialFunction.Count(); j++)
                            drift += SpatialFunction[j].Solve(includedPoints[i]);

                        driftPoints.Add(new LocationTimeValue() { X = includedPoints[i].X, Y = includedPoints[i].Y, Z = includedPoints[i].Z, Value = new List<double> { drift }, Date = includedPoints[i].Date, Name = includedPoints[i].Name });
                    }

                    //Calculating the drift mean
                    double driftMean = driftPoints.Select(x => x.Value[0]).Average();

                    double[] semivarianceVector = new double[includedPoints.Count() + SpatialFunction.Count() + 1];

                    //Semivariance matrix for the ordinary kriging system
                    double[,] semivarianceMatrix = new double[includedPoints.Count() + SpatialFunction.Count() + 1, includedPoints.Count() + SpatialFunction.Count() + 1];

                    //Buffer of the original data values where the number of points defined for validation will be removed before kriging 
                    BindableCollection<LocationTimeValue> validationPoints = new BindableCollection<LocationTimeValue>(includedPoints.ToList());

                    //Semivariance matrix for RMSE
                    double[,] semivarianceMatrixRMSE = new double[,] { };

                    //Semivariance vector for RMSE
                    double[] semivarianceVector2 = new double[] { };

                    //Calculating the semivariance matrix based on the variogram model
                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        for (int j = 0; j < includedPoints.Count(); j++)
                        {
                            semivarianceMatrix[i, j] = Vh.CalculateSemivariance(includedPoints[i], includedPoints[j]);
                            if (i == j && IncludeErrorVariance)
                                semivarianceMatrix[i, j] += includedPoints[i].Value[0] * ErrorVariance;
                        }
                    }

                    //Calculating the semivariance matrix based on the variogram model
                    for (int i = 0; i < includedPoints.Count() + 1; i++)
                    {

                        if (i != includedPoints.Count())
                        {
                            semivarianceMatrix[i, includedPoints.Count()] = 1;
                            semivarianceMatrix[includedPoints.Count(), i] = 1;
                        }
                        else
                        {
                            semivarianceMatrix[i, i] = 0;
                        }
                    }

                    for (int i = 0; i < SpatialFunction.Count(); i++)
                        for (int j = 0; j < includedPoints.Count(); j++)
                        {
                            if (i == 0)
                                semivarianceMatrix[includedPoints.Count() + i, j] = 1;
                            else
                                semivarianceMatrix[includedPoints.Count() + i, j] = SpatialFunction[i].Solve(includedPoints[j]);

                            if (i == 0)
                                semivarianceMatrix[j, includedPoints.Count() + i] = 1;
                            else
                                semivarianceMatrix[j, includedPoints.Count() + i] = SpatialFunction[i].Solve(includedPoints[j]);

                        }

                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        semivarianceVector[i] = Vh.CalculateSemivariance(OriginalLocationValues[k], includedPoints[i]);
                    }

                    semivarianceVector[includedPoints.Count()] = 1;

                    for (int i = 0; i < SpatialFunction.Count(); i++)
                    {
                        if (i == 0)
                            semivarianceVector[includedPoints.Count() + i] = 1;
                        else
                        {
                            semivarianceVector[includedPoints.Count() + i] = SpatialFunction[i].Solve(OriginalLocationValues[k]);
                        }
                    }

                    semivarianceMatrixRMSE = semivarianceMatrix.Copy();
                    semivarianceVector2 = semivarianceVector.Copy();

                    //Removing the defined number of points from the validation data set
                    for (int l = 0; l < CrossValidationRemovePointCount; l++)
                    {
                        int m = rnd.Next(0, validationPoints.Count() - 1);
                        validationPoints.RemoveAt(m);
                        semivarianceMatrixRMSE = ArrayHelper.TrimArray(m, m, semivarianceMatrixRMSE);
                        semivarianceVector2 = semivarianceVector2.Where((val, idx) => idx != m).ToArray();
                    }

                    //Calculating the weights of the original data values on the interpolated value
                    double[] weights = Matrix.Solve(semivarianceMatrixRMSE, semivarianceVector2, true);

                    double weightSum = weights.Sum();

                    //Initializing the calculated value
                    double value = 0;

                    //Calculating the value at the point which is the sum of the weights times values in the original data table
                    for (int i = 0; i < validationPoints.Count(); i++)
                    {
                        switch (InterpolationFeature)
                        {
                            case InterpolationFeature.Value:
                                value += weights[i] * validationPoints[i].Value[0];
                                break;
                            case InterpolationFeature.Longitude:
                                value += weights[i] * validationPoints[i].X;
                                break;
                            case InterpolationFeature.Latitutde:
                                value += weights[i] * validationPoints[i].Y;
                                break;
                            case InterpolationFeature.Elevation:
                                value += weights[i] * validationPoints[i].Z;
                                break;
                        }
                    }

                    pointPairs.Add(new List<double>() { OriginalLocationValues[k].Value[0], value });
                    Residuals.Vertices.Add(new LocationTimeValue(OriginalLocationValues[k]));
                    Residuals.Vertices[k].Value[0] = OriginalLocationValues[k].Value[0] - pointPairs[k][1];

                }

                if (TransformToOriginalDistribution)
                {
                    double zMax = pointPairs.Select(x => x[1]).Max();
                    double zMin = pointPairs.Select(x => x[1]).Min();
                    double zAverage = pointPairs.Select(x => x[1]).Average();

                    double tMax = pointPairs.Select(x => x[0]).Max();
                    double tMin = pointPairs.Select(x => x[0]).Min();

                    for (int i = 0; i < pointPairs.Count(); i++)
                    {
                        pointPairs[i][1] = ((pointPairs[i][1] - zMin) / (zMax - zMin)) * (tMax - tMin) + tMin;
                    }
                }

                for (int i = 0; i < pointPairs.Count(); i++)
                {
                    switch (InterpolationFeature)
                    {
                        case InterpolationFeature.Value:
                            //Increasing the value by the constrain value and it's weight
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Longitude:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Latitutde:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Elevation:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                    }
                }

                MAE = mae / OriginalLocationValues.Count();
                RMSE = Math.Sqrt(RMSE / OriginalLocationValues.Count());
            }
            catch
            {

            }
        }

        /// <summary>
        /// Calculate the residuals
        /// </summary>
        public async Task<Mesh> ComputeResiduals()
        {
            OriginalLocationValues.Clear();

            DiscretizedLocationValues = new Mesh();
            OriginalLocationValues.Clear();

            foreach (var d in SelectedMeasPoints)
            {
                try
                {
                    if (SelectedInterpolationMeasPoints[0].Vertices.Count() == 0)
                        DiscretizedLocationValues.Vertices.AddRange(new BindableCollection<LocationTimeValue>(d.Data.AsEnumerable()
                            .Select(x => new LocationTimeValue()
                            {
                                Value = new List<double> { 0, 0 },
                                X = (x.Field<double?>(1) == -9999 || x.Field<double?>(1) == -999999 || x.Field<double?>(1) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(1)),
                                Y = (x.Field<double?>(2) == -9999 || x.Field<double?>(2) == -999999 || x.Field<double?>(2) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(2)),
                                Z = (x.Field<double?>(3) == -9999 || x.Field<double?>(3) == -999999 || x.Field<double?>(3) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(3))
                            }).ToList()));
                    else
                    {
                        DiscretizedLocationValues.Vertices = new System.Collections.ObjectModel.ObservableCollection<LocationTimeValue>(d.Vertices.Select(x =>
                        new LocationTimeValue()
                        {
                            X = x.X,
                            Y = x.Y,
                            Z = x.Z,
                            Value = new List<double>() { 0, 0 },
                            MeshIndex = x.MeshIndex,
                            IsActive = x.IsActive,
                            IsDiscretized = x.IsDiscretized,
                            Brush = x.Brush,
                            Date = x.Date,
                            Name = x.Name,
                            Neighbors = x.Neighbors,
                            Geographic = x.Geographic,
                            IsExterior = x.IsExterior
                        }).ToList());
                    }
                }
                catch
                {
                    continue;
                }
            }

            //Creating the residuals with respect to a spatial polynomial function 
            foreach (LocationTimeValue loc in DiscretizedLocationValues.Vertices)
            {
                try
                {
                    double drift = 0;
                    switch(DriftType)
                    {
                        case Drift.SpatialFunction:
                            for (int j = 0; j < SpatialFunction.Count(); j++)
                                drift += SpatialFunction[j].Solve(loc);
                            break;
                        case Drift.DriftParameter:
                            for (int j = 0; j < RegressionParameter.Count(); j++)
                                drift += RegressionParameter[j] * Math.Pow(loc.Value[0], j);
                            break;
                    }

                    loc.Value = new List<double>() { loc.Value[0] - drift };
                }
                catch
                {

                }

            }

            try
            {
                //Adding interpolated values and variances to the original data set
                DiscretizedLocationValues.Name = "Residuals";
                DiscretizedLocationValues.Data =
                    CollectionHelper.ConvertTo<Tuple<double, double, double, double, DateTime, string>>(
                        new List<Tuple<double, double, double, double, DateTime, string>>(DiscretizedLocationValues.Vertices.Select(a =>
                           new Tuple<double, double, double, double, DateTime, string>(
                               a.Value[0],
                               a.X,
                               a.Y,
                               a.Z,
                               a.Date,
                               a.Name
                               )).ToList()));

                DiscretizedLocationValues.MeshCellType = MeshCellType.Hexahedral;

                DiscretizedLocationValues.CellsFromPointCloud();

                DiscretizedLocationValues.Dimensionality = SelectedInterpolationMeasPoints[0].Dimensionality;
                DiscretizedLocationValues.Faces = SelectedInterpolationMeasPoints[0].Faces;

                return DiscretizedLocationValues;
            }
            catch
            {
                return new Mesh();
            }


        }

        /// <summary>
        /// Simulating a distribution of field parameters
        /// </summary>
        /// <param name="locVal">List of location values</param>
        /// <param name="steps">discretization size</param>
        /// <returns></returns>
        public async Task ComputeSimulatedAnnealing()
        {
            try
            {
                //Randomly seeding values according to the original distribution in the mesh
                DiscretizedLocationValues.ReproduceDistribution(OriginalLocationValues.Select(x => x.Value[0]).ToArray(), Vh.NumberBins);

                double G = 0;

                //Performing the kriging calculation for each point
                //Parallel.For(0, NumberOfSwaps, j =>
                for (int j = 0; j < NumberOfSwaps; j++)
                {
                    if (j != 0 && j % 100 == 0)
                        Status = (Convert.ToDouble(j) / Convert.ToDouble(DiscretizedLocationValues.Vertices.Count())) * 100;
                    else if (IsCancelled == true)
                        break;

                    int rnd1 = rnd.Next(0, DiscretizedLocationValues.Vertices.Count());
                    int rnd2 = rnd.Next(0, DiscretizedLocationValues.Vertices.Count());

                    double val1 = Convert.ToDouble(DiscretizedLocationValues.Vertices[rnd1].Value[0]);
                    double val2 = Convert.ToDouble(DiscretizedLocationValues.Vertices[rnd2].Value[0]);

                    DiscretizedLocationValues.Vertices[rnd1].Value[0] = Convert.ToDouble(val2);
                    DiscretizedLocationValues.Vertices[rnd2].Value[0] = Convert.ToDouble(val1);

                    List<LocationTimeValue> shuffledValues = new List<LocationTimeValue>(DiscretizedLocationValues.Vertices.ToList());
                    shuffledValues.Shuffle();

                    List<LocationTimeValue> samplePoints1 = new List<LocationTimeValue>();
                    samplePoints1.Add(DiscretizedLocationValues.Vertices[rnd1]);
                    samplePoints1.Add(DiscretizedLocationValues.Vertices[rnd2]);

                    //Buffer of the original data values where the number of points defined for validation will be removed before kriging
                    BindableCollection<LocationTimeValue> includedPoints = new BindableCollection<LocationTimeValue>();
                    includedPoints.AddRange(await SpatialNeighborhoodHelper.SearchByDistance(OriginalLocationValues, DiscretizedLocationValues.Vertices[rnd1], Vh.RangeX, Vh.RangeY, Vh.RangeZ, Vh.Azimuth, Vh.Dip, Vh.Plunge));

                    if (includedPoints.Count > MaximumNeighborCount - 2)
                        includedPoints = new BindableCollection<LocationTimeValue>(includedPoints.Take(MaximumNeighborCount - 2).ToList());

                    samplePoints1.AddRange(OriginalLocationValues);
                    samplePoints1.AddRange(includedPoints);

                    //Computing new variogram
                    VariogramHelper vh1 = new VariogramHelper(new BindableCollection<LocationTimeValue>(samplePoints1))
                    {
                        NumberBins = Vh.NumberBins,
                        Range = Vh.Range,
                        Nugget = Vh.Nugget,
                        Sill = Vh.Sill,
                        Model = Vh.Model
                    };

                    vh1.ComputeExperimentalVariogram();
                    vh1.CalculateVariogramModel();

                    //Calculating the new difference from the previous variogram.
                    //If the new deviation from the original variogram is less than before, 
                    //the points remained swapped and the new value will be used for further comparison.
                    double G1 = 0;

                    if (j == 0)
                        G += Vh.SquaredDifferences;

                    G1 += vh1.SquaredDifferences;

                    if (G1 <= G)
                        G = G1;
                    else
                    {
                        DiscretizedLocationValues.Vertices[rnd1].Value[0] = val1;
                        DiscretizedLocationValues.Vertices[rnd2].Value[0] = val2;
                    }
                }

            }
            catch
            {
                return;
            }
            try
            {
                //Computing the cross validation
                if (MaximumNeighborCount > CrossValidationRemovePointCount)
                    ComputeOrdinaryKrigingCrossValidation().AsResult();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Kriging a dataset of location values
        /// </summary>
        /// <param name="locVal">List of location values</param>
        /// <param name="steps">discretization size</param>
        /// <returns></returns>
        public async Task ComputeSequentialGaussianSimulation()
        {
            if (OriginalLocationValues.Count >= 1)

                try
                {
                    double globalMean = 0;

                    //Calculating global mean for error treatment
                    switch (InterpolationFeature)
                    {
                        case InterpolationFeature.Value:
                            globalMean = OriginalLocationValues.ToArray().Average(x => x.Value[0]);
                            break;
                        case InterpolationFeature.Longitude:
                            globalMean = OriginalLocationValues.ToArray().Average(x => x.X);
                            break;
                        case InterpolationFeature.Latitutde:
                            globalMean = OriginalLocationValues.ToArray().Average(x => x.Y);
                            break;
                        case InterpolationFeature.Elevation:
                            globalMean = OriginalLocationValues.ToArray().Average(x => x.Z);
                            break;
                    }

                    int[] randomPath = Enumerable.Range(0, DiscretizedLocationValues.Vertices.Count()).Shuffle().ToArray();

                    BlockingCollection<LocationTimeValue> simulatedValues = new BlockingCollection<LocationTimeValue>();

                    //Determining the maximum degree of parallelism
                    System.Threading.Tasks.ParallelOptions opt = new System.Threading.Tasks.ParallelOptions();
                    opt.MaxDegreeOfParallelism = MaximumDegreeOfParallelism;

                    //Performing the kriging calculation for each point
                    Parallel.For(0, DiscretizedLocationValues.Vertices.Count(), opt, (j, loopState) =>
                    //for (int j = 0; j < DiscretizedLocationValues.Vertices.Count(); j++)
                    {
                        if (j != 0 && j % 100 == 0)
                            Status = (Convert.ToDouble(simulatedValues.Count()) / Convert.ToDouble(DiscretizedLocationValues.Vertices.Count())) * 100;
                        else if (IsCancelled == true)
                            loopState.Break();

                        DiscretizedLocationValues.Vertices[randomPath[j]].Value[0] = 0;

                        //Defining the neighborhood
                        List<LocationTimeValue> includedPoints = new List<LocationTimeValue>();

                        includedPoints.AddRange((SpatialNeighborhoodHelper.SearchByDistance(OriginalLocationValues, DiscretizedLocationValues.Vertices[randomPath[j]], Vh.RangeX, Vh.RangeY, Vh.RangeZ, Vh.Azimuth, Vh.Dip, Vh.Plunge)).Result);

                        if(simulatedValues.Count > 0)
                        {
                                includedPoints.AddRange(SpatialNeighborhoodHelper.SearchByDistance(simulatedValues.ToList(), DiscretizedLocationValues.Vertices[randomPath[j]], Vh.RangeX, Vh.RangeY, Vh.RangeZ, Vh.Azimuth, Vh.Dip, Vh.Plunge).Result);
                        }

                        //If selection is too big or too small either a subset is selected or a new selection is made
                        if (includedPoints == null || includedPoints.Count() == 0)
                            includedPoints = OriginalLocationValues.Where(x => ShouldTargetVertexFitSourceGridName ? DiscretizedLocationValues.Vertices[randomPath[j]].Name == x.Name : 0 == 0).OrderBy(x => x.GetEuclideanDistance(DiscretizedLocationValues.Vertices[randomPath[j]])).Take(MaximumNeighborCount).ToList();
                        else if (includedPoints.Count() > MaximumNeighborCount)
                        {
                            includedPoints = includedPoints.AsParallel().MinBy(x => x.GetEuclideanDistance(DiscretizedLocationValues.Vertices[randomPath[j]])).Take(MaximumNeighborCount).ToList();
                        }

                        double mean = includedPoints.Average(x => x.Value[0]);

                        //Covariance vector
                        double[] semivarianceVector = new double[includedPoints.Count()];
                        //Semivariance matrix for the ordinary kriging system
                        double[,] semivarianceMatrix = new double[includedPoints.Count(), includedPoints.Count()];

                        //Calculating the semivariance matrix based on the variogram model
                        for (int i = 0; i < includedPoints.Count(); i++)
                        {
                            for (int k = 0; k < includedPoints.Count(); k++)
                            {
                                semivarianceMatrix[i, k] = Vh.CalculateCovariance(includedPoints[i], includedPoints[k]);

                                if (i == k && IncludeErrorVariance)
                                    semivarianceMatrix[i, k] += includedPoints[i].Value[0] * ErrorVariance;
                            }
                        }

                        for (int i = 0; i < includedPoints.Count(); i++)
                        {
                            semivarianceVector[i] = Vh.CalculateCovariance(DiscretizedLocationValues.Vertices[randomPath[j]], includedPoints[i]);
                        }

                        //Calculating the weights of the original data values on the interpolated value
                        double[] weights = Matrix.Solve(semivarianceMatrix, semivarianceVector, true);

                        double weightSum = weights.Sum();

                        //Calculating the value at the point which is the sum of the weights times values in the original data table
                        for (int i = 0; i < includedPoints.Count(); i++)
                        {

                            //Calculating global mean for error treatment
                            switch (InterpolationFeature)
                            {
                                case InterpolationFeature.Value:
                                    DiscretizedLocationValues.Vertices[randomPath[j]].Value[0] += weights[i] * (includedPoints[i].Value[0] - mean);
                                    break;
                                case InterpolationFeature.Longitude:
                                    DiscretizedLocationValues.Vertices[randomPath[j]].X += weights[i] * (includedPoints[i].X - mean);
                                    break;
                                case InterpolationFeature.Latitutde:
                                    DiscretizedLocationValues.Vertices[randomPath[j]].Y += weights[i] * (includedPoints[i].Y - mean);
                                    break;
                                case InterpolationFeature.Elevation:
                                    DiscretizedLocationValues.Vertices[randomPath[j]].Z += weights[i] * (includedPoints[i].Z - mean);
                                    break;
                            }

                            //Calculating variance
                            DiscretizedLocationValues.Vertices[randomPath[j]].Value[1] += weights[i] * Vh.CalculateCovariance(includedPoints[i], DiscretizedLocationValues.Vertices[randomPath[j]]);

                        }

                        //Calculating global mean for error treatment
                        switch (InterpolationFeature)
                        {
                            case InterpolationFeature.Value:
                                DiscretizedLocationValues.Vertices[randomPath[j]].Value[0] += mean;
                                break;
                            case InterpolationFeature.Longitude:
                                DiscretizedLocationValues.Vertices[randomPath[j]].X += mean;
                                break;
                            case InterpolationFeature.Latitutde:
                                DiscretizedLocationValues.Vertices[randomPath[j]].Y += mean;
                                break;
                            case InterpolationFeature.Elevation:
                                DiscretizedLocationValues.Vertices[randomPath[j]].Z += mean;
                                break;
                        }

                        //Calculating variance
                        DiscretizedLocationValues.Vertices[randomPath[j]].Value[1] = Vh.CalculateCovariance(DiscretizedLocationValues.Vertices[randomPath[j]], DiscretizedLocationValues.Vertices[randomPath[j]]) - DiscretizedLocationValues.Vertices[randomPath[j]].Value[1];

                        double krigingMean = DiscretizedLocationValues.Vertices[randomPath[j]].Value[0];
                        double stdDev = Math.Sqrt((!IncludeLocalVariance ? DiscretizedLocationValues.Vertices[randomPath[j]].Value[1] : SelectedGlobalVariance[0].Vertices[randomPath[j]].Value[0]));

                        //Using the Box-Muller algorithm to produce Gaussian random number
                        DiscretizedLocationValues.Vertices[randomPath[j]].Value[0] = DistributionHelper.SampleFromGaussian(krigingMean, stdDev);

                        simulatedValues.Add(DiscretizedLocationValues.Vertices[randomPath[j]]);
                    //}
                    });
                }
                catch
                {

                }
                finally
                {
                    Status = 0;
                }
            try
            {
                if (MaximumNeighborCount > CrossValidationRemovePointCount)
                    ComputeSimpleKrigingCrossValidation().AsResult();
            }
            catch
            {

            }

            try
            {
                for (int i = 0; i < DiscretizedLocationValues.Vertices.Count(); i++)
                {
                    
                }
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Calculating the p value cross validation for the simple kriging method
        /// </summary>
        /// <returns></returns>
        private async Task ComputeSequentialGaussianSimulationCrossValidation()
        {
            double mae = 0;
            double rmseSum = 0;
            double globalMean = 0;
            List<List<double>> pointPairs = new List<List<double>>();
            List<LocationTimeValue> simulatedValues = new List<LocationTimeValue>();

            try
            {
                //Calculating global mean
                switch (InterpolationFeature)
                {
                    case InterpolationFeature.Value:
                        globalMean = OriginalLocationValues.ToArray().Average(x => x.Value[0]);
                        break;
                    case InterpolationFeature.Longitude:
                        globalMean = OriginalLocationValues.ToArray().Average(x => x.X);
                        break;
                    case InterpolationFeature.Latitutde:
                        globalMean = OriginalLocationValues.ToArray().Average(x => x.Y);
                        break;
                    case InterpolationFeature.Elevation:
                        globalMean = OriginalLocationValues.ToArray().Average(x => x.Z);
                        break;
                }

                //Calculating the RMSE
                //Parallel.For(0, OriginalLocationValues.Count(), k =>
                for (int k = 0; k < OriginalLocationValues.Count(); k++)
                {
                    await Task.Delay(0);

                    //Buffer of the original data values where the number of points defined for validation will be removed before kriging
                    List<LocationTimeValue> includedPoints = new List<LocationTimeValue>();

                    includedPoints.AddRange(await SpatialNeighborhoodHelper.SearchByDistance(OriginalLocationValues, OriginalLocationValues[k], Vh.RangeX, Vh.RangeY, Vh.RangeZ, Vh.Azimuth, Vh.Dip, Vh.Plunge));
                    includedPoints.AddRange(await SpatialNeighborhoodHelper.SearchByDistance(simulatedValues, OriginalLocationValues[k], Vh.RangeX, Vh.RangeY, Vh.RangeZ, Vh.Azimuth, Vh.Dip, Vh.Plunge));

                    if (includedPoints.Count > MaximumNeighborCount)
                        includedPoints = includedPoints.OrderBy(x => x.GetEuclideanDistance(OriginalLocationValues[k])).Take(MaximumNeighborCount).ToList();
                    
                    //Semivariance matrix for the ordinary kriging system
                    double[,] semivarianceMatrix = new double[includedPoints.Count(), includedPoints.Count()];

                    //Buffer of the original data values where the number of points defined for validation will be removed before kriging 
                    BindableCollection<LocationTimeValue> validationPoints = new BindableCollection<LocationTimeValue>(includedPoints.ToList());

                    //Semivariance matrix for RMSE
                    double[,] semivarianceMatrixRMSE = new double[,] { };

                    //Semivariance vector for RMSE
                    double[] semivarianceVector2 = new double[] { };

                    //Calculating the semivariance matrix based on the variogram model
                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        for (int j = 0; j < includedPoints.Count(); j++)
                        {
                            semivarianceMatrix[i, j] = Vh.CalculateCovariance(includedPoints[i], includedPoints[j]);
                            if (i == j && IncludeErrorVariance)
                                semivarianceMatrix[i, j] += includedPoints[i].Value[0] * ErrorVariance;
                        }
                    }

                    semivarianceMatrixRMSE = semivarianceMatrix.Copy();

                    semivarianceVector2 = new double[includedPoints.Count()];

                    for (int i = 0; i < includedPoints.Count(); i++)
                    {
                        semivarianceVector2[i] = Vh.CalculateCovariance(OriginalLocationValues[k], includedPoints[i]);
                    }

                    //Removing the defined number of points from the validation data set
                    for (int l = 0; l < CrossValidationRemovePointCount; l++)
                    {
                        int m = rnd.Next(0, validationPoints.Count() - 1);
                        validationPoints.RemoveAt(m);
                        semivarianceMatrixRMSE = ArrayHelper.TrimArray(m, m, semivarianceMatrixRMSE);
                        semivarianceVector2 = semivarianceVector2.Where((val, idx) => idx != m).ToArray();
                    }

                    //Calculating the weights of the original data values on the interpolated value
                    double[] weights = Matrix.Solve(semivarianceMatrixRMSE, semivarianceVector2, true);

                    double weightSum = weights.Sum();

                    //Initializing the calculated value
                    double value = 0;

                    double variance = 0;

                    //Calculating the value at the point which is the sum of the weights times values in the original data table
                    for (int i = 0; i < validationPoints.Count(); i++)
                    {
                        //Calculating global mean for error treatment
                        switch (InterpolationFeature)
                        {
                            case InterpolationFeature.Value:
                                value += weights[i] * (validationPoints[i].Value[0] - globalMean);
                                break;
                            case InterpolationFeature.Longitude:
                                value += weights[i] * (validationPoints[i].X - globalMean);
                                break;
                            case InterpolationFeature.Latitutde:
                                value += weights[i] * (validationPoints[i].Y - globalMean);
                                break;
                            case InterpolationFeature.Elevation:
                                value += weights[i] * (validationPoints[i].Z - globalMean);
                                break;
                        }

                        variance += weights[i] * Vh.CalculateCovariance(validationPoints[i], OriginalLocationValues[k]);
                    }

                    value += globalMean;

                    //Calculating the kriging variance
                    variance = Vh.CalculateCovariance(OriginalLocationValues[k], OriginalLocationValues[k]) - variance;

                    //Deriving the standard deviation
                    double stdDev = Math.Sqrt((!IncludeLocalVariance ? Math.Abs(variance) : SelectedGlobalVariance[0].Vertices.OrderBy(x => x.GetEuclideanDistance(OriginalLocationValues[k])).Take(3).Average(x => x.Value[0])));

                    //Simulating the value
                    double simulatedValue = DistributionHelper.SampleFromGaussian(value, stdDev);

                    pointPairs.Add(new List<double>() { OriginalLocationValues[k].Value[0], simulatedValue});
                    Residuals.Vertices.Add(new LocationTimeValue(OriginalLocationValues[k]));
                    Residuals.Vertices[k].Value[0] = OriginalLocationValues[k].Value[0] - simulatedValue;

                    LocationTimeValue loc = new LocationTimeValue(OriginalLocationValues[k]);
                    loc.Value[0] = simulatedValue;
                    simulatedValues.Add(loc);
                }

                //Transforming results to the extend of the original data distribution
                if (TransformToOriginalDistribution)
                {
                    double zMax = pointPairs.Select(x => x[1]).Max();
                    double zMin = pointPairs.Select(x => x[1]).Min();
                    double zAverage = pointPairs.Select(x => x[1]).Average();

                    double tMax = pointPairs.Select(x => x[0]).Max();
                    double tMin = pointPairs.Select(x => x[0]).Min();

                    for (int i = 0; i < pointPairs.Count(); i++)
                    {
                        pointPairs[i][1] = ((pointPairs[i][1] - zMin) / (zMax - zMin)) * (tMax - tMin) + tMin;
                    }
                }

                for (int i = 0; i < pointPairs.Count(); i++)
                {
                    switch (InterpolationFeature)
                    {
                        case InterpolationFeature.Value:
                            //Increasing the value by the constrain value and it's weight
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Longitude:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Latitutde:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                        case InterpolationFeature.Elevation:
                            mae += Math.Abs(pointPairs[i][0] - pointPairs[i][1]);
                            rmseSum += Math.Abs(Math.Pow(pointPairs[i][0] - pointPairs[i][1], 2));
                            break;
                    }
                }

                MAE = mae / OriginalLocationValues.Count();
                RMSE = Math.Sqrt(rmseSum / OriginalLocationValues.Count());

                double[,] covarianceSimulated = Vh.GetVarianceCovarianceMatrix(simulatedValues);
                
                //TODO implementing the frobenius norm validation
                //double[,] covarianceOriginal = Accord.Statistics.Measures.Covariance();
                //double[,] subtracted = covarianceSimulated.Subtract(covarianceOriginal);

                //double normBoth = Norm.Frobenius(covarianceSimulated.Subtract(covarianceOriginal));
                //double normOriginal = Norm.Frobenius(Vh.GetVarianceCovarianceMatrix(OriginalLocationValues.ToList()));

                //FrobeniusNorm = normBoth / normOriginal;
            }
            catch
            {
                MAE = double.NaN;
                RMSE = double.NaN;
                FrobeniusNorm = double.NaN;
            }
        }

        #endregion

        #region Helper

        /// <summary>
        /// Adds a polynomial to the function
        /// </summary>
        public void AddPolynomial()
        {
            try
            {
                SpatialFunction.Add(new Polynomial());
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Removes the selected polynomial from a function
        /// </summary>
        public void RemovePolynomial()
        {
            try
            {
                SpatialFunction.Remove(SelectedPolynomial);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Adds a regression parameter
        /// </summary>
        public void AddRegressionParameter()
        {
            try
            {
                RegressionParameter.Add(0);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Clears the regression parameters
        /// </summary>
        public void RemoveRegressionParameter()
        {
            try
            {
                RegressionParameter.Clear();
            }
            catch
            {
                return;
            }
        }

        #endregion
    }
}
