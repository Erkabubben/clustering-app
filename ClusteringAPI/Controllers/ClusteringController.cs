using Microsoft.AspNetCore.Mvc;
using ClusteringAPI.Models;
using ClusteringAPI.Services;

namespace ClusteringAPI.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    [Route("Clustering")]
    public class ClusteringController : ControllerBase
    {
        private readonly ClusteringService _clusteringService;

        public ClusteringController()
        {
            _clusteringService = new ClusteringService("blogdata");
        }

        [HttpGet(Name = "Index")]
        public ActionResult<bool> Index()
        {
            return true;
        }

        // {baseURL}/api/recommendation/{methodName}
        [HttpGet][Route("KMeansClustering")]
        public ActionResult<KMeansClusteringResponse> KMeansClustering()
        {
            return _clusteringService.GetKMeansClusters();
        }

        /*[HttpPost]
        [Route("HierarchichalClustering")]
        public ActionResult<MovieRecommendationsResponse> HierarchichalClustering(UserRequest request)
        {
            return _clusteringService.FindMovieRecommendationsForUser(request);
        }*/
    }
}
