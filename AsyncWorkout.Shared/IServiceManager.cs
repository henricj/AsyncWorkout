using System.Threading.Tasks;

namespace AsyncWorkout
{
    public interface IServiceManager
    {
        Task Initializer { get; }
        void Launching();
        void Activated();
        void Closing();
        void Deactivated();
    }

    public static class ServiceManagerExtensions
    {
        public static bool IsReady(this IServiceManager serviceManager)
        {
            if (null == serviceManager)
                return false;

            var initializer = serviceManager.Initializer;
            
            if (null == initializer)
                return false;

            return TaskStatus.RanToCompletion == initializer.Status;
        }
    }
}
