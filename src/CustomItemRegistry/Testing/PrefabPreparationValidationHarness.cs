using System.Linq;
using UnityEngine;

namespace ValheimCustomItemRegistry
{
    internal static class PrefabPreparationValidationHarness
    {
        public static bool SimplePrefabFailsStrictValidation()
        {
            GameObject prefab = new GameObject("HarnessSimplePrefab");
            try
            {
                CustomItemDefinition definition = CustomItemRegistry.Item("HarnessSimpleItem")
                    .FromBundle("unused", "HarnessSimplePrefab")
                    .AsMaterial()
                    .Build();

                return !CustomItemRegistry.PrepareItemPrefabForTest(definition, prefab, out _, out _, out string error)
                    && error.Contains(nameof(ItemDrop))
                    && error.Contains(nameof(Rigidbody))
                    && error.Contains(nameof(ZNetView))
                    && error.Contains(nameof(ZSyncTransform))
                    && error.Contains(nameof(Collider));
            }
            finally
            {
                Object.DestroyImmediate(prefab);
            }
        }

        public static bool OptInSimplePrefabGetsItemDropAndPhysics()
        {
            GameObject prefab = new GameObject("HarnessSimplePrefab");
            try
            {
                CustomItemDefinition definition = CustomItemRegistry.Item("HarnessSimpleItem")
                    .FromBundle("unused", "HarnessSimplePrefab")
                    .AsMaterial()
                    .PrefabPreparation(prep => prep
                        .AutoAddItemDrop()
                        .AutoAddPhysics()
                        .WarnOnMissingCollider())
                    .Build();

                bool prepared = CustomItemRegistry.PrepareItemPrefabForTest(definition, prefab, out var addedComponents, out var warnings, out _);
                return prepared
                    && prefab.GetComponent<ItemDrop>()
                    && prefab.GetComponent<Rigidbody>()
                    && prefab.GetComponent<ZNetView>()
                    && prefab.GetComponent<ZSyncTransform>()
                    && addedComponents.Contains(nameof(ItemDrop))
                    && addedComponents.Contains(nameof(Rigidbody))
                    && addedComponents.Contains(nameof(ZNetView))
                    && addedComponents.Contains(nameof(ZSyncTransform))
                    && warnings.Count > 0;
            }
            finally
            {
                Object.DestroyImmediate(prefab);
            }
        }

        public static bool RequireItemDropKeepsStrictBehavior()
        {
            GameObject prefab = new GameObject("HarnessStrictPrefab");
            try
            {
                CustomItemDefinition definition = CustomItemRegistry.Item("HarnessStrict")
                    .FromBundle("unused", "HarnessStrictPrefab")
                    .PrefabPreparation(prep => prep.RequireItemDrop())
                    .AsMaterial()
                    .Build();

                return !CustomItemRegistry.PrepareItemPrefabForTest(definition, prefab, out _, out _, out string error)
                    && error.Contains("ItemDrop");
            }
            finally
            {
                Object.DestroyImmediate(prefab);
            }
        }

        public static bool ExistingSharedDataIsPreserved()
        {
            GameObject prefab = new GameObject("HarnessExistingPrefab");
            try
            {
                ItemDrop itemDrop = prefab.AddComponent<ItemDrop>();
                itemDrop.m_itemData = new ItemDrop.ItemData();
                itemDrop.m_itemData.m_shared = new ItemDrop.ItemData.SharedData
                {
                    m_name = "Existing Name",
                    m_description = "Existing Description",
                    m_weight = 9f
                };

                CustomItemDefinition definition = CustomItemRegistry.Item("HarnessExisting")
                    .FromBundle("unused", "HarnessExistingPrefab")
                    .PrefabPreparation(prep => prep.AutoAddPhysics().WarnOnMissingCollider())
                    .Build();

                bool prepared = CustomItemRegistry.PrepareItemPrefabForTest(definition, prefab, out _, out _, out _);
                return prepared
                    && itemDrop.m_itemData.m_shared.m_name == "Existing Name"
                    && itemDrop.m_itemData.m_shared.m_description == "Existing Description"
                    && itemDrop.m_itemData.m_shared.m_weight == 9f;
            }
            finally
            {
                Object.DestroyImmediate(prefab);
            }
        }
    }
}
