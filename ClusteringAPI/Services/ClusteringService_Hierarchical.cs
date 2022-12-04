using System;
using System.Text;
using System.Diagnostics;
using ClusteringAPI.Models;

namespace ClusteringAPI.Services
{
    public partial class ClusteringService
    {
        /// <summary>
        /// A Cluster used in Hierarchical Clustering.
        /// </summary>
        private class Cluster
        {
            private Cluster _parent;
            private Cluster _left;
            private Cluster _right;
            private Blog _blog;
            private double _distance;
            public Cluster Left { get => _left; set => _left = value; }
            public Cluster Right { get => _right; set => _right = value; }
            public Blog Blog { get => _blog; set => _blog = value; }
            public double Distance { get => _distance; set => _distance = value; }
            public Cluster Parent { get => _parent; set => _parent = value; }
            public Cluster() { }
            public Cluster(Blog blog) => _blog = blog;
        }

        /// <summary>
        /// Runs the ExecuteHierarchicalClustering() algorithm and returns the result as a HierarchicalClusteringResponse.
        /// </summary>
        /// <returns>HierarchicalClusteringResponse</returns>
        public HierarchicalClusteringResponse GetHierarchicalClusters()
        {
            void AddClusterAndChildrenToAllClustersList(Cluster cluster, Dictionary<Cluster, int> indexDictionary, List<Cluster> allClusters)
            {
                if (!indexDictionary.ContainsKey(cluster))
                {
                    indexDictionary.Add(cluster, allClusters.Count);
                    allClusters.Add(cluster);
                }
                if (cluster.Left != null)
                    AddClusterAndChildrenToAllClustersList(cluster.Left, indexDictionary, allClusters);
                if (cluster.Right != null)
                    AddClusterAndChildrenToAllClustersList(cluster.Right, indexDictionary, allClusters);
            }

            var mainCluster = ExecuteHierarchicalClustering();
            var allClusters = new List<Cluster>();
            var responseClusters = new List<HierarchicalClusteringResponse.ResponseCluster>();
            var indexDictionary = new Dictionary<Cluster, int>();

            // Populate the allClusters list by recursively adding all clusters.
            AddClusterAndChildrenToAllClustersList(mainCluster, indexDictionary, allClusters);

            // Convert allClusters to HierarchicalClusteringResponse.ResponseClusters and adds them to responseClusters list.
            foreach (var cluster in allClusters)
            {
                var responseCluster = new HierarchicalClusteringResponse.ResponseCluster();
                responseCluster.Blog = cluster.Blog != null ? cluster.Blog.Name : null;
                responseCluster.Left = cluster.Left != null && indexDictionary.ContainsKey(cluster.Left) ? indexDictionary[cluster.Left] : -1;
                responseCluster.Right = cluster.Right != null && indexDictionary.ContainsKey(cluster.Right) ? indexDictionary[cluster.Right] : -1;
                responseCluster.Parent = cluster.Parent != null && indexDictionary.ContainsKey(cluster.Parent) ? indexDictionary[cluster.Parent] : -1;
                responseClusters.Add(responseCluster);
            }
            // Creates and returns a HierarchicalClusteringResponse from the responseClusters list.
            return new HierarchicalClusteringResponse(responseClusters);
        }

        /// <summary>
        /// Generates clusters by the Hierarchical Clustering algorithm and returns the top parent cluster.
        /// </summary>
        /// <returns>Cluster</returns>
        private Cluster ExecuteHierarchicalClustering()
        {
            var clusters = new List<Cluster>();

            foreach (var blog in _blogs)
                clusters.Add(new Cluster(blog));

            for (int i = 0; i < 10000; i++)
            {
                clusters = Iterate(clusters);
                if (clusters.Count <= 1)
                    break;
            }

            return clusters[0];
        }

        /// <summary>
        /// Recursively iterates the children of a Cluster and prints their contents to the console.
        /// </summary>
        /// <param name="cluster">The top parent Cluster to start from.</param>
        /// <param name="indents">The current amount of indentations to be applied to the console print.</param>
        private void PrintCluster(Cluster cluster, int indents)
        {
            string MultiplyIndents(int indents)
            {
                string s = "";
                for (int i = 0; i < indents; i++)
                    s += '-';
                return s;
            }

            indents++;

            if (cluster.Blog != null && cluster.Blog.Id != -1)
                Console.WriteLine("\n" + MultiplyIndents(indents) + cluster.Blog.Name);
            else
                Console.WriteLine("\n" + MultiplyIndents(indents) + "x");
            if (cluster.Left != null)
                PrintCluster(cluster.Left, indents);
            if (cluster.Right != null)
                PrintCluster(cluster.Right, indents);
        }

        /// <summary>
        /// Iteration function used by the ExecuteHierarchicalClustering method.
        /// </summary>
        /// <param name="clusters">A list of clusters.</param>
        /// <returns>List<Cluster></returns>
        private List<Cluster> Iterate(List<Cluster> clusters)
        {
            // Find two closest nodes
            double closest = double.MaxValue;
            Cluster A = null;
            Cluster B = null;
            foreach (var clusterA in clusters)
            {
                foreach (var clusterB in clusters)
                {
                    double distance = Pearson(clusterA.Blog, clusterB.Blog);
                    if (distance < closest && clusterA != clusterB)
                    {
                        // New set of closest nodes found
                        closest = distance;
                        A = clusterA;
                        B = clusterB;
                    }
                }
            }
            // Merge the two clusters
            if (A == null || B == null)
                return null;

            Cluster newCluster = MergeClusters(A, B, closest);
            // Add new cluster
            clusters.Add(newCluster);
            // Remove old clusters
            clusters.Remove(A);
            clusters.Remove(B);
            return clusters;
        }

        /// <summary>
        /// Function for merging two clusters into one.
        /// </summary>
        /// <param name="A">Cluster to be merged.</param>
        /// <param name="B">Cluster to be merged.</param>
        /// <param name="distance">Cluster distance.</param>
        /// <returns>Cluster</returns>
        private Cluster MergeClusters(Cluster A, Cluster B, double distance)
        {
            // Number of words
            int n = _totalAmountOfWords;
            // Create new Cluster
            Cluster P = new Cluster();
            // Fill data
            P.Left = A;
            A.Parent = P;
            P.Right = B;
            B.Parent = P;
            // Merge blog data by averaging word counts for each word
            Blog newBlog = new Blog("", -1);
            for (int i = 0; i < n; i++)
            {
                double countA = A.Blog.Wordcounts[i];
                double countB = B.Blog.Wordcounts[i];
                // Average word count
                double count = (countA + countB) / 2;
                // Set word count to new blog
                newBlog.Wordcounts.Add(count);
            }
            // Set blog to new cluster
            P.Blog = newBlog;
            // Set distance
            P.Distance = distance;
            // Return new cluster
            return P;
        }
    }
}
