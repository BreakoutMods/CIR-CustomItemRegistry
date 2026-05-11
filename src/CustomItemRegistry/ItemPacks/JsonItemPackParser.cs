using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ValheimCustomItemRegistry
{
    internal sealed class JsonItemPackParser : IItemPackParser
    {
        private const string AssemblyName = "Newtonsoft.Json";

        public string FormatName => "JSON";
        public string MissingDependencyMessage => "JSON item packs require ValheimModding-JsonDotNET (Newtonsoft.Json).";
        public bool IsAvailable => GetJsonConvertType() != null;

        public bool CanParse(string filePath)
        {
            return string.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase);
        }

        public ItemPackDto Parse(string text)
        {
            if (!IsAvailable)
            {
                throw new InvalidOperationException(MissingDependencyMessage);
            }

            Type jsonConvertType = GetJsonConvertType();
            MethodInfo method = jsonConvertType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(candidate =>
                    candidate.Name == "DeserializeObject"
                    && !candidate.IsGenericMethod
                    && candidate.GetParameters().Length == 2
                    && candidate.GetParameters()[0].ParameterType == typeof(string)
                    && candidate.GetParameters()[1].ParameterType == typeof(Type));

            return (ItemPackDto)method.Invoke(null, new object[] { text, typeof(ItemPackDto) });
        }

        private static Type GetJsonConvertType()
        {
            return FindAssembly(AssemblyName)?.GetType("Newtonsoft.Json.JsonConvert");
        }

        private static Assembly FindAssembly(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(candidate => candidate.GetName().Name == assemblyName);
        }
    }
}
