using Serilog;
using Spotify2YT.Components;
using Spotify2YT.Services;
using Spotify2YT.States;
using Spotify2YT.ViewModel;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Registrar en la consola
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Registrar en archivo
    .CreateLogger();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton(typeof(MainStateService<>));
builder.Services.AddSingleton<SpotifyService>();
builder.Services.AddSingleton<YouTubeService>();
builder.Services.AddSingleton<CounterViewModel>();

builder.Logging.ClearProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
