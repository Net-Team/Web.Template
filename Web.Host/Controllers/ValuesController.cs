using Application.Baidus;
using Application.UserCenters;
using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Core.Controllers;

namespace Web.Host.Controllers
{
    /// <summary>
    /// 演示控制器
    /// </summary>
    [Route("api/[controller]")]
    public class ValuesController : ApiController
    {
        // GET api/values
        [HttpGet]
        public async Task<ApiResult<IEnumerable<string>>> Get([FromServices] BaiduService baidu, [FromServices] IUserApi userApi)
        {
            var where = this.GetQueryPredicate<Baidu>();
            var sum = baidu.Sum(1, 3);
            return new string[] { sum.ToString() };
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
        public void Post([FromBody] Baidu value)
        {
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
