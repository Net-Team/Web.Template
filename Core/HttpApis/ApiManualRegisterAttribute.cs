using System;

namespace Core.HttpApis
{
    /// <summary>
    /// 表示Api接口需要手动注册
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class ApiManualRegisterAttribute : Attribute
    {
    }
}
