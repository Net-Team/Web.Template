using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// 枚举扩展
    /// </summary>
    public static partial class EnumExtensions
    {
        /// <summary>
        /// 获取枚举类型所有字段的名称
        /// </summary>
        /// <param name="e">枚举类型</param>
        /// <returns></returns>
        public static string[] GetNames(this Enum e)
        {
            return Enum.GetNames(e.GetType());
        }


        /// <summary>
        /// 获取枚举类型所有字段的值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="e">枚举类型</param>
        /// <returns></returns>
        public static T[] GetValues<T>(this Enum e)
        {
            return Enum.GetValues(e.GetType()).Cast<T>().ToArray();
        }

        /// <summary>
        /// 获取枚举类型所有字段的值
        /// </summary>
        /// <param name="e">枚举类型</param>
        /// <returns></returns>
        public static Enum[] GetValues(this Enum e)
        {
            return e.GetValues<Enum>();
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Enum e) where T : class
        {
            var field = e.GetType().GetField(e.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(T)) as T;
            return attribute;
        }


        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static T[] GetAttributes<T>(this Enum e) where T : class
        {
            var field = e.GetType().GetField(e.ToString());
            var attributes = Attribute.GetCustomAttributes(field, typeof(T)) as T[];
            return attributes;
        }


        /// <summary>
        /// 获取枚举值包含的位域值
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IEnumerable<Enum> GetFlagEnums(this Enum e)
        {
            if (e.GetHashCode() == 0)
            {
                return new Enum[0];
            }
            return e.GetValues().Where(item => e.HasFlag(item));
        }

        /// <summary>
        /// 获取枚举值包含的位域值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetFlagEnums<T>(this Enum e) where T : struct
        {
            return e.GetFlagEnums().Cast<T>();
        }


        /// <summary>
        /// 是否声明特性
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="e">枚举</param>
        /// <returns></returns>
        public static bool IsDefinedAttribute<T>(this Enum e) where T : Attribute
        {
            var field = e.GetType().GetField(e.ToString());
            return Attribute.IsDefined(field, typeof(T));
        }
    }
}
