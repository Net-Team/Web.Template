using Microsoft.AspNetCore.Mvc;
using PredicateLib;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
            foreach (var property in ConditionItem<T>.TypeProperties)
            {
                if (this.Request.Query.TryGetValue(property.Name, out var values) == true)
                {
                    var value = values[0];
                    yield return new ConditionItem<T>(property, value, null);
                }

                if (property.PropertyType.IsValueType == true)
                {
                    if (this.Request.Query.TryGetValue("min" + property.Name, out var minValues) == true)
                    {
                        var value = minValues[0];
                        yield return new ConditionItem<T>(property, value, Operator.GreaterThanOrEqual);
                    }

                    if (this.Request.Query.TryGetValue("max" + property.Name, out var maxValues) == true)
                    {
                        var value = maxValues[0];
                        yield return new ConditionItem<T>(property, value, Operator.LessThanOrEqual);
                    }
                }
            }
        }
    }
}
