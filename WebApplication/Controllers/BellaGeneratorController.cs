using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BellaGeneratorController : ControllerBase
    {
        private readonly ILogger<BellaGeneratorController> _logger;
        private readonly IConfiguration _configuration;

        public BellaGeneratorController(ILogger<BellaGeneratorController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("GenerateService")]
        public async Task<string> GenerateService([FromBody] GenerateServiceRequest request)
        {
            var url = _configuration.GetValue<string>("BellaGeneratorUrl");
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("BellaGeneratorUrl");
            }

            _logger.LogInformation($"Start process GenerateServiceRequest");
            var client = new HttpClient() { BaseAddress = new Uri(url) };
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var contentString = JsonSerializer.Serialize<GenerateServiceRequest>(request, options);
            var content = new StringContent(contentString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/BellaGenerator/GenerateService", content);
            return await response.Content.ReadAsStringAsync();
        }
    }
}