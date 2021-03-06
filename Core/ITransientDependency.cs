﻿using Microsoft.Extensions.DependencyInjection;

namespace Core
{
    /// <summary>
    /// 定义Transient模式的对象注入接口
    /// </summary>
    [Register(ServiceLifetime.Transient)]
    public interface ITransientDependency
    {
    }
}
