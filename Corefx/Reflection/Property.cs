using System.Collections.Concurrent;
using System.Linq;

namespace System.Reflection
{
    /// <summary>
    /// 表示属性
    /// </summary>
    public class Property : Property<object, object>
    {
        /// <summary>
        /// 类型的属性缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Property[]> cache = new ConcurrentDictionary<Type, Property[]>();

        /// <summary>
        /// 属性
        /// </summary>
        /// <param name="property">属性信息</param>
        public Property(PropertyInfo property)
            : base(property)
        {
        }

        /// <summary>
        /// 从类型的属性获取属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static Property[] GetProperties(Type type)
        {
            return cache.GetOrAdd(type, t => t.GetProperties().Select(p => new Property(p)).ToArray());
        }
    }


    /// <summary>
    /// 表示属性
    /// </summary>
    /// <typeparam name="TDeclaring">定义属性的类型</typeparam>
    public class Property<TDeclaring> : Property<TDeclaring, object>
    {
        /// <summary>
        /// 从类型的属性获取属性
        /// </summary>       
        public static Property<TDeclaring>[] Properties { get; } = typeof(TDeclaring).GetProperties().Select(p => new Property<TDeclaring>(p)).ToArray();

        /// <summary>
        /// 属性
        /// </summary>
        /// <param name="property">属性信息</param>
        public Property(PropertyInfo property)
            : base(property)
        {
        }
    }



    /// <summary>
    /// 表示属性
    /// </summary>
    /// <typeparam name="TDeclaring">定义属性的类型</typeparam>
    /// <typeparam name="TProperty">属性类型</typeparam>
    public class Property<TDeclaring, TProperty>
    {
        /// <summary>
        /// 获取器
        /// </summary>
        private readonly Func<TDeclaring, TProperty> geter;

        /// <summary>
        /// 设置器
        /// </summary>
        private readonly Action<TDeclaring, TProperty> seter;

        /// <summary>
        /// 获取属性名称
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 获取属性信息
        /// </summary>
        public PropertyInfo Info { get; }

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
                this.geter = Lambda.CreateGetFunc<TDeclaring, TProperty>(property);
            }

            if (property.CanWrite == true)
            {
                this.seter = Lambda.CreateSetAction<TDeclaring, TProperty>(property);
            }
        }

        /// <summary>
        /// 获取属性的值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        public TProperty GetValue(TDeclaring instance)
        {
            return this.geter.Invoke(instance);
        }

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="value">值</param>
        public void SetValue(TDeclaring instance, TProperty value)
        {
            this.seter?.Invoke(instance, value);
        }
    }
}