using System;
using AndroidXml.Utils;

namespace AndroidXml.Res
{
    [Serializable]
    public class ResTable_typeSpec
    {
        public ResChunk_header Header { get; set; }
        public uint RawID { get; set; }
        public uint EntryCount { get; set; }

        public byte ID
        {
            get { return (byte) Helper.GetBits(RawID, 0xFFu, 24); }
            set { RawID = Helper.SetBits(RawID, value, 0xFFu, 24); }
        }

        public byte Res0
        {
            get { return (byte) Helper.GetBits(RawID, 0xFFu, 16); }
            set { RawID = Helper.SetBits(RawID, value, 0xFFu, 16); }
        }

        public ushort Res1
        {
            get { return (ushort) Helper.GetBits(RawID, 0xFFFFu, 0); }
            set { RawID = Helper.SetBits(RawID, value, 0xFFFFu, 0); }
        }
    }

    // enum SPEC_PUBLIC = 0x40000000 ???
}