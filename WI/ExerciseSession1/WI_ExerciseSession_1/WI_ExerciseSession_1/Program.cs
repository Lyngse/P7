using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WI_ExerciseSession_1
{
    class Program
    {
        static void Main(string[] args)
        {
            string sample1 = "do not worry about your difficulties in mathematics";
            string sample2 = "I would not worry about your difficulties you can easily learn what is needed";

            List<string> sample1Shingles = new List<string>();
            List<string> sample2Shingles = new List<string>();

            double Jaccard = Program.nearDuplicate(sample1, sample2, sample1Shingles, sample2Shingles);
            Console.WriteLine(Jaccard);

            List<int> sample1ShingleHashes = new List<int>();
            List<int> sample2ShingleHashes = new List<int>();

            GetShingleHashes(sample1Shingles, sample1ShingleHashes);
            GetShingleHashes(sample2Shingles, sample2ShingleHashes);

            Console.WriteLine(sample1ShingleHashes.Min() +" "+ sample2ShingleHashes.Min());

            Console.Read();

        }

        private static void GetShingleHashes(List<string> sample1Shingles, List<int> sample1ShingleHashes)
        {
            foreach (var shingle in sample1Shingles)
            {
                sample1ShingleHashes.Add(shingle.GetHashCode());
            }
        }

        static double nearDuplicate(string sample1, string sample2, List<string> sample1Shingles, List<string> sample2Shingles)
        {
            

            List<string> sample1Bits = new List<string>(sample1.Split(' '));
            List<string> sample2Bits = new List<string>(sample2.Split(' '));

            GetValue(sample1Bits, sample1Shingles);
            GetValue(sample2Bits, sample2Shingles);

            IEnumerable<string> unionSet = sample1Shingles.Union(sample2Shingles);
            IEnumerable<string> intersectionSet = sample1Shingles.Intersect(sample2Shingles);

            double intersectionResult = intersectionSet.Count();
            double unionResult = unionSet.Count();

            Console.WriteLine(intersectionResult +" "+ unionResult);
            return ((double)intersectionResult / unionResult);

        }

        private static void GetValue(List<string> sample1Bits, List<string> sample1Shingles)
        {
            StringBuilder builder = new StringBuilder();

            int currentWordStart = 0;
            int currentShingleEnd = 3;
            int cycles = 0;

            while (currentWordStart <= currentShingleEnd)
            {
                for (int i = currentWordStart; i < currentShingleEnd; i++)
                {
                    builder.Append(sample1Bits[i] + " ");
                    cycles++;
                }

                if (cycles == 3)
                {
                    sample1Shingles.Add(builder.ToString());
                    Console.WriteLine(builder.ToString());
                    builder.Clear();
                }

                cycles = 0;
                currentWordStart++;
                if (currentShingleEnd < sample1Bits.Count)
                {
                    currentShingleEnd++;
                }
            }
        }
    }
}
