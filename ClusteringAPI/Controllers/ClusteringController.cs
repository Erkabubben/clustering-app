using Microsoft.AspNetCore.Mvc;
using ClusteringAPI.Models;
using ClusteringAPI.Services;

namespace ClusteringAPI.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    [Route("Recommendations")]
    public class ClusteringController : ControllerBase
    {
        private readonly RecommendationService _recommendationSystemService;

        public ClusteringController()
        {
            //_recommendationSystemService = new ClusteringService("large");
        }

        [HttpGet(Name = "Index")]
        public ActionResult<bool> Index()
        {
            return true;
        }

        // {baseURL}/api/recommendation/{methodName}
        [HttpPost][Route("FindTopMatchingUsers")]
        public ActionResult<TopMatchingUserResponse> FindTopMatchingUsers(UserRequest request)
        {
            return _recommendationSystemService.FindTopMatchingUsers(request);
        }

        [HttpPost]
        [Route("FindMovieRecommendationsForUser")]
        public ActionResult<MovieRecommendationsResponse> FindMovieRecommendationsForUser(UserRequest request)
        {
            return _recommendationSystemService.FindMovieRecommendationsForUser(request);
        }

        [HttpPost]
        [Route("FindMovieRecommendationsForUserItemBased")]
        public ActionResult<MovieRecommendationsResponse> FindMovieRecommendationsForUserItemBased(UserRequest request)
        {
            return _recommendationSystemService.FindMovieRecommendationsForUserItemBased(request);
        }

        [HttpGet]
        [Route("GetUsersList")]
        public ActionResult<UserNamesListResponse> GetUsersList()
        {
            return _recommendationSystemService.GetUsersList();
        }
    }
}
