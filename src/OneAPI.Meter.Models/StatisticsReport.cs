namespace OneAPI.Meter.Models
{
    public class StatisticsReport
    {
        public string Name { get; set; } = string.Empty;

        public double Usage { get; set; } = 0d;

        public Dictionary<string, double> Month { get; private set; } = [];

        public Dictionary<string, double> Day { get; private set; } = [];
    }
}