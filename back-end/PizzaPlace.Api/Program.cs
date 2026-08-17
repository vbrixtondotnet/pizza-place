using PizzaPlace.Application;
using PizzaPlace.Infrastructure;
using PizzaPlace.Infrastructure.Persistence.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "PizzaPlace API",
        Version = "v1",
        Description = "RESTful API for Pizza Place sales and catalog data"
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

var csvPathSetting = builder.Configuration["SeedData:CsvPath"] ?? "Data";
var csvDirectory = Path.IsPathRooted(csvPathSetting)
    ? csvPathSetting
    : Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, csvPathSetting));

await DatabaseInitializer.InitializeAsync(app.Services, csvDirectory);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaPlace API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("FrontendDev");
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
