namespace OneAPI.Meter.Options
{
    public class OneApiOptions
    {
        public const string OneApi = "OneApi";

        public Server Server { get; set; } = new();

        public StatisticsGroup[] Statistics { get; set; } = [];
    }
}
