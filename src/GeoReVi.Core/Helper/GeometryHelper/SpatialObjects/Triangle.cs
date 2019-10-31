using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GeoReVi
{
    public class Triangle : Face
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Triangle()
        {

        }

        /// <summary>
        /// Explicit constructor
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Triangle(LocationTimeValue a, LocationTimeValue b, LocationTimeValue c)
        {
            Vertices = new LocationTimeValue[] { a, b, c };
        }


        #endregion


        #region Public methods

        /// <summary>
        /// Creates a model of the tetrahedron. Transparency is applied to the color.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="radius"></param>
        /// <returns>A model representing the tetrahedron</returns>
        public override Model3D CreateModel(Material material, double radius)
        {
            var points = new Point3DCollection(Enumerable.Range(0, 2).Select(i => GetPosition(i)));

            // Create a mesh builder and add a box to it
            HelixToolkit.Wpf.MeshBuilder meshBuilder = new HelixToolkit.Wpf.MeshBuilder(false, false);

            meshBuilder.AddTriangle(Vertices[0].ToPoint3D(), Vertices[1].ToPoint3D(), Vertices[2].ToPoint3D());

            MeshGeometry3D mesh = meshBuilder.ToMesh(true);

            return new GeometryModel3D { Geometry = mesh, Material = material, BackMaterial = material };
        }

        /// <summary>
        /// This function adds indices for a triangle representing the face.
        /// The order is in the CCW (counter clock wise) order so that the automatically calculated normals point in the right direction.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <param name="center"></param>
        /// <param name="indices"></param>
        public override void MakeFace(int i, int j, int k, Vector3D center, Int32Collection indices)
        {
            var u = GetPosition(j) - GetPosition(i);
            var v = GetPosition(k) - GetPosition(j);

            // compute the normal and the plane corresponding to the side [i,j,k]
            var n = Vector3D.CrossProduct(u, v);
            var d = -Vector3D.DotProduct(n, center);

            // check if the normal faces towards the center
            var t = Vector3D.DotProduct(n, (Vector3D)GetPosition(i)) + d;
            if (t >= 0)
            {
                // swapping indices j and k also changes the sign of the normal, because cross product is anti-commutative
                indices.Add(k); indices.Add(j); indices.Add(i);
            }
            else
            {
                // indices are in the correct order
                indices.Add(i); indices.Add(j); indices.Add(k);
            }
        }

        /// <summary>
        /// Checks if the triangle is oriented clockwise
        /// </summary>
        /// <returns></returns>
        public bool IsOrientedClockwise()
        {
            bool isClockWise = true;

            float determinant = (float)(Vertices[0].X * Vertices[1].Y + Vertices[2].X * Vertices[0].Y + Vertices[1].X * Vertices[2].Y - Vertices[0].X * Vertices[2].Y - Vertices[2].X * Vertices[1].Y - Vertices[1].X * Vertices[0].Y);

            if (determinant > 0f)
            {
                isClockWise = false;
            }

            return isClockWise;
        }

        /// <summary>
        /// Checks if the triangle shares an edge with another triangle
        /// </summary>
        /// <param name="triangle1"></param>
        /// <returns></returns>
        public bool SharesEdge(Triangle triangle1)
        {
            try
            {
                if (triangle1.Equals(this))
                    return true;

                if (triangle1.Vertices.Where(x => this.Vertices.Contains(x)).Count() > 2)
                    return true;

                for (int i = 0; i < 3; i++)
                    for (int j = 2; j >= 0; j++)
                        if (triangle1.Vertices[i].Equals(this.Vertices[j]) && triangle1.Vertices[j].Equals(this.Vertices[i]))
                            return true;

            }
            catch
            {

            }

            return false;

        }

        /// <summary>
        /// Checks if the triangle shares an edge with another triangle
        /// </summary>
        /// <param name="triangle1"></param>
        /// <returns></returns>
        public bool SharesEdge(LocationTimeValue loc1, LocationTimeValue loc2)
        {
            try
            {
                if (this.Vertices.Contains(loc1) && this.Vertices.Contains(loc2))
                    return true;
            }
            catch
            {

            }

            return false;

        }
        #endregion
    }
}
