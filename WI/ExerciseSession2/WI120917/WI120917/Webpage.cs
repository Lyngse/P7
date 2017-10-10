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
