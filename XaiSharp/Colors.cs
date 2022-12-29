using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XaiSharp
{
    public static class Colors
    {
        public static uint Random()
        {
            var randomColor = (uint)Math.Floor(new Random().NextDouble() * (0xffffff + 1));

            return randomColor;
        }

        public static uint Success => 0x57F287;
        public static uint Error => 0xED4245;
        public static uint Negative => 0x660000;
        public static uint Positive => 0x006600;
    }
}
