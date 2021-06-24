using OpinionatedUids.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OpinionatedUids.Tests
{
    // a "raw" guid maker which alternates between two different ids, rather than returning actually random ids
    public sealed class TwoValueRawGuidMaker : IRawGuidMaker
    {
        private const byte _hexones = 0x11;
        private const byte _hextwos = 0x22;
        private readonly Guid _one;
        private readonly Guid _two;

        private long _cnt = -1;

        public TwoValueRawGuidMaker(bool startAtTwo = false, int len = 16)
        {
            _one = new Guid(_hexones.Elongate(len, 16 - len));
            _two = new Guid(_hextwos.Elongate(len, 16 - len));

            if (startAtTwo)
            {
                _cnt = 0;
            }
        }

        Guid IRawGuidMaker.NewGuid()
        {
            var cnt = Interlocked.Increment(ref _cnt);
            return ((cnt % 2L) == 0L) ? _one : _two;
        }
    }
}
