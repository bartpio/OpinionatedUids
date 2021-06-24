using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpinionatedUids.Tests
{
    public static class UnitExtensions
    {
        public static byte[] Elongate(this byte byt, int times, int zeros = 0)
        {
            var thething = Enumerable.Repeat(byt, times);
            var thezeros = Enumerable.Repeat((byte)0x00, zeros);
            return thething.Concat(thezeros).ToArray();
        }
    }
}
