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

        private readonly Type trimAttributeType;

        /// <summary>
        /// String属性trim处理的模型绑定提供者
        /// </summary>
        /// <param name="mvcOptions">mvc配置项</param>
        /// <param name="trimAttributeType">属性声明的trim标记特性类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        public StringPropertyTrimModelBinderProvider(MvcOptions mvcOptions, Type trimAttributeType)
        {
            this.mvcOptions = mvcOptions ?? throw new ArgumentNullException(nameof(mvcOptions));
            this.trimAttributeType = trimAttributeType ?? throw new ArgumentNullException(nameof(trimAttributeType));
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
                return new StringPropertyTrimModelBinder(bodyModelBinder, this.trimAttributeType);
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
            /// <param name="trimAttributeType"></param>           
            public StringPropertyTrimModelBinder(BodyModelBinder bodyModelBinder, Type trimAttributeType)
            {
                this.bodyModelBinder = bodyModelBinder;
                this.stringPropertyCache = new StringPropertyCache(trimAttributeType);
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
            private readonly Type trimAttributeType;

            private readonly ConcurrentDictionary<Type, StringProperty[]> trimPropertyCache = new ConcurrentDictionary<Type, StringProperty[]>();

            /// <summary>
            /// 类型的StringProperty缓存
            /// </summary>
            /// <param name="trimAttributeType"></param>
            public StringPropertyCache(Type trimAttributeType)
            {
                this.trimAttributeType = trimAttributeType;
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
                    .Where(item => item.PropertyType == typeof(string) && item.CanWrite && item.CanRead && item.IsDefined(this.trimAttributeType))
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
