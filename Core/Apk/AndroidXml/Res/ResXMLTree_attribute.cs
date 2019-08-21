using System;

namespace AndroidXml.Res
{
    [Serializable]
    public class ResXMLTree_attribute
    {
        public ResStringPool_ref Namespace { get; set; }
        public ResStringPool_ref Name { get; set; }
        public ResStringPool_ref RawValue { get; set; }
        public Res_value TypedValue { get; set; }
    }
}