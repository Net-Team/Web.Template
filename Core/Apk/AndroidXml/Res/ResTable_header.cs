using System;

namespace AndroidXml.Res
{
    [Serializable]
    public class ResTable_header
    {
        public ResChunk_header Header { get; set; }
        public uint PackageCount { get; set; }
    }
}