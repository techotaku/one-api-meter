using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneAPI.Meter.Clients;
using OneAPI.Meter.Options;

namespace OneAPI.Meter.Services
{
    public sealed class OneApiMeterService(OneApiClient client, IOptions<OneApiOptions> options, ILogger<OneApiMeterService> logger)
    {
        private readonly OneApiClient _client = client ?? throw new ArgumentNullException(nameof(client));
        private readonly StatisticsGroup[] _statisticsGroups = options?.Value?.Statistics ?? throw new ArgumentNullException(nameof(options));
        private readonly ILogger<OneApiMeterService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task RunAsync()
        {
            foreach (var group in _statisticsGroups) 
            {
                var stat = await _client.GetMonthlyStatAsync(group.Channels);
                var usage = stat.Quota / 500000d;

                _logger.LogInformation($"{group.Name}\t${usage}");
            }
        }
    }
}
