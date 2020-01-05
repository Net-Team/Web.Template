#if NETCOREAPP3_0
namespace System.Buffers
{
    /// <summary>
    /// 表示字节缓冲区写入对象
    /// </summary>
    public sealed class ByteBufferWriter : Disposable, IBufferWriter<byte>
    {
        private int index = 0;
        private byte[] rentedBuffer;
        private const int MinimumBufferSize = 256;

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

            this.rentedBuffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
        }

        /// <summary>
        /// 获取已数入的数据
        /// </summary>
        public ReadOnlyMemory<byte> WrittenMemory
        {
            get => this.rentedBuffer.AsMemory(0, this.index);
        }

        /// <summary>
        /// 获取已写入的字节数
        /// </summary>
        public int WrittenCount
        {
            get => this.index;
        }

        /// <summary>
        /// 获取容量
        /// </summary>
        public int Capacity
        {
            get => this.rentedBuffer.Length;
        }

        /// <summary>
        /// 获取空余容量
        /// </summary>
        public int FreeCapacity
        {
            get => this.rentedBuffer.Length - this.index;
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear()
        {
            this.rentedBuffer.AsSpan(0, this.index).Clear();
            this.index = 0;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (this.rentedBuffer != null)
            {
                ArrayPool<byte>.Shared.Return(this.rentedBuffer);
            }
        }

        /// <summary>
        /// 设置向前推进
        /// </summary>
        /// <param name="count"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Advance(int count)
        {
            if (count < 0 || this.index + count > this.rentedBuffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            this.index += count;
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
            return this.rentedBuffer.AsMemory(this.index);
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
            return rentedBuffer.AsSpan(this.index);
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

            int availableSpace = this.rentedBuffer.Length - this.index;
            if (sizeHint > availableSpace)
            {
                var growBy = Math.Max(sizeHint, this.rentedBuffer.Length);
                var newSize = checked(this.rentedBuffer.Length + growBy);

                var oldBuffer = this.rentedBuffer;
                this.rentedBuffer = ArrayPool<byte>.Shared.Rent(newSize);

                var oldSpan = oldBuffer.AsSpan(0, this.index);
                oldSpan.CopyTo(this.rentedBuffer);

                ArrayPool<byte>.Shared.Return(oldBuffer);
            }
        }
    }
}
#endif