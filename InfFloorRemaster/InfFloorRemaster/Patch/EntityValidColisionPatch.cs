using HarmonyLib;
using InfFloorRemaster.Scripts.Game.Components;
using System.Reflection;

namespace InfFloorRemaster.Patch
{
    [HarmonyPatch(typeof(Entity))]
    [HarmonyPatch("CollisionValid")]
    internal class EntityCollisionValidPatch
    {
        private static readonly FieldInfo flipInfo = AccessTools.Field(typeof(Entity), "flipped");
        private static readonly FieldInfo ignoreOrientationInfo = AccessTools.Field(typeof(Entity), "ignoreOrientation");

        public static bool Prefix(Entity __instance, Entity otherEntity, ref bool __result)
        {
            bool myFlip = (bool)flipInfo.GetValue(__instance);
            bool otherFlip = (bool)flipInfo.GetValue(otherEntity);
            bool ignoreOrientation = (bool)ignoreOrientationInfo.GetValue(__instance);

            if (myFlip != otherFlip && !ignoreOrientation)
            {
                __result = ignoreOrientation;
                return false;
            }

            if (otherEntity.CompareTag("Player"))
            {
                __result = !otherEntity.GetComponent<PlayerParametrs>().triggerOff;
                return false;
            }

            __result = true;
            return false;
        }
    }
}
