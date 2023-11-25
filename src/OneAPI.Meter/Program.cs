using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneAPI.Meter.Clients;
using OneAPI.Meter.Email.Options;
using OneAPI.Meter.Email.Services;
using OneAPI.Meter.Options;
using OneAPI.Meter.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<OneApiOptions>(builder.Configuration.GetSection(OneApiOptions.OneApi));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.Email));
builder.Services.AddHttpClient<OneApiClient>();
builder.Services.AddSingleton<EmailSender>();
builder.Services.AddSingleton<OneApiMeterService>();
var host = builder.Build();

await host.StartAsync();

var service = host.Services.GetRequiredService<OneApiMeterService>();
await service.RunAsync();

await host.StopAsync();