using Kms;
using Kms.Components;
using Kms.Services;
using Kms.Interop.TeamsSDK;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// Configurez Serilog pour écrire dans un fichier
var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "app-logs.txt");
var logger = new LoggerConfiguration()
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(logger);

// Configurez la gestion des exceptions
builder.WebHost.UseSetting(WebHostDefaults.CaptureStartupErrorsKey, "true");
builder.WebHost.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
builder.WebHost.CaptureStartupErrors(true);



builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazorBootstrap();
var config = builder.Configuration.Get<ConfigOptions>();
builder.Services.AddTeamsFx(config.TeamsFx.Authentication);
builder.Services.AddScoped<MicrosoftTeams>();
//Ajoutez pour permettre l'injection dans les composants
builder.Services.AddScoped<GraphClientService>();
builder.Services.Configure<SharePointList>(builder.Configuration.GetSection("SharePointList"));

builder.Services.AddScoped<SharePointList>();

builder.Services.AddControllers();
builder.Services.AddHttpClient("WebClient", client => client.Timeout = TimeSpan.FromSeconds(600));
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);




var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();