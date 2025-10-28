using Microsoft.SemanticKernel;
using _01_startup_app.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure ApiSettings
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection(nameof(ApiSettings)));

var apiSettings = builder.Configuration
    .GetSection(nameof(ApiSettings))
    .Get<ApiSettings>();

if (string.IsNullOrEmpty(apiSettings?.BaseUrl))
{
    throw new InvalidOperationException("BaseUrl must be configured in ApiSettings");
}

if (string.IsNullOrEmpty(apiSettings?.ModelId))
{
    throw new InvalidOperationException("ModelId must be configured in ApiSettings");
}

builder.Services.AddOllamaChatCompletion(
    modelId: apiSettings.ModelId,
    endpoint: new Uri(apiSettings.BaseUrl));

builder.Services.AddTransient(sp => new Kernel(sp));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
