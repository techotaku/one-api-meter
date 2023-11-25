using OneAPI.Meter.Models;
using System.Text;

namespace OneAPI.Meter.Email.Models
{
    public class EmailReport
    {
        private readonly List<StatisticsReport> reports = [];

        public void AddReport(StatisticsReport report) 
        {
            reports.Add(report);
        }

        public string ToHtml()
        {
            var builder = new StringBuilder();
            builder
                .AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">")
                .AppendLine("<h2>One API Usage Report</h2>")
                .AppendLine("<i>If it's the first day of each month, the previous month usage displays.</i><br /><br />");

            foreach (var report in reports)
            {
                builder
                .AppendLine($"<h4>Statistics Group: {report.Name}</h4>")
                .AppendLine($"Monthly Consumption: <strong>${report.Usage}</strong><br /><br />");

                if (report.Month.Count > 0)
                {
                    builder.AppendLine("<table border=\"3\">\r\n  <tr>\r\n    <th>User</th>\r\n    <th>Monthly Consumption</th>\r\n  </tr>");
                    foreach(var stat in report.Month)
                    {
                        if (stat.Value > 0d)
                        {
                            builder.AppendLine($"  <tr>\r\n    <td>&nbsp;{stat.Key}&nbsp;</td>\r\n    <td>&nbsp;${stat.Value}&nbsp;</td>\r\n  </tr>");
                        }
                    }
                    builder.AppendLine("</table><br />");
                }

                if (report.Day.Count > 0)
                {
                    builder.AppendLine("<table border=\"3\">\r\n  <tr>\r\n    <th>User</th>\r\n    <th>Last Day Consumption</th>\r\n  </tr>");
                    foreach (var stat in report.Day)
                    {
                        if (stat.Value > 0d)
                        {
                            builder.AppendLine($"  <tr>\r\n    <td>&nbsp;{stat.Key}&nbsp;</td>\r\n    <td>&nbsp;${stat.Value}&nbsp;</td>\r\n  </tr>");
                        }
                    }
                    builder.AppendLine("</table><br />");
                }

                builder.AppendLine("<br />");
            }

            return builder.ToString();
        }
    }
}
