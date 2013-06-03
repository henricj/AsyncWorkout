using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncWorkout
{
    public interface ITaskWorkout
    {
        Task<IEnumerable<string>> LoadAsync();
    }
}
