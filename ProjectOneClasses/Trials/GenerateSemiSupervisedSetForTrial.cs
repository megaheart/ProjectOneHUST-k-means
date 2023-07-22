using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.Trials
{
    public class GenerateSemiSupervisedSetForTrial
    {
        int step = 0;
        int semiSupervisedCount;
        int idx = 0;
        IReadOnlyList<int> expect;
        List<int>[] clusters;
        int N, C;
        Dictionary<int, int> Y = new Dictionary<int, int>();
        int[] randomIndexes;
        bool[] hasCluster;
        public GenerateSemiSupervisedSetForTrial(int C, int semiSupervisedCount, [NotNull] IReadOnlyList<int> expect, int step = 0)
        {
            this.step = step;
            this.semiSupervisedCount = semiSupervisedCount;
            this.expect = expect;
            this.N = expect.Count;
            this.C = C;

            //hasCluster = new bool[C];

            randomIndexes = new int[N];
            for (int i = 0; i < N; i++)
            {
                randomIndexes[i] = i;
            }

            //clusters = new List<int>[C];
            //for(int i = 0; i < C; i++)
            //{
            //    clusters[i] = new List<int>();
            //}

            //for(int i = 0; i < N; i++)
            //{
            //    clusters[expect[i]].Add(i);
            //}

            //clusters = clusters.OrderByDescending(x => x.Count).ToArray();
        }

        //private void ShuffleIndexes()
        //{
        //    for(int k = 0; k < C; k++)
        //    {
        //        int Nk = clusters[k].Count;
        //        var list = clusters[k];
        //        for (int i = 0; i < Nk; i++)
        //        {
        //            int j = Random.Shared.Next(0, Nk);
        //            int temp = list[i];
        //            list[i] = list[j];
        //            list[j] = temp;
        //        }
        //    }

        //}


        public Dictionary<int, int> Generate()
        {
            if (semiSupervisedCount == 0) return Y;
            if (semiSupervisedCount == N)
            {
                if (Y.Count == 0)
                {
                    for (int i = 0; i < N; i++)
                    {
                        Y.Add(i, expect[i]);
                    }
                }
                return Y;
            }

            Y.Clear();
            //Shuffle Indexes
            if (idx < semiSupervisedCount)
            {
                for (int i = 0; i < N; i++)
                {
                    int j = Random.Shared.Next(0, N);
                    int temp = randomIndexes[i];
                    randomIndexes[i] = randomIndexes[j];
                    randomIndexes[j] = temp;
                }
            }

            
            for (int j = 0; j < semiSupervisedCount; j++)
            {
                int index = randomIndexes[(j + idx) % N];
                int cluster = expect[index];
                Y.Add(index, cluster);
            }

            idx += semiSupervisedCount; 

            if (idx >= N)
            {
                idx = 0;
            }

            return Y;
        }


            //public Dictionary<int, int> Generate()
            //{
            //    if (semiSupervisedCount == 0) return Y;
            //    if (semiSupervisedCount == N)
            //    {
            //        if (Y.Count == 0)
            //        {
            //            for (int i = 0; i < N; i++)
            //            {
            //                Y.Add(i, expect[i]);
            //            }
            //        }
            //        return Y;
            //    }

            //    int clusterCount = 0;

            //    while (true)
            //    {
            //        if (idx >= N)
            //        {
            //            idx = 0;
            //        }
            //        Y.Clear();
            //        if (idx < semiSupervisedCount)
            //        {
            //            //Shuffle Indexes
            //            if (idx < semiSupervisedCount)
            //            {
            //                for (int i = 0; i < N; i++)
            //                {
            //                    int j = Random.Shared.Next(0, N);
            //                    int temp = randomIndexes[i];
            //                    randomIndexes[i] = randomIndexes[j];
            //                    randomIndexes[j] = temp;
            //                }
            //            }
            //            idx = 0;
            //        }

            //        for (int i = 0; i < C; i++)
            //        {
            //            hasCluster[i] = false;
            //        }

            //        bool hasAllClusters = false;
            //        for (int j = 0; j < semiSupervisedCount; j++)
            //        {
            //            int index = randomIndexes[(j + idx) % N];
            //            int cluster = expect[index];
            //            if (hasCluster[cluster] == false)
            //            {
            //                clusterCount++;
            //                hasCluster[cluster] = true;
            //            }

            //            if(clusterCount == C)
            //            {
            //                hasAllClusters = true;
            //                break;
            //            }
            //        }

            //        if(hasAllClusters)
            //        {
            //            for (int j = 0; j < semiSupervisedCount; j++)
            //            {
            //                int index = randomIndexes[(j + idx) % N];
            //                int cluster = expect[index];
            //                Y.Add(index, cluster);
            //            }

            //            idx += semiSupervisedCount;
            //            break;
            //        }

            //        idx += semiSupervisedCount;

            //    }


            //    return Y;
            //}
            //public Dictionary<int, int> Generate()
            //{
            //    if (semiSupervisedCount == 0) return Y;
            //    Y.Clear();
            //    if(step % 2 == 1 && step < 10)
            //    {
            //        for (int j = 0; j < semiSupervisedCount; j++)
            //        {
            //            int index = (j + idx) % N;
            //            Y.Add(index, expect[index]);
            //            j++;
            //        }
            //        idx += semiSupervisedCount;
            //        if (idx >= N)
            //        {
            //            idx = Random.Shared.Next(0, semiSupervisedCount);
            //            step++;
            //        }
            //        return Y;
            //    }
            //    else
            //    {
            //        if(idx < semiSupervisedCount)
            //        {
            //            for (int i = 0; i < N; i++)
            //            {
            //                int j = Random.Shared.Next(0, N);
            //                int temp = randomIndexes[i];
            //                randomIndexes[i] = randomIndexes[j];
            //                randomIndexes[j] = temp;
            //            }
            //        }

            //        for (int j = 0; j < semiSupervisedCount; j++)
            //        {
            //            int index = randomIndexes[(j + idx) % N];
            //            Y.Add(index, expect[index]);
            //            j++;
            //        }
            //        idx += semiSupervisedCount;
            //        if (idx >= N)
            //        {
            //            idx = Random.Shared.Next(0, semiSupervisedCount);
            //            step++;
            //        }
            //        return Y;
            //    }   
            //}
        }
}
