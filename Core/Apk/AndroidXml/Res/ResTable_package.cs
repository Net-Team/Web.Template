using System;

namespace AndroidXml.Res
{
    [Serializable]
    public class ResTable_package
    {
        public ResChunk_header Header { get; set; }
        public uint Id { get; set; }
        public string Name { get; set; } // 128 x char16_t
        public uint TypeStrings { get; set; }
        public uint LastPublicType { get; set; }
        public uint KeyStrings { get; set; }
        public uint LastPublicKey { get; set; }
    }
}