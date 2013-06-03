using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncWorkout
{
    public static class RandomUtils
    {
        static readonly Random RandomGenerator;
        static readonly object RandomLock = new object();

        static RandomUtils()
        {
            var seed = new byte[4];

            GlobalServices.Services.CryptographyService.GetBytes(seed);

            RandomGenerator = new Random(BitConverter.ToInt32(seed, 0));
        }

        public static double RandomNumber
        {
            get
            {
                lock (RandomLock)
                {
                    return RandomGenerator.NextDouble();
                }
            }
        }

        public static Task Delay(int milliseconds, CancellationToken token)
        {
            return Delay(TimeSpan.FromMilliseconds(milliseconds), token);
        }

        public static Task Delay(TimeSpan nominalDelay, CancellationToken token)
        {
            var randomDelay = TimeSpan.FromSeconds(nominalDelay.TotalSeconds * RandomNumber);

            var delay = nominalDelay + randomDelay;

            return TaskEx.Delay(delay, token);
        }
    }
}
