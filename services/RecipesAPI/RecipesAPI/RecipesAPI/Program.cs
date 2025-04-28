using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

await SeedDataAsync(app.Services);
app.Run();


async Task SeedDataAsync(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {

        var seeder = new RecipeDatabaseSeeder(
            scope.ServiceProvider.GetRequiredService(typeof(ILogger<RecipeDatabaseSeeder>)) as ILogger<RecipeDatabaseSeeder>,
            scope.ServiceProvider.GetRequiredService(typeof(RecipeDbContext)) as RecipeDbContext);
        await seeder.Seed();
    }

}