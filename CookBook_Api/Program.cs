using CookBook_Api.Data;
using CookBook_Api.Interfaces.IRepositories;
using CookBook_Api.Mappings;
using CookBook_Api.Repositories;
using dotenv.net;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000",
                "http://192.168.178.252:3000",
                "http://192.168.178.252:5173"
                )
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

// Add services to the container.
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Db Config
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
DotEnv.Load(options: new DotEnvOptions(envFilePaths: [$".env.{environment}"]));
var connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRING__DEFAULTCONNECTION");

builder.Services.AddDbContext<CookBookContext>(options => 
    options.UseNpgsql(connectionString));

builder.Services.AddAutoMapper(typeof(RecipeProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //try to run it on local network
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<CookBookContext>();
    context.Database.Migrate();
    
    var env = services.GetRequiredService<IHostEnvironment>();
    if (env.IsDevelopment())
        DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowSpecificOrigins");
app.UseAuthorization();

app.MapControllers();

app.Run();
