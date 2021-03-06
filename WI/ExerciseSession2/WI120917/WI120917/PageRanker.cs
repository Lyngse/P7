﻿using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace WI120917
{
    // Corners cut: We implementd the LinearAlgebra library, to be able to work with matrices.
    // PageRanker calculates the pagerank of a specific page based on the formula from the 5th lecture. 
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
                    TransitionProbabilityMatrix[page.Id-1, linkId-1] = 1.0 / linkIdCount;
                }
            }
        }

        //Generates the pagerank of the page, returned as a vector, which will be iterated on n times, parsed to the function.
        public DenseVector GeneratePageRank (int n)
        {
            DenseVector resultVector = DenseVector.Create(nrOfPages, 0.0);
            resultVector[0] = 1.0;
            var pageRank = ((1 - randomProbability) * TransitionProbabilityMatrix) + (randomProbability * TransitionRandomMatrix);
            //Console.WriteLine(TransitionProbabilityMatrix.Column(30));
            //Console.WriteLine(TransitionRandomMatrix.Column(30));
            //Console.WriteLine(pageRank.Column(30));

            for (int i = 0; i < n; i++)
            {
                resultVector = (DenseVector)(resultVector * pageRank);
            }
            return resultVector;
        }
    }
}
