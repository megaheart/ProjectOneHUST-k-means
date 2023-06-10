using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.ResultTypes
{
    public record class MC_FCM_Result(
        [NotNull] IReadOnlyList<double[]> V, 
        [NotNull] IReadOnlyList<IReadOnlyList<double>> U, 
        int l
        ) : IFCM_Result
    {
        public void printToConsole()
        {
            Console.WriteLine("Display V:");
            foreach (var row in V)
            {
                string s = "", ss;
                foreach (var col in row)
                {
                    ss = col.ToString();
                    int n = 22 - ss.Length;
                    ss = string.Concat(Enumerable.Repeat(" ", n)) + ss;
                    s += ", " + ss;
                }
                s = s.Substring(2);

                Console.WriteLine("({0})", s);
            }
            Console.WriteLine("Display U:");
            foreach (var row in U)
            {
                foreach (var col in row)
                {
                    Console.Write("{0,23}", col);
                }
                Console.WriteLine();
            }
        }
    }
}
