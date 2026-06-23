using System.Text.RegularExpressions;

namespace P1Simulator.Telegrams
{
    public static class TimestampExtractor
    {
        private static readonly Regex _regex =
            new Regex(@"0-0:1\.0\.0\((\d{12}[SW])\)", RegexOptions.Compiled);

        public static string? Extract(string telegram)
        {
            var m = _regex.Match(telegram);
            return m.Success ? m.Groups[1].Value : null;
        }
    }
}
