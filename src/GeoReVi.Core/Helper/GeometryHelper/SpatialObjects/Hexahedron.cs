using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GeoReVi
{
    public class Hexahedron : Cell
    {
        #region Public properties

        /// <summary>
        /// Helper function to get the position of the i-th vertex.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>Position of the i-th vertex</returns>
        public override Point3D GetPosition(int i, Mesh mesh)
        {
            return base.GetPosition(i, mesh);
        }

        /// <summary>
        /// Creates a model of the hexahedron
        /// </summary>
        /// <param name="color"></param>
        /// <param name="radius"></param>
        /// <returns>A model representing the tetrahedron</returns>
        public override void CreateFaces(Mesh mesh)
        {
            LocationTimeValue[] vert = GetVertices(mesh);

            int xMinIndex = vert.Min(x => x.MeshIndex[0]);
            int xMaxIndex = vert.Max(x => x.MeshIndex[0]);
            int yMinIndex = vert.Min(x => x.MeshIndex[1]);
            int yMaxIndex = vert.Max(x => x.MeshIndex[1]);
            int zMinIndex = vert.Min(x => x.MeshIndex[2]);
            int zMaxIndex = vert.Max(x => x.MeshIndex[2]);

            LocationTimeValue[] XZFrontFaceVertices = vert.Where(x => x.MeshIndex[1] == yMinIndex).ToArray();
            LocationTimeValue[] XZBackFaceVertices = vert.Where(x => x.MeshIndex[1] == yMaxIndex).ToArray();

            LocationTimeValue[] YZFrontFaceVertices = vert.Where(x => x.MeshIndex[0] == xMinIndex).ToArray();
            LocationTimeValue[] YZBackFaceVertices = vert.Where(x => x.MeshIndex[0] == xMaxIndex).ToArray();

            LocationTimeValue[] XYFrontFaceVertices = vert.Where(x => x.MeshIndex[2] == zMinIndex).ToArray();
            LocationTimeValue[] XYBackFaceVertices = vert.Where(x => x.MeshIndex[2] == zMaxIndex).ToArray();

            Faces.Clear();

            Faces.Add(new Quadrilateral(XZFrontFaceVertices[0], XZFrontFaceVertices[1], XZFrontFaceVertices[3], XZFrontFaceVertices[2]) { FaceType = FaceType.Quadrilateral });
            Faces.Add(new Quadrilateral(XZBackFaceVertices[0], XZBackFaceVertices[1], XZBackFaceVertices[3], XZBackFaceVertices[2]) { FaceType = FaceType.Quadrilateral }) ;
            Faces.Add(new Quadrilateral(YZFrontFaceVertices[0], YZFrontFaceVertices[1], YZFrontFaceVertices[2], YZFrontFaceVertices[3]) { FaceType = FaceType.Quadrilateral });
            Faces.Add(new Quadrilateral(YZBackFaceVertices[0], YZBackFaceVertices[1], YZBackFaceVertices[2], YZBackFaceVertices[3]) { FaceType = FaceType.Quadrilateral });
            Faces.Add(new Quadrilateral(XYFrontFaceVertices[0], XYFrontFaceVertices[1], XYFrontFaceVertices[3], XYFrontFaceVertices[2]) { FaceType = FaceType.Quadrilateral });
            Faces.Add(new Quadrilateral(XYBackFaceVertices[0], XYBackFaceVertices[1], XYBackFaceVertices[3], XYBackFaceVertices[2]) { FaceType = FaceType.Quadrilateral });
        }

        /// <summary>
        /// Calculating the value of the hexahedron
        /// </summary>
        /// <returns></returns>
        public override double GetVolume(Mesh mesh)
        {
            //returning value
            double ret = 0;

            try
            {
                LocationTimeValue[] vert = GetVertices(mesh);

                //Splitting up the hexahedron into six tetrahedons
                LocationTimeValue centerPoint = new LocationTimeValue(
                        vert.Average(x => x.X),
                        vert.Average(x => x.Y),
                        vert.Average(x => x.Z)
                    );

                CreateFaces(mesh);

                for(int i = 0; i<6;i++)
                {
                    double height = Faces[i].PointDistanceToPlane(centerPoint.ToVector3D());

                    ret += (1.0 / 3.0) * height * Faces[i].GetArea();
                }
            }
            catch
            {
                ret = 0;
            }

            return ret;
        }

        /// <summary>
        /// Converts a hexahedron to a list of tetrahedons
        /// </summary>
        /// <returns></returns>
        public List<Cell> ToTetrahedons(Mesh mesh)
        {
            //return object
            List<Cell> ret = new List<Cell>();

            try
            {
                LocationTimeValue[] vert = GetVertices(mesh);

                CreateFaces(mesh);

                LocationTimeValue loc1 = vert[0];
                LocationTimeValue loc2 = vert.OrderByDescending(x => GeographyHelper.EuclideanDistance(x, loc1)).First();

                for(int i = 0; i<Faces.Count();i++)
                {
                    Faces[i].Vertices.OrderBy(x => GeographyHelper.EuclideanDistance(x, Faces[i].Vertices[0]));

                    Tetrahedron tet = new Tetrahedron();
                    tet.Vertices = new List<int>()
                                   {
                                       mesh.Vertices.IndexOf(Faces[i].Vertices[0]),
                                       mesh.Vertices.IndexOf(Faces[i].Vertices[1]),
                                       mesh.Vertices.IndexOf(Faces[i].Vertices[2])
                                   };

                    if(Faces[i].Vertices.Contains(loc1))
                        tet.Vertices.Add(mesh.Vertices.IndexOf(loc2));
                    else
                        tet.Vertices.Add(mesh.Vertices.IndexOf(loc1));

                    tet.CreateFaces(mesh);

                    tet = new Tetrahedron();
                    tet.Vertices = new List<int>()
                                   {
                                       mesh.Vertices.IndexOf(Faces[i].Vertices[3]),
                                       mesh.Vertices.IndexOf(Faces[i].Vertices[1]),
                                       mesh.Vertices.IndexOf(Faces[i].Vertices[2])
                                   };

                    if (Faces[i].Vertices.Contains(loc1))
                        tet.Vertices.Add(mesh.Vertices.IndexOf(loc2));
                    else
                        tet.Vertices.Add(mesh.Vertices.IndexOf(loc1));

                    tet.CreateFaces(mesh);

                    ret.Add(tet);
                }   
            }
            catch
            {
                ret = new List<Cell>();
            }

            return ret;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Getting the element stiffness matrix necessary for finite element simulations
        /// </summary>
        /// <returns></returns>
        public override double[,] GetElementStiffnessMatrix()
        {
            double[,] ret = new double[,] { };

            try
            {

            }
            catch
            {

            }

            return ret;
        }

        /// <summary>
        /// Returns the node indices
        /// </summary>
        /// <returns></returns>
        public override double[] GetNodeIndices(Mesh mesh)
        {
            double[] g_num = new double[8];

            try
            {
                LocationTimeValue[] vert = GetVertices(mesh);

                if (mesh.locs.Length == 0)
                    mesh.CreateVerticeArray();

                int xMinIndex = vert.Min(x => x.MeshIndex[0]);
                int yMinIndex = vert.Min(x => x.MeshIndex[1]);
                int zMinIndex = vert.Min(x => x.MeshIndex[2]);

                g_num[0] = mesh.locs[xMinIndex, yMinIndex, zMinIndex]; // node 1
                g_num[1] = mesh.locs[xMinIndex, yMinIndex, zMinIndex + 1]; //node 2
                g_num[2] = mesh.locs[xMinIndex + 1, yMinIndex, zMinIndex + 1]; // node 3
                g_num[3] = mesh.locs[xMinIndex + 1, yMinIndex, zMinIndex]; // node 4
                g_num[4] = mesh.locs[xMinIndex, yMinIndex + 1, zMinIndex]; // node 5
                g_num[5] = mesh.locs[xMinIndex, yMinIndex + 1, zMinIndex + 1]; // node 6
                g_num[6] = mesh.locs[xMinIndex + 1, yMinIndex + 1, zMinIndex + 1]; // node 7
                g_num[7] = mesh.locs[xMinIndex + 1, yMinIndex + 1, zMinIndex]; // node 8
            }
            catch
            {

            }

            return g_num;
        }

        /// <summary>
        /// Identifies the vertice objects of the mesh
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public override LocationTimeValue[] GetVertices(Mesh mesh)
        {
            LocationTimeValue[] ret = new LocationTimeValue[8];

            for(int i = 0; i<Vertices.Count(); i++)
            {
                ret[i] = mesh.Vertices[Vertices[i]];
            }

            return ret;
        }

        /// <summary>
        /// Subdividing an hexahedron based on the octree-logic
        /// </summary>
        /// <returns></returns>
        public override async Task<ICell[]> Subdivide()
        {
            Hexahedron[] ret = new Hexahedron[8];

            try
            {
                await Task.Delay(0);

                LocationTimeValue loc12 = new LocationTimeValue();

                Hexahedron hex1 = new Hexahedron();
                hex1.CellType = CellType.Hexahedron;
            }
            catch
            {

            }


            return ret;
        }

        #endregion
    }
}
