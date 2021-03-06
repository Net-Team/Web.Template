﻿using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace System
{
    /// <summary>
    /// 类型扩展
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 类型的默认值缓存
        /// </summary>
        private static readonly ConcurrentCache<Type, object> typeDefaultValueCache = new ConcurrentCache<Type, object>();


        /// <summary>
        /// 返回类型的默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DefaultValue(this Type type)
        {
            if (type == null)
            {
                return null;
            }

            return typeDefaultValueCache.GetOrAdd(type, t =>
            {
                var value = Expression.Convert(Expression.Default(t), typeof(object));
                return Expression.Lambda<Func<object>>(value).Compile().Invoke();
            });
        }


        /// <summary>
        /// 是否可以从TBase类型派生
        /// </summary>
        /// <typeparam name="TBase"></typeparam>
        /// <param name="type"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static bool IsInheritFrom<TBase>(this Type type)
        {
            return typeof(TBase).IsAssignableFrom(type);
        }

        /// <summary>
        /// 返回类型的非可空类型
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Type NonNullableType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        /// <summary>
        /// 值是可以为null      
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static bool CanBeNullValue(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsValueType == false)
            {
                return true;
            }

            return Nullable.GetUnderlyingType(type) != null;
        }
    }
}