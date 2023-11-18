using PythonInteractive.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PythonInteractive
{
    public abstract class ModelCalling : IDisposable, IAsyncDisposable
    {
        protected string programPath;
        protected PythonIO pythonIO;
        public ModelCalling(string programPath)
        {
            this.programPath = programPath;
        }
        public virtual async Task Train([NotNull] IReadOnlyList<double[]> X, int C,
            [NotNull] IReadOnlyDictionary<int, int> Y)
        {
            var y = new int[X.Count];
            foreach (var item in Y)
            {
                y[item.Key] = item.Value;
            }

            await Train(X, C, y);
        }

        public virtual async Task Train([NotNull] IReadOnlyList<double[]> X, int C,
            [NotNull] IReadOnlyList<int> Y)
        {
            if (pythonIO != null)
            {
                throw new Exception("Model is trained.");
            }

            if (Y == null)
            {
                throw new ArgumentNullException(nameof(Y));
            }
            if (X == null)
            {
                throw new ArgumentNullException(nameof(X));
            }

            if (C < 1) throw new Exception("C must be more than 0");
            if (C > X.Count) throw new Exception("C cannot be more than number of X");

            var inputLines = new List<string>();
            inputLines.Add($"{X.Count} {C}");
            foreach (var x in X)
            {
                inputLines.Add($"{string.Join(" ", x)}");
            }

            inputLines.Add($"{string.Join(" ", Y)}");

            var input = string.Join(Environment.NewLine, inputLines);

            //input.AppendLine($"{string.Join(" ", Y)}");

            //Console.WriteLine(input.ToString());


            pythonIO = new PythonIO(programPath);
            pythonIO.Start();

            var output = await pythonIO.AddInput(input);

            if (!output.IsSuccess)
            {
                throw new Exception("PythonIO throws Python program errors: " + output.Error);
            }
        }

        public virtual async Task<IReadOnlyList<int>> Predict([NotNull] IReadOnlyList<double[]> examples)
        {
            if (examples == null)
            {
                throw new ArgumentNullException(nameof(examples));
            }

            if (pythonIO == null)
            {
                throw new Exception("Model is not trained.");
            }
            var inputLines = new List<string>();
            inputLines.Add($"{examples.Count}");
            foreach (var x in examples)
            {
                inputLines.Add($"{string.Join(" ", x)}");
            }

            var input = string.Join(Environment.NewLine, inputLines);

            //Console.WriteLine(input.ToString());

            //return new int[examples.Count];

            var output = await pythonIO.AddInput(input);

            if (!output.IsSuccess)
            {
                throw new Exception("PythonIO throws Python program errors: " + output.Error);
            }

            var result = JsonSerializer.Deserialize<int[]>(output.Output);

            return result ?? throw new Exception("Output invalid format");
        }

        public ValueTask DisposeAsync()
        {
            pythonIO.Close();
            return pythonIO.DisposeAsync();
        }

        public void Dispose()
        {
            pythonIO.Close();
            pythonIO.Dispose();
        }
    }
}
