using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using VideoApi.Data;
using VideoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the DI container.
builder.Services.AddControllers()
    // Use NewtonsoftJson for better collection/reference handling.
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Configure a simple SQLite database.  You can swap this out for any other provider via configuration.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Increase the default multipart body length limit to 100MB (100 * 1024 * 1024 bytes)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100MB
});

// Register application services
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Add Swagger/OpenAPI support.  AddEndpointsApiExplorer registers minimal API endpoints
// with the API explorer while AddSwaggerGen generates the OpenAPI specification and UI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS to allow the Angular client to communicate with this API.
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "*" };
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply any pending migrations on startup.  This will create the database and schema if they
// do not already exist and update them if the model has changed.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable Swagger middleware.  This will serve the generated OpenAPI document at /swagger/v1/swagger.json
// and the interactive Swagger UI at /swagger.  It's added unconditionally to make the API easier to explore.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Video API v1");
    // Optionally serve the Swagger UI at the root ("/") by setting RoutePrefix to an empty string.
    // c.RoutePrefix = string.Empty;
});
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();