using HarmonyLib;
using System.Reflection;

namespace InfFloorRemaster.Patch
{
    public static class GluePatch
    {
        private static bool[] glued = new bool[4];
        public static int[] floorPoints = new int[4];
        private static int MaxLevels
        {
            get
            {
                return 5 + InfFloorMod.save.GetUpgradeCount("Glue") * 5;
            }
        }

        public static void EndFloor()
        {
            FieldInfo gluedSlotsInfo = AccessTools.Field(typeof(StickerManager), "slotUpgraded");
            glued = gluedSlotsInfo.GetValue(Singleton<StickerManager>.Instance) as bool[];
            for (int i = 0; i < 4; i++)
            {
                if (glued[i] == true)
                {
                    floorPoints[i]++;
                    if (floorPoints[i] >= MaxLevels)
                    {
                        DowngradeSlot(i);
                    }
                }
            }
        }

        public static void DowngradeSlot(int id)
        {
            FieldInfo gluedSlotsInfo = AccessTools.Field(typeof(StickerManager), "slotUpgraded");
            floorPoints[id] = 0;
            glued[id] = false;
            gluedSlotsInfo.SetValue(Singleton<StickerManager>.Instance, glued);
        }
    }
}
