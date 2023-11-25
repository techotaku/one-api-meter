namespace OneAPI.Meter.Options
{
    public class StatisticsGroup
    {
        public string Name { get; set; } = string.Empty;

        public int[] Channels { get; set; } = [];

        public Dictionary<int, int> Mapping { get; set; } = [];
    }
}
