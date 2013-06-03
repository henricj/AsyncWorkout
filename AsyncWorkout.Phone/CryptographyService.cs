using System.Security.Cryptography;

namespace AsyncWorkout
{
    public class CryptographyService : ICryptographyService
    {
        static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();

        #region ICryptographyService Members

        public void GetBytes(byte[] buffer)
        {
            Rng.GetBytes(buffer);
        }

        #endregion
    }
}
