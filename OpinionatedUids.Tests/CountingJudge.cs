using OpinionatedUids.Opinions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpinionatedUids.Tests
{
    public sealed class CountingJudge : IUidJudge
    {
        private int _invocations;
        public int Invocations => _invocations;

        private readonly HashSet<Guid> _seen = new HashSet<Guid>(10_000_000);

        public int UniquesSeen => _seen.Count;

        bool IUidJudge.IsUidAlright(Guid uid)
        {
            _invocations++;
            _seen.Add(uid);
            return true;
        }
    }
}
