using ElasticSearchTraining.ElasticConfiguration;
using Microsoft.Extensions.Options;
using Nest;

namespace ElasticSearchTraining.ElasticService
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly ElasticSearchOptions _elasticSearchOptions;

        public ElasticSearchService(IOptions<ElasticSearchOptions> elasticSearchOptions, IElasticClient elasticClient)
        {
            _elasticSearchOptions = elasticSearchOptions.Value;
            _elasticClient = elasticClient;
        }

        private ElasticClient CreateElasticClient()
        {
            string host = _elasticSearchOptions.Host;
            string port = _elasticSearchOptions.Port;
            string username = _elasticSearchOptions.UserName;
            string password = _elasticSearchOptions.Password;

            var settings = new ConnectionSettings(new Uri(host + ":" + port));

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                settings.BasicAuthentication(username, password);
            }

            return new ElasticClient(settings);
        }

        public async Task ChekIndex(string indexName)
        {
            var any = await _elasticClient.Indices.ExistsAsync(indexName);

            if (any.Exists)
                return;

            var response = await _elasticClient.Indices.CreateAsync(indexName,
                ci => ci.Index(indexName).Map<Product>(m => m.Properties(p => p
                                 .Keyword(k => k.Name(n => n.Id))
                                 .Wildcard(k => k.Name(n => n.Barcode))
                                 .Text(t => t.Name(n => n.Name))
                                 .Text(t => t.Name(n => n.Description))
                                 .Keyword(t => t.Name(n => n.DescriptionKeyword).IgnoreAbove(25))
                                 .Wildcard(t => t.Name(n => n.DescriptionWildcard))
                                 .Number(t => t.Name(n => n.Price))))
                                 .Settings(s => s.NumberOfShards(3).NumberOfReplicas(1))
                    );

            return;
        }

        public async Task InsertDocument(string indexName, Product product)
        {

            var response = await _elasticClient.CreateAsync(product, q => q.Index(indexName));
            if (response.ApiCall?.HttpStatusCode == 409)
            {
               await _elasticClient.UpdateAsync<Product>(response.Id, a => a.Index(indexName).Doc(product));
            }

        }

        public async Task InsertDocuments(string indexName, List<Product> products)
        {
            await _elasticClient.IndexManyAsync(products, index: indexName);
        }

        public async Task<Product> GetDocument(string indexName, int id)
        {
            var response = await _elasticClient.GetAsync<Product>(id, q => q.Index(indexName));

            return response.Source;

        }

        public async Task<List<Product>> GetDocuments(string indexName)
        {
            var response = await _elasticClient.SearchAsync<Product>(q => q.Index(indexName).Scroll("5m"));
            return response.Documents.ToList();
        }
    }
}
