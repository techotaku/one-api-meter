namespace OneAPI.Meter.Options
{
    public class StatisticsGroup
    {
        public string Name { get; set; } = string.Empty;
        public int[] Channels { get; set; } = [];

        public bool MonthlyUsagePerUser { get; set; } = false;

        public bool LastDayUsagePerUser { get; set; } = false;

        public Dictionary<string, string> Mapping { get; set; } = [];
    }
}
