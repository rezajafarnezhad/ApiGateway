using ApiGateway.Model;
using DiscountService.Proto;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpcClient<DiscountServiceProto.DiscountServiceProtoClient>(c => c.Address = new Uri("https://localhost:7276/"));

builder.Services.AddScoped<IDiscountService, ApiGateway.Model.DiscountService>();

var configuration = builder.Configuration;
var webHostEnvironment = builder.Environment;

builder.Configuration.SetBasePath(webHostEnvironment.ContentRootPath)
    .AddJsonFile("Ocelot.json")
    .AddOcelot(webHostEnvironment)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(configuration).AddPolly().AddCacheManager(c =>
{
    c.WithDictionaryHandle();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

});


app.UseOcelot().Wait();
app.Run();
