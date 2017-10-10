using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace WI120917
{
    class Webpage
    {
        private int _id;
        public Uri uri;
        private Dictionary<string, int> _tokenList = new Dictionary<string, int>();
        public string htmlContent;
        public List<Uri> htmlLinks = new List<Uri>();
        public double pageRank;

        public int Id { get => _id; }

        public Webpage(int id, Uri Uri)
        {
            this._id = id;
            this.uri = Uri;
        }

        public bool Equals(int id)
        {
            if(this._id == id)
            {
                return true;
            }
            return false;
        }

        public int GetTokenFrequency(string token)
        {
            if (_tokenList.ContainsKey(token))
            {
                return _tokenList[token];
            }
            else
            {
                // The case where the page does not contain the token
                return 0;
            }
        }

        public bool HasToken(string token)
        {
            if (_tokenList.ContainsKey(token))
            {
                return true;
            }
            else
            {
                // The case where the page does not contain the token
                return false;
            }
        }


        public void AddToTokenList(string token)
        {
            if (this._tokenList.ContainsKey(token))
            {
                this._tokenList[token]++;
            } else
            {
                this._tokenList.Add(token, 1);
            }
        }

        public List<int> InitLinks(List<Webpage> pages)
        {
            HtmlWeb p = new HtmlWeb();
            HtmlDocument pageContent = p.Load(uri);

            try
            {
                foreach (HtmlNode link in pageContent.DocumentNode.SelectNodes("//a[@href]"))
                {
                    Uri extractedLink;
                    HtmlAttribute attribute = link.Attributes["href"];
                    if (attribute.Value.StartsWith("http"))
                    {
                        try
                        {
                            extractedLink = new Uri(attribute.Value);
                            htmlLinks.Add(extractedLink);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Could not parse: " + attribute.Value);
                        }
                    }
                    else if (attribute.Value.StartsWith("/"))
                    {
                        try
                        {
                            string baseUrl = "http://";
                            try
                            {
                                baseUrl += uri.Host;
                                baseUrl += attribute.Value;
                                extractedLink = new Uri(baseUrl);
                                htmlLinks.Add(extractedLink);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Bad url: {0}", uri);
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
                Console.WriteLine("Selected nodes not parsed on content: " + pageContent);
            }

            List<int> result = new List<int>();
            foreach (var page in pages)
            {
                if(htmlLinks.Exists(x => x == page.uri))
                {
                    result.Add(page.Id);
                }
            }
            return result;
        }
    }
}
