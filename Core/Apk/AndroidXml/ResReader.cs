using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using AndroidXml.Res;
using AndroidXml.Utils;

namespace AndroidXml
{
    public class ResReader : BinaryReader
    {
        public ResReader(Stream input)
            : base(input) {}

        public virtual Res_value ReadRes_value()
        {
            return new Res_value
            {
                Size = ReadUInt16(),
                Res0 = ReadByte(),
                DataType = (ValueType) ReadByte(),
                RawData = ReadUInt32()
            };
        }

        public virtual ResChunk_header ReadResChunk_header()
        {
            return new ResChunk_header
            {
                Type = (ResourceType) ReadUInt16(),
                HeaderSize = ReadUInt16(),
                Size = ReadUInt32(),
            };
        }

        public virtual ResStringPool_header ReadResStringPool_header(ResChunk_header header)
        {
            return new ResStringPool_header
            {
                Header = header,
                StringCount = ReadUInt32(),
                StyleCount = ReadUInt32(),
                Flags = (StringPoolFlags) ReadUInt32(),
                StringStart = ReadUInt32(),
                StylesStart = ReadUInt32(),
            };
        }

        public virtual ResStringPool_ref ReadResStringPool_ref()
        {
            uint index = ReadUInt32();
            return new ResStringPool_ref
            {
                Index = index == 0xFFFFFFFFu ? (uint?) null : index,
            };
        }

        public virtual ResStringPool_span ReadResStringPool_span()
        {
            return new ResStringPool_span
            {
                Name = ReadResStringPool_ref(),
                FirstChar = ReadUInt32(),
                LastChar = ReadUInt32(),
            };
        }

        public virtual ResTable_config ReadResTable_config()
        {
            return new ResTable_config
            {
                Size = ReadUInt32(),
                IMSI = ReadUInt32(),
                Locale = ReadUInt32(),
                ScreenType = ReadUInt32(),
                Input = ReadUInt32(),
                ScreenSize = ReadUInt32(),
                Version = ReadUInt32(),
                ScreenConfig = ReadUInt32(),
                ScreenSizeDp = ReadUInt32(),
            };
        }

        public virtual ResTable_entry ReadResTable_entry()
        {
            return new ResTable_entry
            {
                Size = ReadUInt16(),
                Flags = (EntryFlags) ReadUInt16(),
                Key = ReadResStringPool_ref(),
            };
        }

        public virtual ResTable_header ReadResTable_header(ResChunk_header header)
        {
            return new ResTable_header
            {
                Header = header,
                PackageCount = ReadUInt32(),
            };
        }

        public virtual ResTable_map ReadResTable_map()
        {
            return new ResTable_map
            {
                Name = ReadResTable_ref(),
                Value = ReadRes_value(),
            };
        }

        public virtual ResTable_map_entry ReadResTable_map_entry()
        {
            return new ResTable_map_entry
            {
                Size = ReadUInt16(),
                Flags = (EntryFlags) ReadUInt16(),
                Key = ReadResStringPool_ref(),
                Parent = ReadResTable_ref(),
                Count = ReadUInt32(),
            };
        }

        public virtual ResTable_package ReadResTable_package(ResChunk_header header)
        {
            return new ResTable_package
            {
                Header = header,
                Id = ReadUInt32(),
                Name = Encoding.Unicode.GetString(ReadBytes(256)),
                TypeStrings = ReadUInt32(),
                LastPublicType = ReadUInt32(),
                KeyStrings = ReadUInt32(),
                LastPublicKey = ReadUInt32(),
            };
        }

        public virtual ResTable_ref ReadResTable_ref()
        {
            uint ident = ReadUInt32();
            return new ResTable_ref
            {
                Ident = ident == 0xFFFFFFFFu ? (uint?) null : ident,
            };
        }

        public virtual ResTable_type ReadResTable_type(ResChunk_header header)
        {
            return new ResTable_type
            {
                Header = header,
                RawID = ReadUInt32(),
                EntryCount = ReadUInt32(),
                EntriesStart = ReadUInt32(),
                Config = ReadResTable_config(),
            };
        }

        public virtual ResTable_typeSpec ReadResTable_typeSpec(ResChunk_header header)
        {
            return new ResTable_typeSpec
            {
                Header = header,
                RawID = ReadUInt32(),
                EntryCount = ReadUInt32(),
            };
        }

        public virtual ResXMLTree_attrExt ReadResXMLTree_attrExt()
        {
            return new ResXMLTree_attrExt
            {
                Namespace = ReadResStringPool_ref(),
                Name = ReadResStringPool_ref(),
                AttributeStart = ReadUInt16(),
                AttributeSize = ReadUInt16(),
                AttributeCount = ReadUInt16(),
                IdIndex = ReadUInt16(),
                ClassIndex = ReadUInt16(),
                StyleIndex = ReadUInt16(),
            };
        }

        public virtual ResXMLTree_attribute ReadResXMLTree_attribute()
        {
            return new ResXMLTree_attribute
            {
                Namespace = ReadResStringPool_ref(),
                Name = ReadResStringPool_ref(),
                RawValue = ReadResStringPool_ref(),
                TypedValue = ReadRes_value()
            };
        }

