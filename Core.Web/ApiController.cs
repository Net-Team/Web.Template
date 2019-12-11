using Microsoft.AspNetCore.Mvc;
using PredicateLib;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Web
{
    /// <summary>
    /// 表示Api控制器
    /// 提供查询条件表达式的获取
    /// </summary>
    [ApiController]
    [ApiExceptionFilter]
    [Route("api/[service]/[controller]")]
    public abstract class ApiController : ControllerBase
    {
        /// <summary>
        /// 从请求Query获取查询表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trueWhenNull"></param>
        /// <returns></returns>
        protected Expression<Func<T, bool>> GetQueryPredicate<T>(bool trueWhenNull = true)
        {
            return this.GetQueryCondition<T>().ToAndPredicate(trueWhenNull);
        }

        /// <summary>
        /// 从请求Query获取查询条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual Condition<T> GetQueryCondition<T>()
        {
            var items = this.GetQueryConditionItems<T>();
            return new Condition<T>(items);
        }

        /// <summary>
        /// 从Query获取条件项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private IEnumerable<ConditionItem<T>> GetQueryConditionItems<T>()
        {
            bool TryGetTrimValue(PropertyInfo property, string prefx, out string value)
            {
                if (this.Request.Query.TryGetValue($"{prefx}{property.Name}", out var stringValues))
                {
                    value = stringValues.Count == 0 ? null : stringValues[0].NullThenEmpty().Trim();
                    return value.IsNullOrEmpty() == false;
                }

                value = null;
                return false;
            }

            foreach (var property in ConditionItem<T>.TypeProperties)
            {
                if (TryGetTrimValue(property, null, out var value))
                {
                    yield return new ConditionItem<T>(property, value, null);
                }

                if (property.PropertyType.IsValueType)
                {
                    if (TryGetTrimValue(property, "min", out var minValue))
                    {
                        yield return new ConditionItem<T>(property, minValue, Operator.GreaterThanOrEqual);
                    }
                    if (TryGetTrimValue(property, "max", out var maxValue))
                    {
                        yield return new ConditionItem<T>(property, maxValue, Operator.LessThanOrEqual);
                    }
                }
            }
        }
    }
}
