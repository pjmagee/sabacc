using Blazored.LocalStorage;
using Blazored.SessionStorage;

using Microsoft.AspNetCore.ResponseCompression;

using Sabacc.Domain;
using Sabacc.Hubs;
using Sabacc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services
    .AddServerSideBlazor(options =>
    {
        options.DetailedErrors = true;
    })
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

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
});
builder.Services.AddScoped<BrowserStorage>();

builder.Services.AddSingleton<SabaccSessionFactory>();
builder.Services.AddSingleton<SabaccSessionService>();
builder.Services.AddSingleton(provider => provider);
builder.Services.AddTransient<ClassicSabaccCloudCityRules>();
builder.Services.AddTransient<CorellianSpikeBlackSpireOutpostRules>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapHub<PlayerNotificationHub>("/update");
app.MapFallbackToPage("/_Host");
app.Run();
