using ClusteringAPI.Services;

namespace ClusteringAPI.Models
{
    public class KMeansClusteringResponse
    {
        public List<string[]> Centroids { get; set; }

        public KMeansClusteringResponse(List<string[]> centroids)
        {
            Centroids = centroids;
        }
    }

    public class HierarchicalClusteringResponse
    {
        public class ResponseCluster
        {
            public int Left { get; set; }
            public int Right { get; set; }
            public int Parent { get; set; }
            public string Blog { get; set; }
        }

        public List<ResponseCluster> Clusters { get; set; }

        public HierarchicalClusteringResponse(List<ResponseCluster> responseClusterList)
        {
            Clusters = responseClusterList;
        }
    }
}
