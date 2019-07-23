using System.ComponentModel.DataAnnotations;

namespace Core.Menus
{
    /// <summary>
    /// 表示菜单项分组
    /// </summary>
    public class MenuGroup
    {
        /// <summary>
        /// 获取或设置分组名
        /// </summary>
        [Required]
        public string Group { get; set; }

        /// <summary>
        /// 获取或设置菜单项
        /// </summary>
        [Required]
        public MenuItem[] Items { get; set; }
    }
}
