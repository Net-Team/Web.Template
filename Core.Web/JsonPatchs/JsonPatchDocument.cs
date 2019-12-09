using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Web.JsonPatchs
{
    /// <summary>
    /// 表示JsonPatch文档
    /// </summary>
    public class JsonPatchDocument : IEnumerable<JsonPatchOperation>
    {           
        /// <summary>
        /// 操作项
        /// </summary>
        private readonly JsonPatchOperation[] operations;

        /// <summary>
        /// JsonPatch文档
        /// </summary>
        /// <param name="operations"></param>
        public JsonPatchDocument(JsonPatchOperation[] operations)
        {
            this.operations = operations;                  
        }

        /// <summary>
        /// 应用所有选项到目标
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">目标</param>
        /// <exception cref="NotSupportedException"></exception>
        public void ApplyTo<T>(T target)
        {
            foreach (var op in this)
            {
                op.ApplyTo(target);
            }
        }

        /// <summary>
        /// 返回操作项迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<JsonPatchOperation> GetEnumerator()
        {
            foreach (var item in this.operations)
            {
                yield return item;
            }
        }

        /// <summary>
        /// 返回操作项迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        } 
    }
}