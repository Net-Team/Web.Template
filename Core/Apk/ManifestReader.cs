using System;
using System.Text;

namespace Core.Apk
{
    /// <summary>
    /// 提供AndroidManifest的xml读取
    /// </summary>
    static class ManifestReader
    {
        private readonly static int startDocTag = 0x00100100;
        private readonly static int endDocTag = 0x00100101;
        private readonly static int startTag = 0x00100102;
        private readonly static int endTag = 0x00100103;
        private readonly static int textTag = 0x00100104;

        /// <summary>
        /// 读取AndroidManifest的xml
        /// </summary>
        /// <param name="manifestByteArray"></param>
        /// <returns></returns>
        public static string ReadAsXmlString(byte[] manifestByteArray)
        {
            if (manifestByteArray.Length == 0)
            {
                throw new ArgumentOutOfRangeException("Failed to read manifest data.  Byte array was empty");
            }

            var xml = new StringBuilder();

            int numbStrings = ReadAsInt32(manifestByteArray, 4 * 4);
            int sitOff = 0x24;
            int stOff = sitOff + numbStrings * 4;
            int xmlTagOff = ReadAsInt32(manifestByteArray, 3 * 4);
            int flag = ReadAsInt32(manifestByteArray, 4 * 6);
            var isUtf8 = (flag & (1 << 8)) > 0;

            for (int ii = xmlTagOff; ii < manifestByteArray.Length - 4; ii += 4)
            {
                if (ReadAsInt32(manifestByteArray, ii) == startTag)
                {
                    xmlTagOff = ii; break;
                }
            }

            int off = xmlTagOff;
            int startDocTagCounter = 1;
            while (off < manifestByteArray.Length)
            {
                int tag0 = ReadAsInt32(manifestByteArray, off);
                int nameSi = ReadAsInt32(manifestByteArray, off + 5 * 4);

                if (tag0 == startTag)
                {
                    int numbAttrs = ReadAsInt32(manifestByteArray, off + 7 * 4);  // Number of Attributes to follow
                    //int tag8 = LEW(manifestFileData, off+8*4);  // Expected to be 00000000
                    off += 9 * 4;  // Skip over 6+3 words of startTag data
                    string name = CompXmlString(manifestByteArray, sitOff, stOff, nameSi, isUtf8);

                    string sb = "";
                    for (int ii = 0; ii < numbAttrs; ii++)
                    {
                        int attrNameSi = ReadAsInt32(manifestByteArray, off + 1 * 4);  // AttrName String Index
                        int attrValueSi = ReadAsInt32(manifestByteArray, off + 2 * 4); // AttrValue Str Ind, or FFFFFFFF

                        int attrResId = ReadAsInt32(manifestByteArray, off + 4 * 4);  // AttrValue ResourceId or dup AttrValue StrInd
                        off += 5 * 4;  // Skip over the 5 words of an attribute

                        string attrName = CompXmlString(manifestByteArray, sitOff, stOff, attrNameSi, isUtf8);
                        string attrValue = attrValueSi != -1
                          ? CompXmlString(manifestByteArray, sitOff, stOff, attrValueSi, isUtf8)
                          : /*"resourceID 0x" + */attrResId.ToString();
                        sb += " " + attrName + "=\"" + attrValue + "\"";
                        //tr.add(attrName, attrValue);
                    }
                    xml.Append("<" + name + sb + ">");

                }
                else if (tag0 == endTag)
                {
                    off += 6 * 4;
                    var name = CompXmlString(manifestByteArray, sitOff, stOff, nameSi, isUtf8);
                    xml.Append($"</{name}>{Environment.NewLine}");
                }
                else if (tag0 == startDocTag)
                {
                    startDocTagCounter++;
                    off += 4;
                }
                else if (tag0 == endDocTag)
                {
                    startDocTagCounter--;
                    if (startDocTagCounter == 0)
                        break;
                }
                else if (tag0 == textTag)
                {
                    uint sentinal = 0xffffffff;
                    while (off < manifestByteArray.Length)
                    {
                        uint curr = (uint)ReadAsInt32(manifestByteArray, off);
                        off += 4;
                        if (off > manifestByteArray.Length)
                        {
                            throw new Exception("Sentinal not found before end of file");
                        }
                        if (curr == sentinal && sentinal == 0xffffffff)
                        {
                            sentinal = 0x00000000;
                        }
                        else if (curr == sentinal)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    xml.Append("Unrecognized tag code '" + tag0.ToString("X") + "' at offset " + off);
                    break;
                }
            }

            return xml.ToString();
        }

        private static string CompXmlString(byte[] xml, int sitOff, int stOff, int strInd, bool isUtf8)
        {
            if (strInd < 0)
            {
                return null;
            }

            var strOff = stOff + ReadAsInt32(xml, sitOff + strInd * 4);
            return CompXmlStringAt(xml, strOff, isUtf8);
        }

        /// <summary>
        /// Return the string stored in StringTable format at offset strOff
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="strOff"></param>
        /// <param name="isUtf8"></param>
        /// <returns></returns>
        private static string CompXmlStringAt(byte[] arr, int strOff, bool isUtf8)
        {
            int strLen = arr[strOff];
            if ((strLen & 0x80) != 0)
            {
                strLen = ((strLen & 0x7f) << 8) + arr[strOff + 1];
            }

            if (!isUtf8)
            {
                strLen *= 2;
            }

            var chars = new byte[strLen];
            for (var i = 0; i < strLen; i++)
            {
                chars[i] = arr[strOff + 2 + i];
            }

            return Encoding.GetEncoding(isUtf8 ? "UTF-8" : "UTF-16").GetString(chars);
        }


        /// <summary>
        /// 读取int32
        /// Little Endian
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="off"></param>
        /// <returns></returns>
        private static int ReadAsInt32(byte[] arr, int off)
        {
            return (int)(((uint)arr[off + 3]) << 24 & 0xff000000 | ((uint)arr[off + 2]) << 16 & 0xff0000 | ((uint)arr[off + 1]) << 8 & 0xff00 | ((uint)arr[off]) & 0xFF);
        }
    }
}
