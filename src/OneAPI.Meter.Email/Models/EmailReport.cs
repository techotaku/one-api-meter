using OneAPI.Meter.Models;
using System.Text;

namespace OneAPI.Meter.Email.Models
{
    public class EmailReport
    {
        #region Email Templates
        private const string EMAIL_REPORT_TEMPLATE = """
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
        <title>Usage Report - %SITE_NAME%</title>
    </head>
    <body>
        <table border="0" cellpadding="0" cellspacing="0" height="100%" width="100%" id="bodyTable">
            <tr>
                <td align="center" valign="top">
                    <table border="0" cellpadding="5" cellspacing="0" width="600" id="emailContainer">
                        <tr>
                            <td align="center" valign="top">
                                <table border="0" cellpadding="5" cellspacing="0" width="100%" id="emailHeader">
                                    <tr>
                                        <td align="center" valign="top">
                                            <h2>Usage Report - %SITE_NAME%</h2>
                                            <i>If it's the first day of each month, <br />the previous month usage displays.</i><br />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" valign="top">
                                <table border="0" cellpadding="5" cellspacing="0" width="100%" id="emailBody">
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
                                <table border="0" cellpadding="5" cellspacing="0" width="100%" id="emailFooter">
                                    <tr>
                                        <td align="center" valign="top">
                                            Reported on %REPORT_DATE% (UTC) - %SITE_NAME%
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

        private const string EMAIL_ALERT_TEMPLATE = """
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
        <title>Usage Report - %SITE_NAME%</title>
    </head>
    <body>
%BODY%
    </body>
</html>
""";
        #endregion

        private readonly List<StatisticsReport> reports = [];

        public void AddReport(StatisticsReport report) 
        {
            reports.Add(report);
        }

        public string ToHtml(string siteName)
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
            return EMAIL_REPORT_TEMPLATE
                .Replace("%SITE_NAME%", siteName)
                .Replace("%REPORT_DATE%", DateTimeOffset.UtcNow.ToString("yyyy, MMM, dd"))
                .Replace("%BODY%", body);
        }
    }
}
