using System.Collections.Generic;

namespace P1Simulator.Telegrams
{
    /// <summary>
    /// Manages all DSMR telegram templates and provides lookup by name.
    /// </summary>
    public class TemplateManager
    {
        private readonly Dictionary<string, TemplateBase> _templates =
            new Dictionary<string, TemplateBase>();

        public TemplateManager()
        {
            // Register built‑in templates
            Register(new Template1Phase());
            Register(new Template3Phase());
            Register(new TemplateGas());
        }

        /// <summary>
        /// Registers a template by its unique Name.
        /// </summary>
        public void Register(TemplateBase template)
        {
            _templates[template.Name] = template;
        }

        /// <summary>
        /// Retrieves a template by name (e.g. "1phase", "3phase", "gas").
        /// Returns null if not found.
        /// </summary>
        public TemplateBase? Get(string name)
        {
            return _templates.TryGetValue(name, out var t) ? t : null;
        }

        /// <summary>
        /// Returns all registered template names.
        /// </summary>
        public IEnumerable<string> List()
        {
            return _templates.Keys;
        }
    }
}
