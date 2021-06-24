using OpinionatedUids.Opinions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpinionatedUids.Tests
{
    public sealed class SpecifyFirstByteTransform : FirstByteOpinion, IUidTransform
    {
        public SpecifyFirstByteTransform(byte firstByte = 0xFF) : base(firstByte)
        {
        }

        public Guid TransformUid(Guid uid)
        {
            var buf = uid.ToByteArray();
            buf[0] = FirstByte;
            return new Guid(buf);
        }
    }
}
