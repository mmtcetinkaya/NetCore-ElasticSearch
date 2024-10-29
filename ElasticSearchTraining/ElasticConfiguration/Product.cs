namespace ElasticSearchTraining.ElasticConfiguration
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public  string Description { get; set; }
        public string DescriptionKeyword { get; set; }
        public string DescriptionWildcard { get; set; }

    }
}
