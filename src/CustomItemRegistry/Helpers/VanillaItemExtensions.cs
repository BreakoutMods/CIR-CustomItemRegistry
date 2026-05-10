using System;

namespace ValheimCustomItemRegistry
{
    public static class VanillaItemExtensions
    {
        public static string ToPrefabName(this VanillaItem item)
        {
            if (TryToPrefabName(item, out string prefabName))
            {
                return prefabName;
            }

            throw new ArgumentOutOfRangeException(nameof(item), item, "Unknown VanillaItem value");
        }

        internal static bool TryToPrefabName(VanillaItem item, out string prefabName)
        {
            switch (item)
            {
                case VanillaItem.Wood: prefabName = "Wood"; return true;
                case VanillaItem.FineWood: prefabName = "FineWood"; return true;
                case VanillaItem.RoundLog: prefabName = "RoundLog"; return true;
                case VanillaItem.ElderBark: prefabName = "ElderBark"; return true;
                case VanillaItem.YggdrasilWood: prefabName = "YggdrasilWood"; return true;
                case VanillaItem.Stone: prefabName = "Stone"; return true;
                case VanillaItem.Flint: prefabName = "Flint"; return true;
                case VanillaItem.Coal: prefabName = "Coal"; return true;
                case VanillaItem.Resin: prefabName = "Resin"; return true;
                case VanillaItem.Amber: prefabName = "Amber"; return true;
                case VanillaItem.AmberPearl: prefabName = "AmberPearl"; return true;
                case VanillaItem.Ruby: prefabName = "Ruby"; return true;
                case VanillaItem.Crystal: prefabName = "Crystal"; return true;
                case VanillaItem.Coins: prefabName = "Coins"; return true;
                case VanillaItem.SurtlingCore: prefabName = "SurtlingCore"; return true;
                case VanillaItem.BlackCore: prefabName = "BlackCore"; return true;
                case VanillaItem.CopperOre: prefabName = "CopperOre"; return true;
                case VanillaItem.Copper: prefabName = "Copper"; return true;
                case VanillaItem.TinOre: prefabName = "TinOre"; return true;
                case VanillaItem.Tin: prefabName = "Tin"; return true;
                case VanillaItem.Bronze: prefabName = "Bronze"; return true;
                case VanillaItem.IronScrap: prefabName = "IronScrap"; return true;
                case VanillaItem.Iron: prefabName = "Iron"; return true;
                case VanillaItem.SilverOre: prefabName = "SilverOre"; return true;
                case VanillaItem.Silver: prefabName = "Silver"; return true;
                case VanillaItem.BlackMetalScrap: prefabName = "BlackMetalScrap"; return true;
                case VanillaItem.BlackMetal: prefabName = "BlackMetal"; return true;
                case VanillaItem.FlametalOre: prefabName = "FlametalOreNew"; return true;
                case VanillaItem.Flametal: prefabName = "FlametalNew"; return true;
                case VanillaItem.LeatherScraps: prefabName = "LeatherScraps"; return true;
                case VanillaItem.DeerHide: prefabName = "DeerHide"; return true;
                case VanillaItem.TrollHide: prefabName = "TrollHide"; return true;
                case VanillaItem.WolfPelt: prefabName = "WolfPelt"; return true;
                case VanillaItem.LoxPelt: prefabName = "LoxPelt"; return true;
                case VanillaItem.ScaleHide: prefabName = "ScaleHide"; return true;
                case VanillaItem.Carapace: prefabName = "Carapace"; return true;
                case VanillaItem.Feathers: prefabName = "Feathers"; return true;
                case VanillaItem.BoneFragments: prefabName = "BoneFragments"; return true;
                case VanillaItem.WitheredBone: prefabName = "WitheredBone"; return true;
                case VanillaItem.HardAntler: prefabName = "HardAntler"; return true;
                case VanillaItem.DragonEgg: prefabName = "DragonEgg"; return true;
                case VanillaItem.Wishbone: prefabName = "Wishbone"; return true;
                case VanillaItem.YmirRemains: prefabName = "YmirRemains"; return true;
                case VanillaItem.QueenDrop: prefabName = "QueenDrop"; return true;
                case VanillaItem.Mushroom: prefabName = "Mushroom"; return true;
                case VanillaItem.MushroomYellow: prefabName = "MushroomYellow"; return true;
                case VanillaItem.MushroomBlue: prefabName = "MushroomBlue"; return true;
                case VanillaItem.MushroomMagecap: prefabName = "MushroomMagecap"; return true;
                case VanillaItem.MushroomJotunPuffs: prefabName = "MushroomJotunPuffs"; return true;
                case VanillaItem.Raspberry: prefabName = "Raspberry"; return true;
                case VanillaItem.Blueberries: prefabName = "Blueberries"; return true;
                case VanillaItem.Cloudberry: prefabName = "Cloudberry"; return true;
                case VanillaItem.Honey: prefabName = "Honey"; return true;
                case VanillaItem.Carrot: prefabName = "Carrot"; return true;
                case VanillaItem.Turnip: prefabName = "Turnip"; return true;
                case VanillaItem.Onion: prefabName = "Onion"; return true;
                case VanillaItem.Barley: prefabName = "Barley"; return true;
                case VanillaItem.BarleyFlour: prefabName = "BarleyFlour"; return true;
                case VanillaItem.Flax: prefabName = "Flax"; return true;
                case VanillaItem.Sap: prefabName = "Sap"; return true;
                case VanillaItem.RoyalJelly: prefabName = "RoyalJelly"; return true;
                case VanillaItem.Dandelion: prefabName = "Dandelion"; return true;
                case VanillaItem.Thistle: prefabName = "Thistle"; return true;
                case VanillaItem.Entrails: prefabName = "Entrails"; return true;
                case VanillaItem.Bloodbag: prefabName = "Bloodbag"; return true;
                case VanillaItem.Ooze: prefabName = "Ooze"; return true;
                case VanillaItem.Guck: prefabName = "Guck"; return true;
                case VanillaItem.Tar: prefabName = "Tar"; return true;
                case VanillaItem.WolfFang: prefabName = "WolfFang"; return true;
                case VanillaItem.WolfClaw: prefabName = "WolfClaw"; return true;
                case VanillaItem.Needle: prefabName = "Needle"; return true;
                case VanillaItem.Obsidian: prefabName = "Obsidian"; return true;
                case VanillaItem.Chitin: prefabName = "Chitin"; return true;
                case VanillaItem.SerpentScale: prefabName = "SerpentScale"; return true;
                case VanillaItem.BoarMeat: prefabName = "RawMeat"; return true;
                case VanillaItem.DeerMeat: prefabName = "DeerMeat"; return true;
                case VanillaItem.WolfMeat: prefabName = "WolfMeat"; return true;
                case VanillaItem.LoxMeat: prefabName = "LoxMeat"; return true;
                case VanillaItem.SerpentMeat: prefabName = "SerpentMeat"; return true;
                case VanillaItem.HareMeat: prefabName = "HareMeat"; return true;
                case VanillaItem.ChickenMeat: prefabName = "ChickenMeat"; return true;
                case VanillaItem.BugMeat: prefabName = "BugMeat"; return true;
                case VanillaItem.FishRaw: prefabName = "FishRaw"; return true;
                case VanillaItem.ArrowWood: prefabName = "ArrowWood"; return true;
                case VanillaItem.ArrowFire: prefabName = "ArrowFire"; return true;
                case VanillaItem.ArrowFlint: prefabName = "ArrowFlint"; return true;
                case VanillaItem.ArrowIron: prefabName = "ArrowIron"; return true;
                case VanillaItem.ArrowObsidian: prefabName = "ArrowObsidian"; return true;
                case VanillaItem.ArrowPoison: prefabName = "ArrowPoison"; return true;
                case VanillaItem.ArrowSilver: prefabName = "ArrowSilver"; return true;
                case VanillaItem.ArrowNeedle: prefabName = "ArrowNeedle"; return true;
                case VanillaItem.ArrowCarapace: prefabName = "ArrowCarapace"; return true;
                case VanillaItem.BoltBone: prefabName = "BoltBone"; return true;
                case VanillaItem.BoltBlackmetal: prefabName = "BoltBlackmetal"; return true;
                case VanillaItem.BoltCarapace: prefabName = "BoltCarapace"; return true;
                case VanillaItem.TrophyBoar: prefabName = "TrophyBoar"; return true;
                case VanillaItem.TrophyDeer: prefabName = "TrophyDeer"; return true;
                case VanillaItem.TrophyEikthyr: prefabName = "TrophyEikthyr"; return true;
                case VanillaItem.TrophyTheElder: prefabName = "TrophyTheElder"; return true;
                case VanillaItem.TrophyBonemass: prefabName = "TrophyBonemass"; return true;
                case VanillaItem.TrophyDragonQueen: prefabName = "TrophyDragonQueen"; return true;
                case VanillaItem.TrophyGoblinKing: prefabName = "TrophyGoblinKing"; return true;
                case VanillaItem.TrophySeekerQueen: prefabName = "TrophySeekerQueen"; return true;
                default:
                    prefabName = null;
                    return false;
            }
        }
    }
}
