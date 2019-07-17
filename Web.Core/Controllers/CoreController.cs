using Microsoft.AspNetCore.Mvc;
using PredicateLib;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Web.Core.Controllers
{
    /// <summary>
    /// 表示基础核心控制器
    /// </summary>
    public abstract class CoreController : ControllerBase
    {
        /// <summary>
        /// 从请求Query获取查询表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trueWhenNull"></param>
        /// <returns></returns>
        public Expression<Func<T, bool>> GetQueryPredicate<T>(bool trueWhenNull = true)
        {
            return this.GetQueryCondition<T>().ToAndPredicate(trueWhenNull);
        }

        /// <summary>
        /// 从请求Query获取查询条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Condition<T> GetQueryCondition<T>()
        {
            var items = this.GetConditionItems<T>();
            return new Condition<T>(items);
        }
        /// <summary>
        /// 从Query获取条件项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private IEnumerable<ConditionItem<T>> GetConditionItems<T>()
        {
            foreach (var property in ConditionItem<T>.TypeProperties)
            {
                if (Request.Query.TryGetValue(property.Name, out var values) == true)
                {
                    var value = values[0];
                    yield return new ConditionItem<T>(property, value, null);
                }
            }
        }
    }
}
