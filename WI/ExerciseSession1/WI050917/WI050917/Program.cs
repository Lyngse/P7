using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WI050917
{
    class Program
    {
        static void Main(string[] args)
        {
            string A = "do not worry about your difficulties in mathematics";
            string B = "i would not worry about your difficulties, you can easily learn what is needed.";

            ShingleSet AShingleSet = new ShingleSet(A, 3);
            ShingleSet BShingleSet = new ShingleSet(B, 3);

            Console.WriteLine(jaccard(AShingleSet, BShingleSet));

            bool jaccard(ShingleSet first, ShingleSet other)
            {
                List<Shingle> union = new List<Shingle>();

                union.AddRange(first.shingles);
                union.AddRange(other.shingles);
                union = union.Distinct().ToList();
                int unionCount = union.Count();
                int overlapCount = 0;

                foreach (var shingle in first.shingles)
                {
                    if (other.shingles.Contains(shingle))
                    {
                        overlapCount++;
                    }
                }

                var Jsimilarity = (double)overlapCount / (double)unionCount;

                return Jsimilarity > 0.9;
            }

            Console.WriteLine(sketch(AShingleSet, BShingleSet));

            bool sketch(ShingleSet one, ShingleSet two)
            {
                
                int[] minsOne = one.hashMin();
                int[] minstwo = two.hashMin();
                var all = minsOne.Length;
                var matches = 0;
                for (int i = 0; i < minsOne.Length; i++)
                {
                    if (minsOne[i] == minstwo[i])
                        matches++;
                }

                return (double)matches / (double)all > 0.9;
            }

            Console.WriteLine("Hello World!");
            Console.Read();
        }
    }

    public class ShingleSet 
    {

        public List<Shingle> shingles = new List<Shingle>();

        public ShingleSet(string doc, int n)
        {
            var charsToRemove = new string[] { ",", "." };
            foreach (var c in charsToRemove)
            {
                doc = doc.Replace(c, string.Empty);
            }
            List<string> splits = doc.Split(' ').ToList();
            for (int i = 0; i < splits.Count - (n - 1); i++)
            {
                Shingle nShingle = new Shingle(splits.GetRange(i,n));
                
                shingles.Add(nShingle);
            }
        }

        public int[] hashMin()
        {
            int[] min = new int[] { int.MaxValue, int.MaxValue, int.MaxValue };
            var algrorithm = SHA512.Create();
            foreach (Shingle shingle in shingles)
            {
                Console.WriteLine(shingle.ToString());
                byte[] hashBytes = algrorithm.ComputeHash(Encoding.UTF8.GetBytes(shingle.ToString()));
                int localmin = BitConverter.ToInt32(hashBytes, 0);
                min[0] = min[0] < localmin ? min[0] : localmin;

                
                Shingle shingle2 = new Shingle(shingle.words.OrderBy(s => s).ToList());
                Console.WriteLine(shingle2.ToString());
                hashBytes = algrorithm.ComputeHash(Encoding.UTF8.GetBytes(shingle2.ToString()));
                localmin = BitConverter.ToInt32(hashBytes, 0);
                min[1] = min[1] < localmin ? min[1] : localmin;

                Shingle shingle3 = new Shingle(shingle.words.OrderByDescending(s => s).ToList());
                Console.WriteLine(shingle3.ToString());
                hashBytes = algrorithm.ComputeHash(Encoding.UTF8.GetBytes(shingle3.ToString()));
                localmin = BitConverter.ToInt32(hashBytes, 0);
                min[2] = min[2] < localmin ? min[2] : localmin;
            }
            
            return min;
        }

    }

    public class Shingle
    {
        public List<string> words = new List<string>();

        public Shingle(List<string> inputWords)
        {
            words = inputWords;
        }

        public override string ToString()
        {
            string result = "";
            foreach (var word in words)
            {
                result += word;
            }
            return result;
        }
    }
}