using System;

namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Exception raised when a custom item definition cannot be loaded, validated, or registered.
    /// </summary>
    public sealed class CustomItemRegistrationException : Exception
    {
        public string ItemName { get; private set; }
        public string AssetBundlePath { get; private set; }
        public string PrefabName { get; private set; }

        public CustomItemRegistrationException(string message)
            : base(message)
        {
        }

        public CustomItemRegistrationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal CustomItemRegistrationException(CustomItemDefinition definition, string message)
            : base(BuildMessage(definition, message))
        {
            Capture(definition);
        }

        internal CustomItemRegistrationException(CustomItemDefinition definition, string message, Exception innerException)
            : base(BuildMessage(definition, message), innerException)
        {
            Capture(definition);
        }

        private void Capture(CustomItemDefinition definition)
        {
            if (definition == null)
            {
                return;
            }

            ItemName = definition.ItemName;
            AssetBundlePath = definition.AssetBundlePath;
            PrefabName = definition.PrefabName;
        }

        private static string BuildMessage(CustomItemDefinition definition, string message)
        {
            if (definition == null)
            {
                return message;
            }

            return $"Item '{definition.ItemName ?? "<null>"}' from bundle '{definition.AssetBundlePath ?? "<null>"}' prefab '{definition.PrefabName ?? "<null>"}': {message}";
        }
    }
}
