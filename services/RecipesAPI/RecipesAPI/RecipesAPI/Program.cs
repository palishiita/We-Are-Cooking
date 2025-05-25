using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecipesAPI.Config.Options;
using RecipesAPI.Database;
using RecipesAPI.Services;
using RecipesAPI.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// not implemented yet
builder.Services.AddDbContext<RecipeDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("RecipesDb")));

Console.WriteLine($"Connection string: {builder.Configuration.GetConnectionString("RecipesDb")}");

SetConfigurationOptions(builder.Services);
ConfiguerServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();

void SetConfigurationOptions(IServiceCollection services)
{
    services.Configure<UserInfoServiceOptions>(builder.Configuration);
    services.AddHttpClient<IUserInfoService, UserInfoService>();
}

void ConfiguerServices(IServiceCollection services)
{
    services.AddScoped<IUserInfoService, UserInfoService>();
    services.AddScoped<IRecipeService, RecipeService>();
    services.AddScoped<IIngredientService, IngredientService>();
    services.AddScoped<IUserDataService, UserDataService>();
}