using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemory.Caching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;

        public ValuesController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet("Set")]
        public void Set(string name)
        {
            _memoryCache.Set("name", name);
        }

        [HttpGet("get")]   
        public string Get()
        {
            if(_memoryCache.TryGetValue<string>("name", out string name))
            {
                return name.Substring(2);
            }
            return "";
        }

        [HttpGet("SetDate")]
        public void SetDate(string name)
        {
            _memoryCache.Set("date", DateTime.Now, options: new()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                SlidingExpiration = TimeSpan.FromSeconds(5)
            });
        }

        [HttpGet("GetDate")]   
        public DateTime GetDate()
        {
            return _memoryCache.Get<DateTime>("date");
        }
    }
}
