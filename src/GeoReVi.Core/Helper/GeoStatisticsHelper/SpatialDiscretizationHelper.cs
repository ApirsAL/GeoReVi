using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GeoReVi
{
    /// <summary>
    /// A class that discretizes a spatial domain into discrete values
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    public class SpatialDiscretizationHelper : GeostatisticalAnalysis
    {
        #region Private members

        private static Random random = new Random();

        #endregion

        #region Public properties

        /// <summary>
        /// The discretization method
        /// </summary>
        private DiscretizationMethod discretizationMethod = DiscretizationMethod.Hexahedral;
        public DiscretizationMethod DiscretizationMethod
        {
            get => this.discretizationMethod;
            set
            {
                this.discretizationMethod = value;
                NotifyOfPropertyChange(() => DiscretizationMethod);
            }
        }

        /// <summary>
        /// The domain that should be discretized
        /// </summary>
        private Refinement refinement = Refinement.None;
        public Refinement Refinement
        {
            get => this.refinement;
            set
            {
                this.refinement = value;
                NotifyOfPropertyChange(() => Refinement);
            }
        }

        /// <summary>
        /// Checks if all data points in the included data points data set should be included
        /// </summary>
        private bool includeAllOriginalPoints = false;
        public bool IncludeOriginalDataPoints
        {
            get => this.includeAllOriginalPoints;
            set
            {
                this.includeAllOriginalPoints = value;
                NotifyOfPropertyChange(() => IncludeOriginalDataPoints);
            }
        }

        /// <summary>
        /// Number of bins in x/longitude direction
        /// </summary>
        private int binsX = 10;
        public int BinsX
        {
            get => this.binsX;
            set
            {
                this.binsX = value;
                NotifyOfPropertyChange(() => BinsX);
            }
        }

        /// <summary>
        /// Number of bins in y/latitude direction
        /// </summary>
        private int binsY = 10;
        public int BinsY
        {
            get => this.binsY;
            set
            {
                this.binsY = value;
                NotifyOfPropertyChange(() => BinsY);
            }
        }

        /// <summary>
        /// Number of bins in z/altitude direction
        /// </summary>
        private int binsZ = 10;
        public int BinsZ
        {
            get => this.binsZ;
            set
            {
                this.binsZ = value;
                NotifyOfPropertyChange(() => BinsZ);
            }
        }

        /// <summary>
        /// Start coordinate in x direction
        /// </summary>
        private double startX = 0;
        public double StartX
        {
            get => this.startX;
            set
            {
                this.startX = value;
                NotifyOfPropertyChange(() => StartX);
            }
        }

        /// <summary>
        /// Start coordinate in x direction
        /// </summary>
        private double endX = 0;
        public double EndX
        {
            get => this.endX;
            set
            {
                this.endX = value;
                NotifyOfPropertyChange(() => EndX);
            }
        }


        /// <summary>
        /// Start coordinate in Y direction
        /// </summary>
        private double startY = 0;
        public double StartY
        {
            get => this.startY;
            set
            {
                this.startY = value;
                NotifyOfPropertyChange(() => StartY);
            }
        }

        /// <summary>
        /// Start coordinate in Y direction
        /// </summary>
        private double endY = 0;
        public double EndY
        {
            get => this.endY;
            set
            {
                this.endY = value;
                NotifyOfPropertyChange(() => EndY);
            }
        }

        /// <summary>
        /// Start coordinate in Z direction
        /// </summary>
        private double startZ = 0;
        public double StartZ
        {
            get => this.startZ;
            set
            {
                this.startZ = value;
                NotifyOfPropertyChange(() => StartZ);
            }
        }

        /// <summary>
        /// Start coordinate in Z direction
        /// </summary>
        private double endZ = 0;
        public double EndZ
        {
            get => this.endZ;
            set
            {
                this.endZ = value;
                NotifyOfPropertyChange(() => EndZ);
            }
        }

        /// <summary>
        /// Boundary type of the griding operation
        /// </summary>
        private BoundaryType boundaryType = BoundaryType.Rectangular;
        public BoundaryType BoundaryType
        {
            get => this.boundaryType;
            set
            {
                this.boundaryType = value;
                NotifyOfPropertyChange(() => BoundaryType);
                NotifyOfPropertyChange(() => IsRectangularGrid);
            }
        }

        /// <summary>
        /// Type of spatial operation
        /// </summary>
        private SpatialOperationType spatialOperationType = SpatialOperationType.Union;
        public SpatialOperationType SpatialOperationType
        {
            get => this.spatialOperationType;
            set
            {
                this.spatialOperationType = value;
                NotifyOfPropertyChange(() => SpatialOperationType);
            }
        }

        /// <summary>
        /// The location values that should be included in the mesh
        /// </summary>
        private BindableCollection<LocationTimeValue> includedLocationValues = new BindableCollection<LocationTimeValue>();
        public BindableCollection<LocationTimeValue> IncludedlLocationValues
        {
            get => includedLocationValues;
            set
            {
                this.includedLocationValues = value;
                NotifyOfPropertyChange(() => IncludedlLocationValues);
            }
        }

        /// <summary>
        /// The original data set
        /// </summary>
        private BindableCollection<LocationTimeValue> originalLocationValues = new BindableCollection<LocationTimeValue>();
        public BindableCollection<LocationTimeValue> OriginalLocationValues
        {
            get => originalLocationValues;
            set
            {
                this.originalLocationValues = value;
                NotifyOfPropertyChange(() => OriginalLocationValues);
            }
        }

        /// <summary>
        /// The discretized values
        /// </summary>
        private Mesh discretizedLocationValues;
        public Mesh DiscretizedLocationValues
        {
            get => this.discretizedLocationValues;
            set
            {
                this.discretizedLocationValues = value;
                NotifyOfPropertyChange(() => DiscretizedLocationValues);
            }
        }

        /// <summary>
        /// The location values used for drift
        /// </summary>
        private List<LocationTimeValue> driftLocationValues;
        public List<LocationTimeValue> DriftLocationValues
        {
            get => this.driftLocationValues;
            set
            {
                this.driftLocationValues = value;
                NotifyOfPropertyChange(() => DriftLocationValues);
            }
        }

        /// <summary>
        /// Selected data sets for discretization
        /// </summary>
        private BindableCollection<Mesh> selectedMeasPoints = new BindableCollection<Mesh>();
        public BindableCollection<Mesh> SelectedMeasPoints
        {
            get => this.selectedMeasPoints;
            set
            {
                this.selectedMeasPoints = value;
                NotifyOfPropertyChange(() => SelectedMeasPoints);
            }
        }

        /// <summary>
        /// Checks if the discretization should be applied in an rectangular grid
        /// </summary>
        public bool IsRectangularGrid
        {
            get => BoundaryType == BoundaryType.Rectangular;

        }


        /// <summary>
        /// Measurement points of a property
        /// </summary>
        private BindableCollection<KeyValuePair<string, DataTable>> measPoints = new BindableCollection<KeyValuePair<string, DataTable>>();
        public BindableCollection<KeyValuePair<string, DataTable>> MeasPoints
        {
            get => this.measPoints;
            set
            {
                this.measPoints = value;
                NotifyOfPropertyChange(() => MeasPoints);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="locationValues"></param>
        public SpatialDiscretizationHelper(BindableCollection<KeyValuePair<string, DataTable>> locationValues,
            int binsX = 20,
            int binsY = 20,
            int binsZ = 20,
            DiscretizationMethod _discretizationMethod = DiscretizationMethod.Hexahedral,
            Refinement _refinement = Refinement.None)
        {
            DiscretizedLocationValues = new Mesh();
            MeasPoints = locationValues;
            BinsX = binsX;
            BinsY = binsY;
            BinsZ = binsZ;
            DiscretizationMethod = _discretizationMethod;
            Refinement = _refinement;
        }


        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="locationValues"></param>
        public SpatialDiscretizationHelper(int _binsX = 20,
            int _binsY = 20,
            int _binsZ = 20,
            DiscretizationMethod _discretizationMethod = DiscretizationMethod.Hexahedral,
            Refinement _refinement = Refinement.None)
        {
            DiscretizedLocationValues = new Mesh();
            BinsX = binsX;
            BinsX = _binsX;
            BinsY = _binsY;
            BinsZ = _binsZ;
            DiscretizationMethod = _discretizationMethod;
            Refinement = _refinement;
        }

        #endregion

        /// <summary>
        /// Discretizes a region of location values according to a number of bins into a regular grid
        /// </summary>
        /// <param name="numberBins"></param>
        public void ComputeDiscretization()
        {

            DiscretizedLocationValues = new Mesh()
            {
                Dimensionality = GetDimensionality()
            };

            if (DiscretizationMethod == DiscretizationMethod.Hexahedral && DiscretizedLocationValues.Dimensionality == Dimensionality.ThreeD)
                DiscretizedLocationValues.MeshCellType = MeshCellType.Hexahedral;
            else if (DiscretizationMethod == DiscretizationMethod.Random && DiscretizedLocationValues.Dimensionality == Dimensionality.ThreeD)
                DiscretizedLocationValues.MeshCellType = MeshCellType.Tetrahedal;
            else if (DiscretizationMethod == DiscretizationMethod.Hexahedral && DiscretizedLocationValues.Dimensionality == Dimensionality.TwoD)
                DiscretizedLocationValues.MeshCellType = MeshCellType.Rectangular;
            else if (DiscretizationMethod == DiscretizationMethod.Random && DiscretizedLocationValues.Dimensionality == Dimensionality.TwoD)
                DiscretizedLocationValues.MeshCellType = MeshCellType.Triangular;

            BindableCollection<LocationTimeValue> convexHull = new BindableCollection<LocationTimeValue>();

            ///Calculating the convex hull
            if (BoundaryType == BoundaryType.ConvexHullXY)
            {
                try
                {
                    convexHull = new BindableCollection<LocationTimeValue>(ConvexHull.ComputeConvexHull2D(OriginalLocationValues.ToList()));
                }
                catch
                {
                    return;
                }

            }

            //Determining the domain where the discrete points should be created in
            double[] xDomain = StartX == EndX ? OriginalLocationValues.Select(x => x.X).ToArray() : new double[2] { StartX, EndX };
            double[] yDomain = StartY == EndY ? OriginalLocationValues.Select(x => x.Y).ToArray() : new double[2] { StartY, EndY };
            double[] zDomain = StartZ == EndZ ? OriginalLocationValues.Select(x => x.Z).ToArray() : new double[2] { StartZ, EndZ };

            double[] xArray = BinsX == 0 ? new double[] { EndX } : DistributionHelper.Subdivide(xDomain, BinsX);
            double[] yArray = BinsY == 0 ? new double[] { EndY } : DistributionHelper.Subdivide(yDomain, BinsY);
            double[] zArray = BinsZ == 0 ? new double[] { EndZ } : DistributionHelper.Subdivide(zDomain, BinsZ);

            double xDiff = xArray.Length > 1 ? xArray[1] - xArray[0] : 0;
            double yDiff = yArray.Length > 1 ? yArray[1] - yArray[0] : 0;
            double zDiff = zArray.Length > 1 ? zArray[1] - zArray[0] : 0;

            if (xArray.Length < 1)
                xArray = new double[] { EndX };

            if (yArray.Length < 1)
                yArray = new double[] { EndY };

            if (zArray.Length < 1)
                zArray = new double[] { EndZ };

            if (BoundaryType != BoundaryType.TwoBoundingSurfaces)
                for (int i = 0; i < xArray.Length; i++)
                {
                    for (int j = 0; j < yArray.Length; j++)
                    {
                        for (int k = 0; k < zArray.Length; k++)
                        {
                            switch (DiscretizationMethod)
                            {

                                //Creates a hexahedral grid
                                case DiscretizationMethod.Hexahedral:

                                    //Creating the new point to be inserted
                                    var pt = new LocationTimeValue()
                                    {
                                        X = xArray[i],
                                        Y = yArray[j],
                                        Z = zArray[k],
                                        Geographic = false,
                                        IsDiscretized = true,
                                        IsExterior = i == 0
                                        || i == xArray.Length - 1
                                        || j == 0
                                        || j == yArray.Length - 1
                                        || k == 0
                                        || k == zArray.Length - 1,
                                        MeshIndex = new int[3] { i, j, k }
                                    };

                                    if (BoundaryType == BoundaryType.ConvexHullXY)
                                    {
                                        if (!convexHull.ContainsXY(pt))
                                        {
                                            pt.IsActive = false;
                                        }
                                    }

                                    DiscretizedLocationValues.Vertices.Add(pt);
                                    break;
                                //Creates a irregular grid
                                case DiscretizationMethod.Random:

                                    LocationTimeValue loc = new LocationTimeValue();
                                    try
                                    {
                                        double r1 = Convert.ToDouble(random.Next(1, 100)) / 100;
                                        double x = i != 0 && i != xArray.Length - 1 ? r1 * xDiff + xArray[i - 1] : xArray[i];

                                        double r2 = Convert.ToDouble(random.Next(1, 100)) / 100;
                                        double y = j != 0 && j != yArray.Length - 1 ? r2 * yDiff + yArray[j - 1] : yArray[j];

                                        double r3 = Convert.ToDouble(random.Next(1, 100)) / 100;
                                        double z = k != 0 && k != zArray.Length - 1 ? r3 * zDiff + zArray[k - 1] : zArray[k];

                                        //the newly created point
                                        loc = new LocationTimeValue()
                                        {
                                            X = x,
                                            Y = y,
                                            Z = z,
                                            Geographic = false,
                                            IsDiscretized = true,
                                            IsExterior = i == 0
                                            || i == xArray.Length - 1
                                            || j == 0
                                            || j == yArray.Length - 1
                                            || k == 0
                                            || k == zArray.Length - 1,
                                            MeshIndex = new int[3] { i, j, k }
                                        };

                                        if (BoundaryType == BoundaryType.ConvexHullXY)
                                        {
                                            if (convexHull.ContainsXY(loc))
                                            {
                                                loc.IsActive = false;
                                            }
                                        }

                                        DiscretizedLocationValues.Vertices.Add(loc);
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                    break;
                                default:
                                    break;

                            }
                        }
                    }
                }
            else if (BoundaryType == BoundaryType.TwoBoundingSurfaces)
            {
                if (SelectedMeasPoints.Count() != 2)
                    return;


                if (SelectedMeasPoints[0].Vertices.Count() != SelectedMeasPoints[1].Vertices.Count())
                    return;

                //Getting the min and max index of each dimension
                int xMinIndex = SelectedMeasPoints[0].Vertices.Min(x => x.MeshIndex[0]);
                int xMaxIndex = SelectedMeasPoints[0].Vertices.Max(x => x.MeshIndex[0]);
                int yMinIndex = SelectedMeasPoints[0].Vertices.Min(x => x.MeshIndex[1]);
                int yMaxIndex = SelectedMeasPoints[0].Vertices.Max(x => x.MeshIndex[1]);
                int zMinIndex = SelectedMeasPoints[0].Vertices.Min(x => x.MeshIndex[2]);
                int zMaxIndex = SelectedMeasPoints[0].Vertices.Max(x => x.MeshIndex[2]);

                //Checking what dimension is zero
                string surfaceDimension = (xMinIndex == xMaxIndex ? "x" : (yMinIndex == yMaxIndex ? "y" : zMinIndex == zMaxIndex ? "z" : ""));

                for (int i = 0; i < (xMaxIndex == xMinIndex ? 1 : xMaxIndex); i++)
                {
                    for (int j = 0; j < (yMaxIndex == yMinIndex ? 1 : yMaxIndex); j++)
                    {
                        for(int k = 0; k<(zMaxIndex == zMinIndex ? 1 : zMaxIndex); k++)
                        {
                            //Determining the points that should be connected
                            LocationTimeValue loc1 = SelectedMeasPoints[0].Vertices.Where(x => x.MeshIndex[0] == i && x.MeshIndex[1] == j && x.MeshIndex[2] == k).First();
                            LocationTimeValue loc2 = SelectedMeasPoints[1].Vertices.Where(x => x.MeshIndex[0] == i && x.MeshIndex[1] == j && x.MeshIndex[2] == k).First();

                            //Determining the lower x,y and z value
                            double lowerX = loc1.X < loc2.X ? loc1.X : loc2.X;
                            double lowerY = loc1.Y < loc2.Y ? loc1.Y : loc2.Y;
                            double lowerZ = loc1.Z < loc2.Z ? loc1.Z : loc2.Z;

                            //Determining the spatial diffences between those points
                            double differenceX = Math.Abs(loc1.X - loc2.X);
                            double differenceY = Math.Abs(loc1.Y - loc2.Y);
                            double differenceZ = Math.Abs(loc1.Z - loc2.Z);

                            //Calculating the step width
                            double stepWidthX = surfaceDimension == "x" ? differenceX / BinsX : differenceX / SelectedMeasPoints[0].Vertices.Select(x => x.MeshIndex[0]).Max(x => x);
                            double stepWidthY = surfaceDimension == "y" ? differenceY / BinsY : differenceY / SelectedMeasPoints[0].Vertices.Select(x => x.MeshIndex[1]).Max(x => x);
                            double stepWidthZ = surfaceDimension == "z" ? differenceZ / BinsZ : differenceZ / SelectedMeasPoints[0].Vertices.Select(x => x.MeshIndex[2]).Max(x => x); ;

                            //Iterating over the array that should be determined
                            for (int l = 0; l < (surfaceDimension == "x" ? xArray.Length : (surfaceDimension == "y" ? yArray.Length : (surfaceDimension == "z" ? zArray.Length : 1))) ; l++)
                            {
                                //Creating the new point as a function of the boundary points
                                LocationTimeValue locBetween = new
                                    LocationTimeValue(
                                    lowerX + (surfaceDimension == "x" ? l * stepWidthX : i * stepWidthX),
                                    lowerY + (surfaceDimension == "y" ? l * stepWidthY : j * stepWidthY),
                                    lowerZ + (surfaceDimension == "z" ? l * stepWidthZ : k * stepWidthZ))
                                {
                                    Geographic = false,
                                    IsDiscretized = true,
                                    IsExterior = (surfaceDimension == "x" ? l == xMinIndex : i == xMinIndex)
                                            || (surfaceDimension == "x" ? l == xArray.Length - 1 : i == xMaxIndex - 1)
                                            || (surfaceDimension == "y" ? l == yMinIndex : i == yMinIndex)
                                            || (surfaceDimension == "y" ? l == yArray.Length - 1 : j == yMaxIndex - 1)
                                            || (surfaceDimension == "z" ? l == zMinIndex : i == zMinIndex)
                                            || (surfaceDimension == "z" ? l == zArray.Length - 1 : k == zMaxIndex - 1)
                                };
                                //new index
                                locBetween.MeshIndex = new int[3] 
                                {
                                    surfaceDimension == "x" ? l : loc1.MeshIndex[0],
                                    surfaceDimension == "y" ? l : loc1.MeshIndex[1],
                                    surfaceDimension == "z" ? l : loc1.MeshIndex[2] };

                                DiscretizedLocationValues.Vertices.Add(locBetween);
                            }

                        }
                    }
                }
            }

            if (DiscretizedLocationValues.Dimensionality == Dimensionality.ThreeD)
                DiscretizedLocationValues.CellsFromPointCloud();
            else if (DiscretizedLocationValues.Dimensionality == Dimensionality.TwoD)
                DiscretizedLocationValues.FacesFromPointCloud();

        }

        /// <summary>
        /// Computes a spatial operation
        /// </summary>
        public void ComputeOperation()
        {
            try
            {
                //Checking for identical dimensionality and cell type
                if (!SameDimensionality() || !SameMeshCellType())
                {
                    DiscretizedLocationValues = new Mesh();
                    return;
                }
                else
                {
                    DiscretizedLocationValues = new Mesh()
                    {
                        Dimensionality = SelectedMeasPoints[0].Dimensionality
                    };
                }

                //Conduct the operation based on the type
                switch (SpatialOperationType)
                {
                    case SpatialOperationType.Join:
                        //Adding all vertices, cells and faces from one mesh to a base mesh
                        int xMaxIndex = 0;
                        int yMaxIndex = 0;
                        int zMaxIndex = 0;

                        for (int i = 0; i < SelectedMeasPoints.Count(); i++)
                        {
                            Mesh meshToAdd = new Mesh(SelectedMeasPoints[i]);

                            //Adapting indices for the mesh
                            if (i != 0)
                            {
                                zMaxIndex += SelectedMeasPoints[i-1].Vertices.Max(x => x.MeshIndex[2]);

                                for (int j = 0; j < meshToAdd.Vertices.Count(); j++)
                                {
                                    meshToAdd.Vertices[j].MeshIndex = new int[]
                                        {
                                            meshToAdd.Vertices[j].MeshIndex[0],
                                            meshToAdd.Vertices[j].MeshIndex[1],
                                            meshToAdd.Vertices[j].MeshIndex[2] + zMaxIndex + 2
                                        };
                                }
                            }

                            zMaxIndex += meshToAdd.Vertices.Max(x => x.MeshIndex[2]);

                            DiscretizedLocationValues.Vertices.AddRange(meshToAdd.Vertices);

                            DiscretizedLocationValues.Data.Merge(meshToAdd.Data);

                        }
                        break;
                }

                if (SelectedMeasPoints[0].Dimensionality == Dimensionality.ThreeD)
                    DiscretizedLocationValues.CellsFromPointCloud();
                else if (SelectedMeasPoints[0].Dimensionality == Dimensionality.TwoD)
                    DiscretizedLocationValues.FacesFromPointCloud();


            }
            catch
            {

            }
        }

        /// <summary>
        /// Determines the dimensionality of the data set
        /// </summary>
        /// <returns></returns>
        private Dimensionality GetDimensionality()
        {
            int dimensionality = 0;

            dimensionality += BinsX != 0 ? 1 : 0;
            dimensionality += BinsY != 0 ? 1 : 0;
            dimensionality += BinsZ != 0 ? 1 : 0;

            Dimensionality dim = Dimensionality.OneD;

            if (dimensionality == 0)
                dim = Dimensionality.OneD;
            else if (dimensionality == 1)
                dim = Dimensionality.OneD;
            else if (dimensionality == 2)
                dim = Dimensionality.TwoD;
            else if (dimensionality == 3)
                dim = Dimensionality.ThreeD;

            return dim;
        }

        /// <summary>
        /// Determines the dimensionality of the data set
        /// </summary>
        /// <returns></returns>
        private bool SameDimensionality()
        {
            bool isSame = true;
            Dimensionality previous = Dimensionality.OneD;

            for (int i = 0; i < SelectedMeasPoints.Count(); i++)
            {
                if (i == 0)
                    previous = SelectedMeasPoints[i].Dimensionality;
                else
                {
                    isSame = SelectedMeasPoints[i].Dimensionality == previous;

                    if (isSame)
                        continue;
                    else
                        break;
                }

            }

            return isSame;
        }

        /// <summary>
        /// Determines the dimensionality of the data set
        /// </summary>
        /// <returns></returns>
        private bool SameMeshCellType()
        {
            bool isSame = true;
            MeshCellType previous = MeshCellType.Combined;

            for (int i = 0; i < SelectedMeasPoints.Count(); i++)
            {
                if (i == 0)
                    previous = SelectedMeasPoints[i].MeshCellType;
                else
                {
                    isSame = SelectedMeasPoints[i].MeshCellType == previous;

                    if (isSame)
                        continue;
                    else
                        break;
                }

            }

            return isSame;
        }
    }
}
