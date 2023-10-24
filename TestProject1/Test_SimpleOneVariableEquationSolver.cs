using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;
using ProjectOneClasses;
using ProjectOneClasses.Utilities;

namespace TestProject1
{
    public class Test_SimpleOneVariableEquationSolver
    {
        private readonly ITestOutputHelper output;

        public Test_SimpleOneVariableEquationSolver(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory(Timeout = 2000)]
        [InlineData(0.1)]
        [InlineData(0.01)]
        [InlineData(0.001)]
        [InlineData(0.0001)]
        [InlineData(0.00001)]
        [InlineData(0.000001)]
        [InlineData(0.0000001)]
        [InlineData(0.00000001)]
        [InlineData(0.000000001)]
        [InlineData(0.0000000001)]
        [InlineData(0.00000000001)]
        [InlineData(double.Epsilon)]
        public void Test1(double epsilon) 
        {
            double root = 1;
            Func<double, double> func1 = x => -Math.Pow(x, 5) - Math.Pow(x, 3) - x + 3;
            var solver = new SimpleOneVariableEquationSolver(func1, epsilon);
            solver.Solve();
            double actual = solver.Root;
            output.WriteLine(actual.ToString());
            Assert.True(Math.Abs(actual - root) <= epsilon);
        }
    }
}