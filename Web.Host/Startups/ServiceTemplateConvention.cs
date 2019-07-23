using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Web.Host.Startups
{
    /// <summary>
    /// 表示控制器路由模板添加[service]的约定
    /// api/[service]/[controller] => api/service001/[controller]
    /// </summary>
    public class ServiceTemplateConvention : IControllerModelConvention
    {
        /// <summary>
        /// 服务名
        /// </summary>
        private readonly string serviceName;

        /// <summary>
        /// 控制器路由模板添加[service]的约定
        /// </summary>
        /// <param name="serviceName">服务名</param>
        public ServiceTemplateConvention(string serviceName)
        {
            this.serviceName = serviceName;
        }

        /// <summary>
        /// 应用约定
        /// </summary>
        /// <param name="controller"></param>
        public void Apply(ControllerModel controller)
        {
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel == null)
                {
                    continue;
                }

                var route = selector.AttributeRouteModel;
                route.Template = route.Template.Replace("[service]", this.serviceName);
            }
        }
    }
}
