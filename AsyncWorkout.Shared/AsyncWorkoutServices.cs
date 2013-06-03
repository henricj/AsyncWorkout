using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncWorkout
{
    public class AsyncWorkoutServices : IServices, IServiceManager
    {
        readonly Func<CancellationToken, Task<IEnumerable<string>>> _workoutFactory;
        CancellationTokenSource _cancellation;
        Task<IEnumerable<string>> _loaderTask;

        public AsyncWorkoutServices(Func<CancellationToken, Task<IEnumerable<string>>> workoutFactory, ICryptographyService cryptographyService)
        {
            if (workoutFactory == null)
                throw new ArgumentNullException("workoutFactory");
            if (cryptographyService == null)
                throw new ArgumentNullException("cryptographyService");

            _workoutFactory = workoutFactory;
            CryptographyService = cryptographyService;
        }

        #region IServiceManager Members

        public async void Launching()
        {
            try
            {
                _cancellation = new CancellationTokenSource();

                var loaderTask = _workoutFactory(_cancellation.Token);

                _loaderTask = loaderTask;

                Strings = await loaderTask.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Load failed: " + ex.Message);
            }
        }

        public void Activated()
        {
            if (null != Strings && null != _loaderTask)
                return;

            Closing();
            Launching();
        }

        public void Closing()
        {
            if (null != _cancellation)
            {
                try
                {
                    _cancellation.Cancel(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Closing: " + ex.Message);
                }
            }
        }

        public void Deactivated()
        {
            if (_loaderTask.Status != TaskStatus.RanToCompletion)
                _loaderTask = null;

            Closing();
        }

        public Task Initializer
        {
            get { return _loaderTask; }
        }

        #endregion

        #region IServices Members

        public IEnumerable<string> Strings { get; private set; }
        public ICryptographyService CryptographyService { get; private set; }

        #endregion
    }
}
