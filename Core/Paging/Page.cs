﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Core.Paging
{
    /// <summary>
    /// 表示分页内容
    /// </summary>
    /// <typeparam name="T">数据</typeparam>
    public class Page<T>
    {
        /// <summary>
        /// 页面索引，0开始
        /// </summary>
        [Required]
        public int PageIndex { get; set; }

        /// <summary>
        /// 页面记录大小
        /// </summary>
        [Required]
        public int PageSize { get; set; }

        /// <summary>
        /// 全部记录条数
        /// </summary>
        [Required]
        public int TotalCount { get; set; }

        /// <summary>
        /// 数据集合
        /// </summary>
        [Required]
        public T[] DataArray { get; set; }

        /// <summary>
        /// 将分页数据内容映射为其它类型
        /// </summary>
        /// <typeparam name="TNew"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public Page<TNew> MapAs<TNew>(Func<T, TNew> selector) where TNew : class
        {
            return new Page<TNew>
            {
                PageIndex = this.PageIndex,
                PageSize = this.PageSize,
                TotalCount = this.TotalCount,
                DataArray = this.DataArray.Select(selector).ToArray()
            };
        }
    }
}
