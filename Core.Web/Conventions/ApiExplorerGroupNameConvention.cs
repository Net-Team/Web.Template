﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Linq;

namespace Core.Web.Conventions
{
    /// <summary>
    /// 表示ApiExplorer以Controllers下的子文件夹分组约定
    /// </summary>
    public class ApiExplorerGroupNameConvention : IControllerModelConvention
    {
        private readonly string defaultGroupName;

        /// <summary>
        /// ApiExplorer以Controllers下的子文件夹分组约定
        /// </summary>
        /// <param name="defaultGroupName">默认分组名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiExplorerGroupNameConvention(string defaultGroupName = "default.v1")
        {
            this.defaultGroupName = defaultGroupName ?? throw new ArgumentNullException(nameof(defaultGroupName));
        }

        /// <summary>
        /// 应用约定
        /// </summary>
        /// <param name="controller"></param>
        public void Apply(ControllerModel controller)
        {
            controller.ApiExplorer.GroupName = GetGroupName(controller.ControllerType, this.defaultGroupName);
        }

        /// <summary>
        /// 获取控制器分组名称
        /// </summary>
        /// <param name="controllerType">控制器类型</param>
        /// <param name="defaultGroupName">默认分组名称</param>
        /// <returns></returns>
        private static string GetGroupName(Type controllerType, string defaultGroupName)
        {
            var names = controllerType.Namespace.Split("Controllers.");
            var groupName = names.Length > 1 ? names.Last().ToLower() : defaultGroupName;
            return string.Join("_", groupName.Split('.').Select(item => FixIfVersion(item)));
        }

        /// <summary>
        /// 修复版本号的显示
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        private static string FixIfVersion(string segment)
        {
            if (segment.IsMatch(@"v\d_*\d*") == false)
            {
                return segment;
            }

            var version = segment.Replace("_", ".").TrimStart('v');
            return "v" + ApiVersion.Parse(version).ToString("VV");
        }

        /// <summary>
        /// 返回版本号
        /// </summary>
        /// <param name="groupName">分组名称</param>
        /// <returns></returns>
        public static ApiVersion GetApiVersion(string groupName)
        {
            var version = groupName.Split('_').FirstOrDefault(item => item.IsMatch(@"v\d+\.*\d*"));
            return version == null ? ApiVersion.Default : ApiVersion.Parse(version.TrimStart('v'));
        }
    }
}
