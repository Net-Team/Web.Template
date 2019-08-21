using System;

namespace AndroidXml.Res
{
    [Serializable]
    public class ResStringPool_span
    {
        public ResStringPool_ref Name { get; set; }
        public uint FirstChar { get; set; }
        public uint LastChar { get; set; }

        public bool IsEnd
        {
            get { return Name.Index == null; }
            set
            {
                if (value)
                {
                    Name.Index = null;
                }
                else if (IsEnd)
                {
                    Name.Index = 0;
                }
            }
        }
    }
}