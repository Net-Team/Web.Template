using Application.Baidus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Core.Controllers;
using Web.Core.FilterAttributes;

namespace Web.Host.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [HttpGet]
        [MenuItem(Name.房间结构, Group.基础数据)]
        public async Task<IEnumerable<string>> Get([FromServices] BaiduService baidu, [FromServices] IBaiduApi baiduApi)
        {
            var where = this.GetQueryPredicate<Baidu>();
            var sum = baidu.Sum(1, 3);
            var html = await baiduApi.GetAsync();
            return new string[] { sum.ToString() };
        }

        /// <summary>
        /// 开锁日志
        /// </summary>
        /// <param name="id">开锁id</param>
        /// <param name="apiDescription"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MenuItem(Name.开锁日志, Group.日志管理, Class = "open")]
        public ActionResult<string> Get(int id, [FromServices]IApiDescriptionGroupCollectionProvider apiDescription)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        [MenuItem(Name.报警日志, Group.日志管理)]
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
