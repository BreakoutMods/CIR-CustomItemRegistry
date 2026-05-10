using System;

namespace ValheimCustomItemRegistry
{
    public static class CraftingStationExtensions
    {
        public static string ToPrefabName(this CraftingStation station)
        {
            if (TryToPrefabName(station, out string prefabName))
            {
                return prefabName;
            }

            throw new ArgumentOutOfRangeException(nameof(station), station, "Unknown CraftingStation value");
        }

        internal static bool TryToPrefabName(CraftingStation station, out string prefabName)
        {
            switch (station)
            {
                case CraftingStation.None: prefabName = null; return true;
                case CraftingStation.Workbench: prefabName = "piece_workbench"; return true;
                case CraftingStation.Forge: prefabName = "forge"; return true;
                case CraftingStation.Stonecutter: prefabName = "piece_stonecutter"; return true;
                case CraftingStation.Cauldron: prefabName = "piece_cauldron"; return true;
                case CraftingStation.ArtisanTable: prefabName = "piece_artisanstation"; return true;
                case CraftingStation.BlackForge: prefabName = "blackforge"; return true;
                case CraftingStation.GaldrTable: prefabName = "piece_magetable"; return true;
                case CraftingStation.EitrRefinery: prefabName = "piece_eitrrefinery"; return true;
                default:
                    prefabName = null;
                    return false;
            }
        }
    }
}
