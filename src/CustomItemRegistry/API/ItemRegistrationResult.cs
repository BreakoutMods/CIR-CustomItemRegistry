using System;
using Jotunn.Entities;
using UnityEngine;

namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Result metadata returned by the CIR 0.2 registration APIs.
    /// </summary>
    public sealed class ItemRegistrationResult
    {
        public bool Success { get; private set; }
        public string ItemName { get; private set; }
        public string AssetBundlePath { get; private set; }
        public string PrefabName { get; private set; }
        public GameObject Prefab { get; private set; }
        public CustomItem CustomItem { get; private set; }
        public Exception Exception { get; private set; }
        public string ErrorMessage => Exception?.Message;

        internal static ItemRegistrationResult Registered(CustomItemDefinition definition, GameObject prefab, CustomItem customItem)
        {
            return new ItemRegistrationResult
            {
                Success = true,
                ItemName = definition.ItemName,
                AssetBundlePath = definition.AssetBundlePath,
                PrefabName = definition.PrefabName,
                Prefab = prefab,
                CustomItem = customItem
            };
        }

        internal static ItemRegistrationResult Failed(CustomItemDefinition definition, Exception exception)
        {
            return new ItemRegistrationResult
            {
                Success = false,
                ItemName = definition?.ItemName,
                AssetBundlePath = definition?.AssetBundlePath,
                PrefabName = definition?.PrefabName,
                Exception = exception
            };
        }
    }
}
