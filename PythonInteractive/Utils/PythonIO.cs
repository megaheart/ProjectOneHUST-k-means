using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonInteractive.Utils
{
    public record ShellOutputEventArgs(string Output, string Error, bool IsSuccess);
    public class PythonIO : IDisposable, IAsyncDisposable
    {
        private static ProcessStartInfo CreateProcessInfo(string programPath)
        {
            //#TODO: check if python.exe exists
            return new ProcessStartInfo()
            {
                FileName = "python.exe",
                Arguments = programPath,
                RedirectStandardInput = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardErrorEncoding = Encoding.ASCII,
                StandardInputEncoding = Encoding.ASCII,
                StandardOutputEncoding = Encoding.ASCII,
            };
        }
        //private ConcurrentQueue<string> outputQueue;
        //private ConcurrentQueue<bool> isErrorQueue;
        private ConcurrentQueue<SemaphoreSlim> inputSemaphoreQueue;
        private Process p;
        private StringBuilder _outputBuilder = new StringBuilder();
        private StringBuilder _errorBuilder = new StringBuilder();
        private ConcurrentQueue<string> _currentOutputBuilder = new ConcurrentQueue<string>();
        private ConcurrentQueue<string> _currentErrorBuilder = new ConcurrentQueue<string>();
        public string ProgramPath { private set; get; }
        public string Output { get => _outputBuilder.ToString(); }
        public string Error { get => _errorBuilder.ToString(); }
        public bool IsCompleted { get => p.HasExited; }

        public PythonIO(string programPath)
        {
            this.ProgramPath = programPath;
            //outputQueue = new ConcurrentQueue<string>();
            //isErrorQueue = new ConcurrentQueue<bool>();
            inputSemaphoreQueue = new ConcurrentQueue<SemaphoreSlim>();
            p = new Process();
            p.StartInfo = CreateProcessInfo(ProgramPath);
        }
        public async Task<ShellOutputEventArgs> AddInput(string input)
        {
            //Debug.WriteLine("hello 55");
            if (p == null)
            {
                throw new Exception("Process has shutdown");
            }
            Debug.WriteLine("AddInput Method");
            Debug.WriteLine(p);
            Debug.WriteLine(p.HasExited);
            if (p.HasExited)
            {
                throw new Exception("Process has exited");
            }

            if (p.StandardInput.BaseStream.CanWrite)
            {
                Debug.WriteLine("hello");
                var semaphore = new SemaphoreSlim(0, 1);
                inputSemaphoreQueue.Enqueue(semaphore);
                Debug.WriteLine("hello input");
                p.StandardInput.WriteLine(input);
                Debug.WriteLine("hello end input");
                await semaphore.WaitAsync();
                Debug.WriteLine("AddInput Method Semaphore Release");
                var output = "";
                var error = "";
                while(_currentOutputBuilder.TryDequeue(out var line))
                {
                    output += line + Environment.NewLine;
                }
                while(_currentErrorBuilder.TryDequeue(out var line))
                {
                    error += line + Environment.NewLine;
                }
                _outputBuilder.Append(output);
                _errorBuilder.Append(error);
                Debug.WriteLine("Output: " + output);
                Debug.WriteLine("Error: " + error);
                var isSuccess = string.IsNullOrWhiteSpace(error);
                if (!isSuccess)
                {
                    p.Kill();
                    p.Dispose();
                    p = null;
                }
                return new ShellOutputEventArgs(output, error, isSuccess);
            }
            else
            {
                throw new Exception("Cannot write to process");
            }
        }
        public async Task WaitForExitAsync()
        {
            await p.WaitForExitAsync();
        }
        public bool WaitForInputIdle()
        {
            return p.WaitForInputIdle();
        }
        //public StreamWriter Input
        //{
        //    get => p.StandardInput;
        //}
        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string data = e.Data;
            Debug.WriteLine("OutputDataReceived Method");
            Debug.WriteLine(p);
            Debug.WriteLine(p.HasExited);
            Debug.WriteLine(data);
            if (string.IsNullOrEmpty(data))
            {
                return;
            }
            if(data.Contains(">>>"))
            {
                if(inputSemaphoreQueue.TryDequeue(out var semaphore))
                {
                    semaphore.Release();
                    //Debug.WriteLine("Semaphore release");
                }
            }
            else
            {
                _currentOutputBuilder.Enqueue(data);
            }
        }
        private long lastErrorTime = 0;
        private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            string data = e.Data;
            Debug.WriteLine("ErrorDataReceived Method");
            Debug.WriteLine(p);
            Debug.WriteLine(p.HasExited);
            Debug.WriteLine(data);
            if (string.IsNullOrEmpty(data))
            {
                return;
            }
            _currentErrorBuilder.Enqueue(data);
            Interlocked.Exchange(ref lastErrorTime, DateTime.Now.Ticks);
            Task.Delay(300).ContinueWith((t) =>
            {
                if (DateTime.Now.Ticks - lastErrorTime > 280)
                {
                    if (inputSemaphoreQueue.TryDequeue(out var semaphore))
                    {
                        semaphore.Release();
                        //Debug.WriteLine("Semaphore release");
                    }
                }
            });
        }
        /// <summary>
        /// Create a powershell process and run all commands which have just been added. After powershell process completed, return output and error lines
        /// </summary>
        public void Start()
        {
            p.Start();
            p.OutputDataReceived += OutputDataReceived;
            p.ErrorDataReceived += ErrorDataReceived;
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            p.Exited += (sender, e) =>
            {
                Debug.WriteLine("Python IO Exited");
            };
            p.Disposed += (sender, e) =>
            {
                Debug.WriteLine("Python IO Disposed");
            };
            //await p.WaitForExitAsync();
            //_HandleOutput();
            //p.Dispose();
        }
        /// <summary>
        /// Clear all commands and <see cref="ShellOutputEventHandler"/> which were added before
        /// </summary>
        //public void Reset()
        //{
        //    isErrorQueue.Clear();
        //    outputQueue.Clear();
        //}
        /// <summary>
        /// Process output and error lines to <see cref="Output"/>, <see cref="Error"/> and <see cref="IsSuccess"/>
        /// </summary>
        //private void _HandleOutput()
        //{
        //    List<string> outputLines = new List<string>();
        //    List<string> errorLines = new List<string>();
        //    var curDir = "";
        //    while (outputQueue.TryDequeue(out var output))
        //    {
        //        isErrorQueue.TryDequeue(out var isError);
        //        if (isError)
        //        {
        //            errorLines.Add(output);
        //        }
        //        else
        //        {
        //            outputLines.Add(output);
        //        }

        //    }

        //    Output = string.Join(Environment.NewLine, outputLines);
        //    Error = string.Join(Environment.NewLine, errorLines);
        //    IsSuccess = string.IsNullOrEmpty(Error);

        //}
        public void Close()
        {
            p.Close();
        }
        public void Kill()
        {
            p.Kill();
        }

        public void Dispose()
        {
            try
            {
                p.WaitForExit();
            }
            catch (Exception)
            {
            }
            p.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await p.WaitForExitAsync();
            }
            catch (Exception)
            {
            }
            p.Dispose();
        }
    }
}
