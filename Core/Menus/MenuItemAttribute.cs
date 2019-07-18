using System;

namespace Core.Menus
{
    /// <summary>
    /// 表示菜单项特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class MenuItemAttribute : Attribute
    {
        /// <summary>
        /// 获取菜单项名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取或设置所在分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 菜单项特性
        /// </summary>
        /// <param name="name">菜单项名称</param>
        public MenuItemAttribute(string name)
        {
            this.Name = name;
        }
    }
}
