using System;
using System.Collections.Generic;
using System.Text;

namespace WI2
{
    public class Feature
    {
        private string _word;
        private bool _negation;

        public Feature(string word, bool negate)
        {
            this._word = word;
            this._negation = negate;
        }

        public string GetWord()
        {
            return this._word;
        }

        public bool IsNegated()
        {
            return this._negation;
        }
    }
}
