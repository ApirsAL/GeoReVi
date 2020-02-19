
using System.Linq;
using System.Windows.Media.Media3D;

namespace GeoReVi
{
    /// <summary>
    /// A quadrilateral element
    /// </summary>
    public class Quadrilateral : Face
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Quadrilateral()
        {
            FaceType = FaceType.Quadrilateral;
        }

        /// <summary>
        /// Explicit constructor
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Quadrilateral(LocationTimeValue a, LocationTimeValue b, LocationTimeValue c, LocationTimeValue d)
        {
            Vertices = new LocationTimeValue[] { a, b, c, d };
            FaceType = FaceType.Quadrilateral;
        }


        #endregion

        #region Public properties

        /// <summary>
        /// Creates a model of the tetrahedron. Transparency is applied to the color.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="radius"></param>
        /// <returns>A model representing the tetrahedron</returns>
        public override Model3D CreateModel(Material material, double radius)
        {
            // Create a mesh builder and add a box to it
            HelixToolkit.Wpf.MeshBuilder meshBuilder = new HelixToolkit.Wpf.MeshBuilder(false, false);

            if(ArePointsCoplanar())
            {
                SortVertices();
                meshBuilder.AddQuad(Vertices[0].ToPoint3D(), Vertices[1].ToPoint3D(), Vertices[3].ToPoint3D(), Vertices[2].ToPoint3D());
            }

            MeshGeometry3D mesh = meshBuilder.ToMesh(true);

            return new GeometryModel3D { Geometry = mesh, Material = material, BackMaterial = material };
        }

        /// <summary>
        /// Checks if face points are coplanar
        /// </summary>
        /// <returns></returns>
        public bool ArePointsCoplanar()
        {
            try
            {
                Vector3D vec1 = Vertices[0].ToVector3D() - Vertices[1].ToVector3D();
                Vector3D vec2 = Vertices[0].ToVector3D() - Vertices[2].ToVector3D();
                Vector3D vec3 = Vertices[0].ToVector3D() - Vertices[3].ToVector3D();

                if (Vector3D.DotProduct(vec1, vec2.CrossProduct(vec3)) == 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sorting vertices clockwise
        /// </summary>
        public override void SortVertices()
        {
            try
            {
                ////Return if the vectors are not coplanar
                //if (!ArePointsCoplanar())
                //    return;

                //Defining all vectors of the quadrilateral
                Vector3D vec1 = Vertices[1].ToVector3D() - Vertices[0].ToVector3D();
                Vector3D vec2 = Vertices[2].ToVector3D() - Vertices[0].ToVector3D();
                Vector3D vec3 = Vertices[3].ToVector3D() - Vertices[0].ToVector3D();

                //Calculating angles between the vectors
                double angle12 = Vector3D.AngleBetween(vec1, vec2);
                double angle13 = Vector3D.AngleBetween(vec1, vec3);
                double angle23 = Vector3D.AngleBetween(vec2, vec3);

                //Sorting based on the biggest angle
                if (angle12 < angle13 && angle23 < angle13)
                    return;
                else if (angle13 < angle12 && angle23 < angle12)
                    Vertices.ShiftElement(1, 3);
                else if (angle13 < angle23 && angle12 < angle23)
                    Vertices.ShiftElement(1, 3);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Subdividing the quadrilateral
        /// </summary
        /// <returns></returns>
        public override Face[] SubdivideFace()
        {

            Face[] interpolatedFaces = new Face[4];

            SortVertices();

            try
            {
                var middle01 = Vertices[0].GetMiddlePoint(Vertices[1]);
                var middle12 = Vertices[1].GetMiddlePoint(Vertices[2]);
                var middle23 = Vertices[2].GetMiddlePoint(Vertices[3]);
                var middle30 = Vertices[3].GetMiddlePoint(Vertices[0]);
                var center = GetCircumCentre();

                if(Vertices.Any(x => x.IsExterior))
                {
                    middle01.IsExterior = true;
                    middle12.IsExterior = true;
                    middle23.IsExterior = true;
                    middle30.IsExterior = true;
                    center.IsExterior = true;
                }

                interpolatedFaces[0] = new Quadrilateral(Vertices[0], middle01, center, middle30);
                interpolatedFaces[1] = new Quadrilateral(middle01, Vertices[1], middle12, center);
                interpolatedFaces[2] = new Quadrilateral(middle12, Vertices[2], middle23, center);
                interpolatedFaces[3] = new Quadrilateral(middle30, center, middle23, Vertices[3]);

                interpolatedFaces[0].FaceType = FaceType.Quadrilateral;
                interpolatedFaces[1].FaceType = FaceType.Quadrilateral;
                interpolatedFaces[2].FaceType = FaceType.Quadrilateral;
                interpolatedFaces[3].FaceType = FaceType.Quadrilateral;
            }
            catch
            {

            }

            return interpolatedFaces;
        }

        #endregion
    }
}
