using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneAPI.Meter.Clients;
using OneAPI.Meter.Options;
using OneAPI.Meter.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<OneApiMeterService>();
builder.Services.Configure<OneApiOptions>(builder.Configuration.GetSection(OneApiOptions.OneApi));
builder.Services.AddHttpClient<OneApiClient>();
var host = builder.Build();

await host.StartAsync();

var service = host.Services.GetRequiredService<OneApiMeterService>();
await service.RunAsync();

await host.StopAsync();