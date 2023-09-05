using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceAccountController : Controller
    {
        [HttpGet("getActiveBundlesForServiceAccount/{serviceAccountId}")]
        public IActionResult getActiveBundlesForServiceAccount(int serviceAccountId)
        {
            var list = new List<ActiveBundle>();

            if (serviceAccountId == 1)
            {

                list.Add(new ActiveBundle { id = 1, name = "Bundle1" });
                list.Add(new ActiveBundle { id = 2, name = "Bundle2" });
                list.Add(new ActiveBundle { id = 3, name = "Bundle3" });
                list.Add(new ActiveBundle { id = 4, name = "Bundle4" });
                list.Add(new ActiveBundle { id = 5, name = "Bundle5" });
                list.Add(new ActiveBundle { id = 6, name = "Bundle6" });
                list.Add(new ActiveBundle { id = 7, name = "Bundle7" });
                return Ok(list);
            }
            if (serviceAccountId == 3)
            {

                list.Add(new ActiveBundle { id = 1, name = "Bundle1" });
                list.Add(new ActiveBundle { id = 2, name = "Bundle2" });
                return Ok(list);
            }

            return NotFound($"There is no servcieAccount with id: {serviceAccountId}");

        }

        [HttpPost("getBundlesWithPrices")]
        public List<BundleWithPrice> getActiveBundlesForServiceAccount([FromBody] List<int> bindleIds)
        {
            var list = new List<BundleWithPrice>();
            foreach (var bindleId in bindleIds) { 
                list.Add(new BundleWithPrice { bundleId = bindleId, name = $"Bundle{bindleId}", amount = 10 + bindleId, startDate = DateTime.Now, endDate = DateTime.Now.AddDays(7) });
            }

            return list;
        }
    }
    public class ActiveBundle
    {
        public int id { get; set; }
        public String name { get; set; }
    }

    public class BundleWithPrice
    {
        public int bundleId { get; set; }
        public String name { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public Decimal amount { get; set; }
    }
}
