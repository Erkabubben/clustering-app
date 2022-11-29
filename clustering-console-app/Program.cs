using System;
using System.Text;
using System.Diagnostics;

class Program
{
    static string appFolderPath =
        Path.GetDirectoryName(
            Path.GetDirectoryName(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        AppDomain.CurrentDomain.BaseDirectory))));

    private static Program _instance;
    private List<Blog> _blogs;

    static void Main(string[] args)
    {
        _instance = new Program(args);
    }

    public Program(string[] args)
    {
        ReadTXTs();
        TestTXTParsing(new string[]{ "The Superficial - Because You're Ugly", "Publishing 2.0" });
    }

    private void TestTXTParsing(string[] blogNames)
    {
        foreach (string blogName in blogNames)
        {
            var blog = _blogs.Find(blog => blog.Name == blogName);
            Console.WriteLine($"Printing data on blog '{blog.Name}':");
            foreach (KeyValuePair<string, int> wordcount in blog.Wordcounts)
                Console.WriteLine($"\t{wordcount.Key} : {wordcount.Value}");
        }
    }

    private class Blog
    {
        private string _name;
        private Dictionary<string, int> _wordcounts;

        public string Name { get => _name; set => _name = value; }
        public Dictionary<string, int> Wordcounts { get => _wordcounts; set => _wordcounts = value; }
        public Blog(string name)
        {
            _name = name;
            _wordcounts = new Dictionary<string,int>();
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
                newBlog.Wordcounts.Add(firstLineValues[i], Int32.Parse(values[i]));
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