using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.Utilities
{
    public class ACIDbDeserializer
    {
        public List<string> IndexToName { get; private set; }
        public Dictionary<string, int> NameToIndex { get; private set; }
        public IReadOnlyList<double[]> X { get; private set; }
        public int C { get; private set; }
        public IReadOnlyList<int> expect { get; private set; }
        public bool IsNormalized { get; private set; }
        public ACIDbDeserializer(string path, char? delimiter = null)
        {
            string[] lines = File.ReadAllLines(path);
            
            IndexToName = new List<string>();
            NameToIndex = new Dictionary<string, int>();
            var expect = new List<int>(lines.Length);
            this.expect = expect;
            var X = new List<double[]>(lines.Length);
            this.X = X;
            char _delimiter = delimiter ?? ',';

            //Automatically find delimiter

            if (!delimiter.HasValue)
            {
                if (lines[0].IndexOf(',') == -1)
                {
                    _delimiter = ' ';
                    if (lines[0].IndexOf(' ') == -1)
                    {
                        throw new Exception("Invalid Input");
                    }
                }
                
            }

            //deserialize csv text -> object
            for (int l = 0; l < lines.Length; l++)
            {
                string[] ds = lines[l].Split(_delimiter);
                if (ds.Length < 2) continue;
                double[] x = new double[ds.Length - 1];
                for(int i = 0; i < x.Length; i++)
                {
                    x[i] = double.Parse(ds[i]);
                }
                X.Add(x);
                int k;
                if(NameToIndex.TryGetValue(ds[ds.Length - 1], out k))
                {
                    expect.Add(k);
                }
                else
                {
                    k = NameToIndex.Count;
                    expect.Add(k);
                    NameToIndex.Add(ds[ds.Length - 1], k);
                    IndexToName.Add(ds[ds.Length - 1]);
                }
            }
            C = NameToIndex.Count;

            //Normalize
            ScaleMinMax();
        }

        public void ScaleMinMax(/*double min = 0, double max = 1*/)
        {
            int dimension = X[0].Length;
            double[] maxVal = new double[dimension];
            double[] minVal = new double[dimension];
            double[] maxMin = new double[dimension];

            Array.Copy(X[0], maxVal, dimension);
            Array.Copy(X[0], minVal, dimension);

            for (int i = 1; i < X.Count; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    maxVal[j] = Math.Max(maxVal[j], X[i][j]);
                    minVal[j] = Math.Min(minVal[j], X[i][j]);
                }
            }

            for (int j = 0; j < dimension; j++)
            {
                maxMin[j] = maxVal[j] - minVal[j];
                if (maxMin[j] == 0) maxMin[j] = 1;
            }

            for (int i = 0; i < X.Count; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    X[i][j] = (X[i][j] - minVal[j]) / maxMin[j];
                }
            }
        }
    }
}
