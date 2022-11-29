using System;
using System.Text;
using System.Diagnostics;
using System.Numerics;

class Program
{
    private static string appFolderPath = PathGetDirectoryNameTimes(4, AppDomain.CurrentDomain.BaseDirectory);

    private static string PathGetDirectoryNameTimes(int times, string s)
    {
        for (int i = 0; i < times; i++)
            s = Path.GetDirectoryName(s);
        return s;
    }

    private static Program? _instance;
    private List<Blog> _blogs;
    private int _totalAmountOfWords;
    private int _totalAmountOfWordOcurrences;
    private string[] _firstLineValues;
    private Random _random;
    private int[] _minOccurences;
    private int[] _maxOccurences;
    private bool _showDebugMessages = true;

    static void Main(string[] args)
    {
        _instance = new Program(args);
    }

    public Program(string[] args)
    {
        ReadTXTs();
        _random = new Random();
        //TestTXTParsing(new string[]{ "The Superficial - Because You're Ugly", "Publishing 2.0" });

        _totalAmountOfWords = _blogs[0].Wordcounts.Count;
        _totalAmountOfWordOcurrences = GetTotalAmountOfWordOccurencesInBlogs();
        InitiateMinAndMaxOccurencesArrays(_totalAmountOfWords);

        Console.WriteLine("Total amount of words: " + _blogs[0].WordcountsDictionary.Count);
        Console.WriteLine("Total amount of word occurences: " + _totalAmountOfWordOcurrences);

        var centroids = ExecuteKMeansClustering(5, 20);
        PrintCentroidClusters(centroids);
    }

    private void PrintCentroidClusters(Centroid[] centroids)
    {
        for (int i = 0; i < centroids.Length; i++)
        {
            Centroid? centroid = centroids[i];
            Console.WriteLine($"Cluster {i} ({centroid.BlogAssignments.Count}):");
            foreach (var blogAssignment in centroid.BlogAssignments)
                Console.WriteLine('\t' + blogAssignment.Name);
        }
    }

    private void InitiateMinAndMaxOccurencesArrays(int wordsTotal)
    {
        _minOccurences = new int[_blogs[0].Wordcounts.Count];
        _maxOccurences = new int[_blogs[0].Wordcounts.Count];
        for (int i = 0; i < _minOccurences.Length; i++)
        {
            int currentMin = (int)_blogs[0].Wordcounts[0];
            int currentMax = (int)_blogs[0].Wordcounts[0];
            foreach (var blog in _blogs)
            {
                if (blog.Wordcounts[i] < currentMin)
                    currentMin = (int)blog.Wordcounts[i];
                if (blog.Wordcounts[i] > currentMax)
                    currentMax = (int)blog.Wordcounts[i];
            }
            _minOccurences[i] = currentMin;
            _maxOccurences[i] = currentMax;
        }
    }

    private int GetTotalAmountOfWordOccurencesInBlogs()
    {
        int words = 0;
        foreach (var blog in _blogs)
        {
            foreach (KeyValuePair<string, int> wordcount in blog.WordcountsDictionary)
                words += wordcount.Value;
        }
        return words;
    }

    private interface IWordcountsList
    {
        public List<double> Wordcounts { get; set; }
    }

    private class Centroid : IWordcountsList
    {
        private double _x;
        private double _y;
        private Vector2 _position;
        private List<Blog> _blogAssignments;

        public Centroid()
        {
            _blogAssignments = new List<Blog>();
            _wordcounts = new List<double>();
        }

        public void Assign(Blog blog) => _blogAssignments.Add(blog);
        public void Assign(List<Blog> blogs, string blogName)
        {
            var blog = blogs.Find(b => b.Name == blogName);
            if (blog != null)
                Assign(blog);
        }

        public double X { get => _x; set => _x = value; }
        public double Y { get => _y; set => _y = value; }
        public List<Blog> BlogAssignments { get => _blogAssignments; set => _blogAssignments = value; }
        public List<double> Wordcounts { get => _wordcounts; set => _wordcounts = value; }
        public Vector2 Position { get => _position; set => _position = value; }

        private List<double> _wordcounts;
    }

