using System;
using System.Linq;
using System.Reflection;

namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Reports which optional item-pack parsers are available at runtime.
    /// </summary>
    public sealed class ItemPackParserStatus
    {
        public bool YamlAvailable { get; private set; }
        public string YamlAssemblyVersion { get; private set; }
        public string YamlAssemblyLocation { get; private set; }
        public bool JsonAvailable { get; private set; }
        public string JsonAssemblyVersion { get; private set; }
        public string JsonAssemblyLocation { get; private set; }

        internal static ItemPackParserStatus Create()
        {
            ItemPackParserStatus status = new ItemPackParserStatus();
            status.Capture("YamlDotNet", true);
            status.Capture("Newtonsoft.Json", false);
            return status;
        }

        private void Capture(string assemblyName, bool yaml)
        {
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(candidate => candidate.GetName().Name == assemblyName);

            if (yaml)
            {
                YamlAvailable = assembly != null;
                YamlAssemblyVersion = assembly?.GetName().Version?.ToString();
                YamlAssemblyLocation = assembly?.Location;
            }
            else
            {
                JsonAvailable = assembly != null;
                JsonAssemblyVersion = assembly?.GetName().Version?.ToString();
                JsonAssemblyLocation = assembly?.Location;
            }
        }
    }
}
