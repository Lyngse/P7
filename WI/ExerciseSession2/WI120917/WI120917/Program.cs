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
using Newtonsoft.Json;
using SF.Snowball;
using SF.Snowball.Ext;
using ShellProgressBar;

namespace WI120917
{
    class Program
    {

        static Dictionary<string, Robots> robotTxts = new Dictionary<string, Robots>();

        static Dictionary<Uri, string> pages = new Dictionary<Uri, string>();

        static Dictionary<string, Dictionary<int, List<int>>> index = new Dictionary<string, Dictionary<int, List<int>>>();

        static void Main(string[] args)
        {
            Uri url = new Uri("http://www.wikipedia.org");


            //crawl(url);

            string paths = AppDomain.CurrentDomain.BaseDirectory+"index.txt";

            if (!File.Exists(paths))
            {
                Console.WriteLine("Starting indexing of local files");
                Indexing();
            }
            else
            {
                index = ReadFromJsonFile<Dictionary<string, Dictionary<int, List<int>>>>(paths);
                Console.WriteLine("Index was successfully loaded from file");
            }

            BooleanSearch("sloterdijk temple");
            //Rank();

            Console.Read();

        }

        static void Rank()
        {

        }

        static Dictionary<int, List<string>> BooleanSearch(string query)
        {
            Dictionary<int, List<string>> res = new Dictionary<int, List<string>>();            

            string[] queryWords = query.Split(" ");

            var stemmer = new EnglishStemmer();
            for (int i = 0; i < queryWords.Length; i++)
            {
                stemmer.SetCurrent(queryWords[i]);
                if (stemmer.Stem())
                {
                    queryWords[i] = stemmer.GetCurrent();
                }
            }

            foreach (var word in queryWords)
            {
                if (index.ContainsKey(word))
                {

                    foreach (var key in index[word].Keys)
                    {
                        if (!res.ContainsKey(key))
                        {
                            List<string> words = new List<string>();
                            words.Add(word);
                            res.Add(key, words);
                        } else
                        {
                            res[key].Add(word);
                        }
                    }
                }
            }

            Dictionary<int, List<string>> tempRes = new Dictionary<int, List<string>>();

            foreach (var key in res.Keys)
            {
                int matches = 0;
                for(int i = 0; i < queryWords.Length; i++)
                {
                    if (res[key].Contains(queryWords[i]))
                    {
                        matches++;
                    }
                }
                if(matches == queryWords.Length)
                {
                    tempRes.Add(key, res[key]);
                }
            }

            return tempRes;
        }

        static void Indexing()
        {
            List<string> paths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\docs\").ToList();
            List<string> stopWords = File.ReadAllText(@"StopWords.txt").Split("\r\n").ToList();

            int pathCount = 1;
            using (var pbar = new ProgressBar(paths.Count / 10, "Starting", ConsoleColor.Cyan))
            {


                foreach (var path in paths)
                {
                    int fileID = int.Parse(path.Split("doc").Last().Replace(".html", ""));
                    List<string> tokens = Tokenizer(File.ReadAllText(path));
                    int tokenPosition = 0;

                    foreach (var stopWord in stopWords)
                    {
                        tokens.RemoveAll(x => x.Equals(stopWord));

                    }

                    var stemmer = new EnglishStemmer();
                    for (int i = 0; i < tokens.Count; i++)
                    {
                        stemmer.SetCurrent(tokens[i]);
                        if (stemmer.Stem())
                        {
                            tokens[i] = stemmer.GetCurrent();
                        }
                    }

                    foreach (var token in tokens)
                    {
                        if (index.ContainsKey(token))
                        {
                            if (index[token].ContainsKey(fileID))
                            {
                                index[token][fileID].Add(tokenPosition);

                            }

                            else
                            {
                                var newIndex = new List<int>();
                                newIndex.Add(tokenPosition);
                                index[token].Add(fileID, newIndex);
                            }
                        }

                        else
                        {
                            var newIndex = new List<int>();
                            newIndex.Add(tokenPosition);
                            var newDictionary = new Dictionary<int, List<int>>();
                            newDictionary.Add(fileID, newIndex);
                            index.Add(token, newDictionary);
                        }

                        tokenPosition++;
                    }
                    if (pathCount % 10 == 0)
                    {
                        pbar.Tick("Currently processing " + pathCount);
                    }
                    pathCount++;

                }
            }
            WriteToJsonFile(AppDomain.CurrentDomain.BaseDirectory + "index.txt", index);
            

            Console.WriteLine("Indexing has finished.");
        }


        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }


