using Caliburn.Micro;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace GeoReVi
{
    /// <summary>
    /// A 3D plane
    /// </summary>
    public class Plane3D : PropertyChangedBase
    {
        #region Public properties

        /// <summary>
        /// Center of the slicing plane
        /// </summary>
        private Vector3DClass center = new Vector3DClass(0, 0, 0);
        public Vector3DClass Center
        {
            get => this.center;
            set
            {
                this.center = value;
                NotifyOfPropertyChange(() => Center);
            }
        }

        /// <summary>
        /// Center of the slicing plane
        /// </summary>
        private Vector3DClass normal = new Vector3DClass(0, 0, 1);
        public Vector3DClass Normal
        {
            get => this.normal;
            set
            {
                this.normal = value;
                NotifyOfPropertyChange(() => Normal);
            }
        }

        /// <summary>
        /// Range of the plane
        /// </summary>
        private int range = 1;
        public int Range
        {
            get => this.range;
            set
            {
                this.range = value;
                NotifyOfPropertyChange(() => Range);
            }
        }

        /// <summary>
        /// If slice plane is active
        /// </summary>
        private bool isActive = false;
        public bool IsActive
        {
            get => this.isActive;
            set
            {
                this.isActive = value;
                NotifyOfPropertyChange(() => IsActive);
            }
        }

        /// <summary>
        /// Plane visibility
        /// </summary>
        private bool isvisible = false;
        public bool IsVisible
        {
            get => this.isvisible;
            set
            {
                this.isvisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }

        /// <summary>
        /// Relative orientation of the structures that should be shown relative to the plane
        /// </summary>
        private RelativeOrientation relativeOrientation = RelativeOrientation.Above;
        public RelativeOrientation RelativeOrientation
        {
            get => this.relativeOrientation;
            set
            {
                this.relativeOrientation = value;
                NotifyOfPropertyChange(() => RelativeOrientation);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Plane3D()
        {

        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns a double value p.
        /// IF p > 0 => point is below plane
        /// IF p < 0 => point is above plane
        /// If p== 0 => point is on plane
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public double PointRelativeToPlane(Vector3D pt)
        {
            return Vector3D.DotProduct(Normal.Vector, (Vector3D.Subtract(pt, Center.Vector)));
        }

        /// <summary>
        /// Calculating the plane constant d for the Cartesian form ax + by + cz + d = 0
        /// </summary>
        /// <returns></returns>
        public double GetPlaneConstant()
        {
            try
            {
                return Normal.Vector.Multiply(Center.Vector);
            }
            catch
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// Retúrning a list of points for the plane based on maximum and minimum values defining a bounding box
        /// </summary>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public IList<Point3D> GetBoundingPoints(Point3D max, Point3D min)
        {
            List<Point3D> points = new List<Point3D>();

            Point3D pt = new Point3D(min.X, min.Y, SolveForZ(min.X, min.Y));

            points.Add(pt);

            pt = new Point3D(max.X, min.Y, SolveForZ(max.X, min.Y));

            points.Add(pt);

                points.Add(new Point3D(max.X, max.Y, SolveForZ(max.X, max.Y)));
                points.Add(new Point3D(min.X, max.Y, SolveForZ(min.X, max.Y)));

            return points;
        }

        //Solving for z coordinate at given X and Y
        public double SolveForZ(double x, double y)
        {
            return (GetPlaneConstant() - (Normal.X * x) - (Normal.Y * y)) / Normal.Z;
        }

        //Solving for y coordinate at given X and Z
        public double SolveForY(double x, double z)
        {
            return (GetPlaneConstant() - (Normal.X * x) - (Normal.Z * z)) / Normal.Y;
        }

        //Solving for x coordinate at given Y and Z
        public double SolveForX(double y, double z)
        {
            return (GetPlaneConstant() - (Normal.Y * y) - (Normal.Z * z)) / Normal.X;
        }

        #endregion

    }

    public class Vector3DClass : PropertyChangedBase
    {
        #region Public properties

        /// <summary>
        /// X coordinate
        /// </summary>
        private double x = 0;
        public double X
        {
            get => this.x;
            set
            {
                this.x = value;
                NotifyOfPropertyChange(() => X);
            }
        }

        /// <summary>
        /// Y coordinate
        /// </summary>
        private double y = 0;
        public double Y
        {
            get => this.y;
            set
            {
                this.y = value;
                NotifyOfPropertyChange(() => Y);
            }
        }

        /// <summary>
        /// Z coordinate
        /// </summary>
        private double z = 0;
        public double Z
        {
            get => this.z;
            set
            {
                this.z = value;
                NotifyOfPropertyChange(() => Z);
            }
        }

        /// <summary>
        /// Vector representation
        /// </summary>
        public Vector3D Vector
        {
            get => new Vector3D(X, Y, Z);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Vector3DClass()
        {

        }

        /// <summary>
        /// Specific constructor
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_z"></param>
        public Vector3DClass(double _x, double _y, double _z)
        {
            this.X = _x;
            this.Y = _y;
            this.Z = _z;
        }

        #endregion
    }

    /// <summary>
    /// Relative orientation
    /// </summary>
    public enum RelativeOrientation
    {
        Above = 1,
        Below = 2,
        Intersect = 3,
        Contained = 4
    }
}
