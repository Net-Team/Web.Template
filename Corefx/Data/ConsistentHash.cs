using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data
{
    /// <summary>
    /// 一致性哈希
    /// </summary>
    /// <typeparam name="T">节点类型</typeparam>
    public class ConsistentHash<T>
    {
        /// <summary>
        /// 缓存所有哈希键
        /// </summary>        
        private int[] hashKeys;

        /// <summary>
        /// 初始环大小
        /// </summary>
        private int defaultReplicate = 100;

        /// <summary>
        /// 哈希键与节点字典
        /// </summary>
        private readonly SortedDictionary<int, T> hashNodeTable = new SortedDictionary<int, T>();

        /// <summary>
        /// 一致性哈希
        /// </summary>
        public ConsistentHash()
            : this(null)
        {
        }

        /// <summary>
        /// 一致性哈希
        /// </summary>
        /// <param name="nodes">节点</param>
        public ConsistentHash(IEnumerable<T> nodes)
        {
            if (nodes != null)
            {
                foreach (T node in nodes)
                {
                    this.Add(node, false);
                }
                this.hashKeys = hashNodeTable.Keys.ToArray();
            }
        }

        /// <summary>
        /// 添加一个节点
        /// </summary>
        /// <param name="node">节点</param>
        public void Add(T node)
        {
            this.Add(node, true);
        }

        /// <summary>
        /// 添加一个节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="updateKeyArray">是否把键更新到局部变量进行缓存</param>
        private void Add(T node, bool updateKeyArray)
        {
            for (int i = 0; i < defaultReplicate; i++)
            {
                int hash = HashAlgorithm.GetHashCode(node.GetHashCode().ToString() + i);
                this.hashNodeTable[hash] = node;
            }

            if (updateKeyArray == true)
            {
                this.hashKeys = this.hashNodeTable.Keys.ToArray();
            }
        }

        /// <summary>
        /// 删除一个节点
        /// </summary>
        /// <param name="node">节点</param>
        public bool Remove(T node)
        {
            for (int i = 0; i < defaultReplicate; i++)
            {
                int hash = HashAlgorithm.GetHashCode(node.GetHashCode().ToString() + i);
                if (this.hashNodeTable.Remove(hash) == false)
                {
                    return false;
                }
            }

            // 重新加载节点
            this.hashKeys = this.hashNodeTable.Keys.ToArray();
            return true;
        }

        /// <summary>
        /// 获取键对应的节点
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T GetNode(string key)
        {
            int firstNode = this.GetHaskKeyIndex(key);
            return this.hashNodeTable[hashKeys[firstNode]];
        }

        /// <summary>
        /// 获取键所对应的键哈希表的索引
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        private int GetHaskKeyIndex(string key)
        {
            int hashCode = HashAlgorithm.GetHashCode(key);

            int begin = 0;
            int end = this.hashKeys.Length - 1;

            if (this.hashKeys[end] < hashCode || this.hashKeys[0] > hashCode)
            {
                return 0;
            }

            while (end - begin > 1)
            {
                int mid = (end + begin) / 2;
                if (this.hashKeys[mid] >= hashCode)
                {
                    end = mid;
                }
                else
                {
                    begin = mid;
                }
            }

            return end;
        }


        /// <summary>
        /// 哈希值算法
        /// </summary>
        private class HashAlgorithm
        {
            private const uint m = 0x5bd1e995;
            private const int r = 24;

            /// <summary>
            /// byte转换为uint
            /// </summary>
            [StructLayout(LayoutKind.Explicit)]
            private struct Byte2Uint
            {
                [FieldOffset(0)]
                public byte[] Bytes;

                [FieldOffset(0)]
                public uint[] UInts;
            }

            /// <summary>
            /// 获取哈希值
            /// </summary>
            /// <param name="key">文本</param>
            /// <returns></returns>
            public static int GetHashCode(string key)
            {
                return (int)GetHashCode(Encoding.ASCII.GetBytes(key));
            }

            /// <summary>
            /// 获取哈希值
            /// </summary>
            /// <param name="bytes">二进制数据</param>
            /// <returns></returns>
            public static uint GetHashCode(byte[] bytes)
            {
                return GetHashCode(bytes, 0xc58f1a7b);
            }

            /// <summary>
            /// 获取哈希值
            /// </summary>
            /// <param name="bytes">二进制数据</param>
            /// <param name="seed">种子</param>
            /// <returns></returns>
            public static uint GetHashCode(byte[] bytes, uint seed)
            {
                int length = bytes.Length;
                if (length == 0)
                {
                    return 0;
                }
                uint h = seed ^ (uint)length;
                int currentIndex = 0;

                uint[] hackArray = new Byte2Uint { Bytes = bytes }.UInts;
                while (length >= 4)
                {
                    uint k = hackArray[currentIndex++];
                    k *= m;
                    k ^= k >> r;
                    k *= m;

                    h *= m;
                    h ^= k;
                    length -= 4;
                }
                currentIndex *= 4;
                switch (length)
                {
                    case 3:
                        h ^= (ushort)(bytes[currentIndex++] | bytes[currentIndex++] << 8);
                        h ^= (uint)bytes[currentIndex] << 16;
                        h *= m;
                        break;
                    case 2:
                        h ^= (ushort)(bytes[currentIndex++] | bytes[currentIndex] << 8);
                        h *= m;
                        break;
                    case 1:
                        h ^= bytes[currentIndex];
                        h *= m;
                        break;
                    default:
                        break;
                }


                h ^= h >> 13;
                h *= m;
                h ^= h >> 15;

                return h;
            }
        }
    }
}