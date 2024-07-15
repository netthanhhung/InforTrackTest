using InforTrack_SEOWeb.Common.Configs;
using InfoTrack_SEOWeb.Handler.Infrastructure;
using InfoTrack_SEOWeb.Providers.Scrapper;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IBroker, Broker>();
builder.Services.AddScoped<IScrapperProvider, ScrapperProvider>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Broker).Assembly);
});

builder.Services.Configure<SEOConfig>(
    builder.Configuration.GetSection("SEOConfig"));

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
