using System.Collections.Generic;

namespace AsyncWorkout
{
    public interface IServices
    {
        IEnumerable<string> Strings { get; }
        ICryptographyService CryptographyService { get; }
    }
}
