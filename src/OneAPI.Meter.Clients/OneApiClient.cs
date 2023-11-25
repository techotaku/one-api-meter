using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneAPI.Meter.Models;
using OneAPI.Meter.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OneAPI.Meter.Clients
{
    public class OneApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OneApiClient> _logger;

        public OneApiClient(HttpClient httpClient, IOptions<OneApiOptions> options, ILogger<OneApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            var serverOptions = options?.Value?.Server ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Set the base address and default headers for the HttpClient
            _httpClient.BaseAddress = new Uri(serverOptions.Url);
            _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(serverOptions.AccessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static (DateTimeOffset, DateTimeOffset) GetLastDay(DateTimeOffset utc)
        {
            var start = new DateTimeOffset(utc.Year, utc.Month, utc.Day, 0, 0, 0, utc.Offset).AddDays(-1);
            var next = start.AddDays(1);

            return (start, next);
        }

        private static (DateTimeOffset, DateTimeOffset) GetMonth(DateTimeOffset utc)
        {
            var start = new DateTimeOffset(utc.Year, utc.Month, 1, 0, 0, 0, utc.Offset);
            if (utc.Day == 1)
            {
                start = start.AddMonths(-1);
            }
            var next = start.AddMonths(1);

            return (start, next);
        }

        public async Task<Stat> GetMonthlyStatAsync(int[] channels)
        {
            var (start, next) = GetMonth(DateTimeOffset.UtcNow);
            return await GetStatAsync(string.Empty, start.ToUnixTimeSeconds(), next.ToUnixTimeSeconds(), channels);
        }

        public async Task<Stat> GetLastDayStatAsync(string username, int[] channels)
        {
            var (start, next) = GetLastDay(DateTimeOffset.UtcNow);
            return await GetStatAsync(username, start.ToUnixTimeSeconds(), next.ToUnixTimeSeconds(), channels);
        }

        public async Task<Stat> GetUserMonthlyStatAsync(string username, int[] channels)
        {
            var (start, next) = GetMonth(DateTimeOffset.UtcNow);
            return await GetStatAsync(username, start.ToUnixTimeSeconds(), next.ToUnixTimeSeconds(), channels);
        }

        public async Task<Stat> GetStatAsync(string username, long startTimestamp, long endTimestamp, int[] channels)
        {
            var stringChannels = string.Join(',', channels);
            try
            {
                // Construct the query string
                var queryString = $"start_timestamp={startTimestamp}&end_timestamp={endTimestamp}&channel={Uri.EscapeDataString(stringChannels)}";
                if (!string.IsNullOrEmpty(username))
                {
                    queryString = $"?username={Uri.EscapeDataString(username)}&" + queryString;
                }
                else
                {
                    queryString = "?" + queryString;
                }

                // Send a GET request
                var response = await _httpClient.GetAsync($"api/log/stat{queryString}");
                response.EnsureSuccessStatusCode();

                // Read the response content and deserialize it
                var result = await response.Content.ReadFromJsonAsync<Response<Stat>>() ?? throw new InvalidOperationException("Failed to deserialize the API response.");
                if (result == null || !result.Success || result.Data == null)
                {
                    return new Stat();
                }
                return result.Data!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching stat for user \"{username}\" (channel {channel}).", username, stringChannels);
                throw;
            }
        }

        public async Task<ICollection<User>> GetUsersAsync(int page)
        {
            try
            {
                // Construct the query string
                var queryString = $"?p={page}";

                // Send a GET request
                var response = await _httpClient.GetAsync($"api/user/{queryString}");
                response.EnsureSuccessStatusCode();

                // Read the response content and deserialize it
                var result = await response.Content.ReadFromJsonAsync<Response<User[]>>() ?? throw new InvalidOperationException("Failed to deserialize the API response.");
                if (result == null || !result.Success || result.Data == null)
                {
                    return Array.Empty<User>();
                }
                return result.Data!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting users (page {page}).", page);
                throw;
            }
        }

        public async Task<ICollection<User>> GetUsersAsync()
        {
            var users = new List<User>();

            var p = 0;
            ICollection<User> result = [];
            do
            {
                result = await GetUsersAsync(p);
                users.AddRange(result);
                p++;
            }
            while (result.Count >= 10);

            return users;
        }
    }
}
