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
            Indexing(); 

            Console.Read();

        }

        static void Indexing()
        {
            List<string> paths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\docs\").ToList();
            List<string> stopWords = File.ReadAllText(@"StopWords.txt").Split("\r\n").ToList();

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
                        newDictionary.Add(fileID,newIndex);
                        index.Add(token, newDictionary);
                    }
                    
                    tokenPosition++;
                }

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
                catch (Exception e)
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
