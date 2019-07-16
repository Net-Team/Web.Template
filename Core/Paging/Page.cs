using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Paging
{
    /// <summary>
    /// 分页信息   
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam> 
    public class Page<T> : IEnumerable<T>
    {
        /// <summary>
        /// 当前页面数据
        /// </summary>
        private IEnumerable<T> datas { get; set; }

        /// <summary>
        /// 页面索引
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总记录条数
        /// </summary>
        public int TotalCount { get; set; }


        /// <summary>
        /// 分页信息
        /// </summary>
        /// <param name="datas">分页数据</param>
        /// <param name="totalCount">所有条目</param>
        public Page(IEnumerable<T> datas)
        {
            this.datas = datas;
        }

        /// <summary>
        /// 总记录条目
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("TotalCount={0}", this.TotalCount);
        }

        /// <summary>
        /// 将分页数据内容映射为其它类型
        /// </summary>
        /// <typeparam name="TNew"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public Page<TNew> MapAs<TNew>(Func<T, TNew> selector) where TNew : class
        {
            var models = this.datas.Select(selector).ToArray();
            return new Page<TNew>(models)
            {
                PageIndex = this.PageIndex,
                PageSize = this.PageSize,
                TotalCount = this.TotalCount
            };
        }

        /// <summary>
        /// 转换为PageInfo类型
        /// </summary>
        /// <returns></returns>
        public PageInfo<T> ToPageInfo()
        {
            return new PageInfo<T>
            {
                PageIndex = this.PageIndex,
                PageSize = this.PageSize,
                TotalCount = this.TotalCount,
                DataArray = this.ToArray()
            };
        }

        #region 接口实现
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.datas.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.datas.GetEnumerator();
        }
        #endregion
    }
}
