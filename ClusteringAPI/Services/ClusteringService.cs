using System;
using System.Text;
using System.Diagnostics;
using ClusteringAPI.Models;

namespace ClusteringAPI.Services
{
    public partial class ClusteringService
    {
        private static string appFolderPath = PathGetDirectoryNameTimes(4, AppDomain.CurrentDomain.BaseDirectory);

        private static string PathGetDirectoryNameTimes(int times, string s)
        {
            for (int i = 0; i < times; i++)
                s = Path.GetDirectoryName(s);
            return s;
        }

        private List<Blog> _blogs;
        private int _totalAmountOfWords;
        private int _totalAmountOfWordOcurrences;
        private string[] _firstLineValues;
        private Random _random;
        private int[] _minOccurences;
        private int[] _maxOccurences;

        /// <summary>
        /// Constructor for ClusteringService.
        /// </summary>
        /// <param name="nameOfDataset">The name of the dataset to load and parse on creation.</param>
        public ClusteringService(string nameOfDataset)
        {
            ReadTXTs(nameOfDataset);
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
        }

        /// <summary>
        /// Interface for classes with a Wordcounts list (Blog and Centroid.)
        /// </summary>
        private interface IWordcountsList
        {
            public List<double> Wordcounts { get; set; }
        }

        /// <summary>
        /// Class representing the word count data of a blog.
        /// </summary>
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

        /// <summary>
        /// Method used to parse a TXT dataset into a list of Blog objects.
        /// </summary>
        /// <param name="nameOfDataset"></param>
        void ReadTXTs(string nameOfDataset)
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
            ReadTXT(@$"\datasets\{nameOfDataset}.txt", (firstLineValues, values) =>
            {
                if (values.Length == 0)
                    return;

                var newBlog = new Blog(values[0], _blogs.Count);
                for (int i = 1; i < values.Length; i++)
                    newBlog.Wordcounts.Add(Int32.Parse(values[i]));

                _blogs.Add(newBlog);
            });
        }

        /// <summary>
        /// Tests the clustering methods.
        /// </summary>
        private void TestKMeansAndHierarchicalClustering()
        {
            Console.WriteLine("Total amount of words: " + _blogs[0].Wordcounts.Count);
            Console.WriteLine("Total amount of word occurences: " + _totalAmountOfWordOcurrences);

            var centroids = ExecuteKMeansClustering(5, 20);
            PrintCentroidClusters(centroids);

            var mainCluster = ExecuteHierarchicalClustering();
            PrintCluster(mainCluster, 0);
        }

        /// <summary>
        /// Sets up arrays of min and max amounts of word occurences in the blogs.
        /// </summary>
        /// <param name="wordsTotal">The total amount of words in the data.</param>
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

        /// <summary>
        /// Iterates all blogs and returns the total amount of word occurences.
        /// </summary>
        /// <returns>int</returns>
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

        /// <summary>
        /// Algorithm for calculating Pearson similarity between two IWordcountsLists.
        /// </summary>
        /// <param name="A">An IWordcountsList.</param>
        /// <param name="B">An IWordcountsList.</param>
        /// <returns>double</returns>
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

        /// <summary>
        /// Prints out the data from the Blog instances with names specified in the argument array.
        /// </summary>
        /// <param name="blogNames">Names of Blog objects to be printed.</param>
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
    }
}
