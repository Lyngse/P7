using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace WI120917
{
    class PageRanker
    {
        SparseMatrix TransitionProbabilityMatrix;
        DenseMatrix TransitionRandomMatrix;
        double randomProbability = 0.1;
        int nrOfPages;

        public PageRanker(List<Webpage> pages)
        {
            nrOfPages = pages.Count;

            TransitionProbabilityMatrix = new SparseMatrix(nrOfPages); //Matrix<double>.Build.Sparse(nrOfPages, nrOfPages);

            double value = 1.0 / nrOfPages;

            TransitionRandomMatrix = DenseMatrix.Create(nrOfPages, nrOfPages, value);

            pages.Sort((x, y) => x.Id.CompareTo(y.Id));

            foreach (var page in pages)
            {
                List<int> linkIds = page.InitLinks(pages);
                int linkIdCount = linkIds.Count;

                foreach (var linkId in linkIds)
                {
                    TransitionProbabilityMatrix[page.Id, linkId] = 1.0 / linkIdCount;
                }
                //Console.WriteLine(TransitionProbabilityMatrix);
            }
        }

        public DenseVector GeneratePageRank (int n)
        {
            DenseVector resultVector = DenseVector.Create(nrOfPages, 0.0);
            resultVector[0] = 1.0;

            for (int i = 0; i < n; i++)
            {
                var pageRank = ((1 - randomProbability) * TransitionProbabilityMatrix) + (randomProbability * TransitionRandomMatrix);
                resultVector = (DenseVector)(resultVector * pageRank);
            }
            return resultVector;
        }
    }
}
