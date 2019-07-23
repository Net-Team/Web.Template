namespace Core.Menus
{
    /// <summary>
    /// 表示菜单信息
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// 获取或设置分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 或取或设置名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置类名
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 获取或设置相对Uri
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable { get; set; }
    }
}
