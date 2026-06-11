
namespace P1Simulator.Telegrams
{
    /// <summary>
    /// Manages DSMR templates stored in DsmrTemplates.cs.
    /// </summary>
    public class TemplateManager
    {
        private readonly Dictionary<string, string> _templates =
            new Dictionary<string, string>();

        public TemplateManager()
        {
            // Register all templates from DsmrTemplates.cs
            Register("basic", DsmrTemplates.Basic);
            Register("1phase", DsmrTemplates.ElectricitySinglePhase);
            Register("3phase", DsmrTemplates.ElectricityThreePhase);
            Register("gas", DsmrTemplates.Gas);
            Register("capacity", DsmrTemplates.CapacityTariff);
            Register("minimal", DsmrTemplates.Minimal);
            Register("fulldsmr", DsmrTemplates.FullDsmr);
            Register("water", DsmrTemplates.Water);
            Register("heat", DsmrTemplates.Heat);
            Register("fulldsmrextended", DsmrTemplates.FullDsmrExtended);
        }

        public void Register(string name, string template)
        {
            _templates[name.ToLower()] = template;
        }

        public string? Get(string name)
        {
            return _templates.TryGetValue(name.ToLower(), out var t) ? t : null;
        }

        public IEnumerable<string> List()
        {
            return _templates.Keys;
        }
    }
}
