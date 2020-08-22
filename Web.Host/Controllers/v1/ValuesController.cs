using Core;
using Core.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Host.Controllers.v1
{

    [Register(ServiceLifetime.Scoped)]
    public class AService
    {
    }

    /// <summary>
    /// 演示控制器
    /// </summary>
    public class ValuesController : ApiController
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResult<string[]> Get([FromServices] AService a)
        {
            return new string[] { "ok" };
        }

        /// <summary>
        /// 获取列表一条
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ApiResult<string> Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public ApiResult<bool> Post([FromBody] string value)
        {
            return true;
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        public ApiResult<bool> Put(int id, [FromBody] string value)
        {
            return true;
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public ApiResult<bool> Delete(int id)
        {
            throw new Exception();
        }
    }
}
