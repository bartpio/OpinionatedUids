using System;
using System.Security.Cryptography;

namespace OpinionatedUids.Internal
{
    /// <summary>
    /// raw guid maker
    /// uses <see cref="RandomNumberGenerator" />
    /// provides vanilla guids
    /// </summary>
    public sealed class RawGuidMaker : IRawGuidMaker, IDisposable
    {
        private readonly RandomNumberGenerator _rng;
        private readonly byte[] _buf = new byte[OpinionatedUidConstants.UidSize];
        private readonly object _locker = new object();

        /// <summary>
        /// construct
        /// </summary>
        public RawGuidMaker()
        {
            _rng = RandomNumberGenerator.Create();
        }

        /// <summary>
        /// provide a raw guid using cryptographic random number generator
        /// </summary>
        /// <returns>
        /// a raw guid
        /// </returns>
        public Guid NewGuid()
        {
            lock (_locker)
            {
                _rng.GetBytes(_buf);
                return new Guid(_buf);
            }
        }

        /// <summary>
        /// disposal
        /// </summary>
        public void Dispose()
        {
            _rng.Dispose();
        }
    }
}
