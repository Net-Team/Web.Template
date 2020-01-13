using System.Collections.Concurrent;
using System.Linq;

namespace System.Threading.Tasks
{
    /// <summary>
    /// 表示任务调度器
    /// </summary>
    public abstract class TaskPipeScheduler
    {
        /// <summary>
        /// 获取非管道调度器
        /// </summary>
        public static TaskPipeScheduler NoPipe { get; } = new ThreadPoolScheduler();

        /// <summary>
        /// 创建一个并发限制的调度器
        /// </summary>
        /// <param name="concurrent"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static TaskPipeScheduler Create(int concurrent) => new ConcurrentScheduler(concurrent);

        /// <summary>
        /// 调度任务
        /// </summary>
        /// <param name="workItem">任务</param>
        public void Schedule(Action workItem)
        {
            if (workItem != null)
            {
                this.Schedule(() =>
                {
                    workItem();
                    return Task.CompletedTask;
                });
            }
        }

        /// <summary>
        /// 调度任务
        /// </summary>
        /// <param name="workItem">任务</param>
        public abstract void Schedule(Func<Task> workItem);


        /// <summary>
        /// 线程池IO队列
        /// </summary>
        private class ThreadPoolScheduler : TaskPipeScheduler
        {
            /// <summary>
            /// 调度任务
            /// </summary>
            /// <param name="workItem"></param>
            public override void Schedule(Func<Task> workItem)
            {
                ThreadPool.UnsafeQueueUserWorkItem(state => workItem(), null);
            }
        }


        /// <summary>
        /// 表示并发控制的IO任务调度器
        /// </summary>
        private class ConcurrentScheduler : TaskPipeScheduler
        {
            private int queueIndex = 0;
            private readonly InlineWorkItemQueue[] inlineQueues;

            /// <summary>
            /// 并发控制的IO任务调度器
            /// </summary>
            /// <param name="concurrent">任务最大并发数</param>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            public ConcurrentScheduler(int concurrent)
            {
                if (concurrent <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(concurrent));
                }
                this.inlineQueues = Enumerable.Range(0, concurrent).Select(item => new InlineWorkItemQueue()).ToArray();
            }


            /// <summary>
            /// 调度任务
            /// </summary>
            /// <param name="workItem">任务工厂</param>
            /// <exception cref="ArgumentNullException"></exception>
            public override void Schedule(Func<Task> workItem)
            {
                if (workItem == null)
                {
                    throw new ArgumentNullException(nameof(workItem));
                }

                this.inlineQueues[queueIndex].Add(workItem);
                this.queueIndex = (this.queueIndex + 1) % this.inlineQueues.Length;
            }

            /// <summary>
            /// 表示排队的IO队列
            /// </summary>
            private class InlineWorkItemQueue
#if NETCOREAPP3_0
                : IThreadPoolWorkItem
#endif
            {
                private int doingWork;
                private readonly ConcurrentQueue<Func<Task>> workItems = new ConcurrentQueue<Func<Task>>();

                /// <summary>
                /// 添加任务
                /// </summary>
                /// <param name="workItem"></param>
                public void Add(Func<Task> workItem)
                {
                    this.workItems.Enqueue(workItem);
                    if (Interlocked.CompareExchange(ref this.doingWork, 1, 0) == 0)
                    {
#if NETCOREAPP3_0
                        ThreadPool.UnsafeQueueUserWorkItem(this, preferLocal: false);
#else
                        ThreadPool.UnsafeQueueUserWorkItem(Execute, this);
#endif
                    }
                }

#if NETCOREAPP3_0
                /// <summary>
                /// 执行
                /// </summary>
                void IThreadPoolWorkItem.Execute()
                {
                    Execute(this);
                }
#endif


                /// <summary>
                /// 执行
                /// </summary>
                /// <param name="state"></param>
                private static async void Execute(object state)
                {
                    var _this = (InlineWorkItemQueue)state;
                    while (true)
                    {
                        while (_this.workItems.TryDequeue(out var item))
                        {
                            await item();
                        }

                        _this.doingWork = 0;
                        Thread.MemoryBarrier();

                        if (_this.workItems.IsEmpty == true)
                        {
                            break;
                        }

                        if (Interlocked.Exchange(ref _this.doingWork, 1) == 1)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
