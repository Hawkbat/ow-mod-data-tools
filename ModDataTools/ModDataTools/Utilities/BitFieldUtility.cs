using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModDataTools.Utilities
{
    public static class BitFieldUtility
    {
        public static IEnumerable<bool> GetValues(int bitField, int length)
        {
            for (int i = 0; i < length; i++)
            {
                yield return ((bitField >> i) & 0x1) != 0;
            }
        }

        public static bool GetValue(int bitField, int index)
            => ((bitField >> index) & 0x1) != 0;

        public static int SetValue(int bitField, int index, bool value)
            => value ? bitField | (1 << index) : bitField & ~(1 << index);

        public static int Truncate(int bitField, int length)
            => bitField & ((1 << length) - 1);
    }
}
