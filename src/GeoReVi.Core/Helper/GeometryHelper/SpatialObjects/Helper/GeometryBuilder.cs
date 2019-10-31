using Microsoft.SqlServer.Types;
using System.Collections.Generic;

namespace GeoReVi
{
    public static class GeometryBuilder
    {
        /// <summary>
        /// Constructs a multi point geometry object from a list of points
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static SqlGeometry BuildMultiPoint(List<LocationTimeValue> points)
        {
            // Create a new instance of the SqlGeographyBuilder
            SqlGeometryBuilder gb2 = new SqlGeometryBuilder();

            // Set the spatial reference identifier
            gb2.SetSrid(27700);

            try
            {
                // Declare the type of collection to be created
                gb2.BeginGeometry(OpenGisGeometryType.MultiPoint);

                for(int i = 0; i< points.Count;i++)
                {
                    // Create the first point in the collection
                    gb2.BeginGeometry(OpenGisGeometryType.Point);
                    gb2.BeginFigure(points[i].X, points[i].Y, points[i].Z, points[i].Value[0]);
                    gb2.EndFigure();
                    gb2.EndGeometry();
                }


            }
            catch
            {

            }
            finally
            {
                // End the geometry and retrieve the constructed instance
                gb2.EndGeometry();
            }

            return gb2.ConstructedGeometry;

        }
    }
}