        static List<string> Tokenizer(string htmlPage)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlPage);
            string cleanText = htmlDocument.DocumentNode.SelectSingleNode("/html/body").InnerText;

            cleanText = HttpUtility.HtmlDecode(cleanText);

            string tagCleanedText = Regex.Replace(cleanText, @"<[^>]*>", "");
            tagCleanedText = tagCleanedText.Replace("-", " ").Replace("_", " ");

            /*tagCleanedText = tagCleanedText.Replace("\n", " ").Replace("\r", " ").Replace("\t", " ").Replace(":", "")
                .Replace("'", "").Replace(".", "").Replace(",", "").Replace(";", "").Replace("-", " ").Replace("_", " ").Replace("/","");*/
            tagCleanedText = Regex.Replace(tagCleanedText, @"[^a-z|A-Z|\s]", "");
            tagCleanedText = Regex.Replace(tagCleanedText, @"[0-9]", "");
            tagCleanedText = Regex.Replace(tagCleanedText, @"\s+", " ");
            tagCleanedText = tagCleanedText.ToLower();

            return tagCleanedText.Split(" ").ToList();
        }

        static void crawl(Uri urlSeed)
        {
            Queue<Uri> frontier = new Queue<Uri>();
            frontier.Enqueue(urlSeed);

            var webClient = new WebClient();


            int fileNameNumber = 1;
            while (pages.Count < 1000 && frontier.Count > 0)
            {
                Uri url = frontier.Dequeue();
                if (pages.ContainsKey(url))
                    continue;
                string baseUrl = "";
                try
                {
                    baseUrl = url.Host;
                }
                catch (Exception)
                {
                    Console.WriteLine("Bad url: {0}", url);
                    continue;
                }
                string html = "";
                try
                {
                    html = webClient.DownloadString(url);
                    string directory = AppDomain.CurrentDomain.BaseDirectory + @"\docs\doc" + fileNameNumber + ".html";
                    webClient.DownloadFile(url, directory);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e);
                    continue;
                }
                pages.Add(url, html);

                string[] splits = html.Split("<a");
                for (int i = 1; i < splits.Length; i++)
                {
                    string aTag = splits[i].Split('>')[0];

                    if (!aTag.Contains("href="))
                        continue;


                    string[] hrefAndAfter = aTag.Split("href=\"");
                    string href = "";
                    if (hrefAndAfter.Length > 1)
                        href = hrefAndAfter[1].Split("\"")[0];
                    else
                    {
                        hrefAndAfter = aTag.Split("href='");
                        if (hrefAndAfter.Length > 1)
                            href = hrefAndAfter[1].Split("'")[0];
                    }
                    string extractedUri = "";
                    if (href.Length == 1)
                        extractedUri = "http://" + baseUrl;
                    else if (href.StartsWith("http"))
                        extractedUri = href;
                    else if (href.StartsWith("//"))
                        extractedUri = "http:" + href;
                    else if (href.StartsWith("/") && !href.Contains("{{"))
                        extractedUri = "http://" + baseUrl + href;
                    else
                    {
                        //Console.WriteLine(href);
                        continue;
                    }
                    Robots robot;
                    if (!robotTxts.TryGetValue(baseUrl, out robot))
                    {
                        robot = new Robots("http://" + baseUrl + "/robots.txt");
                    }

                    if (!robot.IsPathAllowed("sw701crawlftwplz", extractedUri))
                        continue;

                    if (!frontier.Contains(new Uri(extractedUri)))
                    {
                        if (extractedUri.Contains("en.wikipedia"))
                            frontier.Enqueue(new Uri(extractedUri));
                    }
                }
                Thread.Sleep(100);
                Console.WriteLine("Pages: " + pages.Count);
                fileNameNumber++;
            }
            Console.WriteLine(pages.Keys);

        }
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
    }
}
