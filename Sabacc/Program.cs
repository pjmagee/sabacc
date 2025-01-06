using Blazored.LocalStorage;
using Blazored.SessionStorage;

using Microsoft.AspNetCore.ResponseCompression;

using Sabacc.Domain;
using Sabacc.Domain.SabaccVariants;
using Sabacc.Hubs;
using Sabacc.Pages;
using Sabacc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR();


builder.Services
    .AddBlazoredLocalStorage()
    .AddBlazoredSessionStorage()
    .AddResponseCompression(options => options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]));

builder.Services
    .AddScoped<BrowserStorage>()
    .AddSingleton<SabaccSessionFactory>()
    .AddSingleton<SessionService>()
    .AddSingleton(provider => provider)
    .AddTransient<ClassicSabaccCloudCityRules>()
    .AddTransient<IWinnerCalculator, WinnerCalculator>()
    .AddTransient<CorellianSpikeBlackSpireOutpostRules>();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseResponseCompression();
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();


app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapBlazorHub(options =>
{
    options.CloseOnAuthenticationExpiration = true;
}).WithOrder(-1);
app.MapHub<PlayerNotificationHub>("/update");

app.Run();
