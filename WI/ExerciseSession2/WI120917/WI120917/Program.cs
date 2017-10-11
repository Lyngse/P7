using RobotsTxt;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using SF.Snowball;
using SF.Snowball.Ext;
using ShellProgressBar;
using MathNet.Numerics.LinearAlgebra.Double;

namespace WI120917
{
    class Program
    {

        static Dictionary<string, Robots> robotTxts = new Dictionary<string, Robots>();

        static List<Webpage> pages = new List<Webpage>();

        //static Dictionary<string, Dictionary<int, List<int>>> index = new Dictionary<string, Dictionary<int, List<int>>>();

        static void Main(string[] args)
        {

            Crawler crawler = new Crawler(new Uri("http://en.wikipedia.org"));

            string indexLocation = AppDomain.CurrentDomain.BaseDirectory + "index.txt";

            //Add crawlerLocation for seperate crawlerfile
            if (!File.Exists(indexLocation))
            {
                Console.WriteLine("Starting indexing of local files");
                Indexer index = new Indexer();
                pages = crawler.Crawl();
                index.Tokenize(pages);
                WriteToBinaryFile(indexLocation, pages);

            }
            else
            {
                pages = ReadFromBinaryFile<List<Webpage>>(indexLocation);
                Console.WriteLine("Index was successfully loaded from file");
            }

            PageRanker pageRanker = new PageRanker(pages);
            DenseVector pageRank = pageRanker.GeneratePageRank(100);

            pages.OrderBy(x => x.Id);
            for (int i = 0; i < pageRank.Count; i++)
            {
                pages[i].pageRank = pageRank[i];
            }
            pages.OrderByDescending(x => x.pageRank).ToList().ForEach(x => Console.WriteLine(x.Id + " - " + x.pageRank));

            //RankSearch ranker = new RankSearch();
            //ranker.Rank("October castle castle castle", pages);

            Console.Read();

        }

        //static void RankSearch(string query)
        //{
        //    List<string> files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\docs\").ToList();
        //    int docCount = files.Count;
        //    List<int> documentIds = new List<int>();
        //    foreach (var file in files)
        //    {
        //        int fileID = int.Parse(file.Split("doc").Last().Replace(".html", ""));
        //        documentIds.Add(fileID);
        //    }

        //    string[] queryWords = query.Split(" ");
        //    queryWords = StringStemmer(queryWords);
        //    List<double> queryVector = new List<double>();
        //    Dictionary<int, double> results = new Dictionary<int, double>();
        //    Dictionary<string, int> termFrequency = new Dictionary<string, int>();

        //    foreach (var qw in queryWords)
        //    {
        //        if(!termFrequency.ContainsKey(qw))
        //        {
        //            termFrequency.Add(qw, 1);
        //        }
        //        else
        //        {
        //            termFrequency[qw]++;
        //        }
        //    }

        //    foreach (var tf in termFrequency)
        //    {
        //        if (!index.ContainsKey(tf.Key))
        //        {
        //            queryVector.Add(0);
        //            continue;
        //        }

        //        double weightedTermFrequency = 1 + Math.Log10(tf.Value);
        //        int docFrequency = index[tf.Key].Count;
        //        double iDocFrequency = Math.Log10(docCount / (double)docFrequency);
        //        double wt = weightedTermFrequency * iDocFrequency;

        //        queryVector.Add(wt);
        //    }
        //    double queryVectorLength = Math.Sqrt(queryVector.Sum(wt => wt = Math.Pow(wt, 2) ));
        //    for (int i = 0; i < queryVector.Count; i++)
        //    {
        //        queryVector[i] /= queryVectorLength;
        //    }

        //    foreach (var id in documentIds)
        //    {
        //        List<double> documentVector = new List<double>();

        //        foreach (var term in termFrequency)
        //        {
        //            if(index.ContainsKey(term.Key))
        //            {
        //                if(index[term.Key].ContainsKey(id))
        //                {
        //                    documentVector.Add(1 + Math.Log10(index[term.Key][id].Count));
        //                }
        //                else
        //                {
        //                    documentVector.Add(0);
        //                }
        //            }
        //            else
        //            {
        //                documentVector.Add(0);
        //            }
        //        }
        //        double documentVectorLength = Math.Sqrt(documentVector.Sum(wt => wt = Math.Pow(wt, 2)));
        //        if (documentVectorLength != 0.0)
        //        {
        //            for (int i = 0; i < documentVector.Count; i++)
        //            {
        //                documentVector[i] /= documentVectorLength;
        //            }
        //        }

