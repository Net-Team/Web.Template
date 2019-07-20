using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    /// <summary>
    /// 菜单模型
    /// </summary>
    [Table("Menu")]
    public class Menu : IStringIdable
    {
        /// <summary>
        /// 菜单Id
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置关联的用户id
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserId { get; set; }

        /// <summary>
        /// 或取或设置名称
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置分组名称
        /// </summary>   
        [MaxLength(20)]
        public string GroupName { get; set; }

        /// <summary>
        /// 获取或设置讲求方式 
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string HttpMethod { get; set; }

        /// <summary>
        /// 获取或设置相对Uri
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string RelativePath { get; set; }

        /// <summary>
        /// 获取或设置是否可用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
