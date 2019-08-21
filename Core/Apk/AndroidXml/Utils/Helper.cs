using System;

namespace AndroidXml.Utils
{
    internal static class Helper
    {
        public static uint SetBits(uint field, uint value, uint mask, int shift)
        {
            return (field & ~(mask << shift)) | ((value & mask) << shift);
        }

        public static uint GetBits(uint field, uint mask, int shift)
        {
            return (field >> shift) & mask;
        }

        public static uint DecodeLengthUtf8(byte[] data, ref uint position)
        {
            uint len = data[position];
            position++;
            if ((len & 0x80u) != 0u)
            {
                len = ((len & 0x7Fu) << 8) | data[position];
                position++;
            }
            return len;
        }

        public static uint DecodeLengthUtf16(byte[] data, ref uint position)
        {
            uint len = BitConverter.ToUInt16(data, (int) position);
            position += 2;
            if ((len & 0x8000u) != 0u)
            {
                len = ((len & 0x7FFFu) << 16) | BitConverter.ToUInt16(data, (int) position);
                position += 2;
            }
            return len;
        }
    }
}