        //        double documentScore = 0;

        //        for (int i = 0; i < documentVector.Count; i++)
        //        {
        //            documentScore += documentVector[i] * queryVector[i];
        //        }
        //        results.Add(id, documentScore);
        //    }
        //    Console.WriteLine("Are we done yet? no");

        //    var sortedResults = results.OrderByDescending(x => x.Value);
        //    sortedResults.Take(10).ToList().ForEach(x => Console.WriteLine(x.Key + " - " + x.Value));
        //}

        //static Dictionary<int, List<string>> BooleanSearch(string query)
        //{
        //    Dictionary<int, List<string>> res = new Dictionary<int, List<string>>();            

        //    string[] queryWords = query.Split(" ");

        //    queryWords = StringStemmer(queryWords);

        //    foreach (var word in queryWords)
        //    {
        //        if (index.ContainsKey(word))
        //        {
        //            foreach (var key in index[word].Keys)
        //            {
        //                if (!res.ContainsKey(key))
        //                {
        //                    List<string> words = new List<string>();
        //                    words.Add(word);
        //                    res.Add(key, words);
        //                } else
        //                {
        //                    res[key].Add(word);
        //                }
        //            }
        //        }
        //    }

        //    Dictionary<int, List<string>> tempRes = new Dictionary<int, List<string>>();

        //    foreach (var key in res.Keys)
        //    {
        //        int matches = 0;
        //        for(int i = 0; i < queryWords.Length; i++)
        //        {
        //            if (res[key].Contains(queryWords[i]))
        //            {
        //                matches++;
        //            }
        //        }
        //        if(matches == queryWords.Length)
        //        {
        //            tempRes.Add(key, res[key]);
        //        }
        //    }

        //    return tempRes;
        //}

        //static void Indexing()
        //{
        //    List<string> paths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\docs\").ToList();
        //    List<string> stopWords = File.ReadAllText(@"StopWords.txt").Split("\r\n").ToList();

        //    int pathCount = 1;
        //    using (var pbar = new ProgressBar(paths.Count / 10, "Starting", ConsoleColor.Cyan))
        //    {


        //        foreach (var path in paths)
        //        {
        //            int fileID = int.Parse(path.Split("doc").Last().Replace(".html", ""));
        //            List<string> tokens = Tokenizer(File.ReadAllText(path));
        //            int tokenPosition = 0;

        //            foreach (var stopWord in stopWords)
        //            {
        //                tokens.RemoveAll(x => x.Equals(stopWord));

        //            }

        //            var stemmer = new EnglishStemmer();
        //            for (int i = 0; i < tokens.Count; i++)
        //            {
        //                stemmer.SetCurrent(tokens[i]);
        //                if (stemmer.Stem())
        //                {
        //                    tokens[i] = stemmer.GetCurrent();
        //                }
        //            }

        //            foreach (var token in tokens)
        //            {
        //                if (index.ContainsKey(token))
        //                {
        //                    if (index[token].ContainsKey(fileID))
        //                    {
        //                        index[token][fileID].Add(tokenPosition);

        //                    }

        //                    else
        //                    {
        //                        var newIndex = new List<int>();
        //                        newIndex.Add(tokenPosition);
        //                        index[token].Add(fileID, newIndex);
        //                    }
        //                }

        //                else
        //                {
        //                    var newIndex = new List<int>();
        //                    newIndex.Add(tokenPosition);
        //                    var newDictionary = new Dictionary<int, List<int>>();
        //                    newDictionary.Add(fileID, newIndex);
        //                    index.Add(token, newDictionary);
        //                }

        //                tokenPosition++;
        //            }
        //            if (pathCount % 10 == 0)
        //            {
        //                pbar.Tick("Currently processing " + pathCount);
        //            }
        //            pathCount++;

        //        }
        //    }
        //    WriteToJsonFile(AppDomain.CurrentDomain.BaseDirectory + "index.txt", index);
            

        //    Console.WriteLine("Indexing has finished.");
        //}

        

        //static void Crawl(Uri urlSeed)
        //{
        //    Queue<Uri> frontier = new Queue<Uri>();
        //    frontier.Enqueue(urlSeed);

