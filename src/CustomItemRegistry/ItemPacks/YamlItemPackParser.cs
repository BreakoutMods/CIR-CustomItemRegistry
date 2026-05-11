using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ValheimCustomItemRegistry
{
    internal sealed class YamlItemPackParser : IItemPackParser
    {
        private const string AssemblyName = "YamlDotNet";

        public string FormatName => "YAML";
        public string MissingDependencyMessage => "YAML item packs require ValheimModding-YamlDotNet.";
        public bool IsAvailable => GetBuilderType() != null;

        public bool CanParse(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            return string.Equals(extension, ".yaml", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".yml", StringComparison.OrdinalIgnoreCase);
        }

        public ItemPackDto Parse(string text)
        {
            if (!IsAvailable)
            {
                throw new InvalidOperationException(MissingDependencyMessage);
            }

            Type builderType = GetBuilderType();
            Assembly assembly = FindAssembly(AssemblyName);
            object builder = Activator.CreateInstance(builderType);
            builder = InvokeIfAvailable(builder, "IgnoreUnmatchedProperties");
            builder = WithCamelCaseNamingConvention(builder, assembly);

            object deserializer = builderType.GetMethod("Build", Type.EmptyTypes).Invoke(builder, null);
            MethodInfo deserialize = FindDeserializeMethod(deserializer.GetType());
            if (deserialize.IsGenericMethodDefinition)
            {
                return (ItemPackDto)deserialize.MakeGenericMethod(typeof(ItemPackDto)).Invoke(deserializer, new object[] { text });
            }

            return (ItemPackDto)deserialize.Invoke(deserializer, new object[] { text, typeof(ItemPackDto) });
        }

        private static object WithCamelCaseNamingConvention(object builder, Assembly assembly)
        {
            Type namingType = assembly?.GetType("YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention");
            object instance = namingType?.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null, null);
            if (instance == null)
            {
                return builder;
            }

            MethodInfo method = builder.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(candidate => candidate.Name == "WithNamingConvention" && candidate.GetParameters().Length == 1);

            return method == null ? builder : method.Invoke(builder, new[] { instance });
        }

        private static object InvokeIfAvailable(object target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, Type.EmptyTypes);
            return method == null ? target : method.Invoke(target, null);
        }

        private static MethodInfo FindDeserializeMethod(Type deserializerType)
        {
            MethodInfo generic = deserializerType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(candidate =>
                    candidate.Name == "Deserialize"
                    && candidate.IsGenericMethodDefinition
                    && candidate.GetParameters().Length == 1
                    && candidate.GetParameters()[0].ParameterType == typeof(string));

            if (generic != null)
            {
                return generic;
            }

            return deserializerType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .First(candidate =>
                    candidate.Name == "Deserialize"
                    && !candidate.IsGenericMethod
                    && candidate.GetParameters().Length == 2
                    && candidate.GetParameters()[0].ParameterType == typeof(string)
                    && candidate.GetParameters()[1].ParameterType == typeof(Type));
        }

        private static Assembly FindAssembly(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(candidate => candidate.GetName().Name == assemblyName);
        }

        private static Type GetBuilderType()
        {
            return FindAssembly(AssemblyName)?.GetType("YamlDotNet.Serialization.DeserializerBuilder");
        }
    }
}
