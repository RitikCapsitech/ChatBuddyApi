using ChatbotFAQApi.Models;
using ChatbotFAQApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ MongoDB Settings
builder.Services.Configure<FaqDatabaseSettings>(
    builder.Configuration.GetSection("FaqDatabaseSettings"));

// ✅ Register MongoDB Service
builder.Services.AddSingleton<FaqService>();
builder.Services.AddSingleton<ChatSessionService>();

// ✅ Add Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
builder.Services.AddCors();

// ✅ Swagger UI (Development Only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(p => p
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(_ => true) // allow any origin
    .AllowCredentials()
    .SetPreflightMaxAge(TimeSpan.FromSeconds(600))
    .WithExposedHeaders("Content-Disposition"));

// ✅ Middleware Pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
