using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.Utilities
{
    public class ChooseBestAccuracyPointForSemiSupervision
    {
        public class Point
        {
            public int idx;
            public int clusterIdx;
            public bool meetExpectation;
            public double degreeOfDependency;
        }
        public Point[] Points { get; private set; }
        public int C { get; private set; }
        public ChooseBestAccuracyPointForSemiSupervision(IReadOnlyList<double[]> X, int C, IReadOnlyList<int> expect, IFCM_Result result)
        {
            Points = new Point[X.Count];
            this.C = C;
            List<int>[] clusters = new List<int>[C];
            for (int i = 0; i < C; i++) clusters[i] = new List<int>(X.Count / C + 1);
            var U = result.U;
            for (int i = 0; i < X.Count; i++)
            {
                var p = Points[i] = new Point() { idx = i, meetExpectation = true };
                int k_max = 0;
                for (int k = 1; k < C; k++)
                {
                    if (U[i][k] > U[i][k_max]) k_max = k;
                }
                clusters[k_max].Add(i);
                p.clusterIdx = k_max;
                p.degreeOfDependency = U[i][k_max];
            }

            int[] tmpA = new int[C];
            int notAsExpect = 0;
            //var notAsExpectPoints = new List<_NotAsExpectPoint>();
            foreach (var cluster in clusters)
            {
                for (int i = 0; i < C; i++) tmpA[i] = 0;
                foreach (var i in cluster)
                {
                    int ii = expect[i];
                    tmpA[ii]++;
                }
                int k_max = 0;
                for (int k = 1; k < C; k++)
                {
                    if (tmpA[k] > tmpA[k_max]) k_max = k;
                }

                foreach (var i in cluster)
                {
                    if (expect[i] != k_max)
                    {
                        notAsExpect++;
                        Points[i].meetExpectation = false;
                        //notAsExpectPoints.Add(new _NotAsExpectPoint(X[i], expect[i], k_max));
                    }
                }
            }
            //Result = new _Result(X.Count - notAsExpect, notAsExpect, notAsExpectPoints);
            
            Array.Sort(Points, (a, b) =>
            {
                if (a.meetExpectation == b.meetExpectation)
                {
                    return -a.degreeOfDependency.CompareTo(b.degreeOfDependency);
                }
                else if (a.meetExpectation) return -1;
                return 1;
            });
        }

        public void SaveAsSemiSupervisionFile(int[] percents, string name, string directoryForSaving)
        {
            Array.Sort(percents);
            StringBuilder[] listSb = new StringBuilder[C];
            for (int i = 0; i < C; i++) listSb[i] = new StringBuilder();

            int idx = 0;
            foreach (var percent in percents)
            {
                int j = percent * Points.Length / 100;
                for(; idx < j; idx++)
                {
                    var p = Points[idx];
                    listSb[p.clusterIdx].Append(p.idx + 1);
                    listSb[p.clusterIdx].Append(' ');
                }

                using(StreamWriter writer = new StreamWriter(File.Open(Path.Combine(directoryForSaving, name + $".best.p{percent}.ss"), FileMode.Create)))
                {
                    for (int i = 0; i < C; i++)
                    {
                        if(listSb[i].Length != 0)
                        {
                            string s = listSb[i].ToString();
                            writer.WriteLine(s.Remove(s.Length - 1));
                        }
                    }
                }
            }
        }

    }
}
