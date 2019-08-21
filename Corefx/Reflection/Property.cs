using System.Collections.Concurrent;
using System.Linq;

namespace System.Reflection
{
    /// <summary>
    /// 表示属性
    /// </summary>
    public class Property
    {
        /// <summary>
        /// 获取器
        /// </summary>
        private readonly Func<object, object> geter;

        /// <summary>
        /// 设置器
        /// </summary>
        private readonly Action<object, object> seter;

        /// <summary>
        /// 获取属性名称
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 获取属性信息
        /// </summary>
        public PropertyInfo Info { get; private set; }

        /// <summary>
        /// 属性
        /// </summary>
        /// <param name="property">属性信息</param>
        public Property(PropertyInfo property)
        {
            this.Name = property.Name;
            this.Info = property;

            if (property.CanRead == true)
            {
                this.geter = Lambda.CreateGetFunc<object, object>(property);
            }

            if (property.CanWrite == true)
            {
                this.seter = Lambda.CreateSetAction<object, object>(property);
            }
        }

        /// <summary>
        /// 获取属性的值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        public object GetValue(object instance)
        {
            return this.geter.Invoke(instance);
        }

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="value">值</param>
        public void SetValue(object instance, object value)
        {
            this.seter.Invoke(instance, value);
        }

        /// <summary>
        /// 类型属性的Setter缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Property[]> cached = new ConcurrentDictionary<Type, Property[]>();

        /// <summary>
        /// 从类型的属性获取属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static Property[] GetProperties(Type type)
        {
            return cached.GetOrAdd(type, t => t.GetProperties().Select(p => new Property(p)).ToArray());
        }
    }


    /// <summary>
    /// 表示属性
    /// </summary>
    /// <typeparam name="TDeclaringType"></typeparam>
    public class Property<TDeclaringType>
    {
        /// <summary>
        /// 获取器
        /// </summary>
        private readonly Func<TDeclaringType, object> geter;

        /// <summary>
        /// 设置器
        /// </summary>
        private readonly Action<TDeclaringType, object> seter;

        /// <summary>
        /// 获取属性名称
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 获取属性信息
        /// </summary>
        public PropertyInfo Info { get; private set; }

        /// <summary>
        /// 从类型的属性获取属性
        /// </summary>       
        public static Property<TDeclaringType>[] Properties { get; } = typeof(TDeclaringType).GetProperties().Select(p => new Property<TDeclaringType>(p)).ToArray();

        /// <summary>
        /// 属性
        /// </summary>
        /// <param name="property">属性信息</param>
        public Property(PropertyInfo property)
        {
            this.Name = property.Name;
            this.Info = property;

            if (property.CanRead == true)
            {
                this.geter = Lambda.CreateGetFunc<TDeclaringType, object>(property);
            }

            if (property.CanWrite == true)
            {
                this.seter = Lambda.CreateSetAction<TDeclaringType, object>(property);
            }
        }

        /// <summary>
        /// 获取属性的值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        public object GetValue(TDeclaringType instance)
        {
            return this.geter.Invoke(instance);
        }

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="value">值</param>
        public void SetValue(TDeclaringType instance, object value)
        {
            this.seter.Invoke(instance, value);
        }
    }
}