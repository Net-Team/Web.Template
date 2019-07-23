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
        /// 获取或设置相对Uri
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string RelativePath { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
