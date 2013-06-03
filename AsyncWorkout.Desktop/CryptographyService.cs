using System.Security.Cryptography;

namespace AsyncWorkout
{
    public class CryptographyService : ICryptographyService
    {
        #region ICryptographyService Members

        public void GetBytes(byte[] buffer)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buffer);
            }
        }

        #endregion
    }
}
