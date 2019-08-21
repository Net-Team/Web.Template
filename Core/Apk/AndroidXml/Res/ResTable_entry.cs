using System;

namespace AndroidXml.Res
{
    [Serializable]
    public class ResTable_entry
    {
        public ushort Size { get; set; }
        public EntryFlags Flags { get; set; }
        public ResStringPool_ref Key { get; set; }
    }

    [Flags]
    public enum EntryFlags
    {
        FLAG_COMPLEX = 0x0001,
        FLAG_PUBLIC = 0x0002,
    }
}