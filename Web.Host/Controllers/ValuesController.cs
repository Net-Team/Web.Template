using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Application;
using Application.Baidu;
using Web.Core.Controllers;

namespace Web.Host.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : CoreController
    {
        // GET api/values
        [HttpGet]
        public  async Task <IEnumerable<string>> Get([FromServices] BaiduService baidu, [FromServices] IBaiduApi baiduApi)
        {
            var sum = baidu.Sum(1, 3);
            var html = await baiduApi.GetAsync();
            return new string[] { sum.ToString() };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<string> Get(int id)
        {
            this.Request.Query
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
