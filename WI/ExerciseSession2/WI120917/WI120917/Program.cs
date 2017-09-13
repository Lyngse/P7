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

            crawl("http://www.twitch.com");


            Console.Read();

        }

        static void crawl(string urlSeed)
        {
            Queue<string> frontier = new Queue<string>();
            frontier.Enqueue(urlSeed);

            var webClient = new WebClient();

            while(pages.Count < 1000 && frontier.Count > 0)
            {
                string url = frontier.Dequeue();
                string html = webClient.DownloadString(url);

                string[] splits = html.Split("<a");
                for (int i = 1; i < splits.Length; i++)
                {
                    string aTag = splits[i].Split('>')[0];

                    if (!aTag.Contains("href="))
                        continue;
                    
                    string href = aTag.Split("href=")[1];
                    if (href.StartsWith('\\'))
                        continue;
                    

                    string éxtractedUrl = splits[i].Split('"')[1];
                }


                Thread.Sleep(3000);
            }

        }

        static public List<string> getRestrictions(string baseUrl)
        {
            List<string> result;
            if (robotTxts.TryGetValue(baseUrl, out result))
            {
                return result;
            }
            result = new List<string>();
            var robotTxt = new WebClient().DownloadString(baseUrl + "/robots.txt");

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
        }

        static Dictionary<string, List<string>> robotTxts = new Dictionary<string, List<string>>();

        static Dictionary<string, string> pages = new Dictionary<string, string>();
    }
}
