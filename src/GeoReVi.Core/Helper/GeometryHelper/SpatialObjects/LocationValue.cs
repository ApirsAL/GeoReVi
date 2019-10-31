
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;

namespace GeoReVi
{
    /// <summary>
    /// A class for a location with a generic value
    /// </summary>
    [Serializable]
    public class LocationTimeValue : ILocationTimeValue
    {
        #region Private members

        /// <summary>
        /// Location
        /// </summary>
        private double x;
        private double y;
        private double z;
        private DateTime date;

        /// <summary>
        /// Value
        /// </summary>
        private List<double> value = new List<double>() { 0, 0 };

        //Check if geographic or geometric
        private bool geographic;

        //check if values are discretized
        private bool isDiscretized;

        //Name
        private string name;

        //Brush
        private Brush brush = Brushes.Black;

        //Is exterior
        private bool isExterior = false;

        List<LocationTimeValue> neighbors = new List<LocationTimeValue>();

        private int[] meshIndex = new int[3] { 0, 0, 0 };

        //Check if geographic or geometric
        private bool isActive = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LocationTimeValue()
        {

        }

        /// <summary>
        /// Explicit constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="name"></param>
        public LocationTimeValue(double x = 0, double y = 0, double z = 0, string name = "default", double value1 = 0)
        {
            X = x;
            Y = y;
            Z = z;
            Name = name;
            Value[0] = value1;
        }

        /// <summary>
        /// Specific constructor
        /// </summary>
        public LocationTimeValue(LocationTimeValue loc)
        {
            X = loc.X;
            Y = loc.Y;
            Z = loc.Z;
            loc.MeshIndex.CopyTo(MeshIndex, 0);
            Name = loc.name;
            Date = loc.Date;
            Geographic = loc.Geographic;
            IsActive = loc.IsActive;
            IsExterior = loc.IsExterior;
            Value = new List<double>(loc.Value);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Local x value/longitude respectively
        /// </summary>
        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        /// <summary>
        /// Local y value/latitude respectively
        /// </summary>
        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        /// <summary>
        /// Local height/altitude respectively
        /// </summary>
        public double Z
        {
            get { return this.z; }
            set { this.z = value; }
        }

        /// <summary>
        /// Date and time of the point value
        /// </summary>
        [XmlIgnore]
        public DateTime Date
        {
            get { return this.date; }
            set { this.date = value; }
        }

        /// <summary>
        /// Check whether geographic or metric representation
        /// </summary>
        public bool Geographic
        {
            get { return this.geographic; }
            set { this.geographic = value; }
        }

        public bool IsDiscretized
        {
            get { return this.isDiscretized; }
            set { this.isDiscretized = value; }
        }

        /// <summary>
        /// The value, the point is holding
        /// </summary>
        public List<double> Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// The name, the point is holding
        /// </summary>
        public string Name
        {
            get => name;
            set => this.name = value;
        }

        /// <summary>
        /// The value, the point is holding
        /// </summary>
        public int[] MeshIndex
        {
            get { return this.meshIndex; }
            set { this.meshIndex = value; }
        }

        /// <summary>
        /// Getter method to retrieve the position as a double array
        /// </summary>
        public double[] Position
        {
            get => new double[] { X, Y, Z };
        }

        /// <summary>
        /// Brush of the point
        /// </summary>
        [XmlIgnore]
        public Brush Brush
        {
            get => this.brush;
            set { this.brush = value; }
        }

        /// <summary>
        /// Is exterior in grid
        /// </summary>
        public bool IsExterior
        {
            get => this.isExterior;
            set { this.isExterior = value; }
        }

        /// <summary>
        /// Checks if a node is active or not
        /// </summary>
        public bool IsActive
        {
            get => this.isActive;
            set { this.isActive = value; }
        }

        /// <summary>
        /// Neighbor nodes
        /// </summary>
        public List<LocationTimeValue> Neighbors
        {
            get => this.neighbors;
            set => this.neighbors = value;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Converts the spatial properties to a Point3D object
        /// </summary>
        /// <returns></returns>
        public Point3D ToPoint3D()
        {
            return new Point3D(Position[0], Position[1], Position[2]);
        }

        /// <summary>
        /// Converts the spatial properties to a Point3D object
        /// </summary>
        /// <returns></returns>
        public Vector3D ToVector3D()
        {
            return new Vector3D(Position[0], Position[1], Position[2]);
        }

        /// <summary>
        /// Clones the actual object
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return new LocationTimeValue()
            {
                X = this.X,
                Y = this.Y,
                Z = this.Z,
                Date = this.Date,
                Geographic = this.Geographic,
                Name = this.Name,
                Value = this.Value,
                Brush = this.Brush,
                IsExterior = this.IsExterior,
                Neighbors = this.Neighbors
            };
        }

        /// <summary>
        /// Comparing method
        /// </summary>
        /// <param name="other">Other points</param>
        /// <returns></returns>
        bool IEquatable<LocationTimeValue>.Equals(LocationTimeValue other)
        {
            return other.X == this.X &&
                other.Y == this.Y &&
                other.Z == this.Z &&
                other.Date == this.Date &&
                other.Geographic == this.Geographic &&
                other.Name == this.Name &&
                other.Value == this.Value &&
                other.Brush == this.Brush &&
                other.IsExterior == this.IsExterior &&
                other.Neighbors == this.Neighbors;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Operator overloading
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static LocationTimeValue operator -(LocationTimeValue a, LocationTimeValue b)
        {
            return new LocationTimeValue()
            {
                X = a.X - b.X,
                Y = a.Y - b.Y,
                Z = a.Z - b.Z
            };
        }

        /// <summary>
        /// Compares the x value for sorting
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(LocationTimeValue other)
        {
            if (this.X > other.x) { return 1; }
            else if (this.X < other.x) { return -1; }
            else { return 0; }
        }
        #endregion
    }
}