        //    var webClient = new WebClient();


        //    int fileNameNumber = 1;
        //    while (pages.Count < 1000 && frontier.Count > 0)
        //    {
        //        Uri url = frontier.Dequeue();
        //        if (pages.ContainsKey(url))
        //            continue;
        //        string baseUrl = "";
        //        try
        //        {
        //            baseUrl = url.Host;
        //        }
        //        catch (Exception)
        //        {
        //            Console.WriteLine("Bad url: {0}", url);
        //            continue;
        //        }
        //        string html = "";
        //        try
        //        {
        //            html = webClient.DownloadString(url);
        //            string directory = AppDomain.CurrentDomain.BaseDirectory + @"\docs\doc" + fileNameNumber + ".html";
        //            webClient.DownloadFile(url, directory);
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("Exception: " + e);
        //            continue;
        //        }
        //        pages.Add(url, html);

        //        string[] splits = html.Split("<a");
        //        for (int i = 1; i < splits.Length; i++)
        //        {
        //            string aTag = splits[i].Split('>')[0];

        //            if (!aTag.Contains("href="))
        //                continue;


        //            string[] hrefAndAfter = aTag.Split("href=\"");
        //            string href = "";
        //            if (hrefAndAfter.Length > 1)
        //                href = hrefAndAfter[1].Split("\"")[0];
        //            else
        //            {
        //                hrefAndAfter = aTag.Split("href='");
        //                if (hrefAndAfter.Length > 1)
        //                    href = hrefAndAfter[1].Split("'")[0];
        //            }
        //            string extractedUri = "";
        //            if (href.Length == 1)
        //                extractedUri = "http://" + baseUrl;
        //            else if (href.StartsWith("http"))
        //                extractedUri = href;
        //            else if (href.StartsWith("//"))
        //                extractedUri = "http:" + href;
        //            else if (href.StartsWith("/") && !href.Contains("{{"))
        //                extractedUri = "http://" + baseUrl + href;
        //            else
        //            {
        //                //Console.WriteLine(href);
        //                continue;
        //            }
        //            Robots robot;
        //            if (!robotTxts.TryGetValue(baseUrl, out robot))
        //            {
        //                robot = new Robots("http://" + baseUrl + "/robots.txt");
        //            }

        //            if (!robot.IsPathAllowed("sw701crawlftwplz", extractedUri))
        //                continue;

        //            if (!frontier.Contains(new Uri(extractedUri)))
        //            {
        //                if (extractedUri.Contains("en.wikipedia"))
        //                    frontier.Enqueue(new Uri(extractedUri));
        //            }
        //        }
        //        Thread.Sleep(100);
        //        Console.WriteLine("Pages: " + pages.Count);
        //        fileNameNumber++;
        //    }
        //    Console.WriteLine(pages.Keys);

        //}
        //
        /*
        static public List<string> getRestrictions(string baseUrl)
        {
            List<string> result;
            if (robotTxts.TryGetValue(baseUrl, out result))
            {
                return result;
            }
            result = new List<string>();
            var robotTxt = new WebClient().DownloadString("http://" + baseUrl + "/robots.txt");

            var robotTxtLines = robotTxt.Split('\n');


            int i = 0;
            while (!robotTxtLines[i].ToLower().StartsWith("user-agent:") || robotTxtLines[i].Split(' ')[1] != "*") {
                if (i == robotTxtLines.Length - 1)
                {
                    robotTxts.Add(baseUrl, result);
                    return result;
                }

                i++;
            }
            i++;
            while (true)
            {
                if (i == robotTxtLines.Length)
                    break;
                string line = robotTxtLines[i].ToLower();
                if (line.StartsWith("disallow"))
                {
                    result.Add(line.Split(' ')[1]);
                }
                //cutting corners by not considering allows in robots.txt
                else if (!line.StartsWith("#") && !line.StartsWith("allow"))
                {
                    break;
                }

                i++;
            }
            robotTxts.Add(baseUrl, result);
            return result;
        }*/
        static string[] StringStemmer(string[] stringArray)
        {
            var stemmer = new EnglishStemmer();
            for (int i = 0; i < stringArray.Length; i++)
            {
                stemmer.SetCurrent(stringArray[i]);
                if (stemmer.Stem())
                {
                    stringArray[i] = stemmer.GetCurrent();
                }
            }

            return stringArray;
        }

        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
    }
}
