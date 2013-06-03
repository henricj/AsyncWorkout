using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncWorkout
{
    public class RunUpdatePolling<T>
    {
        readonly Func<string> _getStatusString;
        readonly Task<T> _task;
        readonly CancellationToken _token;

        public RunUpdatePolling(Task<T> task, CancellationToken token, Func<string> getStatusString)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (getStatusString == null)
                throw new ArgumentNullException("getStatusString");

            _task = task;
            _token = token;
            _getStatusString = getStatusString;
        }

        public async Task<T> RunAsync()
        {
            try
            {
                var sw = Stopwatch.StartNew();

                do
                {
                    ShowStatus(_getStatusString(), sw);
#if WINDOWS_PHONE7
                } while (!_token.IsCancellationRequested && _task != await TaskEx.WhenAny(_task, TaskEx.Delay(TimeSpan.FromSeconds(1), _token)));
#else
                } while (!_token.IsCancellationRequested && _task != await Task.WhenAny(_task, Task.Delay(TimeSpan.FromSeconds(1), _token)));
#endif

                var ret = await _task.ConfigureAwait(false);

                sw.Stop();

                ShowStatus(_getStatusString(), sw, "done: ");

                return ret;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RunUpdatePolling<" + typeof(T).Name + ">.RunAsync(): " + ex.Message);
                throw;
            }
        }

        static void ShowStatus(string status, Stopwatch sw, string prefix = null)
        {
#if WINDOWS_PHONE
            Debug.WriteLine("{0}{1} elapsed {2} memory {3:F2}MB",
                prefix ?? string.Empty, status, sw.Elapsed, GC.GetTotalMemory(false) * (1.0 / (1024 * 1024)));
#else
                    Debug.WriteLine("{0}{1} threads {2} elapsed {3} memory {4:F2}MB",
                        prefix ?? string.Empty, status, Process.GetCurrentProcess().Threads.Count, sw.Elapsed, GC.GetTotalMemory(false) * (1.0 / (1024 * 1024)));
#endif
        }
    }
}
