using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Middlewares
{
    /// <summary>
    /// 提供中间件创建器
    /// </summary>
    public static class MiddlewareBuilder
    {
        /// <summary>
        /// 创建一个PipelineBuilder
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="appServices">服务提供者</param>
        /// <returns></returns>
        public static IMiddlewareBuilder<TContext> Create<TContext>(IServiceProvider appServices)
        {
            return Create<TContext>(appServices, context => Task.CompletedTask);
        }

        /// <summary>
        /// 创建一个PipelineBuilder
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="appServices">服务提供者</param>
        /// <param name="completeDelegate">完成执行内容委托</param>
        /// <returns></returns>
        public static IMiddlewareBuilder<TContext> Create<TContext>(IServiceProvider appServices, InvokeDelegate<TContext> completeDelegate)
        {
            return new PipelineBuilderOf<TContext>(appServices, completeDelegate);
        }

        /// <summary>
        /// 中间件创建器
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        private class PipelineBuilderOf<TContext> : IMiddlewareBuilder<TContext>
        {
            private readonly IServiceProvider appServices;
            private readonly InvokeDelegate<TContext> completeDelegate;
            private readonly IList<Func<InvokeDelegate<TContext>, InvokeDelegate<TContext>>> pipelines = new List<Func<InvokeDelegate<TContext>, InvokeDelegate<TContext>>>();

            /// <summary>
            /// 中间件创建器
            /// </summary>
            /// <param name="appServices"></param>
            /// <param name="completeDelegate">完成执行内容委托</param>
            public PipelineBuilderOf(IServiceProvider appServices, InvokeDelegate<TContext> completeDelegate)
            {
                this.appServices = appServices;
                this.completeDelegate = completeDelegate;
            }


            /// <summary>
            /// 使用中间件
            /// </summary>
            /// <typeparam name="TMiddleware"></typeparam>
            /// <returns></returns>
            public IMiddlewareBuilder<TContext> Use<TMiddleware>() where TMiddleware : class, IMiddleware<TContext>
            {
                var middleware = this.appServices.GetService(typeof(TMiddleware)) as TMiddleware;
                return this.Use(middleware.InvokeAsync);
            }

            /// <summary>
            /// 使用中间件
            /// </summary>
            /// <param name="func"></param>
            /// <returns></returns>
            public IMiddlewareBuilder<TContext> Use(Func<TContext, Func<Task>, Task> func)
            {
                return this.Use(next => context => func(context, () => next(context)));
            }


            /// <summary>
            /// 使用中间件
            /// </summary>
            /// <param name="middleware"></param>
            /// <returns></returns>
            public IMiddlewareBuilder<TContext> Use(Func<InvokeDelegate<TContext>, InvokeDelegate<TContext>> middleware)
            {
                this.pipelines.Add(middleware);
                return this;
            }


            /// <summary>
            /// 创建所有中间件执行委托
            /// </summary>
            /// <returns></returns>
            public InvokeDelegate<TContext> Build()
            {
                var @delegate = completeDelegate;
                foreach (var middleware in this.pipelines.Reverse())
                {
                    @delegate = middleware(@delegate);
                }
                return @delegate;
            }
        }
    }
}