using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// Defines, how a mesh can be joined
    /// </summary>
    public enum JoinMethod
    {
        Exact = 0,
        Threshold = 1,
        Name = 2
    }

    /// <summary>
    /// Helps to perform joining operations on meshes
    /// </summary>
    public static class MeshJoiner
    {
        #region Methods

        /// <summary>
        /// Joining the meshes according to their shared position
        /// </summary>
        /// <param name="meshes"></param>
        /// <returns></returns>
        public static DataTable JoinMeshesToDataTable(List<Mesh> meshes, JoinMethod joinMethod = JoinMethod.Exact, double threshold = 0.01)
        {
            DataTable dat = new DataTable();
            dat.TableName = "MultivariateDataTable";

            if (meshes == null || meshes.Count() == 0)
                throw new ArgumentNullException("Meshes");

            try
            {
                for (int i = 0; i < meshes.Count(); i++)
                    dat.Columns.Add(new DataColumn(meshes[i].Name, typeof(double)));

                //Iterate through all points in mesh 0
                for (int i = 0; i < meshes[0].Vertices.Count(); i++)
                {
                    DataRow dr = dat.NewRow();

                    dr[0] = meshes[0].Vertices[i].Value[0];

                    if (meshes.Count() > 1)
                        for (int j = 1; j < meshes.Count(); j++)
                        {
                            double value = 0;

                            //Searches the point, which either exactly fits to the point of mesh 0 or is the closest
                            if (joinMethod == JoinMethod.Exact)
                                value = meshes[j].Vertices.FirstOrDefault(x => meshes[0].Vertices[i].X == x.X && meshes[0].Vertices[i].Y == x.Y && meshes[0].Vertices[i].Z == meshes[0].Vertices[i].Z).Value[0];
                            else
                            {
                                LocationTimeValue loc = meshes[j].Vertices.OrderBy(x => meshes[0].Vertices[i].GetEuclideanDistance(x)).FirstOrDefault();
                                if (loc.GetEuclideanDistance(meshes[0].Vertices[i]) <= threshold)
                                    value = loc.Value[0];
                                else
                                    throw new Exception("All points exceed the threshold value.");
                            }

                            dr[j] = value;

                        }

                    //Adding the row to the data table
                    dat.Rows.Add(dr);
                }
            }
            catch
            {
                throw new Exception("Cannot join the tables");
            }

            return dat;
        }

        /// <summary>
        /// Joining the meshes according to their shared position
        /// </summary>
        /// <param name="meshes"></param>
        /// <returns></returns>
        public static Mesh JoinMeshes(List<Mesh> meshes, JoinMethod joinMethod = JoinMethod.Exact, double threshold = 0.01)
        {
            if (meshes == null || meshes.Count() == 0)
                throw new ArgumentNullException("Meshes");

            Mesh ret = new Mesh(meshes[0]);

            try
            {

                //Iterate through all points in mesh 0
                for (int i = 0; i < ret.Vertices.Count(); i++)
                {
                    if (meshes.Count() > 1)
                        for (int j = 1; j < meshes.Count(); j++)
                        {
                            double value = 0;

                            //Searches the point, which either exactly fits to the point of mesh 0 or is the closest
                            if (joinMethod == JoinMethod.Exact)
                            {
                                value = meshes[j].Vertices.FirstOrDefault(x => ret.Vertices[i].X == x.X && ret.Vertices[i].Y == x.Y && ret.Vertices[i].Z == x.Z).Value[0];
                            }
                            else if (joinMethod == JoinMethod.Threshold)
                            {
                                LocationTimeValue loc = meshes[j].Vertices.OrderBy(x => meshes[0].Vertices[i].GetEuclideanDistance(x)).FirstOrDefault();
                                if (loc.GetEuclideanDistance(meshes[0].Vertices[i]) <= threshold)
                                    value = loc.Value[0];
                                else
                                    throw new Exception("All points exceed the threshold value.");
                            }
                            else if (joinMethod == JoinMethod.Name)
                            {
                                value = meshes[j].Vertices.FirstOrDefault(x => ret.Vertices[i].Name == x.Name).Value[0];
                            }

                            if (value == 0)
                            {
                                ret.Vertices.RemoveAt(i);
                                i -= 1;
                                break;
                            }

                            ret.Vertices[i].Value.Add(value);

                        }
                }

                for(int i = 0; i<meshes.Count();i++)
                {
                    ret.Properties.Add(new KeyValuePair<int, string>(i, meshes[i].Name));
                }
            }
            catch
            {
                throw new Exception("Cannot join the meshes");
            }

            return ret;
        }


        /// <summary>
        /// Extracts a data table from a mesh
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static DataTable ExtractDataTable(Mesh mesh, bool includeCoordinates = false)
        {
            DataTable dat = new DataTable();
            dat.TableName = "MultivariateDataTable";

            if (mesh.Vertices == null || mesh.Vertices.Count() == 0)
                throw new ArgumentNullException("Meshes");

            try
            {
                for (int i = 0; i < mesh.Properties.Count(); i++)
                    dat.Columns.Add(new DataColumn(mesh.Properties[i].Value, typeof(double)));

                //Iterate through all points in mesh 0
                for (int i = 0; i < mesh.Vertices.Count(); i++)
                {
                    DataRow dr = dat.NewRow();

                    for (int j = 0; j < mesh.Properties.Count(); j++)
                    {
                        double value = 0;

                        //Searches the point and extracts the value
                        value = mesh.Vertices[i].Value[j];

                        dr[j] = value;

                    }

                    //Adding the row to the data table
                    dat.Rows.Add(dr);
                }
            }
            catch
            {
            }

            return dat;
        }

        #endregion
    }
}
