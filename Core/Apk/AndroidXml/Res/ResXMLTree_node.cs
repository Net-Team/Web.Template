using System;

namespace AndroidXml.Res
{
    [Serializable]
    public class ResXMLTree_node
    {
        public ResChunk_header Header { get; set; }
        public uint LineNumber { get; set; }
        public ResStringPool_ref Comment { get; set; }
    }
}