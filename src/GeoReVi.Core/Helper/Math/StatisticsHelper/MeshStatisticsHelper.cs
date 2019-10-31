using Caliburn.Micro;
using System.Linq;

namespace GeoReVi
{
    public class MeshStatisticsHelper : PropertyChangedBase
    {

        #region Properties

        private Mesh _mesh = new Mesh();

        /// <summary>
        /// Volume of the grid
        /// </summary>
        private double volume = 0;
        public double Volume
        {
            get => this.volume;
            private set
            {
                this.volume = value;
                NotifyOfPropertyChange(() => Volume);
            }
        }

        /// <summary>
        /// Average volume of the cells
        /// </summary>
        private double averageCellVolume = 0;
        public double AverageCellVolume
        {
            get => this.averageCellVolume;
            private set
            {
                this.averageCellVolume = value;
                NotifyOfPropertyChange(() => AverageCellVolume);
            }
        }

        /// <summary>
        /// Count of vertices
        /// </summary>
        private int countVertices = 0;
        public int CountVertices
        {
            get => this.countVertices;
            private set
            {
                this.countVertices = value;
                NotifyOfPropertyChange(() => CountVertices);
            }
        }

        /// <summary>
        /// Count of cells
        /// </summary>
        private int countCells = 0;
        public int CountCells
        {
            get => this.countCells;
            private set
            {
                this.countCells = value;
                NotifyOfPropertyChange(() => CountCells);
            }
        }

        /// <summary>
        /// Count of faces
        /// </summary>
        private int countFaces = 0;
        public int CountFaces
        {
            get => this.countFaces;
            private set
            {
                this.countFaces = value;
                NotifyOfPropertyChange(() => CountFaces);
            }
        }

        /// <summary>
        /// Dimensionality of the mesh
        /// </summary>
        private int dimensionality = 0;
        public int Dimensionality
        {
            get => this.dimensionality;
            private set
            {
                this.dimensionality = value;
                NotifyOfPropertyChange(() => Dimensionality);
            }
        }

        /// <summary>
        /// Name of the mesh
        /// </summary>
        private string name = "";
        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        #region Constructor

        /// <summary>
        /// Specific constructor
        /// </summary>
        /// <param name="mesh"></param>
        public MeshStatisticsHelper(Mesh mesh)
        {
            _mesh = mesh;
            Compute();
        }

        #endregion

        /// <summary>
        /// Compute
        /// </summary>
        public void Compute()
        {
            try
            {
                Name = _mesh.Name;
                Volume = _mesh.GetVolume();
                CountVertices = _mesh.Vertices.Count();
                CountCells = _mesh.Cells.Count();
                CountFaces = _mesh.Faces.Count();
                AverageCellVolume = Volume / CountVertices != 0 ? (double)CountVertices : 1;

                if (_mesh.Dimensionality == GeoReVi.Dimensionality.OneD)
                    Dimensionality = 1;
                else if (_mesh.Dimensionality == GeoReVi.Dimensionality.TwoD)
                    Dimensionality = 2;
                else if (_mesh.Dimensionality == GeoReVi.Dimensionality.ThreeD)
                    Dimensionality = 3;
            }
            catch
            {
                return;
            }
        }

        #endregion

        #region Helper methods


        #endregion
    }
}
