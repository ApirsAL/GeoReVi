using Caliburn.Micro;
using MIConvexHull;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;

namespace GeoReVi
{
    public class Mesh : PropertyChangedBase, IGeometry
    {
        #region Public properties

        private LocationTimeValue[,,] locs = new LocationTimeValue[,,] { };

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

        /// <summary>
        /// Dimensionality of the mesh
        /// </summary>
        private Dimensionality dimensionality = Dimensionality.ThreeD;
        public Dimensionality Dimensionality
        {
            get => this.dimensionality;
            set
            {
                this.dimensionality = value;
                NotifyOfPropertyChange(() => Dimensionality);
            }
        }

        /// <summary>
        /// Cell type of the mesh
        /// </summary>
        private MeshCellType meshCellType = MeshCellType.Hexahedral;
        public MeshCellType MeshCellType
        {
            get => this.meshCellType;
            set
            {
                this.meshCellType = value;
                NotifyOfPropertyChange(() => MeshCellType);
            }
        }

        /// <summary>
        /// The 3D model representation of the mesh
        /// </summary>
        private Model3DGroup model;
        [XmlIgnore]
        public Model3DGroup Model
        {
            get => this.model;
            set
            {
                this.model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }

        /// <summary>
        /// Cells of the mesh
        /// </summary>
        private ObservableCollection<Cell> cells = new ObservableCollection<Cell>();
        public ObservableCollection<Cell> Cells
        {
            get => this.cells;
            set
            {
                this.cells = value;
                NotifyOfPropertyChange(() => Cells);
            }
        }

        /// <summary>
        /// Vertices of the mesh
        /// </summary>
        private ObservableCollection<LocationTimeValue> vertices = new ObservableCollection<LocationTimeValue>();
        public ObservableCollection<LocationTimeValue> Vertices
        {
            get => this.vertices;
            set
            {
                this.vertices = value;
            }
        }

        /// <summary>
        /// Faces of the mesh
        /// </summary>
        private ObservableCollection<Face> faces = new ObservableCollection<Face>();
        public ObservableCollection<Face> Faces
        {
            get => this.faces;
            set
            {
                this.faces = value;
            }
        }

        /// <summary>
        /// Data associated with the grid
        /// </summary>
        private DataTable data = new DataTable() { TableName = "MyTableName" };
        public DataTable Data
        {
            get => this.data;
            set
            {
                this.data = value;
            }
        }

        /// <summary>
        /// The count of the cells.
        /// </summary>
        public int Count
        {
            get
            {
                return
                    Cells.Count();
            }
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Mesh()
        {

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Mesh(Mesh _mesh)
        {
            Dimensionality = _mesh.Dimensionality;
            Data = new DataTable();
            Data.Merge(_mesh.Data);
            Name = _mesh.Name.ToString();
            MeshCellType = _mesh.MeshCellType;

            Vertices = new ObservableCollection<LocationTimeValue>(_mesh.Vertices.Select( x => new LocationTimeValue(x)).ToList());
        }

        #endregion

        #region Public methods

        public void SetVerticesFromData()
        {
            try
            {

            }
            catch
            {

            }
        }

        /// <summary>
        /// Triangulating a set of 3D points
        /// </summary>
        /// <param name="radius">radius of the points</param>
        /// <param name="cm">colormap</param>
        /// <param name="filterValues"></param>
        /// <param name="minVisibility"></param>
        /// <param name="maxVisibility"></param>
        public void TriangulateBody()
        {
            // calculate the triangulation
            var a = Triangulation.CreateDelaunay<LocationTimeValue, TriangulationCellHelper>(Vertices, Math.Pow(1, -20)).Cells;

            Cells.Clear();

            foreach (var b in a)
            {
                Cells.Add(new Tetrahedron() { Vertices = b.Vertices.ToList(), CellType = CellType.Tetrahedon });
            }
        }

        /// <summary>
        /// Triangulates a set of points to a surface
        /// </summary>
        public void TriangulateSurface()
        {
            // calculate the triangulation
            var a = new TriangulationCellHelper().TriangulateSurface(Vertices.ToList());

            foreach (var b in a)
            {
                try
                {
                    if (b != null)
                        if (b.Vertices.Count() == 3)
                            Faces.Add(new Triangle(b.Vertices[0], b.Vertices[1], b.Vertices[2]));
                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Creates a set of faces from a 2D point cloud
        /// </summary>
        public void FacesFromPointCloud()
        {
            try
            {
                Faces.Clear();

                if (Dimensionality == Dimensionality.TwoD)
                {
                    switch (MeshCellType)
                    {
                        case MeshCellType.Rectangular:

                            int xMinIndex = Vertices.Min(x => x.MeshIndex[0]);
                            int xMaxIndex = Vertices.Max(x => x.MeshIndex[0]);
                            int yMinIndex = Vertices.Min(x => x.MeshIndex[1]);
                            int yMaxIndex = Vertices.Max(x => x.MeshIndex[1]);

                            //3D collection of vertices
                            LocationTimeValue[,] locs = new LocationTimeValue[(xMaxIndex - xMinIndex) + 1, (yMaxIndex - yMinIndex) +1];

                            //Adding vertices to the collection based on the count
                            for (int i = 0; i < Vertices.Count(); i++)
                                locs[Vertices[i].MeshIndex[0], Vertices[i].MeshIndex[1]] = Vertices[i];

                            for (int i = xMinIndex; i < xMaxIndex; i++)
                            {
                                for (int j = yMinIndex; j < yMaxIndex; j++)
                                {
                                    Quadrilateral quad = new Quadrilateral();

                                    quad.Vertices = new LocationTimeValue[4] { locs[i, j], locs[i, j + 1], locs[i + 1, j + 1], locs[i + 1, j] };

                                    quad.SortVertices();

                                    Faces.Add(quad);
                                }
                            }
                            break;
                        case MeshCellType.Triangular:
                            TriangulateSurface();
                            break;

                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Meshing the point cloud
        /// </summary>
        public void CellsFromPointCloud()
        {
            try
            {
                ObservableCollection<Cell> hexs = new ObservableCollection<Cell>();

                if (Dimensionality == Dimensionality.ThreeD)
                {
                    int xMinIndex = 0;
                    int xMaxIndex = 0;
                    int yMinIndex = 0;
                    int yMaxIndex = 0;
                    int zMinIndex = 0;
                    int zMaxIndex = 0;


                    xMinIndex = Vertices.Min(x => x.MeshIndex[0]);
                    xMaxIndex = Vertices.Max(x => x.MeshIndex[0]);
                    yMinIndex = Vertices.Min(x => x.MeshIndex[1]);
                    yMaxIndex = Vertices.Max(x => x.MeshIndex[1]);
                    zMinIndex = Vertices.Min(x => x.MeshIndex[2]);
                    zMaxIndex = Vertices.Max(x => x.MeshIndex[2]);

                    CreateVerticeArray();

                    for (int i = xMinIndex; i < xMaxIndex; i++)
                    {
                        for (int j = yMinIndex; j < yMaxIndex; j++)
                        {
                            for (int k = zMinIndex; k < zMaxIndex; k++)
                            {
                                try
                                {
                                    Hexahedron hex = new Hexahedron();

                                    hex.Vertices.Add(locs[i, j, k]);
                                    hex.Vertices.Add(locs[i, j + 1, k]);
                                    hex.Vertices.Add(locs[i, j + 1, k + 1]);
                                    hex.Vertices.Add(locs[i, j, k + 1]);
                                    hex.Vertices.Add(locs[i + 1, j, k]);
                                    hex.Vertices.Add(locs[i + 1, j + 1, k]);
                                    hex.Vertices.Add(locs[i + 1, j + 1, k + 1]);
                                    hex.Vertices.Add(locs[i + 1, j, k + 1]);

                                    if (hex.Vertices.Count() == 0)
                                        continue;

                                    if (hex.Vertices.Any(x => x == null))
                                        continue;

                                    if (!hex.Vertices.Any(x => !x.IsExterior))
                                        continue;

                                    if (MeshCellType == MeshCellType.Hexahedral)
                                        Cells.Add(hex);
                                    else if (MeshCellType == MeshCellType.Tetrahedal)
                                        Cells.AddRange(hex.ToTetrahedons());

                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Creating an index array of the vertices for faster searches
        /// </summary>
        /// <returns></returns>
        public async Task CreateVerticeArray()
        {
            try
            {
                int xMinIndex = 0;
                int xMaxIndex = 0;
                int yMinIndex = 0;
                int yMaxIndex = 0;
                int zMinIndex = 0;
                int zMaxIndex = 0;


                xMinIndex = Vertices.Min(x => x.MeshIndex[0]);
                xMaxIndex = Vertices.Max(x => x.MeshIndex[0]);
                yMinIndex = Vertices.Min(x => x.MeshIndex[1]);
                yMaxIndex = Vertices.Max(x => x.MeshIndex[1]);
                zMinIndex = Vertices.Min(x => x.MeshIndex[2]);
                zMaxIndex = Vertices.Max(x => x.MeshIndex[2]);

                //3D collection of vertices
                this.locs = new LocationTimeValue[(xMaxIndex - xMinIndex) + 1, (yMaxIndex - yMinIndex) + 1, (zMaxIndex - zMinIndex) + 1];

                //Adding vertices to the collection based on the count
                for (int i = 0; i < Vertices.Count(); i++)
                {
                    locs[Vertices[i].MeshIndex[0], Vertices[i].MeshIndex[1], Vertices[i].MeshIndex[2]] = Vertices[i];
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Calculates the volume of the mesh
        /// </summary>
        /// <returns></returns>
        public double GetVolume()
        {
            //returning value
            double ret = 0;

            try
            {
                for (int i = 0; i < Cells.Count(); i++)
                    ret += Cells[i].GetVolume();
            }
            catch
            {
                ret = 0;
            }

            return ret;
        }

        /// <summary>
        /// Calculating the gradient of function at a point
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public double[] CalculateGradientFunction(LocationTimeValue loc)
        {
            double[] grad = new double[3] { 0, 0, 0 };

            try
            {
                LocationTimeValue[] locs = GetNeighbors(loc);

                for (int i = 0; i < locs.Length; i++)
                {
                    grad[0] += loc.X != locs[i].X ? (loc.Value[0] - locs[i].Value[0]) / Math.Abs(loc.X - locs[i].X) : 0;
                    grad[1] += loc.Y != locs[i].Y ? (loc.Value[0] - locs[i].Value[0]) / Math.Abs(loc.Y - locs[i].Y) : 0;
                    grad[2] += loc.Z != locs[i].Z ? (loc.Value[0] - locs[i].Value[0]) / Math.Abs(loc.Z - locs[i].Z) : 0;
                }
            }
            catch
            {

            }

            return grad;
        }

        /// <summary>
        /// Calculating the gradient of a point
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public Vector3D CalculateGradient2D(LocationTimeValue loc)
        {
            Vector3D grad = new Vector3D();

            try
            {
                LocationTimeValue[] locs = GetNeighbors(loc);

                for (int i = 0; i < locs.Length; i++)
                {
                    grad.X += loc.X != locs[i].X ? Math.Abs(loc.Z - locs[i].Z) / Math.Abs(loc.X - locs[i].X) : 0;
                    grad.Y += loc.Y != locs[i].Y ? Math.Abs(loc.Z - locs[i].Z) / Math.Abs(loc.Y - locs[i].Y) : 0;
                    grad.Z += 0;
                }
            }
            catch
            {

            }

            return grad;
        }

        /// <summary>
        /// Getting the neighbor nodes of a node
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public LocationTimeValue[] GetNeighbors(LocationTimeValue loc)
        {
            List<LocationTimeValue> retLocs = new List<LocationTimeValue>();

            try
            {
                int indexX = loc.MeshIndex[0];
                int indexY = loc.MeshIndex[1];
                int indexZ = loc.MeshIndex[2];


                
                //Checking if the correct count of points is returned
                switch(Dimensionality)
                {
                    case Dimensionality.TwoD:

                        retLocs.Add(locs[indexX - 1, indexY, indexZ]);
                        retLocs.Add(locs[indexX, indexY - 1, indexZ]);
                        retLocs.Add(locs[indexX + 1, indexY, indexZ]);
                        retLocs.Add(locs[indexX, indexY + 1, indexZ]);

                        if (retLocs.Count() > 4 && !loc.IsExterior)
                        {
                            retLocs = retLocs.OrderBy(x => x.GetEuclideanDistance(loc)).Take(4).ToList();
                        }
                        break;
                    case Dimensionality.ThreeD:

                        retLocs.Add(locs[indexX - 1, indexY, indexZ]);
                        retLocs.Add(locs[indexX, indexY - 1, indexZ]);
                        retLocs.Add(locs[indexX, indexY, indexZ - 1]);
                        retLocs.Add(locs[indexX + 1, indexY, indexZ]);
                        retLocs.Add(locs[indexX, indexY + 1, indexZ]);
                        retLocs.Add(locs[indexX, indexY, indexZ + 1]);

                        if (retLocs.Count() > 6 && !loc.IsExterior)
                        {
                            retLocs = retLocs.OrderBy(x => x.GetEuclideanDistance(loc)).Take(6).ToList();
                        }
                        else if (retLocs.Count() > 4 && loc.IsExterior)
                        {
                            retLocs = retLocs.OrderBy(x => x.GetEuclideanDistance(loc)).Take(4).ToList();
                        }
                        break;
                }
            }
            catch
            {
                retLocs = new List<LocationTimeValue>();
            }

            return retLocs.ToArray();
        }

        /// <summary>
        /// Adds a parameter to the list of values
        /// </summary>
        /// <param name="parameter"></param>
        public void AddParameter(Mesh parameter)
        {
            try
            {
                for(int i = 0; i<Vertices.Count();i++)
                {
                    Vertices[i].Value.Add(parameter.Vertices.Where(x => x.Equals(Vertices[i])).First().Value[0]);
                }
            }
            catch
            {
                throw new Exception("Meshes are not identical");
            }
        }

        /// <summary>
        /// Adds a constant parameter to the list of values
        /// </summary>
        /// <param name="parameter"></param>
        public void AddParameter(double parameter)
        {
            try
            {
                for (int i = 0; i < Vertices.Count(); i++)
                {
                    Vertices[i].Value.Add(parameter);
                }
            }
            catch
            {
                throw new Exception("Meshes are not identical");
            }
        }

        #endregion

    }
}
