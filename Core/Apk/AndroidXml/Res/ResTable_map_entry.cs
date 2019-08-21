using System;

namespace AndroidXml.Res
{
    [Serializable]
    public class ResTable_map_entry : ResTable_entry
    {
        public ResTable_ref Parent { get; set; }
        public uint Count { get; set; }
    }
}