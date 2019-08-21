using System;

namespace AndroidXml.Res
{
    [Serializable]
    public class ResXMLTree_attrExt
    {
        public ResStringPool_ref Namespace { get; set; }
        public ResStringPool_ref Name { get; set; }
        public ushort AttributeStart { get; set; }
        public ushort AttributeSize { get; set; }
        public ushort AttributeCount { get; set; }
        public ushort IdIndex { get; set; }
        public ushort ClassIndex { get; set; }
        public ushort StyleIndex { get; set; }
    }
}