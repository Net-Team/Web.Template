using System;

namespace AndroidXml.Res
{
    [Serializable]
    public class ResStringPool_header
    {
        public ResChunk_header Header { get; set; }
        public uint StringCount { get; set; }
        public uint StyleCount { get; set; }
        public StringPoolFlags Flags { get; set; }
        public uint StringStart { get; set; }
        public uint StylesStart { get; set; }
    }

    [Flags]
    public enum StringPoolFlags
    {
        SORTED_FLAG = 1 << 0,
        UTF8_FLAG = 1 << 8
    }
}