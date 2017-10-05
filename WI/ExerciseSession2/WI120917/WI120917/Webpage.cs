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
    }
}
