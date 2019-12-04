using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Core.Web.ModelBinding
{
    /// <summary>
    /// 表示String属性trim处理的模型绑定提供者
    /// </summary>
    public class StringPropertyTrimModelBinderProvider : IModelBinderProvider
    {
        private readonly MvcOptions mvcOptions;

        private readonly Func<PropertyInfo, bool> propertyFilter;

        /// <summary>
        /// String属性trim处理的模型绑定提供者
        /// </summary>
        /// <param name="mvcOptions">mvc配置项</param>
        /// <param name="propertyFilter">属性过滤器</param>
        /// <exception cref="ArgumentNullException"></exception>
        public StringPropertyTrimModelBinderProvider(MvcOptions mvcOptions, Func<PropertyInfo, bool> propertyFilter)
        {
            this.mvcOptions = mvcOptions ?? throw new ArgumentNullException(nameof(mvcOptions));
            this.propertyFilter = propertyFilter ?? throw new ArgumentNullException(nameof(propertyFilter));
        }

        /// <summary>
        /// 获取Binder
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.BindingInfo.BindingSource != null && context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Body))
            {
                var readerFactory = context.Services.GetService<IHttpRequestStreamReaderFactory>();
                var loggerFactory = context.Services.GetService<ILoggerFactory>();
                var bodyModelBinder = new BodyModelBinder(this.mvcOptions.InputFormatters, readerFactory, loggerFactory, this.mvcOptions);
                return new StringPropertyTrimModelBinder(bodyModelBinder, this.propertyFilter);
            }

            return null;
        }


        /// <summary>
        /// 表示String属性trim处理的模型绑定者
        /// </summary>
        private class StringPropertyTrimModelBinder : IModelBinder
        {
            private readonly BodyModelBinder bodyModelBinder;

            private readonly StringPropertyCache stringPropertyCache;

            /// <summary>
            /// String属性trim处理的模型绑定者
            /// </summary>
            /// <param name="bodyModelBinder"></param>
            /// <param name="propertyFilter"></param>           
            public StringPropertyTrimModelBinder(BodyModelBinder bodyModelBinder, Func<PropertyInfo, bool> propertyFilter)
            {
                this.bodyModelBinder = bodyModelBinder;
                this.stringPropertyCache = new StringPropertyCache(propertyFilter);
            }

            /// <summary>
            /// 进行模型绑定
            /// </summary>
            /// <param name="bindingContext"></param>
            /// <returns></returns>
            public async Task BindModelAsync(ModelBindingContext bindingContext)
            {
                // 调用原始body绑定数据
                await this.bodyModelBinder.BindModelAsync(bindingContext);
                if (bindingContext.Result.IsModelSet == false)
                {
                    return;
                }

                var properties = this.stringPropertyCache.Get(bindingContext.ModelType);
                foreach (var property in properties)
                {
                    property.Trim(bindingContext.Result.Model);
                }
            }
        }

        /// <summary>
        /// 表示类型的StringProperty缓存
        /// </summary>
        private class StringPropertyCache
        {
            private readonly Func<PropertyInfo, bool> propertyFilter;

            private readonly ConcurrentDictionary<Type, StringProperty[]> trimPropertyCache = new ConcurrentDictionary<Type, StringProperty[]>();

            /// <summary>
            /// 类型的StringProperty缓存
            /// </summary>
            /// <param name="propertyFilter"></param>
            public StringPropertyCache(Func<PropertyInfo, bool> propertyFilter)
            {
                this.propertyFilter = propertyFilter;
            }

            /// <summary>
            /// 获取类型的StringProperty缓存
            /// </summary>
            /// <param name="modelType"></param>
            /// <returns></returns>
            public StringProperty[] Get(Type modelType)
            {
                return this.trimPropertyCache.GetOrAdd(modelType, type =>
                    type.GetProperties()
                    .Where(item => item.PropertyType == typeof(string) && item.CanWrite && item.CanRead && this.propertyFilter(item))
                    .Select(p => new StringProperty(p))
                    .ToArray());
            }
        }

        /// <summary>
        /// 表示String类型的属性
        /// </summary>
        private class StringProperty
        {
            private readonly Func<object, string> geter;

            private readonly Action<object, string> seter;

            /// <summary>
            /// 属性
            /// </summary>
            /// <param name="property">属性信息</param>
            public StringProperty(PropertyInfo property)
            {
                this.geter = Lambda.CreateGetFunc<object, string>(property);
                this.seter = Lambda.CreateSetAction<object, string>(property);
            }

            /// <summary>
            /// 将属性值进行Trim操作
            /// </summary>
            /// <param name="instance">实例</param>
            /// <returns></returns>
            public void Trim(object instance)
            {
                if (instance == null)
                {
                    return;
                }

                var value = this.geter.Invoke(instance);
                var trimVal = value?.Trim();
                if (value != trimVal)
                {
                    this.seter.Invoke(instance, trimVal);
                }
            }
        }
    }
}
