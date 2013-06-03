using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncWorkout
{
    public sealed class TaskWorkout : ITaskWorkout
    {
        readonly CancellationToken _token;
        volatile int _getsCompleted;
        volatile int _tasksCompleted;
        volatile int _tasksStarted;
        readonly Uri _targetUrl;
#if DEBUG
        static int IdCounter;
        readonly int _instanceId = Interlocked.Increment(ref IdCounter);
#endif

        public TaskWorkout(Uri targetUrl, CancellationToken token)
        {
            if (targetUrl == null)
                throw new ArgumentNullException("targetUrl");

            _token = token;
            _targetUrl = targetUrl;
        }

        public IEnumerable<string> Strings { get; private set; }

        #region ITaskWorkout Members

        public Task<IEnumerable<string>> LoadAsync()
        {
            return ReadManyStrings();
        }

        #endregion

        async Task<string> GetString(HttpClient httpClient)
        {
            using (var response = await httpClient.GetAsync(_targetUrl, HttpCompletionOption.ResponseContentRead, _token).ConfigureAwait(false))
            {
                //response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        async Task<string> ReadString(HttpClient httpClient)
        {
            // Simulate some work...
            await RandomUtils.Delay(10, _token).ConfigureAwait(false);
#pragma warning disable 0420
            Interlocked.Increment(ref _tasksStarted);
#pragma warning restore 0420

            string ret;

            for (; ; )
            {
                try
                {
                    //DoWork();

                    //ret = "hello";

                    //var wc = new WebClient();

                    //ret = await wc.DownloadStringTaskAsync(TargetUrl).ConfigureAwait(false);

                    //ret = await httpClient.GetStringAsync(_targetUrl).ConfigureAwait(false);

                    ret = await GetString(httpClient);

#pragma warning disable 0420
                    Interlocked.Increment(ref _getsCompleted);
#pragma warning restore 0420

                    break;
                }
                catch (OperationCanceledException)
                {
                    // This is what GetStringAsync throws on a timeout.
                }
                catch (Exception ex)
                {
                    //Debug.WriteLine("Get failed: " + ex.Message);
                }

                if (_token.IsCancellationRequested)
                    return null;

                // Retry delay.
                await RandomUtils.Delay(150, _token).ConfigureAwait(false);
            }

            // Simulate some work...
            await RandomUtils.Delay(10, _token).ConfigureAwait(false);

#pragma warning disable 0420
            Interlocked.Increment(ref _tasksCompleted);
#pragma warning restore 0420

            return ret;
        }

        async Task<IEnumerable<string>> ReadStrings()
        {
            using (var httpClient = new HttpClient())
            {
                using (var registration = _token.Register(() => httpClient.CancelPendingRequests()))
                {
                    var tasks = new Task<string>[100];

                    for (var i = 0; i < tasks.Count() && !_token.IsCancellationRequested; ++i)
                        tasks[i] = TaskEx.Run(() => ReadString(httpClient), _token);

                    var completed = await TaskEx.WhenAll(tasks).ConfigureAwait(false);

                    // Do a little CPU-bound work...
                    return completed.Distinct();
                }
            }
        }

        async Task<IEnumerable<string>> ReadManyStrings()
        {
            var allStrings = new Dictionary<string, bool>();
            var allStringsLock = new object();

            var work = Enumerable.Range(0, 5)
                                 .Select(i => TaskEx.Run(async () =>
                                                               {
                                                                   var strings = await ReadStrings().ConfigureAwait(false);

                                                                   if (_token.IsCancellationRequested)
                                                                       return;

                                                                   lock (allStringsLock)
                                                                   {
                                                                       foreach (var s in strings.Where(s => null != s))
                                                                           allStrings[s] = false;
                                                                   }
                                                               }, _token));

            await TaskEx.WhenAll(work)
                        .ConfigureAwait(false);

            return allStrings.Keys;
        }

        public override string ToString()
        {
#if DEBUG
            return string.Format("#{0} reads {1}/{2}/{3}", _instanceId, _getsCompleted, _tasksCompleted, _tasksStarted);
#else
            return string.Format("reads {0}/{1}/{2}", _getsCompleted, _tasksCompleted, _tasksStarted);
#endif
        }
    }
}
