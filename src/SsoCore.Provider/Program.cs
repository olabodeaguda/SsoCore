using SsoCore.Infrastructure.Configurations;
using SsoCore.Provider.Configurations;
using SsoCore.Infrastructure.Data;
using SsoCore.Application.Configurations;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddAutoMapper(typeof(InfrastructureProfile), typeof(ApplicationProfile));
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/Accounts/Login", "");
});
builder.Services.AddApplicationConfiguration(configuration);
builder.Services.AddInfrastructure(configuration);
builder.Services.AddIdentityConfiguration();
builder.Services.AddOpenIdConfiguration(configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCors", corsPolicyBuilder => corsPolicyBuilder
    .WithOrigins((configuration.GetValue<string>("AllowedHosts")?.Split(",")) ?? ["*"])
           .AllowAnyOrigin()
                  .AllowAnyMethod()
                         .AllowAnyHeader());
});
var app = builder.Build();

app.UseCors("AllowCors");
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
  
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

await app.RunMigrationAsync();
app.Run();
