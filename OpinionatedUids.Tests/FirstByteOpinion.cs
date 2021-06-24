using System;
using System.Collections.Generic;
using System.Text;

namespace OpinionatedUids.Tests
{
    public abstract class FirstByteOpinion
    {
        public FirstByteOpinion(byte firstByte)
        {
            FirstByte = firstByte;
        }

        public byte FirstByte { get; }
    }
}
