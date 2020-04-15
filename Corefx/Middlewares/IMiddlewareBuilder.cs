using System.Threading.Tasks;

namespace System.Middlewares
{
    /// <summary>
    /// 定义中间件管道创建者的接口
    /// </summary>
    /// <typeparam name="TContext">中间件上下文</typeparam>
    public interface IMiddlewareBuilder<TContext>
    {
        /// <summary>
        /// 使用中间件
        /// </summary>
        /// <typeparam name="TMiddleware"></typeparam>
        /// <returns></returns>
        IMiddlewareBuilder<TContext> Use<TMiddleware>() where TMiddleware : class, IMiddleware<TContext>;

        /// <summary>
        /// 使用中间件
        /// </summary>
        /// <param name="func">中间件方法</param>
        /// <returns></returns>
        IMiddlewareBuilder<TContext> Use(Func<TContext, Func<Task>, Task> func);

        /// <summary>
        /// 使用中间件
        /// </summary>
        /// <param name="middleware">中间件</param>
        /// <returns></returns>
        IMiddlewareBuilder<TContext> Use(Func<InvokeDelegate<TContext>, InvokeDelegate<TContext>> middleware);

        /// <summary>
        /// 创建所有中间件执行委托
        /// </summary>
        /// <returns></returns>
        InvokeDelegate<TContext> Build();
    }
}
