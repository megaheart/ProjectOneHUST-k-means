using ProjectOneClasses.ValidityCriterias.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.Utilities
{
    public class SemiSupervisedGenerator
    {
        public IReadOnlyDictionary<int, int> semiSupervised { get; set; }
        private int C;
        public SemiSupervisedGenerator(IReadOnlyList<int> expect, int semiSupervisedNumber)
        {
            List<Tuple<int, int>> tmp = new List<Tuple<int, int>>();
            Random rnd = new Random();
            var semiSupervisedDic = new Dictionary<int, int>();
            int n = expect.Count;
            C = -1;
            for(int i = 0; i < n; i++)
            {
                if (C < expect[i]) C = expect[i];
                tmp.Add(new Tuple<int, int>(i, expect[i]));
            }
            var choose = tmp.OrderBy(x => rnd.Next()).Take(semiSupervisedNumber);
            C++;
            foreach(var tup in choose)
            {
                semiSupervisedDic.Add(tup.Item1, tup.Item2);
            }
            semiSupervised = semiSupervisedDic;
        }
        public SemiSupervisedGenerator(string path)
        {
            var semiSupervisedDic = new Dictionary<int, int>();
            const char delimiter = ' ';
            string[] lines = File.ReadAllLines(path);
            C = lines.Length;
            for (int l = 0; l < lines.Length; l++)
            {
                string[] ds = lines[l].Split(delimiter);
                for (int i = 0; i < ds.Length; i++)
                {
                    int x = int.Parse(ds[i]) - 1;
                    semiSupervisedDic.Add(x, l);
                }
            }
            semiSupervised = semiSupervisedDic;

        }
        public void WriteFile(string path)
        {
            StringBuilder[] lines = new StringBuilder[C];
            for (int ix = 0; ix < C; ix++)
                lines[ix] = new StringBuilder();
            foreach (var p in semiSupervised)
            {
                lines[p.Value].Append(p.Key + 1);
                lines[p.Value].Append(' ');
            }
            int i = 0;
            int j = -1;
            for (; i < C; i++)
            {
                if (lines[i].Length != 0)
                {
                    lines[i].Remove(lines[i].Length - 1, 1);
                    j = i;
                    i++;
                    break;
                }
            }
            for (; i < C; i++)
            {
                if (lines[i].Length != 0)
                {
                    lines[i].Remove(lines[i].Length - 1, 1);
                    lines[j].Append('\n');
                    lines[j].Append(lines[i].ToString());
                }
            }
            File.WriteAllText(path, lines[j].ToString());
        }
    }
}
