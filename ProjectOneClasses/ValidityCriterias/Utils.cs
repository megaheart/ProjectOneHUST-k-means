using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.ValidityCriterias
{
    public class Utils
    {
        public static IReadOnlyList<int> DictionaryToList(IReadOnlyDictionary<int, int> dict)
        {
            var list = new List<int>(dict.Count);

            foreach (var item in dict)
            {
                list[item.Key] = item.Value;
            }
            return list;
        }
        public static List<int>[] PredictionsToClusters(int C, IReadOnlyList<int> predicts)
        {
            var labels = new HashSet<int>(predicts);

            if (labels.Count > C) throw new Exception("Number of clusters cannot be greater number of labels");

            var labelMapping = labels.Select((label, index) => new { label, index }).ToDictionary(x => x.label, x => x.index);

            var predictsMapped = predicts.Select(x => labelMapping[x]).ToList();

            List<int>[] clusters = new List<int>[C];
            for (int i = 0; i < C; i++) clusters[i] = new List<int>(predicts.Count / C + 1);

            for (int i = 0; i < predicts.Count; i++)
            {
                clusters[predictsMapped[i]].Add(i);
            }

            return clusters;
        }

        public static List<int>[] PredictionsToClusters(int C, IReadOnlyDictionary<int, int> predicts)
        {
            var labels = new HashSet<int>(predicts.Values);

            if (labels.Count > C) throw new Exception("Number of clusters must be equal to number of labels");

            var labelMapping = labels.Select((label, index) => new { label, index }).ToDictionary(x => x.label, x => x.index);

            List<int>[] clusters = new List<int>[C];
            for (int i = 0; i < C; i++) clusters[i] = new List<int>(predicts.Count / C + 1);

            foreach (var item in predicts)
            {
                clusters[labelMapping[item.Value]].Add(item.Key);
            }

            return clusters;
        }
    }
}
