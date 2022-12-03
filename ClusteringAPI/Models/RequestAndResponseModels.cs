namespace ClusteringAPI.Models
{
    public class UserRequest
    {
        public string User { get; set; }
        public string Similarity { get; set; }
        public int Results { get; set; }
    }

    public class TopMatchingUserResponse
    {
        public string[] Users { get; set; }
        public double[] Similarities { get; set; }

        public TopMatchingUserResponse(string[] users, double[] similarities, int maxAmount = -1)
        {
            if (maxAmount != -1)
            {
                Array.Resize(ref users, Math.Min(users.Length, maxAmount));
                Array.Resize(ref similarities, Math.Min(similarities.Length, maxAmount));
            }
            Users = users;
            Similarities = similarities;
        }
    }

    public class UserNamesListResponse
    {
        public List<string> UserNames { get; set; }
        public UserNamesListResponse(List<string> userNames) => UserNames = userNames;
    }

    public class MovieRecommendationsResponse
    {
        public List<string> Movies { get; set; }
        public List<string> Ids { get; set; }
        public List<string> Scores { get; set; }
        public MovieRecommendationsResponse(List<string> movies, List<string> ids, List<string> scores, int maxAmount = -1)
        {
            List<T> CutList<T>(List<T> originalList, int maxAmount)
            {
                var newList = new List<T>();
                for (int i = 0; i < Math.Min(originalList.Count, maxAmount); i++)
                    newList.Add(originalList[i]);
                return newList;
            }
            Movies = (maxAmount == -1) ? movies : CutList<string>(movies, maxAmount);
            Ids = (maxAmount == -1) ? ids : CutList<string>(ids, maxAmount);
            Scores = (maxAmount == -1) ? scores : CutList<string>(scores, maxAmount);
        }
    }
}
