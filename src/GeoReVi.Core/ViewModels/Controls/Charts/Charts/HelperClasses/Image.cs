using Caliburn.Micro;
using System.Windows.Media.Media3D;

namespace GeoReVi
{
    public class Image : PropertyChangedBase
    {
        #region Public properties

        /// <summary>
        /// Path to the image
        /// </summary>
        private string imagePath = "";
        public string ImagePath
        {
            get => this.imagePath;
            set
            {
                this.imagePath = value;
                NotifyOfPropertyChange(() => ImagePath);
            }
        }

        /// <summary>
        /// Lower left point coordinate
        /// </summary>
        private LocationTimeValue lowerLeft = new LocationTimeValue(0,0,0);
        public LocationTimeValue LowerLeft
        {
            get => this.lowerLeft;
            set
            {
                this.lowerLeft = value;
                NotifyOfPropertyChange(() => LowerLeft);
            }
        }

        /// <summary>
        /// Upper left point coordinate
        /// </summary>
        private LocationTimeValue upperLeft = new LocationTimeValue(100, 0, 0);
        public LocationTimeValue UpperLeft
        {
            get => this.upperLeft;
            set
            {
                this.upperLeft = value;
                NotifyOfPropertyChange(() => UpperLeft);
            }
        }

        /// <summary>
        /// Lower right point coordinate
        /// </summary>
        private LocationTimeValue lowerRight = new LocationTimeValue(0, 100, 0);
        public LocationTimeValue LowerRight
        {
            get => this.lowerRight;
            set
            {
                this.lowerRight = value;
                NotifyOfPropertyChange(() => LowerRight);
            }
        }

        /// <summary>
        /// Upper right point coordinate
        /// </summary>
        private LocationTimeValue upperRight = new LocationTimeValue(100, 100, 0);
        public LocationTimeValue UpperRight
        {
            get => this.upperRight;
            set
            {
                this.upperRight = value;
                NotifyOfPropertyChange(() => UpperRight);
            }
        }

        #endregion
    }
}
