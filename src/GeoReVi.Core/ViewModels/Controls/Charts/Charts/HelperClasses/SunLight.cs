using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class SunLight : PropertyChangedBase
    {
        #region Properties

        /// <summary>
        /// Altitude
        /// </summary>
        private double altitude = 90;
        public double Altitude
        {
            get => this.altitude;
            set
            {
                this.altitude = value;
                NotifyOfPropertyChange(() => Altitude);
            }
        }

        /// <summary>
        /// Azimuth
        /// </summary>
        private double azimuth = 0;
        public double Azimuth
        {
            get => this.azimuth;
            set
            {
                this.azimuth = value;
                NotifyOfPropertyChange(() => Azimuth);
            }
        }

        /// <summary>
        /// Brightness
        /// </summary>
        private double brightness = 0.01;
        public double Brightness
        {
            get => this.brightness;
            set
            {
                this.brightness = value;
                NotifyOfPropertyChange(() => Brightness);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Specific constructor
        /// </summary>
        /// <param name="_altitude"></param>
        /// <param name="_azimuth"></param>
        /// <param name="_brightness"></param>
        public SunLight(double _altitude = 90, double _azimuth = 0, double _brightness = 0.1)
        {
            Altitude = _altitude;
            Azimuth = _azimuth;
            Brightness = _brightness;
        }

        #endregion
    }
}
