using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using HtmlAgilityPack;
using RobotsTxt;

namespace WI120917
{
    //Corners cut: HtmlAgilityPack and RobotsTxt libraries are used for:
    //HtmlAgilityPack to extract any link from a webpage.
    //RobotsTxt are used to enforce politeness by the RobotsTxt files associated to the link.
    class Crawler
    {
        private Uri _urlSeed;
        private WebClient _webClient = new WebClient();
        public List<Webpage> pages = new List<Webpage>();
        private Queue<Uri> frontier = new Queue<Uri>();
        private Dictionary<string, Robots> robotTxts = new Dictionary<string, Robots>();

        public Crawler(Uri urlSeed)
        {
            this._urlSeed = urlSeed;
        }

        //Crawler sets the htmlcontent, htmlLinks and ID of a Webpage.
        //Corners cut: Does only crawl the English Wikipedia webpages.
        //Crawls until 1000 webpages have been retrieved.
        //Near-duplicate detection: Only checks whether or not a webpage has the same URL.
        public List<Webpage> Crawl()
        {            
            frontier.Enqueue(this._urlSeed);

            int fileNameNumber = 1;            

            while(pages.Count < 1000 && frontier.Count > 0)

            {
                Uri url = frontier.Dequeue();
                Webpage currentPage = new Webpage(fileNameNumber, url);

                if (pages.Contains(currentPage))
                {
                    continue;
                }               

                try
                {
                    currentPage.htmlContent = _webClient.DownloadString(currentPage.uri);
                    string directory = AppDomain.CurrentDomain.BaseDirectory + @"\docs\doc" + fileNameNumber + ".html";
                    _webClient.DownloadFile(url, directory);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e);
                    continue;
                }

                List<Uri> pageLinks = ExtractLinks(currentPage.uri);

                currentPage.htmlLinks = pageLinks;

                pages.Add(currentPage);
                

                Robots robot;

                foreach (Uri link in pageLinks)
                {
                    if (!frontier.Contains(link) && !pages.Exists(x => x.uri == link))
                    {
                        if(!robotTxts.TryGetValue(currentPage.uri.Host, out robot))
                        {
                            robot = new Robots("http://" + currentPage.uri.Host + "/robots.txt");
                        }
                        if(currentPage.uri.Host == "en.wikipedia.org")
                        {
                            if (robot.IsPathAllowed("sw701crawlftwplz", link.ToString()))
                            {
                                frontier.Enqueue(link);
                            }
                        }                        
                    }
                }
                Console.WriteLine("Pages: " + pages.Count);
                fileNameNumber++;
            }
            return pages;
        }

        //ExtractLinks returns a list of Uri's, which are put into a Webpage's htmlLinks.
        //HtmlAgilityPack is used here to extract links from an HtmlDocument.
        List<Uri> ExtractLinks(Uri extractSeed)
        {
            List<Uri> extractedLinks = new List<Uri>();
            HtmlWeb page = new HtmlWeb();

            try
            {
                HtmlDocument pageContent = page.Load(extractSeed);
                foreach (HtmlNode link in pageContent.DocumentNode.SelectNodes("//a[@href]"))
                {
                    Uri extractedLink;
                    HtmlAttribute attribute = link.Attributes["href"];
                    if (attribute.Value.StartsWith("http"))
                    {
                        try
                        {
                            extractedLink = new Uri(attribute.Value);
                            extractedLinks.Add(extractedLink);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Could not parse: " + attribute.Value);
                        }
                    } else if (attribute.Value.StartsWith("/"))
                    {
                        try
                        {
                            string baseUrl = "http://";
                            try
                            {
                                baseUrl += extractSeed.Host;
                                baseUrl += attribute.Value;
                                extractedLink = new Uri(baseUrl);
                                extractedLinks.Add(extractedLink);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Bad url: {0}", extractSeed);
                            }                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Could not parse: " + attribute.Value);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Selected nodes not parsed on content: " + extractSeed.ToString());
            }
            return extractedLinks;
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
