using Application.Qinius;
using Application.TcIots;
using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Core;
using System;

namespace Web.Admin.Controllers
{
    /// <summary>
    /// 演示控制器
    /// </summary>
    [Route("api/[service]/values")]
    public class ValuesController : ApiController
    {
        // GET api/values
        [HttpGet]
        public async Task<ApiResult<IEnumerable<int>>> Get(
            [FromServices] QiniuTokenService qiniuTokenService,
            [FromServices] IIotBasicApi iotApi)
        {
            var test = await iotApi.SysTestAsync("C5190724000002", "test");
            return new[] { 1, 2, 3 };
        }

        /// <summary>
        /// 开锁日志
        /// </summary>
        /// <param name="id">开锁id</param>
        /// <param name="apiDescription"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ApiResult<string> Get(int id, [FromServices]IApiDescriptionGroupCollectionProvider apiDescription)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
            //var tmp = value.AsMap().To<str>
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
