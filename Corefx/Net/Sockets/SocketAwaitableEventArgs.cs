using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Sockets
{
    /// <summary>
    /// 表示可等待的SocketAsyncEventArgs
    /// </summary>
    public class SocketAwaitableEventArgs : SocketAsyncEventArgs, ICriticalNotifyCompletion
    {
        private static readonly Action _callbackCompleted = () => { };

        private Action _callback;

        /// <summary>
        /// 获取是否已完成
        /// </summary>
        public bool IsCompleted
        {
            get => ReferenceEquals(_callback, _callbackCompleted);
        }

        /// <summary>
        /// 获取等待器
        /// </summary>
        /// <returns></returns>
        public SocketAwaitableEventArgs GetAwaiter()
        {
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <returns></returns>
        public int GetResult()
        {
            _callback = null;
            if (SocketError != SocketError.Success)
            {
                throw new SocketException((int)SocketError);
            }
            return this.BytesTransferred;
        }

        /// <summary>
        /// 完成时
        /// </summary>
        /// <param name="continuation"></param>
        void INotifyCompletion.OnCompleted(Action continuation)
        {
            if (ReferenceEquals(_callback, _callbackCompleted) ||
                ReferenceEquals(Interlocked.CompareExchange(ref _callback, continuation, null), _callbackCompleted))
            {
                Task.Run(continuation);
            }
        }

        /// <summary>
        /// 完成时
        /// </summary>
        /// <param name="continuation"></param>
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation)
        {
            ((INotifyCompletion)this).OnCompleted(continuation);
        }

        /// <summary>
        /// 设置为完成
        /// </summary>
        public void Complete()
        {
            this.OnCompleted(this);
        }

        /// <summary>
        /// 完成时
        /// </summary>
        /// <param name="_"></param>
        protected override void OnCompleted(SocketAsyncEventArgs _)
        {
            var continuation = Interlocked.Exchange(ref _callback, _callbackCompleted);
            if (continuation != null)
            {
                ThreadPool.UnsafeQueueUserWorkItem(state => ((Action)state)(), continuation);
            }
        }
    }
}
