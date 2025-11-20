using Microsoft.EntityFrameworkCore;
using Serilog;
using YazOkulu.Application;
using YazOkulu.Infrastructure;
using YazOkulu.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Serilog yapılandırması
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/yazokulu-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Yaz Okulu API", Version = "v1" });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Application & Infrastructure Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Database migration ve seeding (automatic)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        // Otomatik migration - veritabanı yoksa oluşturur
        Log.Information("Veritabanı migration başlatılıyor...");
        dbContext.Database.Migrate();
        Log.Information("Veritabanı migration tamamlandı");

        // Seed data (development only)
        if (app.Environment.IsDevelopment())
        {
            DbSeeder.SeedData(dbContext);
            Log.Information("Database seeding tamamlandı");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Veritabanı migration/seeding sırasında hata oluştu");
        throw;
    }
}

Log.Information("Yaz Okulu API başlatıldı");

app.Run();
