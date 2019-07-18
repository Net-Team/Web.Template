using Application.Baidus;
using Core.Menus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Core.Controllers;

namespace Web.Host.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ApiController
    {
        // GET api/values
        [HttpGet]
        [MenuItem("测试", GroupName = "系统")]
        public async Task<IEnumerable<string>> Get([FromServices] BaiduService baidu, [FromServices] IBaiduApi baiduApi)
        {
            var where = this.GetQueryPredicate<Baidu>();
            var sum = baidu.Sum(1, 3);
            var html = await baiduApi.GetAsync();
            return new string[] { sum.ToString() };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id, [FromServices]IApiDescriptionGroupCollectionProvider apiDescription)
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
