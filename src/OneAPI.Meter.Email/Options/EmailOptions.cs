namespace OneAPI.Meter.Email.Options
{
    public class EmailOptions
    {
        public const string Email = "Email";

        public string Host { get; set; } = string.Empty;

        public int Port { get; set; } = 587;

        public string User { get; set; } = string.Empty;

        public string Pwd { get; set; } = string.Empty;

        public string From { get; set; } = string.Empty;

        public string[] To { get; set; } = [];

        public string Subject { get; set; } = "One API Usage Report";

    }
}
