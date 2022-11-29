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

    private static Program _instance;
    private List<Blog> _blogs;
    private int _totalAmountOfWords;
    private int _totalAmountOfWordOcurrences;
    private string[] _firstLineValues;

    static void Main(string[] args)
    {
        _instance = new Program(args);
    }

    public Program(string[] args)
    {
        ReadTXTs();
        TestTXTParsing(new string[]{ "The Superficial - Because You're Ugly", "Publishing 2.0" });

        Console.WriteLine("Total amount of words: " + _blogs[0].WordcountsDictionary.Count);
        _totalAmountOfWordOcurrences = GetTotalAmountOfWordOccurencesInBlogs();
        Console.WriteLine("Total amount of word occurences: " + _totalAmountOfWordOcurrences);

        ExecuteKMeansClustering(5);
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

    private class Centroid
    {
        private double _x;
        private double _y;
        private List<Blog> _blogAssignments;

        public Centroid()
        {
            _blogAssignments = new List<Blog>();
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
        private List<Blog> BlogAssignments { get => _blogAssignments; set => _blogAssignments = value; }
    }

    private void ExecuteKMeansClustering(int clustersAmount)
    {
        int n = _totalAmountOfWords;

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
                int wordcount = blog.Wordcounts[i];
                Console.WriteLine($"\t{_firstLineValues[i]} : {wordcount}");
            }
        }
    }

    private class Blog
    {
        private string _name;
        private Dictionary<string, int> _wordcountsDictionary;
        private List<int> _wordcounts;

        public string Name { get => _name; set => _name = value; }
        public Dictionary<string, int> WordcountsDictionary { get => _wordcountsDictionary; set => _wordcountsDictionary = value; }
        public List<int> Wordcounts { get => _wordcounts; set => _wordcounts = value; }

        public Blog(string name)
        {
            _name = name;
            _wordcountsDictionary = new Dictionary<string,int>();
            _wordcounts = new List<int>();
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