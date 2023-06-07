using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Utilities
{
    public static class PrintExtension
    {
        public static string Print<T>(this IEnumerable<T> values)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (T item in values)
            {
                sb.Append($"{item:F6}, ");
            }
            if(sb.Length > 1)
            {
                sb.Remove(sb.Length - 2, 2);
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
