﻿using Caliburn.Micro;
using MathNet.Numerics.LinearAlgebra;
using MIConvexHull;
using MoreLinq;
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

        #region Private Properties

        /// <summary>
        /// Time steps resulting from a numerical solution in seconds
        /// </summary>
        private List<double> timeSteps = new List<double>();
        [XmlIgnore]
        private List<double> TimeSteps
        {
            get => this.timeSteps;
            set
            {
                this.timeSteps = value;
                NotifyOfPropertyChange(() => TimeSteps);
            }
        }

        /// <summary>
        /// Vertice values resulting from a numerical solution
        /// </summary>
        private double[,] timeStepValues = new double[,] { };
        [XmlIgnore]
        public double[,] TimeStepValues
        {
            get => this.timeStepValues;
            set
            {
                this.timeStepValues = value;
                NotifyOfPropertyChange(() => TimeStepValues);
            }
        }

        /// <summary>
        /// The properties of the mesh
        /// </summary>
        private BindableCollection<KeyValuePair<int, string>> properties = new BindableCollection<KeyValuePair<int, string>>();
        [XmlIgnore]
        public BindableCollection<KeyValuePair<int, string>> Properties
        {
            get => this.properties;
            set
            {
                this.properties = value;
                NotifyOfPropertyChange(() => Properties);
            }
        }

        /// <summary>
        /// The selected property of the mesh
        /// </summary>
        private KeyValuePair<int, string> selectedProperty = new KeyValuePair<int, string>();
        [XmlIgnore]
        public KeyValuePair<int, string> SelectedProperty
        {
            get => this.selectedProperty;
            set
            {
                this.selectedProperty = value;
                NotifyOfPropertyChange(() => SelectedProperty);
            }
        }

        #endregion

        #region Public properties

        [XmlIgnore]
        public int[,,] locs = new int[,,] { };

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
        [XmlIgnore]
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
                NotifyOfPropertyChange(() => Vertices);
            }
        }

        /// <summary>
        /// Faces of the mesh
        /// </summary>
        private ObservableCollection<Face> faces = new ObservableCollection<Face>();
        [XmlIgnore]
        public ObservableCollection<Face> Faces
        {
            get => this.faces;
            set
            {
                this.faces = value;
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
            Name = _mesh.Name.ToString();
            MeshCellType = _mesh.MeshCellType;
            Properties = new BindableCollection<KeyValuePair<int, string>>(_mesh.Properties);
            Vertices = new ObservableCollection<LocationTimeValue>(_mesh.Vertices.Select( x => new LocationTimeValue(x)).ToList());
        }

        #endregion

        #region Public methods

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
                Cells.Add(new Tetrahedron() { Vertices = b.Vertices.Select((v,i) => new { Vertice = v, index = i }).Select(x => x.index).ToList(), CellType = CellType.Tetrahedon });
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
                            int zMinIndex = Vertices.Min(x => x.MeshIndex[2]);
                            int zMaxIndex = Vertices.Max(x => x.MeshIndex[2]);

                            if(zMinIndex == 0 && zMaxIndex == 0)
                            {
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

                            }
                            else if(yMinIndex == 0 && yMaxIndex == 0)
                            {
                                //3D collection of vertices
                                LocationTimeValue[,] locs = new LocationTimeValue[(xMaxIndex - xMinIndex) + 1, (zMaxIndex - zMinIndex) + 1];

                                //Adding vertices to the collection based on the count
                                for (int i = 0; i < Vertices.Count(); i++)
                                    locs[Vertices[i].MeshIndex[0], Vertices[i].MeshIndex[2]] = Vertices[i];

                                for (int i = xMinIndex; i < xMaxIndex; i++)
                                {
                                    for (int j = zMinIndex; j < zMaxIndex; j++)
                                    {
                                        Quadrilateral quad = new Quadrilateral();

                                        quad.Vertices = new LocationTimeValue[4] { locs[i, j], locs[i, j + 1], locs[i + 1, j + 1], locs[i + 1, j] };

                                        quad.SortVertices();

                                        Faces.Add(quad);
                                    }
                                }
                            }
                            else if (xMinIndex == 0 && xMaxIndex == 0)
                            {
                                //3D collection of vertices
                                LocationTimeValue[,] locs = new LocationTimeValue[(yMaxIndex - yMinIndex) + 1, (zMaxIndex - zMinIndex) + 1];

                                //Adding vertices to the collection based on the count
                                for (int i = 0; i < Vertices.Count(); i++)
                                    locs[Vertices[i].MeshIndex[1], Vertices[i].MeshIndex[2]] = Vertices[i];

                                for (int i = yMinIndex; i < yMaxIndex; i++)
                                {
                                    for (int j = zMinIndex; j < zMaxIndex; j++)
                                    {
                                        Quadrilateral quad = new Quadrilateral();

                                        quad.Vertices = new LocationTimeValue[4] { locs[i, j], locs[i, j + 1], locs[i + 1, j + 1], locs[i + 1, j] };

                                        quad.SortVertices();

                                        Faces.Add(quad);
                                    }
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
                throw new Exception("Couldn't create mesh.");
            }
        }

        /// <summary>
        /// Meshing the point cloud
        /// </summary>
        public async Task CellsFromPointCloud()
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

                    await CreateVerticeArray();

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

                                    if (!hex.GetVertices(this).Any(x => !x.IsExterior))
                                        continue;

                                    if (MeshCellType == MeshCellType.Hexahedral)
                                        Cells.Add(hex);
                                    else if (MeshCellType == MeshCellType.Tetrahedal)
                                        Cells.AddRange(hex.ToTetrahedons(this));

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
                this.locs = new int[(xMaxIndex - xMinIndex) + 1, (yMaxIndex - yMinIndex) + 1, (zMaxIndex - zMinIndex) + 1];

                //Adding vertices to the collection based on the count
                for (int i = 0; i < Vertices.Count(); i++)
                {
                    locs[Vertices[i].MeshIndex[0], Vertices[i].MeshIndex[1], Vertices[i].MeshIndex[2]] = i;
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
                    ret += Cells[i].GetVolume(this);
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
                LocationTimeValue[] locs = GetNeighbors(loc).Result;

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
                LocationTimeValue[] locs = GetNeighbors(loc).Result;

                for (int i = 0; i < locs.Length; i++)
                {
                    grad.X += loc.X != locs[i].X ? Math.Abs(loc.Z - locs[i].Z) / Math.Abs(loc.X - locs[i].X) : 0;
                    grad.Y += loc.Y != locs[i].Y ? Math.Abs(loc.Z - locs[i].Z) / Math.Abs(loc.Y - locs[i].Y) : 0;
                    grad.Z += 0;
                }
            }
            catch
            {
                throw new Exception("Couldn't calculate gradient 2D.");
            }

            return grad;
        }

        /// <summary>
        /// Getting the neighbor nodes of a node
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public async Task<LocationTimeValue[]> GetNeighbors(LocationTimeValue loc)
        {
            List<LocationTimeValue> retLocs = new List<LocationTimeValue>();

            if (locs.Length == 0)
                await CreateVerticeArray();

            try
            {
                int indexX = loc.MeshIndex[0];
                int indexY = loc.MeshIndex[1];
                int indexZ = loc.MeshIndex[2];

                //Checking if the correct count of points is returned
                switch(Dimensionality)
                {
                    case Dimensionality.TwoD:

                        retLocs.Add(Vertices[locs[indexX - 1, indexY, indexZ]]);
                        retLocs.Add(Vertices[locs[indexX, indexY - 1, indexZ]]);
                        retLocs.Add(Vertices[locs[indexX + 1, indexY, indexZ]]);
                        retLocs.Add(Vertices[locs[indexX, indexY + 1, indexZ]]);

                        if (retLocs.Count() > 4 && !loc.IsExterior)
                        {
                            retLocs = retLocs.OrderBy(x => x.GetEuclideanDistance(loc)).Take(4).ToList();
                        }
                        break;
                    case Dimensionality.ThreeD:

                        if(indexX != 0)
                            retLocs.Add(Vertices[locs[indexX - 1, indexY, indexZ]]);
                        if(indexY != 0)
                            retLocs.Add(Vertices[locs[indexX, indexY - 1, indexZ]]);
                        if(indexZ != 0)
                            retLocs.Add(Vertices[locs[indexX, indexY, indexZ - 1]]);
                        if(indexX != locs.GetLength(0)-1)
                            retLocs.Add(Vertices[locs[indexX + 1, indexY, indexZ]]);
                        if(indexY != locs.GetLength(1)-1)
                            retLocs.Add(Vertices[locs[indexX, indexY + 1, indexZ]]);
                        if(indexZ != locs.GetLength(2)-1)
                            retLocs.Add(Vertices[locs[indexX, indexY, indexZ + 1]]);

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

        /// <summary>
        /// Finds the equivalent location time value
        /// </summary>
        /// <returns></returns>
        public LocationTimeValue FindLocationEquivalent(LocationTimeValue loc)
        {
            try
            {
                ///Finding value with the same location
                return Vertices.Where(a =>
                    a.X == loc.X &&
                    a.Y == loc.Y &&
                    a.Z == loc.Z
                ).First();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the coordinate matrix of the vertices
        /// </summary>
        /// <returns></returns>
        public Matrix<double> GetCoordinateMatrix()
        {
            try
            {
                //Getting dimensionality of the problem
                int dim = Dimensionality == Dimensionality.OneD ? 1 : (Dimensionality == Dimensionality.TwoD ? 2 : (Dimensionality == Dimensionality.ThreeD ? 3 : 0));

                Matrix<double> g_coord = Matrix<double>.Build.Dense(dim, Vertices.Count(), 0);

                //Getting all coordinates
                for(int i = 0; i<Vertices.Count();i++)
                {
                    g_coord[0, i] = Vertices[i].X;
                    g_coord[1, i] = Vertices[i].Y;
                    g_coord[2, i] = Vertices[i].Z;
                }

                return g_coord;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the connectivity matrix of the vertices
        /// </summary>
        /// <returns></returns>
        public Matrix<double> GetConnectivityMatrix()
        {
            try
            {
                //Getting dimensionality of the problem
                int dim = Dimensionality == Dimensionality.OneD ? 1 : (Dimensionality == Dimensionality.TwoD ? 2 : (Dimensionality == Dimensionality.ThreeD ? 3 : 0));

                Matrix<double> g_num = Matrix<double>.Build.Dense(Cells[0].Vertices.Count(), Cells.Count(), 0);


                for (int i = 0; i < Cells.Count(); i++)
                {
                    LocationTimeValue[] vert = Cells[i].GetVertices(this);

                    int[] lowest = vert.OrderBy(x => x.MeshIndex[0]).OrderBy(x => x.MeshIndex[1]).OrderBy(x => x.MeshIndex[2]).Select(x => x.MeshIndex).First();
                    int xMinIndex = lowest[0];
                    int yMinIndex = lowest[1];
                    int zMinIndex = lowest[2];

                    g_num[0, i] = locs[xMinIndex, yMinIndex, zMinIndex]; // node 1
                    g_num[1, i] = locs[xMinIndex, yMinIndex, zMinIndex + 1]; //node 2
                    g_num[2, i] = locs[xMinIndex + 1, yMinIndex, zMinIndex + 1]; // node 3
                    g_num[3, i] = locs[xMinIndex + 1, yMinIndex, zMinIndex]; // node 4
                    g_num[4, i] = locs[xMinIndex, yMinIndex + 1, zMinIndex]; // node 5
                    g_num[5, i] = locs[xMinIndex, yMinIndex + 1, zMinIndex + 1]; // node 6
                    g_num[6, i] = locs[xMinIndex + 1, yMinIndex + 1, zMinIndex + 1]; // node 7
                    g_num[7, i] = locs[xMinIndex + 1, yMinIndex + 1, zMinIndex]; // node 8
                }

                return g_num;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Extracting a 2D section
        /// </summary>
        /// <param name="_mesh"></param>
        /// <returns></returns>
        public async Task<Mesh> ExtractSection(Point3D point1, Point3D point2, int horizontalCells, int verticalCells, bool vertical = true)
        {
            Mesh mesh = new Mesh();
            mesh.Dimensionality = Dimensionality.TwoD;
            mesh.MeshCellType = MeshCellType.Rectangular;

            try
            {
                //Interpolating the horizontal line
                double[,] xyCoordinates = MeshingHelper.InterpolateLine2D(point1.X, point1.Y, point2.X, point2.Y, horizontalCells);
                double[] zArray = DistributionHelper.Subdivide(new double[2] { Vertices.Max(x => x.Z), Vertices.Min(x => x.Z) }, verticalCells);

                for(int i = 0; i<xyCoordinates.GetLength(0);i++)
                //Parallel.For(0, xyCoordinates.GetLength(0), (i, loopState) =>
                {
                    for (int j = 0; j < verticalCells; j++)
                    {
                        try
                        {
                            //Creating the new point to be inserted
                            var pt = new LocationTimeValue()
                            {
                                X = xyCoordinates[i, 0],
                                Y = xyCoordinates[i, 1],
                                Z = zArray[j],
                                Geographic = false,
                                IsDiscretized = true,
                                IsExterior = true,
                                MeshIndex = new int[3] { i, j, 0 }
                            };

                            var sortedList = Vertices.MinBy(x => x.GetEuclideanDistance(pt)).Take(3);

                            pt.Value[0] = sortedList.Average(x => x.Value[0]);
                            pt.Z = sortedList.Take(1).First().Z;

                            mesh.Vertices.Add(pt);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                };

                //Removing null vertices
                for (int i = 0; i < mesh.Vertices.Count(); i++)
                    if (mesh.Vertices[i] == null)
                    {
                        mesh.Vertices.RemoveAt(i);
                        i -= 1;
                    }
                
                //Adding interpolated values and variances to the original data set
                mesh.Name = "Vertical section";

                mesh.FacesFromPointCloud();
            }
            catch
            {
                throw new Exception("Couldn't exctract section.");
            }

            return mesh;
        }

        /// <summary>
        /// Getting the properties of the mesh
        /// </summary>
        /// <returns></returns>
        public bool GetProperties()
        {

            bool ret = true;

            try
            {
                List<string> names = new List<string>();

                if (Properties != null && Properties.Count() > 0 )
                {
                    names = Properties.Select(x => x.Value).ToList();
                }

                Properties = new BindableCollection<KeyValuePair<int, string>>();

                for(int i = 0; i<Vertices[0].Value.Count(); i++)
                {
                    if(names.Count() >= i && names.Count() > 0)
                        Properties.Add(new KeyValuePair<int, string>(i,  names[i]));
                    else
                        Properties.Add(new KeyValuePair<int, string>(i,  "Property " + (i+1).ToString()));
                }
            }
            catch
            {
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// Checks if the list of properties fits to the vertices values
        /// </summary>
        /// <returns>True if properties are consistent</returns>
        public bool CheckProperties()
        {
            bool check = true;

            try
            {
                for(int i = 0; i<Vertices.Count(); i++)
                {
                    if(Vertices[i].Value.Count() != Properties.Count())
                    {
                        check = false;
                        break;
                    }
                }
            }
            catch
            {
                throw new Exception("Mesh validation check failed.");
            }

            return check;
        }

        /// <summary>
        /// Removes the acutally selected property
        /// </summary>
        /// <returns></returns>
        public async Task RemoveProperty()
        {
            try
            {
                int index = Properties.IndexOf(SelectedProperty);

                for(int i = 0; i<Vertices.Count(); i++)
                {
                    Vertices[i].Value.RemoveAt(index);
                }

                Properties.RemoveAt(index);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Brings the selected property to the first position
        /// </summary>
        /// <returns></returns>
        public async Task BringToFront()
        {
            try
            {
                int s = SelectedProperty.Key;
                
                // Swapping the values hold by the vertices
                for (int i = 0; i < Vertices.Count(); i++)
                {
                    await Task.Delay(0);

                    Vertices[i].SwapProperties(0, s);
                }

                KeyValuePair<int, string> prop1 = Properties[0];

                // Swapping the properties
                Properties.Swap(0, SelectedProperty.Key);

                GetProperties();

                Vertices = new ObservableCollection<LocationTimeValue>(Vertices);
            }
            catch
            {
                throw new Exception("Couldn't bring property to front.");
            }
        }


        #endregion

    }
}
