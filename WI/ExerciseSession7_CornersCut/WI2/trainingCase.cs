using System.Collections.Generic;
using System.Linq;

namespace WI2
{
    public class TrainingCase
    {
        public int Score;
        public List<string> Review;

        public TrainingCase(int score, string review)
        {
            Score = score;
            Review = review.Split(" ").Distinct().ToList();
        }
    }
}
