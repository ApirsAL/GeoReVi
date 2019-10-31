
using System;

namespace GeoReVi
{
    /// <summary>
    /// A class for a location with a value (double or string)
    /// </summary>
    public class LocationTimeValue<T> : ILocationTimeValue<T> where T : struct
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
        private T value;

        //Check if geographic or geometric
        private bool geographic;

        //check if values are discretized
        private bool isDiscretized;

        //Name
        private string name;

        #endregion

        #region Public propeties

        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        public double Z
        {
            get { return this.z; }
            set { this.z = value; }
        }

        public DateTime Date
        {
            get { return this.date; }
            set { this.date = value; }
        }

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

        public T Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public string Name
        {
            get => name;
            set => this.name = value;
        }

        #endregion

    }
}
