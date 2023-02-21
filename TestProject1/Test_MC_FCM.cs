using ProjectOneClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace TestProject1
{
    public class Test_MC_FCM
    {
        private readonly ITestOutputHelper output;

        public Test_MC_FCM(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory(Timeout = 2000)]
        [InlineData(new double[]{1,2,3,4,5,6,7,8,9})]
        [InlineData(new double[] { 9,4,6,5,7,3,1,8,2 })]
        [InlineData(new double[] { 65, 16, 42, 100, 14, 13, 33, 91, 43, 53, 41, 28, 50, 58, 95,
            46, 56, 18, 64, 39, 22, 75, 29, 47, 90, 5, 94, 88, 99, 73, 20, 23, 86, 84, 96, 49, 
            25, 83, 59, 57, 72, 17, 24, 45, 1, 71, 69, 85, 4, 6, 48, 11, 38, 51, 37, 9, 26, 98,
            36, 2, 78, 66, 55, 87, 44, 89, 27, 31, 21, 63, 61, 74, 3, 7, 35, 34, 82, 76, 54, 8,
            81, 80, 30, 70, 77, 67, 52, 62, 15, 32, 68, 97, 60, 93, 92, 19, 79, 10, 12, 40 })]
        [InlineData(new double[] { 1, 2, 3,3, 4, 5,5,5, 6, 7,7, 8, 9 })]
        public void FindMedian(double[] list)
        {
            double median = MC_FCM.GetMedian(list);
            Array.Sort(list);
            if(list.Length % 2 == 1)
            {
                Assert.Equal(list[list.Length / 2], median);
            }
            else
            {
                Assert.True(median == list[list.Length / 2] || median == list[list.Length / 2 - 1]);
            }
        }
        [Theory(Timeout = 2000)]
        [InlineData(new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })]
        [InlineData(new double[] { 9, 4, 6, 5, 7, 3, 1, 8, 2 })]
        [InlineData(new double[] { 65, 16, 42, 100, 14, 13, 33, 91, 43, 53, 41, 28, 50, 58, 95,
            46, 56, 18, 64, 39, 22, 75, 29, 47, 90, 5, 94, 88, 99, 73, 20, 23, 86, 84, 96, 49,
            25, 83, 59, 57, 72, 17, 24, 45, 1, 71, 69, 85, 4, 6, 48, 11, 38, 51, 37, 9, 26, 98,
            36, 2, 78, 66, 55, 87, 44, 89, 27, 31, 21, 63, 61, 74, 3, 7, 35, 34, 82, 76, 54, 8,
            81, 80, 30, 70, 77, 67, 52, 62, 15, 32, 68, 97, 60, 93, 92, 19, 79, 10, 12, 40 })]
        [InlineData(new double[] { 1, 2, 3, 3, 4, 5, 5, 5, 6, 7, 7, 8, 9 })]
        public void Immutability_Of_FindMedian(double[] list)
        {
            double[] _l = new double[list.Length];
            Array.Copy(list, _l, list.Length);
            double median = MC_FCM.GetMedian(list);
            if (list.Length % 2 == 1)
            {
                Assert.Equal(_l, list);
            }
            else
            {
                Assert.Equal(_l, list);
            }
        }
    }
}
