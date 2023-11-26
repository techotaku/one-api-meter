using OneAPI.Meter.Models;
using System.Text;

namespace OneAPI.Meter.Email.Models
{
    public class EmailReport
    {
        #region Email Template
        private const string EMAIL_TEMPLATE = """
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
        <title>One API Usage Report</title>
    </head>
    <body>
        <table border="0" cellpadding="0" cellspacing="0" height="100%" width="100%" id="bodyTable">
            <tr>
                <td align="center" valign="top">
                    <table border="0" cellpadding="20" cellspacing="0" width="600" id="emailContainer">
                        <tr>
                            <td align="center" valign="top">
                                <table border="0" cellpadding="20" cellspacing="0" width="100%" id="emailHeader">
                                    <tr>
                                        <td align="center" valign="top">
                                            <h2>One API Usage Report</h2>
                                            <i>If it's the first day of each month, <br />the previous month usage displays.</i><br />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" valign="top">
                                <table border="0" cellpadding="20" cellspacing="0" width="100%" id="emailBody">
                                    <tr>
                                        <td align="center" valign="top">
%BODY%
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" valign="top">
                                <table border="0" cellpadding="20" cellspacing="0" width="100%" id="emailFooter">
                                    <tr>
                                        <td align="center" valign="top">
%FOOTER%
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </body>
</html>
""";
        #endregion

        private readonly List<StatisticsReport> reports = [];

        public void AddReport(StatisticsReport report) 
        {
            reports.Add(report);
        }

        public string ToHtml()
        {
            var builder = new StringBuilder();

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
                        builder.AppendLine($"  <tr>\r\n    <td>&nbsp;{stat.Key}&nbsp;</td>\r\n    <td>&nbsp;${stat.Value}&nbsp;</td>\r\n  </tr>");
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

            var body = builder.ToString();
            return EMAIL_TEMPLATE.Replace("%FOOTER%", $"Reported on {DateTimeOffset.UtcNow:yyyy, MMM, dd}").Replace("%BODY%", body);
        }
    }
}
