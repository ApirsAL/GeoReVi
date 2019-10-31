using Caliburn.Micro;

namespace GeoReVi
{
    public class MatrixSeries : PropertyChangedBase
    {
        //Point collection that build up the line series
        public double[,] LinePoints { get; set; }

        public double Spacing { get; set; }

        public MatrixSeries()
        {

        }
    }
}
