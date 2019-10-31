using Caliburn.Micro;
using MIConvexHull;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types; // Required for SqlGeoemtry
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// A helper for triangulation tasks
    /// </summary>
    public class TriangulationCellHelper : TriangulationCell<LocationTimeValue, TriangulationCellHelper>
    {
        #region Public methods

        /// <summary>
        /// Triangulates a surface
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public List<Face> TriangulateSurface(List<LocationTimeValue> inputVertices)
        {
            List<Face> mesh = new List<Face>();

            try
            {
                // Important! Sort the list so that points sweep from left - right
                inputVertices.Sort();

                SqlGeometry MultiPoint = inputVertices.ToSqlGeometry();

                // Calculate the "supertriangle" that encompasses the pointset
                SqlGeometry Envelope = MultiPoint.STEnvelope();

                // Width
                double dx = (double)(Envelope.STPointN(2).STX - Envelope.STPointN(1).STX);

                // Height 
                double dy = (double)(Envelope.STPointN(4).STY - Envelope.STPointN(1).STY);

                // Maximum dimension
                double dmax = (dx > dy) ? dx : dy;

                // Centre
                double avgx = (double)Envelope.STCentroid().STX;
                double avgy = (double)Envelope.STCentroid().STY;

                // Create the points at corners of the supertriangle
                LocationTimeValue a = new LocationTimeValue(avgx - 2 * dmax, avgy - dmax);
                LocationTimeValue b = new LocationTimeValue(avgx + 2 * dmax, avgy - dmax);
                LocationTimeValue c = new LocationTimeValue(avgx, avgy + 2 * dmax);

                Triangle superTriangle = new Triangle(a, b, c);

                List<Triangle> Triangles = new List<Triangle>();
                Triangles.Add(superTriangle);


                // Loop through each point
                Parallel.For(0, inputVertices.Count(), i =>
                {
                    // Initialise the edge buffer
                    List<LocationTimeValue[]> Edges = new List<LocationTimeValue[]>();

                    try
                    {
                        // Loop through each triangle
                        for (int j = Triangles.Count - 1; j >= 0; j--)
                        {
                            // If the point lies within the circumcircle of this triangle
                            if (GeographyHelper.EuclideanDistance(Triangles[j].GetCircumCentre(), inputVertices[i]) <= Triangles[j].GetRadius())
                            {
                                // Add the triangle edges to the edge buffer
                                Edges.Add(new LocationTimeValue[] { Triangles[j].Vertices[0], Triangles[j].Vertices[1] });
                                Edges.Add(new LocationTimeValue[] { Triangles[j].Vertices[1], Triangles[j].Vertices[2] });
                                Edges.Add(new LocationTimeValue[] { Triangles[j].Vertices[2], Triangles[j].Vertices[0] });

                                // Remove this triangle from the list
                                Triangles.RemoveAt(j);
                            }

                            // If this triangle is complete
                            else if (inputVertices[i].X >= Triangles[j].GetCircumCentre().X + Triangles[j].GetRadius())
                            {
                                mesh.Add(Triangles[j]);
                                Triangles.RemoveAt(j);
                            }
                        }

                        // Remove duplicate edges
                        for (int j = Edges.Count - 1; j > 0; j--)
                        {
                            for (int k = j - 1; k >= 0; k--)
                            {
                                // Compare if this edge match in either direction
                                if (Edges[j][0].Equals(Edges[k][1]) && Edges[j][1].Equals(Edges[k][0]))
                                {
                                    // Remove both duplicates
                                    Edges.RemoveAt(j);
                                    Edges.RemoveAt(k);

                                    // We've removed an item from lower down the list than where j is now, so update j
                                    j--;
                                    break;
                                }
                            }
                        }

                        // Create new triangles for the current point
                        for (int j = 0; j < Edges.Count; j++)
                        {
                            Triangle T = new Triangle(Edges[j][0], Edges[j][1], inputVertices[i]);
                            Triangles.Add(T);
                        }

                    }
                    catch
                    {
                    }
                });

                mesh.AddRange(Triangles);

                int count = mesh.Count();

                //We've finished triangulation. Move any remaining triangles onto the completed list
                for (int i = 0; i < count; i++)
                    if (mesh[i].Vertices.Any(x => superTriangle.Vertices.Contains(x)))
                    {
                        mesh.RemoveAt(i);
                        count--;
                        i--;
                    }
            }
            catch
            {

            }


            return mesh;
        }

        #endregion
    }
}
