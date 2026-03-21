using ImageApi.Data;
using ImageApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Services ────────────────────────────────────────────────────────────────

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Image Processing API — Вариант 3",
        Version = "v1",
        Description = "Модуль обработки и работы с изображениями"
    });

    // Include XML comments if present
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// SQLite database stored next to the executable
var dbPath = Path.Combine(AppContext.BaseDirectory, "images.db");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<ImageService>();

// ── Pipeline ────────────────────────────────────────────────────────────────

var app = builder.Build();

// Apply migrations / create DB automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Image API v1");
    c.RoutePrefix = string.Empty; // Swagger UI at root
});

app.UseAuthorization();
app.MapControllers();

app.Run();
