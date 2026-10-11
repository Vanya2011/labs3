using LabProject.Application.Configuration;
using LabProject.WebApi.Extentions;
using LabProject.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<PricingOptions>(
    builder.Configuration.GetSection(PricingOptions.SectionName));
builder.Services.ConfigureCors();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
}); 
builder.Services.ConfigureIISIntegration();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "LabProject_";
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.MapControllers();
app.Run();