using Caliburn.Micro;
using MIConvexHull;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;

namespace GeoReVi
{
    /// <summary>
    /// A face is a class that contains information about a face of a body
    /// </summary>
    [XmlRoot(Namespace = "GeoReVi")]
    [XmlInclude(typeof(Quadrilateral))]
    [XmlInclude(typeof(Triangle))]
    public abstract class Face : ConvexFace<LocationTimeValue, Face>
    {
        #region Public properties

        /// <summary>
        /// Type of the face
        /// </summary>
        private FaceType faceType = FaceType.Triangular;
        public FaceType FaceType
        {
            get => this.faceType;
            set
            {
                this.faceType = value;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Getting the circumcentre of the face
        /// </summary>
        /// <returns></returns>
        public LocationTimeValue GetCircumCentre()
        {
            LocationTimeValue loc = new LocationTimeValue();

            try
            {
                Vector3D vec1 = new Vector3D();
                Vector3D vec2 = new Vector3D();
                Vector3D cp = new Vector3D();

                switch (FaceType)
                {
                    case FaceType.Triangular:

                        //From
                        //https://gamedev.stackexchange.com/questions/60630/how-do-i-find-the-circumcenter-of-a-triangle-in-3d
                        double a = GeographyHelper.EuclideanDistance(Vertices[0], Vertices[1]);
                        double b = GeographyHelper.EuclideanDistance(Vertices[0], Vertices[2]);

                        vec1 = (Vertices[1] - Vertices[0]).ToVector3D();
                        vec2 = (Vertices[2] - Vertices[0]).ToVector3D();
                        cp = Vector3D.CrossProduct(vec1, vec2);

                        loc = Vector3D.Add(Vertices[0].ToVector3D(), Vector3D.Add(cp.CrossProduct(vec1).Multiply(Math.Pow(b, 2)), vec2.CrossProduct(cp).Multiply(Math.Pow(a, 2))).Divide(2 * Math.Pow(cp.Length, 2))).ToLocationTimeValue();

                        break;
                    case FaceType.Quadrilateral:
                        loc = new LocationTimeValue((Vertices[0].X + Vertices[1].X + Vertices[2].X + Vertices[3].X) / 4,
                                                    (Vertices[0].Y + Vertices[1].Y + Vertices[2].Y + Vertices[3].Y) / 4,
                                                    (Vertices[0].Z + Vertices[1].Z + Vertices[2].Z + Vertices[3].Z) / 4,
                                                    "default",
                                                    (Vertices[0].Value[0] + Vertices[1].Value[0] + Vertices[2].Value[0] + Vertices[3].Value[0]) / 4);
                        break;
                    default:
                        break;
                }

            }
            catch(Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
            }

            return loc;

        }

        /// <summary>
        /// Getting the area of the face
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {
            double area = 0;

            try
            {
                Vector3D vec1 = new Vector3D();
                Vector3D vec2 = new Vector3D();

                switch (FaceType)
                {
                    case FaceType.Triangular:
                        //Calculating the area with the formula A = |AB||AC|*sin(teta)/2

                        vec1 = (Vertices[1].ToVector3D() - Vertices[0].ToVector3D());
                        vec2 = (Vertices[2].ToVector3D() - Vertices[0].ToVector3D());

                        double angle = Math.Round(Vector3D.AngleBetween(vec1, vec2),7);
                        area = 0.5 * (vec1.Length) * vec2.Length * Math.Sin(angle.DegreeToRadians());
                        break;
                    case FaceType.Quadrilateral:
                        //Calculating area by two triangles
                        var orderedVerts = Vertices.OrderBy(x => GeographyHelper.EuclideanDistance(Vertices[0], x)).ToList();
                        double area1 = new Triangle(orderedVerts[0], orderedVerts[1], orderedVerts[2]).GetArea();
                        double area2 = new Triangle(orderedVerts[3], orderedVerts[1], orderedVerts[2]).GetArea();
                        area = area1 + area2;
                        break;
                    default:
                        break;
                }

            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
            }

            return area;
        }

        /// <summary>
        /// Returns the radius of the triangle
        /// </summary>
        /// <returns></returns>
        public double GetRadius()
        {
            double radius = 0;

            try
            {
                switch (FaceType)
                {
                    case FaceType.Triangular:
                        //Calculating the radius
                        double a = GeographyHelper.EuclideanDistance(Vertices[0], Vertices[1]);
                        double b = GeographyHelper.EuclideanDistance(Vertices[0], Vertices[2]);
                        double c = GeographyHelper.EuclideanDistance(Vertices[1], Vertices[2]);

                        double area = GetArea();

                        radius = a * b * c / (4 * area);

                        break;
                    case FaceType.Quadrilateral:
                        //Calculating area with the formula A = a*b
                        area = GeographyHelper.EuclideanDistance(Vertices[1], Vertices[0]) * GeographyHelper.EuclideanDistance(Vertices[2], Vertices[1]);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
            }

            return radius;
        }

        /// <summary>
        /// Gets the normal vector of the plane
        /// </summary>
        /// <returns></returns>
        public Vector3D GetNormal()
        {
            try
            {
                Vector3D vec1 = (Vertices[1] - Vertices[0]).ToVector3D();
                Vector3D vec2 = (Vertices[2] - Vertices[0]).ToVector3D();

                //returning perpendicular vector
                return vec1.CrossProduct(vec2);
            }
            catch
            {
                return new Vector3D();
            }
        }

        /// <summary>
        /// Creates a model of the tetrahedron. Transparency is applied to the color.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="radius"></param>
        /// <returns>A model representing the tetrahedron</returns>
        public virtual Model3D CreateModel(Material material, double radius)
        {
            return new GeometryModel3D { };
        }

        /// <summary>
        /// Helper function to get the position of the i-th vertex.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>Position of the i-th vertex</returns>
        public virtual Point3D GetPosition(int i)
        {
            return Vertices[i].ToPoint3D();
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
        public virtual void MakeFace(int i, int j, int k, Vector3D center, Int32Collection indices)
        {

        }

        /// <summary>
        /// Returns a double value p.
        /// IF p > 0 => point is below plane
        /// IF p < 0 => point is above plane
        /// If p== 0 => point is on plane
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public double PointDistanceToPlane(Vector3D pt)
        {
            Vector3D normal = GetNormal();

            //According to https://math.stackexchange.com/questions/2758190/shortest-distance-from-point-to-a-plane
            return Math.Abs(normal.X * pt.X + normal.Y*pt.Y + normal.Z * pt.Z - GetPlaneConstant())/Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2)+ Math.Pow(normal.Z, 2));
        }

        /// <summary>
        /// Calculating the plane constant d for the Cartesian form ax + by + cz + d = 0
        /// </summary>
        /// <returns></returns>
        public double GetPlaneConstant()
        {
            try
            {
                Vector3D normal = GetNormal();
                return normal.X * Vertices[0].X + normal.Y * Vertices[0].Y + normal.Z * Vertices[0].Z;
            }
            catch
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// Sorting elements clockwise
        /// </summary>
        public virtual void SortVertices()
        {
            
        }

        /// <summary>
        /// Interpolating a face
        /// </summary>
        /// <returns></returns>
        public virtual Face[] SubdivideFace()
        {
            return new Face[0];
        }

        #endregion
    }
}
