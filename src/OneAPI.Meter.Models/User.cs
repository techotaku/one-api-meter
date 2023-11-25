using System.Text.Json.Serialization;

namespace OneAPI.Meter.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;

        public int Role { get; set; }

        public int Status { get; set; }

        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("github_id")]
        public string GithubId { get; set; } = string.Empty;

        [JsonPropertyName("wechat_id")]
        public string WechatId { get; set; } = string.Empty;

        [JsonPropertyName("verification_code")]
        public string VerificationCode { get; set; } = string.Empty;

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        public int Quota { get; set; }

        [JsonPropertyName("used_quota")]
        public int UsedQuota { get; set; }

        [JsonPropertyName("request_count")]
        public int RequestCount { get; set; }

        public string Group { get; set; } = string.Empty;

        [JsonPropertyName("aff_code")]
        public string AffCode { get; set; } = string.Empty;

        [JsonPropertyName("inviter_id")]
        public int InviterId { get; set; }
    }

}
