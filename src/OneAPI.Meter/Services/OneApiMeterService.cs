using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneAPI.Meter.Clients;
using OneAPI.Meter.Email.Models;
using OneAPI.Meter.Email.Services;
using OneAPI.Meter.Models;
using OneAPI.Meter.Options;

namespace OneAPI.Meter.Services
{
    public sealed class OneApiMeterService(OneApiClient client, IOptions<OneApiOptions> options, EmailSender sender, ILogger<OneApiMeterService> logger)
    {
        private const double QuotaFactor = 500000d;

        private readonly OneApiClient _client = client ?? throw new ArgumentNullException(nameof(client));
        private readonly StatisticsGroup[] _statisticsGroups = options?.Value?.Statistics ?? throw new ArgumentNullException(nameof(options));
        private readonly EmailSender _sender = sender ?? throw new ArgumentNullException(nameof(options));
        private readonly ILogger<OneApiMeterService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        private static void SetUserUsage(string user, long quota, Dictionary<string, double> collection, Dictionary<string, string> mapping)
        {
            var usage = quota / QuotaFactor;

            var target = user;
            if (mapping.TryGetValue(user, out string? value))
            {
                target = value;
            }

            if (!collection.ContainsKey(target))
            {
                collection[target] = usage;
            }
            else
            {
                collection[target] += usage;
            }
        }

        public async Task RunAsync()
        {
            if (_statisticsGroups.Length <= 0)
            {
                _logger.LogWarning("Statistics groups not defined. Please check application configuration.");
                return;
            }

            var users = await _client.GetUsersAsync();
            if (users == null || users.Count <= 0)
            {
                _logger.LogWarning("Unable to get users. Please check application configuration.");
                return;
            }
            _logger.LogInformation("{count} user(s) fetchd.", users.Count);

            var emailReport = new EmailReport();
            foreach (var group in _statisticsGroups) 
            {
                var mapping = new Dictionary<string, string>();
                foreach (var m in group.Mapping)
                {
                    var user = users.FirstOrDefault(u => u.Username == m.Key);
                    var targetUser = users.FirstOrDefault(u => u.Username == m.Value);
                    if (user != null && !string.IsNullOrEmpty(user.DisplayName) && 
                        targetUser != null && !string.IsNullOrEmpty(targetUser.DisplayName))
                    {
                        mapping[user.DisplayName] = targetUser.DisplayName;
                    }
                }

                var report = new StatisticsReport 
                { 
                    Name = group.Name,
                    Usage = (await _client.GetMonthlyStatAsync(group.Channels)).Quota / QuotaFactor
                };
                _logger.LogInformation("Monthly total usage for \"{group}\" obtained.", group.Name);

                if (group.MonthlyUsagePerUser)
                {
                    foreach(var user in users)
                    {
                        var stat = await _client.GetUserMonthlyStatAsync(user.Username, group.Channels);
                        SetUserUsage(user.DisplayName, stat.Quota, report.Month, mapping);
                    }
                    _logger.LogInformation("Monthly usage per user for \"{group}\" obtained.", group.Name);
                }

                if (group.LastDayUsagePerUser)
                {
                    foreach (var user in users)
                    {
                        var stat = await _client.GetLastDayStatAsync(user.Username, group.Channels);
                        SetUserUsage(user.DisplayName, stat.Quota, report.Day, mapping);
                    }
                    _logger.LogInformation("Last day usage per user for \"{group}\" obtained.", group.Name);
                }

                emailReport.AddReport(report);
            }

           await _sender.SendAsync(emailReport);
        }
    }
}
