using System.ComponentModel.DataAnnotations;

namespace Core.Paging
{
    /// <summary>
    /// 表示分页内容
    /// </summary>
    /// <typeparam name="T">数据</typeparam>
    public class PageInfo<T>
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
    }
}