    private Centroid[] ExecuteKMeansClustering(int clustersAmount, int maxIterations)
    {
        // Number of words
        int n = _totalAmountOfWords;
        var centroids = new List<Centroid>();

        // Generate K random Centroids
        for (int i = 0; i < clustersAmount; i++)
        {
            var centroid = new Centroid();
            for (int j = 0; j < n; j++)
                centroid.Wordcounts.Add(_random.Next(_minOccurences[i], _maxOccurences[i] + 1));
            centroids.Add(centroid);
        }

        // Iteration loop
        for (int i = 0; i < maxIterations; i++)
        {
            // Clear assignments for all centroids
            foreach (var centroid in centroids)
                centroid.BlogAssignments = new List<Blog>();

            // Assign each blog to closest centroid
            foreach (var blog in _blogs)
            {
                double distance = double.MaxValue;
                Centroid? bestCentroid = null;
                // Find closest centroid
                foreach (Centroid centroid in centroids)
                {
                    double centroidDistance = Pearson(centroid, blog);
                    //Console.WriteLine(centroidDistance);
                    if (centroidDistance < distance)
                    {
                        bestCentroid = centroid;
                        distance = centroidDistance;
                    }
                }
                if (bestCentroid != null)
                {
                    bestCentroid.Assign(blog);
                    //Console.WriteLine(bestCentroid.BlogAssignments.Count);
                }
                    
                
            }

            // Recalculate center for each centroid
            foreach (var centroid in centroids)
            {
                // Find average count for each word
                for (int j = 0; j < n; j++)
                {
                    double avg = 0;
                    // Iterate over all blogs assigned to this centroid
                    foreach (var blog in centroid.BlogAssignments)
                        avg += blog.Wordcounts[j];

                    avg /= centroid.BlogAssignments.Count;

                    // Update word count for the centroid
                    centroid.Wordcounts[j] = avg;
                }
            }
            // End of iteration loop - all done.
        }
        return centroids.ToArray();
    }

    private double Pearson(IWordcountsList A, IWordcountsList B)
    {
        // Init variables
        double sumA = 0;
        double sumB = 0;
        double sumAsq = 0;
        double sumBsq = 0;
        double pSum = 0;

        // Number of words
        var n = _totalAmountOfWords;

        // Iterate over all words
        for (int i = 0; i < n; i++)
        {
            var countA = A.Wordcounts[i]; // Word counts for each word in A
            var countB = B.Wordcounts[i]; // Word counts for each word in B
            sumA += countA; // Sum of word counts for A
            sumB += countB; // Sum of word counts for B
            sumAsq += Math.Pow(countA, 2); // Sum of squared word counts for A
            sumBsq += Math.Pow(countB, 2); // Sum of squared word counts for B
            pSum += countA * countB; // Product of word counts from A and B
            //Console.WriteLine(countA + ':' + countB);
        }
        //Console.WriteLine(n);
        //Console.WriteLine(sumAsq + ':' + sumBsq);

        // Calculate Pearson
        var num = pSum - (sumA * sumB / n);
        var den = Math.Sqrt((sumAsq - Math.Pow(sumA, 2) / n) * (sumBsq - Math.Pow(sumB, 2) / n));
        // Return inverted Pearson score
        //Console.WriteLine(num + ':' + den);
        return 1.0 - num / den;
    }

    private void TestTXTParsing(string[] blogNames)
    {
        foreach (string blogName in blogNames)
        {
            var blog = _blogs.Find(blog => blog.Name == blogName);
            Console.WriteLine($"Printing data on blog '{blog.Name}':");
            //foreach (KeyValuePair<string, int> wordcount in blog.WordcountsDictionary)
            //    Console.WriteLine($"\t{wordcount.Key} : {wordcount.Value}");
            for (int i = 0; i < blog.Wordcounts.Count; i++)
            {
                int wordcount = (int)blog.Wordcounts[i];
                Console.WriteLine($"\t{_firstLineValues[i]} : {wordcount}");
            }
        }
    }

    private class Blog : IWordcountsList
    {
        private string _name;
        private Dictionary<string, int> _wordcountsDictionary;
        private List<double> _wordcounts;

        //public Vector2 Position { get => _position; set => _position = value; }

        public string Name { get => _name; set => _name = value; }
        public Dictionary<string, int> WordcountsDictionary { get => _wordcountsDictionary; set => _wordcountsDictionary = value; }
        public List<double> Wordcounts { get => _wordcounts; set => _wordcounts = value; }

        public Blog(string name)
        {
            _name = name;
            _wordcountsDictionary = new Dictionary<string,int>();
            _wordcounts = new List<double>();
        }
    }

    void ReadTXTs()
    {
        void ReadTXT(string path, Action<string[], string[]> onReadLineAction)
        {
            using (var reader = new StreamReader(appFolderPath + path))
            {
                bool isFirstLine = true;
                string[] firstLineValues = null;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('\t');

                    if (!isFirstLine)
                        onReadLineAction(firstLineValues, values);
                    else
                    {
                        firstLineValues = values;
                        _firstLineValues = firstLineValues;
                        isFirstLine = false;
                    }
                }
            }
        }

        _blogs = new List<Blog>();
        ReadTXT(@"\datasets\blogdata.txt", (firstLineValues, values) =>
        {
            if (values.Length == 0)
                return;

            var newBlog = new Blog(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                newBlog.WordcountsDictionary.Add(firstLineValues[i], Int32.Parse(values[i]));
                newBlog.Wordcounts.Add(Int32.Parse(values[i]));
            }
            _blogs.Add(newBlog);
        });
        /*ReadCSV(@"\datasets\example\movies.csv", (values)
            => _movies.Add(new Movie(Int32.Parse(values[0]), values[1], Int32.Parse(values[2]))));
        ReadCSV(@"\datasets\example\ratings.csv", (values)
            => _movieRatings.Add(
                new MovieRating(Int32.Parse(values[0]), Int32.Parse(values[1]), double.Parse(values[2].Replace('.', ',')))));*/
    }
}
