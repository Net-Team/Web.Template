using System;
using System.IO;
using System.Text;
using AndroidXml.Res;

namespace AndroidXml
{
    public class ResWriter
    {
        protected readonly BinaryWriter _writer;

        public ResWriter(Stream output)
            : this(new BinaryWriter(output)) {}

        public ResWriter(BinaryWriter writer)
        {
            _writer = writer;
        }

        /// <summary>
        /// Gets the underlying <c>BinaryWriter</c> to write primitive values.
        /// </summary>
        public BinaryWriter Writer
        {
            get { return _writer; }
        }

        public virtual void Write(Res_value data)
        {
            _writer.Write(data.Size);
            _writer.Write(data.Res0);
            _writer.Write((byte) data.DataType);
            _writer.Write(data.RawData);
        }

        public virtual void Write(ResChunk_header data)
        {
            _writer.Write((ushort) data.Type);
            _writer.Write(data.HeaderSize);
            _writer.Write(data.Size);
        }

        public virtual void Write(ResStringPool_header data)
        {
            Write(data.Header);
            _writer.Write(data.StringCount);
            _writer.Write(data.StyleCount);
            _writer.Write((uint) data.Flags);
            _writer.Write(data.StringStart);
            _writer.Write(data.StylesStart);
        }

        public virtual void Write(ResStringPool_ref data)
        {
            _writer.Write(data.Index ?? 0xFFFFFFFFu);
        }

        public virtual void Write(ResStringPool_span data)
        {
            Write(data.Name);
            _writer.Write(data.FirstChar);
            _writer.Write(data.LastChar);
        }

        public virtual void Write(ResTable_config data)
        {
            _writer.Write(data.Size);
            _writer.Write(data.IMSI);
            _writer.Write(data.Locale);
            _writer.Write(data.ScreenType);
            _writer.Write(data.Input);
            _writer.Write(data.ScreenSize);
            _writer.Write(data.Version);
            _writer.Write(data.ScreenConfig);
            _writer.Write(data.ScreenSizeDp);
        }

        public virtual void Write(ResTable_entry data)
        {
            _writer.Write(data.Size);
            _writer.Write((ushort) data.Flags);
            Write(data.Key);
        }

        public virtual void Write(ResTable_header data)
        {
            Write(data.Header);
            _writer.Write(data.PackageCount);
        }

        public virtual void Write(ResTable_map data)
        {
            Write(data.Name);
            Write(data.Value);
        }

        public virtual void Write(ResTable_map_entry data)
        {
            _writer.Write(data.Size);
            _writer.Write((ushort) data.Flags);
            Write(data.Key);
            Write(data.Parent);
            _writer.Write(data.Count);
        }

        public virtual void Write(ResTable_package data)
        {
            Write(data.Header);
            _writer.Write(data.Id);
            var stringData = new byte[256];
            byte[] tempData = Encoding.Unicode.GetBytes(data.Name);
            int length = Math.Min(255, tempData.Length); // last pair of bytes must be 0
            Array.Copy(tempData, stringData, length);
            _writer.Write(stringData);
            _writer.Write(data.TypeStrings);
            _writer.Write(data.LastPublicType);
            _writer.Write(data.KeyStrings);
            _writer.Write(data.LastPublicKey);
        }

        public virtual void Write(ResTable_ref data)
        {
            _writer.Write(data.Ident ?? 0xFFFFFFFFu);
        }

        public virtual void Write(ResTable_type data)
        {
            Write(data.Header);
            _writer.Write(data.RawID);
            _writer.Write(data.EntryCount);
            _writer.Write(data.EntriesStart);
            Write(data.Config);
        }

        public virtual void Write(ResTable_typeSpec data)
        {
            Write(data.Header);
            _writer.Write(data.RawID);
            _writer.Write(data.EntryCount);
        }

        public virtual void Write(ResXMLTree_attrExt data)
        {
            Write(data.Namespace);
            Write(data.Name);
            _writer.Write(data.AttributeStart);
            _writer.Write(data.AttributeSize);
            _writer.Write(data.AttributeCount);
            _writer.Write(data.IdIndex);
            _writer.Write(data.ClassIndex);
            _writer.Write(data.StyleIndex);
        }

        public virtual void Write(ResXMLTree_attribute data)
        {
            Write(data.Namespace);
            Write(data.Name);
            Write(data.RawValue);
            Write(data.TypedValue);
        }

        public virtual void Write(ResXMLTree_cdataExt data)
        {
            Write(data.Data);
            Write(data.TypedData);
        }

        public virtual void Write(ResXMLTree_endElementExt data)
        {
            Write(data.Namespace);
            Write(data.Name);
        }

        public virtual void Write(ResXMLTree_header data)
        {
            Write(data.Header);
        }

        public virtual void Write(ResXMLTree_namespaceExt data)
        {
            Write(data.Prefix);
            Write(data.Uri);
        }

        public virtual void Write(ResXMLTree_node data)
        {
            Write(data.Header);
            _writer.Write(data.LineNumber);
            Write(data.Comment);
        }
    }
}