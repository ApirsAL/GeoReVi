using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class LithologicalLayer : Layer
    {
        /// <summary>
        /// Grain size of the base
        /// </summary>
        private double grainSizeBase = 1;
        public double GrainSizeBase
        {
            get => this.grainSizeBase;
            set
            {
                this.grainSizeBase = value;
                NotifyOfPropertyChange(() => GrainSizeBase);
            }
        }

        /// <summary>
        /// Grain size of the top
        /// </summary>
        private double grainSizeTop = 1;
        public double GrainSizeTop
        {
            get => this.grainSizeTop;
            set
            {
                this.grainSizeTop = value;
                NotifyOfPropertyChange(() => GrainSizeTop);
            }
        }
    }
}
