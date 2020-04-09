using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Linq;
using HelixToolkit.Wpf;
using System.Collections.Generic;

namespace GeoReVi
{
    /// <summary>
    /// A vertex is a simple class that stores the position of a point, node or vertex.
    /// </summary>
    public class Tetrahedron : Cell
    {
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
        /// This function adds indices for a triangle representing the face.
        /// The order is in the CCW (counter clock wise) order so that the automatically calculated normals point in the right direction.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <param name="center"></param>
        /// <param name="indices"></param>
        public override void MakeFace(int i, int j, int k, Vector3D center, Int32Collection indices, Mesh mesh)
        {
            var u = GetPosition(j,mesh) - GetPosition(i, mesh);
            var v = GetPosition(k, mesh) - GetPosition(j, mesh);

            // compute the normal and the plane corresponding to the side [i,j,k]
            var n = Vector3D.CrossProduct(u, v);
            var d = -Vector3D.DotProduct(n, center);

            // check if the normal faces towards the center
            var t = Vector3D.DotProduct(n, (Vector3D)GetPosition(i, mesh)) + d;
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
        /// Creates a model of the tetrahedron. Transparency is applied to the color.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="radius"></param>
        /// <returns>A model representing the tetrahedron</returns>
        public override void CreateFaces(Mesh mesh)
        {
           
            var points = new Point3DCollection(Enumerable.Range(0, 4).Select(i => GetPosition(i, mesh)));

            Faces.Clear();

            Faces.Add( new Triangle(points[0].ToLocationTimeValue(), points[1].ToLocationTimeValue(), points[2].ToLocationTimeValue()));
            Faces.Add( new Triangle(points[0].ToLocationTimeValue(), points[1].ToLocationTimeValue(), points[3].ToLocationTimeValue()));
            Faces.Add( new Triangle(points[0].ToLocationTimeValue(), points[2].ToLocationTimeValue(), points[3].ToLocationTimeValue()));
            Faces.Add(new Triangle(points[1].ToLocationTimeValue(), points[2].ToLocationTimeValue(), points[3].ToLocationTimeValue()));
        }

        /// <summary>
        /// Calculates the volume of the tetrahedon
        /// </summary>
        /// <returns></returns>
        public override double GetVolume(Mesh mesh)
        {
            double ret = 0;
            try
            {
                LocationTimeValue[] vert = GetVertices(mesh);
                LocationTimeValue loc = vert.OrderByDescending(x => Faces[0].PointDistanceToPlane(x.ToVector3D())).First();
                double a = Faces[0].PointDistanceToPlane(loc.ToVector3D());
                ret = Faces[0].GetArea() * Faces[0].PointDistanceToPlane(loc.ToVector3D()) * (1.0 / 3.0); 
            }
            catch
            {
                ret = double.NaN;
            }

            return ret;
        }
    }
}
