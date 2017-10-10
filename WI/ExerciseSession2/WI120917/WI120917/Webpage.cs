using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace WI120917
{
    [Serializable]
    class Webpage
    {
        public int _id;
        public Uri uri;
        public Dictionary<string, int> _tokenList = new Dictionary<string, int>();
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

            List<int> result = new List<int>();
            foreach (var page in pages)
            {
                foreach (var hl in htmlLinks)
                {
                    if(page.uri == hl)
                    {
                        result.Add(page.Id);
                    }
                }
                //if(htmlLinks.Exists(x => x == page.uri))
                //{
                    
                //}
            }
            return result;
        }
    }
}
