using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class DijkstraHelper
    {
        #region Properties

        /// <summary>
        /// Point to start the search
        /// </summary>
        public LocationTimeValue Start { get; set; }

        /// <summary>
        /// Destination
        /// </summary>
        public LocationTimeValue End { get; set; }

        /// <summary>
        /// Number of visits
        /// </summary>
        public int NodeVisits { get; private set; }

        /// <summary>
        /// Length of the shortest path
        /// </summary>
        public double ShortestPathLength { get; set; }

        /// <summary>
        /// Costs of the shortest path
        /// </summary>
        public double ShortestPathCost { get; private set; }

        /// <summary>
        /// Temporal mesh
        /// </summary>
        public Mesh TemporalMesh { get; private set; }

        /// <summary>
        /// The shortest path
        /// </summary>
        public List<LocationTimeValue> ShortestPath { get; set; }

        #endregion

        #region Constructor

        #endregion

        #region Methods

        /// <summary>
        /// Calculating the Dijkstra distance
        /// </summary>
        /// <returns></returns>
        public double GetDijkstraDistance(LocationTimeValue a, LocationTimeValue b, Mesh mesh)
        {
            double ret = 0;

            try
            {
                TemporalMesh = new Mesh(mesh);

                End = TemporalMesh.Vertices.OrderBy(x => GeographyHelper.EuclideanDistance(b, x)).First();

                for (int i = 0;i<TemporalMesh.Vertices.Count();i++)
                {
                    //Preparing the mesh for distance storage
                    if (TemporalMesh.Vertices[i].Value.Count() == 1)
                        TemporalMesh.Vertices[i].Value.Add(0);
                    else if (TemporalMesh.Vertices[i].Value.Count() == 0)
                        TemporalMesh.Vertices[i].Value.AddRange(new List<double>() { 0, 0 });

                    //Initializing the distances
                    TemporalMesh.Vertices[i].Value[0] = double.PositiveInfinity;
                    TemporalMesh.Vertices[i].Neighbors = new List<LocationTimeValue>();
                    TemporalMesh.Vertices[i].IsActive = false;

                }

                Start = TemporalMesh.Vertices.OrderBy(x => GeographyHelper.EuclideanDistance(a, x)).First();

                Start.Value[0] = 0;

                End = TemporalMesh.Vertices.OrderBy(x => GeographyHelper.EuclideanDistance(b, x)).First();

                if (Start.GetEuclideanDistance(End) == 0)
                    return 0;

                //Building the shortest path
                BuildShortestPathDijkstra();

                ret = ShortestPathCost;
            }
            catch
            {

            }

            return ret;
        }

        /// <summary>
        /// Calculating the Dijkstra distance
        /// </summary>
        /// <returns></returns>
        public double GetAstarDistance(LocationTimeValue a, LocationTimeValue b, Mesh mesh)
        {
            double ret = 0;

            try
            {
                TemporalMesh = new Mesh(mesh);

                End = TemporalMesh.Vertices.OrderBy(x => GeographyHelper.EuclideanDistance(b, x)).First();

                for (int i = 0; i < TemporalMesh.Vertices.Count(); i++)
                {
                    //Preparing the mesh for distance storage
                    if (TemporalMesh.Vertices[i].Value.Count() == 1)
                        TemporalMesh.Vertices[i].Value.Add(0);
                    else if (TemporalMesh.Vertices[i].Value.Count() == 0)
                        TemporalMesh.Vertices[i].Value.AddRange(new List<double>() { 0, 0 });

                    //Initializing the distances
                    TemporalMesh.Vertices[i].Value[0] = double.PositiveInfinity;
                    TemporalMesh.Vertices[i].Value[1] = End.GetEuclideanDistance(TemporalMesh.Vertices[i]);
                    TemporalMesh.Vertices[i].Neighbors = new List<LocationTimeValue>();
                    TemporalMesh.Vertices[i].IsActive = false;

                }

                Start = TemporalMesh.Vertices.OrderBy(x => GeographyHelper.EuclideanDistance(a, x)).First();

                Start.Value[0] = 0;

                End = TemporalMesh.Vertices.OrderBy(x => GeographyHelper.EuclideanDistance(b, x)).First();

                if (Start.GetEuclideanDistance(End) == 0)
                    return 0;

                //Building the shortest path
                BuildShortestPathAstar();

                ret = ShortestPathCost;
            }
            catch
            {

            }

            return ret;
        }

        /// <summary>
        /// Builds the shortest path
        /// </summary>
        /// <returns></returns>
        private List<LocationTimeValue> BuildShortestPathDijkstra()
        {
            var shortestPath = new List<LocationTimeValue>();

            try
            {
                DijkstraSearch();
                shortestPath.Add(End);
                BuildShortestPath(shortestPath, End);
                shortestPath.Reverse();
            }
            catch
            {

            }

            return shortestPath;
        }

        /// <summary>
        /// Builds the shortest path
        /// </summary>
        /// <returns></returns>
        private List<LocationTimeValue> BuildShortestPathAstar()
        {
            var shortestPath = new List<LocationTimeValue>();

            try
            {
                AstarSearch();
                shortestPath.Add(End);
                BuildShortestPath(shortestPath, End);
                shortestPath.Reverse();
            }
            catch
            {

            }

            return shortestPath;
        }

        /// <summary>
        /// Building the shortest path
        /// </summary>
        /// <param name="list"></param>
        /// <param name="node"></param>
        private void BuildShortestPath(List<LocationTimeValue> list, LocationTimeValue node)
        {
            if (node.Neighbors.Count() == 0)
                return;

            LocationTimeValue nearestNeighbor = node.Neighbors.Where(x => x.Neighbors.Count > 0).First();

            list.Add(nearestNeighbor);

            ShortestPathLength += GeographyHelper.EuclideanDistance(nearestNeighbor, node);

            ShortestPathCost += GeographyHelper.EuclideanDistance(nearestNeighbor, node);

            nearestNeighbor.Neighbors.Remove(node);

            if (nearestNeighbor.Equals(Start))
                return;

            BuildShortestPath(list, nearestNeighbor);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Performing a dijkstra search
        /// </summary>
        private void DijkstraSearch()
        {
            NodeVisits = 0;

            //Initializing priority queue
            var prioQueue = new List<LocationTimeValue>();

            //Adding start point to the queue
            prioQueue.Add(Start);

            //Filling priority queue and iterating through it until break conditions are met
            do
            {
                NodeVisits++;

                //Order points by distance to start
                prioQueue = prioQueue.OrderBy(x => x.Value[0]).ToList();

                var node = prioQueue.First();

                prioQueue.Remove(node);

                //Getting the neighbors of the node
                var neighbors = TemporalMesh.GetNeighbors(node).Result.OrderBy(x => GeographyHelper.EuclideanDistance(x, node)).ToList();

                //Iterating over the neighbors and detect which one has the lowest distance to the node
                for (int i = 0; i < neighbors.Count(); i++)
                {
                    //Check that the point does not equals the node where we came from
                    if (node.Neighbors.Count > 0)
                        if (neighbors[i].Equals(node.Neighbors.First()))
                            continue;

                    //Determining the euclidean distance to the neighbor
                    double dist = GeographyHelper.EuclideanDistance(neighbors[i], node);

                    neighbors[i].Value[0] = node.Value[0] + dist;

                    if (neighbors[i].Neighbors.Count == 0 || !neighbors.Where(x => !x.Equals(node)).Any(x => x.Value[0] < neighbors[i].Value[0]))
                    {
                        //Setting the node as a neighbor
                        neighbors[i].Neighbors.Clear();
                        neighbors[i].Neighbors.Add(node);

                        //If the node equals the end, we reached our aim
                        if (node.Equals(End))
                            return;

                        //Adding node to the priority queue
                        if (!prioQueue.Contains(neighbors[i]))
                            prioQueue.Add(neighbors[i]);
                    }
                }

                //"Deactivating" the node that it will not be used again
                node.IsActive = true;

            } while (prioQueue.Any());
        }

        /// <summary>
        /// Performing a A* search
        /// </summary>
        private void AstarSearch()
        {
            NodeVisits = 0;

            //Initializing priority queue
            var prioQueue = new List<LocationTimeValue>();

            //Adding start point to the queue
            prioQueue.Add(Start);

            //Filling priority queue and iterating through it until break conditions are met
            do
            {
                NodeVisits++;

                //Order points by distance to start
                prioQueue = prioQueue.OrderBy(x => x.Value[0] + x.Value[1]).ToList();

                var node = prioQueue.First();

                prioQueue.Remove(node);

                //Getting the neighbors of the node
                var neighbors = TemporalMesh.GetNeighbors(node).Result.OrderBy(x => GeographyHelper.EuclideanDistance(x, node)).ToList();

                //Iterating over the neighbors and detect which one has the lowest distance to the node
                for (int i = 0; i < neighbors.Count(); i++)
                {
                    //Check that the point does not equals the node where we came from
                    if (node.Neighbors.Count > 0)
                        if (neighbors[i].Equals(node.Neighbors.First()))
                            continue;

                    //Determining the euclidean distance to the neighbor
                    double dist = GeographyHelper.EuclideanDistance(neighbors[i], node);

                    neighbors[i].Value[0] = node.Value[0] + dist;

                    if (neighbors[i].Neighbors.Count == 0 || !neighbors.Where(x => !x.Equals(node)).Any(x => x.Value[0] < neighbors[i].Value[0]))
                    {
                        //Setting the node as a neighbor
                        neighbors[i].Neighbors.Clear();
                        neighbors[i].Neighbors.Add(node);

                        //If the node equals the end, we reached our aim
                        if (node.Equals(End))
                            return;

                        //Adding node to the priority queue
                        if (!prioQueue.Contains(neighbors[i]))
                            prioQueue.Add(neighbors[i]);
                    }
                }

                //"Deactivating" the node that it will not be used again
                node.IsActive = true;

            } while (prioQueue.Any());
        }

        #endregion
    }
}
