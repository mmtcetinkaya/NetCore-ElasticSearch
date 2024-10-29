using Nest;

namespace ElasticSearchTraining.ElasticConfiguration
{
    public static class ElasticClientConfigureService
    {
        public static IServiceCollection AddElasticClientService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IElasticClient>(CreateElasticClient(configuration));
            return services;
        }

        public static ElasticClient CreateElasticClient(IConfiguration configuration)
        {
            ElasticSearchOptions? options = configuration.GetSection(nameof(ElasticSearchOptions)).Get<ElasticSearchOptions>();

            if (options != null)
            {
                var settings = new ConnectionSettings(new Uri(options.Host + ":" + options.Port));

                if (!string.IsNullOrEmpty(options.UserName) && !string.IsNullOrEmpty(options.Password))
                {
                    settings.BasicAuthentication(options.UserName, options.Password);
                }

                return new ElasticClient(settings);
            }

            throw new ArgumentNullException(nameof(ElasticSearchOptions) + " not found on congiguration file");
        }
    }
}
