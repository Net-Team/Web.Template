using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    /// <summary>
    /// 表示手动管理的任务抽象类
    /// </summary>
    public abstract class ManualTask
    {
        /// <summary>
        /// 获取任务的结果值类型
        /// </summary>
        public abstract Type ResultType { get; }

        /// <summary>
        /// 设置任务结果
        /// </summary>
        /// <param name="result">结果值</param>
        /// <returns></returns>
        public abstract bool SetResult(object result);

        /// <summary>
        /// 设置任务异常
        /// </summary>
        /// <param name="exception">异常</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public abstract bool SetException(Exception exception);

    }

    /// <summary>
    /// 表示手动管理的任务
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    [DebuggerDisplay("IsCompleted = {IsCompleted}")]
    public class ManualTask<TResult> : ManualTask, ICriticalNotifyCompletion
    {
        /// <summary>
        /// 结果值
        /// </summary>
        private TResult result;

        /// <summary>
        /// 异常值
        /// </summary>
        private Exception exception;

        /// <summary>
        /// 延续的任务
        /// </summary>
        private Action continuation;

        /// <summary>
        /// 延时设置结果的定时器
        /// </summary>

        private Timer delayTimer;

        /// <summary>
        /// 是否已完成
        /// </summary>
        private long isCompleted = default;




        /// <summary>
        /// 获取任务的结果值类型
        /// </summary>
        public override Type ResultType { get; } = typeof(TResult);

        /// <summary>
        /// 获取任务是否已完成
        /// </summary>
        public bool IsCompleted => Interlocked.Read(ref this.isCompleted) != default;

        /// <summary>
        /// 获取等待对象
        /// </summary>
        /// <returns></returns>
        public ManualTask<TResult> GetAwaiter()
        {
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public TResult GetResult()
        {
            if (this.IsCompleted == false)
            {
                throw new InvalidOperationException();
            }

            if (this.exception != null)
            {
                throw this.exception;
            }
            return this.result;
        }

        /// <summary>
        /// 设置任务结果
        /// </summary>
        /// <param name="result">结果值</param>
        /// <returns></returns>
        public override bool SetResult(object result)
        {
            return this.SetResult((TResult)result);
        }

        /// <summary>
        /// 设置任务结果
        /// </summary>
        /// <param name="result">结果值</param>
        /// <returns></returns>
        public bool SetResult(TResult result)
        {
            return this.SetCompleted(result, exception: default);
        }

        /// <summary>
        /// 设置任务异常
        /// </summary>
        /// <param name="exception">异常</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public override bool SetException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            return this.SetCompleted(result: default, exception);
        }

        /// <summary>
        /// 延时自动设置任务异常
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="delay">延时时长</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void SetExceptionAfter(Exception exception, TimeSpan delay)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            if (delay <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(delay));
            }

            if (this.IsCompleted == false)
            {
                this.delayTimer?.Dispose();
                this.delayTimer = new Timer(this.DelayTimerCallback, new TimerState(exception), delay, Timeout.InfiniteTimeSpan);
            }
        }

        /// <summary>
        /// 延时自动设置任务结果
        /// </summary>
        /// <param name="result">结果值</param>
        /// <param name="delay">延时时长</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void SetResultAfter(TResult result, TimeSpan delay)
        {
            if (delay <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(delay));
            }

            if (this.IsCompleted == false)
            {
                this.delayTimer?.Dispose();
                this.delayTimer = new Timer(this.DelayTimerCallback, new TimerState(result), delay, Timeout.InfiniteTimeSpan);
            }
        }

        /// <summary>
        /// 延时timer回调
        /// </summary>
        /// <param name="state"></param>
        private void DelayTimerCallback(object state)
        {
            this.delayTimer.Dispose();
            var timerState = state as TimerState;
            this.SetCompleted(timerState.Result, timerState.Exception);
        }

        /// <summary>
        /// 设置为已完成状态
        /// 只有第一次设置有效
        /// </summary>
        /// <param name="result">结果</param>
        /// <param name="exception">异常</param>
        /// <returns></returns>
        private bool SetCompleted(TResult result, Exception exception)
        {
            if (Interlocked.CompareExchange(ref this.isCompleted, 1L, default) != default)
            {
                return false;
            }

            this.result = result;
            this.exception = exception;
            this.delayTimer?.Dispose();

            if (this.continuation != null)
            {
                ThreadPool.UnsafeQueueUserWorkItem(state => ((Action)state)(), this.continuation);
            }
            return true;
        }


        /// <summary>
        /// 完成通知
        /// </summary>
        /// <param name="continuation">延续的任务</param>
        void INotifyCompletion.OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }

        /// <summary>
        /// 完成通知
        /// </summary>
        /// <param name="continuation">延续的任务</param>
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }

        /// <summary>
        /// timer的状态数据
        /// </summary>
        private class TimerState
        {
            /// <summary>
            /// 结果值
            /// </summary>
            public TResult Result { get; }

            /// <summary>
            /// 异常值
            /// </summary>
            public Exception Exception { get; }

            /// <summary>
            /// 状态数据
            /// </summary>
            /// <param name="exception"></param>
            public TimerState(Exception exception)
            {
                this.Exception = exception;
            }

            /// <summary>
            /// 状态数据
            /// </summary>
            /// <param name="result"></param>
            public TimerState(TResult result)
            {
                this.Result = result;
            }
        }
    }
}