using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypesController : Controller
    {
        private static readonly Dictionary<int, CustomType> types = new Dictionary<int, CustomType>();

        [HttpGet("{id}")]
        public CustomType Get(int id)
        {
            if (!types.ContainsKey(id))
                throw new ArgumentException($"No type with id {id}");
            return types[id];
        }

        [HttpGet("getAll")]
        public List<CustomType> GetAll()
        {
            return types.Values.ToList();
        }

        // POST: TypesController/Create
        [HttpPost("create")]
        public ActionResult Create([FromBody] CustomType type)
        {
            //if (types.ContainsKey(type.Id))
            //    throw new Exception($"Type with id {type.Id} already exists");
            //types[type.Id] = type;
            return Ok();
        }


        // GET: TypesController/Delete/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (!types.ContainsKey(id))
                throw new ArgumentException($"No type with id {id}");
            types.Remove(id);
            return Ok();
        }

    }
}
