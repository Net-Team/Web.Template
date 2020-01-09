#if NETCOREAPP3_0
namespace System.Buffers
{
    /// <summary>
    /// 表示字节缓冲区写入对象
    /// </summary>
    public sealed class ByteBufferWriter : Disposable, IBufferWriter<byte>
    {
        private const int MinimumBufferSize = 256;
        private IArrayOwner<byte> byteArrayOwner;      

        /// <summary>
        /// 获取已写入的字节数
        /// </summary>
        public int WrittenCount { get; private set; }

        /// <summary>
        /// 获取已数入的数据
        /// </summary>
        public ReadOnlyMemory<byte> WrittenMemory
        {
            get => this.byteArrayOwner.Array.AsMemory(0, this.WrittenCount);
        }

        /// <summary>
        /// 获取容量
        /// </summary>
        public int Capacity
        {
            get => this.byteArrayOwner.Array.Length;
        }

        /// <summary>
        /// 获取空余容量
        /// </summary>
        public int FreeCapacity
        {
            get => this.Capacity - this.WrittenCount;
        }


        /// <summary>
        /// 字节缓冲区写入对象
        /// </summary>
        /// <param name="initialCapacity">初始容量</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ByteBufferWriter(int initialCapacity)
        {
            if (initialCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity));
            }
            this.byteArrayOwner = ArrayPool.Rent<byte>(initialCapacity);
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear()
        {
            this.byteArrayOwner.Array.AsSpan(0, this.WrittenCount).Clear();
            this.WrittenCount = 0;
        }

        /// <summary>
        /// 设置向前推进
        /// </summary>
        /// <param name="count"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Advance(int count)
        {
            if (count < 0 || this.WrittenCount + count > this.Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            this.WrittenCount += count;
        }

        /// <summary>
        /// 返回用于写入数据的Memory
        /// </summary>
        /// <param name="sizeHint"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            this.CheckAndResizeBuffer(sizeHint);
            return this.byteArrayOwner.Array.AsMemory(this.WrittenCount);
        }

        /// <summary>
        /// 返回用于写入数据的Span
        /// </summary>
        /// <param name="sizeHint"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public Span<byte> GetSpan(int sizeHint = 0)
        {
            this.CheckAndResizeBuffer(sizeHint);
            return byteArrayOwner.Array.AsSpan(this.WrittenCount);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.byteArrayOwner?.Dispose();
        }

        /// <summary>
        /// 检测和扩容
        /// </summary>
        /// <param name="sizeHint"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void CheckAndResizeBuffer(int sizeHint)
        {
            if (sizeHint < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeHint));
            }

            if (sizeHint == 0)
            {
                sizeHint = MinimumBufferSize;
            }

            if (sizeHint > this.FreeCapacity)
            {
                var growBy = Math.Max(sizeHint, this.Capacity);
                var newSize = checked(this.Capacity + growBy);

                var oldByteArrayOwner = this.byteArrayOwner;
                this.byteArrayOwner = ArrayPool.Rent<byte>(newSize);

                oldByteArrayOwner.Array.AsSpan(0, this.WrittenCount).CopyTo(this.byteArrayOwner.Array);
                oldByteArrayOwner.Dispose();
            }
        }
    }
}
#endif