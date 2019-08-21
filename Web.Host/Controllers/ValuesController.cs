using Core;
using Core.Web;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Host.Controllers
{
    /// <summary>
    /// 演示控制器
    /// </summary>
    public class ValuesController : ApiController
    {
        // GET api/values
        [HttpGet]
        public async Task<ApiResult<IEnumerable<string>>> Get()
        {
            return new string[] { "ok" };
        }

        /// <summary>
        /// 开锁日志
        /// </summary>
        /// <param name="id">开锁id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ApiResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
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
