using System;
using System.Text;
using System.Diagnostics;

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

    static void Main(string[] args)
    {
        _instance = new Program(args);
    }

    public Program(string[] args)
    {
        ReadTXTs();
        _random = new Random();
        //TestTXTParsing(new string[]{ "The Superficial - Because You're Ugly", "Publishing 2.0" });

        if (_blogs == null || _blogs.Count <= 0)
        {
            Console.WriteLine("ERROR: No blogs were found - exiting.");
            return;
        }

        _totalAmountOfWords = _blogs[0].Wordcounts.Count;
        _totalAmountOfWordOcurrences = GetTotalAmountOfWordOccurencesInBlogs();
        InitiateMinAndMaxOccurencesArrays(_totalAmountOfWords);

        Console.WriteLine("Total amount of words: " + _blogs[0].Wordcounts.Count);
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
        double words = 0;
        foreach (var blog in _blogs)
        {
            foreach (var wordcount in blog.Wordcounts)
                words += wordcount;
        }
        return (int)words;
    }

    private Cluster MergeClusters(Cluster clusterA, Cluster clusterB, double distance)
    {
        // Number of words
        int n = _totalAmountOfWords;
        // Create new Cluster
        Cluster newCluster = new Cluster();
        // Fill data
        newCluster.Left = clusterA;
        clusterA.Parent = newCluster;
        newCluster.Right = clusterB;
        clusterB.Parent = newCluster;
        // Merge blog data by averaging word counts for each word
        Blog newBlog = new Blog("", -1);
        for (int i = 0; i < n; i++)
        {
            double countA = clusterA.Blog.Wordcounts[i];
            double countB = clusterB.Blog.Wordcounts[i];
            // Average word count
            double count = (countA + countB) / 2;
            // Set word count to new blog
            newBlog.Wordcounts[i] = count;
        }
        // Set blog to new cluster
        newCluster.Blog = newBlog;
        // Set distance
        newCluster.Distance = distance;
        // Return new cluster
        return newCluster;
    }

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
        public Cluster(){}
        public Cluster(Blog blog) => _blog = blog;
        public Cluster(Cluster left, Cluster right, double distance)
        {
            _left = left;
            _right = right;
            _distance = distance;
        }    
    }

    private interface IWordcountsList
    {
        public List<double> Wordcounts { get; set; }
    }

    private class Centroid : IWordcountsList
    {
        private List<Blog> _blogAssignments;
        public List<Blog> BlogAssignments { get => _blogAssignments; set => _blogAssignments = value; }
        public List<double> Wordcounts { get => _wordcounts; set => _wordcounts = value; }
        public string PreviousAssignments { get => _previousAssignments; set => _previousAssignments = value; }
        public bool IsFinished { get => _isFinished; set => _isFinished = value; }

        private List<double> _wordcounts;
        private string _previousAssignments = "";
        private bool _isFinished = false;

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
                    if (centroidDistance < distance)
                    {
                        bestCentroid = centroid;
                        distance = centroidDistance;
                    }
                }
                if (bestCentroid != null)
                    bestCentroid.Assign(blog);
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
            // Check AssignmentsStrings against strings from previous iteration.
            foreach (var centroid in centroids)
            {
                var assignmentsString = GetAssignmentsString(centroid);
                if (centroid.PreviousAssignments == assignmentsString)
                    centroid.IsFinished = true;
                else
                    centroid.PreviousAssignments = assignmentsString;
            }
            int amountOfFinishedCentroids = 0;
            // Break iteration loop if all centroids have finished.
            foreach (var centroid in centroids)
                amountOfFinishedCentroids += centroid.IsFinished ? 1 : 0;
            if (amountOfFinishedCentroids == centroids.Count)
                break;
            // End of iteration loop - all done.
        }
        return centroids.ToArray();
    }

    private string GetAssignmentsString(Centroid centroid)
    {
        string s = "";
        foreach (var blog in centroid.BlogAssignments)
            s += blog.Id;
        return s;
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
        }
        // Calculate Pearson
        var num = pSum - (sumA * sumB / n);
        var den = Math.Sqrt((sumAsq - Math.Pow(sumA, 2) / n) * (sumBsq - Math.Pow(sumB, 2) / n));
        // Return inverted Pearson score
        return 1.0 - num / den;
    }

    private void TestTXTParsing(string[] blogNames)
    {
        foreach (string blogName in blogNames)
        {
            var blog = _blogs.Find(blog => blog.Name == blogName);
            Console.WriteLine($"Printing data on blog '{blog.Name}':");
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
        private int _id;
        private List<double> _wordcounts;

        public string Name { get => _name; set => _name = value; }
        public List<double> Wordcounts { get => _wordcounts; set => _wordcounts = value; }
        public int Id => _id;

        public Blog(string name, int id)
        {
            _name = name;
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

            var newBlog = new Blog(values[0], _blogs.Count);
            for (int i = 1; i < values.Length; i++)
                newBlog.Wordcounts.Add(Int32.Parse(values[i]));

            _blogs.Add(newBlog);
        });
    }
}