        public virtual ResXMLTree_cdataExt ReadResXMLTree_cdataExt()
        {
            return new ResXMLTree_cdataExt
            {
                Data = ReadResStringPool_ref(),
                TypedData = ReadRes_value(),
            };
        }

        public virtual ResXMLTree_endElementExt ReadResXMLTree_endElementExt()
        {
            return new ResXMLTree_endElementExt
            {
                Namespace = ReadResStringPool_ref(),
                Name = ReadResStringPool_ref(),
            };
        }

        public virtual ResXMLTree_header ReadResXMLTree_header(ResChunk_header header)
        {
            return new ResXMLTree_header
            {
                Header = header,
            };
        }


        public virtual ResXMLTree_namespaceExt ReadResXMLTree_namespaceExt()
        {
            return new ResXMLTree_namespaceExt
            {
                Prefix = ReadResStringPool_ref(),
                Uri = ReadResStringPool_ref(),
            };
        }

        public virtual ResXMLTree_node ReadResXMLTree_node(ResChunk_header header)
        {
            return new ResXMLTree_node
            {
                Header = header,
                LineNumber = ReadUInt32(),
                Comment = ReadResStringPool_ref(),
            };
        }

        public virtual ResStringPool ReadResStringPool(ResStringPool_header header)
        {
            var pool = new ResStringPool
            {
                Header = header,
                //StringIndices = new List<uint>(),
                //StyleIndices = new List<uint>(),
                StringData = new List<string>(),
                StyleData = new List<ResStringPool_span>()
            };
            var stringIndices = new List<uint>();
            for (int i = 0; i < header.StringCount; i++)
            {
                stringIndices.Add(ReadUInt32());
            }
            for (int i = 0; i < header.StyleCount; i++)
            {
                ReadUInt32(); // Skip
            }

            long bytesLeft = header.Header.Size;
            bytesLeft -= header.Header.HeaderSize;
            bytesLeft -= 4*header.StringCount;
            bytesLeft -= 4*header.StyleCount;

            uint stringsEnd = header.StyleCount > 0 ? header.StylesStart : header.Header.Size;
            byte[] rawStringData = ReadBytes((int) stringsEnd - (int) header.StringStart);

            bytesLeft -= rawStringData.Length;

            bool isUtf8 = (header.Flags & StringPoolFlags.UTF8_FLAG) == StringPoolFlags.UTF8_FLAG;

            foreach (uint startingIndex in stringIndices)
            {
                uint pos = startingIndex;
                if (isUtf8)
                {
                    uint charLen = Helper.DecodeLengthUtf8(rawStringData, ref pos);
                    uint byteLen = Helper.DecodeLengthUtf8(rawStringData, ref pos);
                    string item = Encoding.UTF8.GetString(rawStringData, (int) pos, (int) byteLen);
                    if (item.Length != charLen)
                    {
                        Debug.WriteLine("Warning: UTF-8 string length ({0}) not matching specified length ({1}).",
                                        item.Length, charLen);
                    }
                    pool.StringData.Add(item);
                }
                else
                {
                    uint charLen = Helper.DecodeLengthUtf16(rawStringData, ref pos);
                    uint byteLen = charLen*2;
                    string item = Encoding.Unicode.GetString(rawStringData, (int) pos, (int) byteLen);
                    pool.StringData.Add(item);
                }
            }

            for (int i = 0; i < header.StyleCount; i++)
            {
                pool.StyleData.Add(ReadResStringPool_span());
            }

            bytesLeft -= header.StyleCount*12; // sizeof(ResStringPool_span) in C++

            if (bytesLeft < 0)
            {
                throw new InvalidDataException("The length of the content exceeds the ResStringPool block boundary.");
            }
            if (bytesLeft > 0)
            {
                // Padding?
                Debug.WriteLine("Warning: Garbage at the end of the StringPool block. Padding?");
                ReadBytes((int) bytesLeft);
            }

            return pool;
        }

        public virtual ResXMLTree_startelement ReadResXMLTree_startelement(ResXMLTree_node node,
                                                                           ResXMLTree_attrExt attrExt)
        {
            var element = new ResXMLTree_startelement
            {
                Node = node,
                AttrExt = attrExt,
                Attributes = new List<ResXMLTree_attribute>()
            };

            uint bytesLeft = node.Header.Size - 0x24u;

            for (int i = 0; i < attrExt.AttributeCount; i++)
            {
                element.Attributes.Add(ReadResXMLTree_attribute());
                bytesLeft -= 0x14u;
            }

            if (bytesLeft < 0)
            {
                throw new InvalidDataException("The length of the content exceeds the ResStringPool block boundary.");
            }
            if (bytesLeft > 0)
            {
                Debug.WriteLine("Warning: Garbage at the end of the StringPool block. Padding?");
                ReadBytes((int) bytesLeft);
            }

            return element;
        }

        public virtual ResResourceMap ReadResResourceMap(ResChunk_header header)
        {
            var result = new ResResourceMap
            {
                Header = header,
                ResouceIds = new List<uint>()
            };
            for (int pos = 8; pos < header.Size; pos += 4)
            {
                result.ResouceIds.Add(ReadUInt32());
            }
            return result;
        }
    }
}