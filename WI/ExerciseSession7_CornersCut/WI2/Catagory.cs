using System;
using System.Collections.Generic;
using System.Text;

namespace WI2
{
    public class Catagory
    {
        public int Score;
        public int NumOfTrainingCases = 0;
        public Dictionary<string, int> Occurences = new Dictionary<string, int>();

        public Catagory(int score, List<string> noWords)
        {
            Score = score;
            foreach (var word in noWords)
            {
                Occurences.Add(word, 0);
            }
        }
    }
}
