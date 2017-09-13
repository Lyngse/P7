using RobotsTxt;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace WI120917
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri url = new Uri("http://www.wikipedia.org");
            crawl(url);

            Console.Read();

        }

        static void crawl(Uri urlSeed)
        {
            Queue<Uri> frontier = new Queue<Uri>();
            frontier.Enqueue(urlSeed);

            var webClient = new WebClient();

            while(pages.Count < 1000 && frontier.Count > 0)
            {
                Uri url = frontier.Dequeue();
                if (pages.ContainsKey(url))
                    continue;
                string baseUrl = url.Host;
                string html = "";
                try
                {
                    html = webClient.DownloadString(url);
                }
                catch(Exception e)
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
                    if(hrefAndAfter.Length > 1)
                        href = hrefAndAfter[1].Split("\"")[0];
                    else
                    {
                        hrefAndAfter = aTag.Split("href='");
                        if(hrefAndAfter.Length > 1)
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
                        Console.WriteLine(href);
                        continue;
                    }
                    Robots robot;
                    if(!robotTxts.TryGetValue(baseUrl, out robot))
                    {
                        robot = new Robots("http://" + baseUrl + "/robots.txt");
                    }

                    if (!robot.IsPathAllowed("sw701crawlftwplz", extractedUri))
                        continue;

                    if (!frontier.Contains( new Uri(extractedUri)))
                        frontier.Enqueue( new Uri(extractedUri));
                }
                Thread.Sleep(100);
                Console.WriteLine("Pages: " + pages.Count);
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

        static Dictionary<string, Robots> robotTxts = new Dictionary<string, Robots>();

        static Dictionary<Uri, string> pages = new Dictionary<Uri, string>();
    }
}
