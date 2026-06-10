using System.Collections.Generic;

namespace P1Simulator.Telegrams
{
    public static class TemplateProcessor
    {
        public static string ApplyPlaceholders(string template, Dictionary<string, string> values)
        {
            string output = template;

            foreach (var kv in values)
            {
                output = output.Replace(kv.Key, kv.Value);
            }

            return output;
        }
    }
}
