using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Core.Web
{
    /// <summary>
    /// Api事件过滤器
    /// 使用IApiEventPublisher发布将请求Api的内容
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ApiEventFilterAttribute : Attribute, IAsyncActionFilter, IOrderedFilter
    {
        /// <summary>
        /// 获取或设置是否启用
        /// 默认为true
        /// </summary>
        public bool Enbale { get; set; } = true;

        /// <summary>
        /// 获取或设置排序顺序
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// Action执行
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next.Invoke();

            if (this.Enbale == true)
            {
                await this.OnActionExecutedAsync(context);
            }
        }

        /// <summary>
        /// Action执行后
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task OnActionExecutedAsync(ActionExecutingContext context)
        {
            var publisher = context.HttpContext.RequestServices.GetService<IApiEventPublisher>();
            if (publisher == null)
            {
                return;
            }

            var request = context.HttpContext.Request;
            var apiEvent = new ApiEvent
            {
                Uri = new Uri(request.Path + request.QueryString, UriKind.Relative),
                UserId = this.GetUserId(context.HttpContext.User),
                CreateTime = DateTime.Now
            };

            var api = context.GetApiDescription();
            var contentParameter = api.ParameterDescriptions
                .Where(item => item.Source.Id.EqualsIgnoreCase("BODY") || item.Source.Id.EqualsIgnoreCase("FORM"))
                .FirstOrDefault();

            if (contentParameter != null && context.ActionArguments.TryGetValue(contentParameter.Name, out var content) == true)
            {
                apiEvent.Content = content;
            }

            await publisher.PulishAsync(api.RelativePath, apiEvent);
        }


        /// <summary>
        /// 返回用户信息Id
        /// </summary>
        /// <param name="user"></param> 
        /// <returns></returns>
        protected virtual string GetUserId(ClaimsPrincipal user)
        {
            return user.FindFirstValue("sub");
        }
    }
}