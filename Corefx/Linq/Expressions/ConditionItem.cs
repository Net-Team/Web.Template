using System.Reflection;

namespace System.Linq.Expressions
{
    /// <summary>
    /// 表示条件项
    /// </summary>
    public class ConditionItem
    {
        /// <summary>
        /// 获取或设置属性名称
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 获取或设置条件值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 获取或设置比较操作符
        /// </summary>
        public Operator? Operator { get; set; }

        /// <summary>
        /// 转换为泛型的ConditionItem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public ConditionItem<T> AsGeneric<T>()
        {
            var member = ConditionItem<T>.TypeProperties
                .FirstOrDefault(item => item.Name.Equals(this.MemberName, StringComparison.OrdinalIgnoreCase));

            return member == null ? null : new ConditionItem<T>(member, this.Value, this.Operator);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.MemberName} {this.Operator} {this.Value}";
        }
    }

    /// <summary>
    /// 表示条件项
    /// </summary>
    public class ConditionItem<T>
    {
        /// <summary>
        /// 获取T类型的所有属性
        /// </summary>
        public static PropertyInfo[] TypeProperties { get; } = typeof(T).GetProperties();

        /// <summary>
        /// 获取属性
        /// </summary>
        public PropertyInfo Member { get; }

        /// <summary>
        /// 获取条件值
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 获取或设置比较操作符
        /// </summary>
        public Operator Operator { get; set; }

        /// <summary>
        /// 条件项
        /// </summary>
        /// <param name="member">属性</param>
        /// <param name="value">条件值</param>
        /// <param name="operator">比较操作符</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public ConditionItem(PropertyInfo member, object value, Operator? @operator)
        {
            this.Member = member ?? throw new ArgumentNullException(nameof(member));
            this.Value = Converter.ConvertToType(value, member.PropertyType);
            if (@operator == null)
            {
                this.Operator = member.PropertyType == typeof(string) ? Operator.Contains : Operator.Equal;
            }
            else
            {
                this.Operator = @operator.Value;
            }
        }


        /// <summary>
        /// 转换为谓词筛选表达式
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public Expression<Func<T, bool>> ToPredicate()
        {
            return Predicate.Create<T>(this.Member, this.Value, this.Operator);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToPredicate().ToString();
        }
    }
}
