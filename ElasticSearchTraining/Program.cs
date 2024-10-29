using ElasticSearchTraining.ElasticConfiguration;
using ElasticSearchTraining.ElasticService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddElasticClientService(builder.Configuration);
builder.Services.AddScoped<IElasticSearchService, ElasticSearchService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
