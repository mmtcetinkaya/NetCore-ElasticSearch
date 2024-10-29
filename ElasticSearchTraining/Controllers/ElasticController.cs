using ElasticSearchTraining.ElasticConfiguration;
using ElasticSearchTraining.ElasticService;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchTraining.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ElasticController : ControllerBase
    {
        private readonly ILogger<ElasticController> _logger;
        private readonly IElasticSearchService _elasticSearchService;

        public ElasticController(ILogger<ElasticController> logger, IElasticSearchService elasticSearchService)
        {
            _elasticSearchService = elasticSearchService;
            _logger = logger;
        }

        [HttpPost]
        [Route("post")]
        public async void PostDocument()
        {
            var product = new Product
            {
                Name = "phone",
                Barcode = "322222",
                Description = "sogulmada gelecek ürün bilgisi ürün",
                Id = Guid.NewGuid(),
                Price = Convert.ToDecimal(45000.52)
            };

            await _elasticSearchService.ChekIndex("deneme");
            await _elasticSearchService.InsertDocuments("deneme", new List<Product> { product });
            _logger.Log(Microsoft.Extensions.Logging.LogLevel.Warning, "product added");
        }
    }
}
