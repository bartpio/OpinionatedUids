using OpinionatedUids.Opinions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpinionatedUids.Tests
{
    public class WantSpecificFirstByteJudge : FirstByteOpinion, IUidJudge
    {
        public WantSpecificFirstByteJudge(byte firstByte = 0xFF) : base(firstByte)
        {
        }

        public bool IsUidAlright(Guid uid)
        {
            var firstbyte = uid.ToByteArray()[0];
            return firstbyte == FirstByte;
        }
    }
}
