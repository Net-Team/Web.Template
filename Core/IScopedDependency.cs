﻿using Microsoft.Extensions.DependencyInjection;

namespace Core
{
    /// <summary>
    /// 定义IScoped模式的对象注入接口
    /// </summary>
    [Register(ServiceLifetime.Scoped)]
    public interface IScopedDependency
    {
    }
}
