using System;
using System.Collections.Generic;

namespace AndroidXml.Res
{
    public class ResStringPool
    {
        public ResStringPool_header Header { get; set; }
        //public List<uint> StringIndices { get; set; }
        //public List<uint> StyleIndices { get; set; }
        public List<string> StringData { get; set; }
        public List<ResStringPool_span> StyleData { get; set; }

        public string GetString(ResStringPool_ref reference)
        {
            return GetString(reference.Index);
        }

        public string GetString(uint? index)
        {
            if (index == null) return "";
            if (index >= StringData.Count)
            {
                throw new ArgumentOutOfRangeException("index", index, string.Format("index >= {0}", StringData.Count));
            }
            return StringData[(int) index];
        }

        public uint? IndexOfString(string target)
        {
            if (string.IsNullOrEmpty(target)) return null;
            uint index = 0;
            foreach (string s in StringData)
            {
                if (s == target) return index;
                index++;
            }
            return null;
        }

        public IEnumerable<ResStringPool_span> GetStyles(uint stringIndex)
        {
            if (stringIndex >= StringData.Count)
            {
                throw new ArgumentOutOfRangeException(
                    "stringIndex", stringIndex, string.Format("index >= {0}", StringData.Count));
            }
            int currentIndex = 0;
            foreach (ResStringPool_span style in StyleData)
            {
                if (style.IsEnd)
                {
                    currentIndex++;
                    if (currentIndex > stringIndex)
                    {
                        break;
                    }
                }
                else if (currentIndex == stringIndex)
                {
                    yield return style;
                }
            }
        }
    }
}