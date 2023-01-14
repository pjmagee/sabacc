using Blazored.LocalStorage;
using Blazored.SessionStorage;

using Microsoft.AspNetCore.ResponseCompression;

using Sabacc.Domain;
using Sabacc.Hubs;
using Sabacc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services
    .AddServerSideBlazor(options => { options.DetailedErrors = true; })
    .AddHubOptions(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
        options.EnableDetailedErrors = false;
        options.HandshakeTimeout = TimeSpan.FromSeconds(15);
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.MaximumParallelInvocationsPerClient = 1;
        options.MaximumReceiveMessageSize = 32 * 1024;
        options.StreamBufferCapacity = 10;
    });

builder.Services
    .AddBlazoredLocalStorage()
    .AddBlazoredSessionStorage()
    .AddResponseCompression(options =>
    {
        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
    });

builder.Services
    .AddScoped<BrowserStorage>()
    .AddSingleton<SabaccSessionFactory>()
    .AddSingleton<SabaccSessionService>()
    .AddSingleton(provider => provider)
    .AddTransient<ClassicSabaccCloudCityRules>()
    .AddTransient<IWinnerCalculator, WinnerCalculator>()
    .AddTransient<CorellianSpikeBlackSpireOutpostRules>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // app.UseHsts();
}

// app.UseHttpsRedirection();
app
    .UseStaticFiles()
    .UseRouting();

app.MapBlazorHub();
app.MapHub<PlayerNotificationHub>("/update");
app.MapFallbackToPage("/_Host");

app.Run();
