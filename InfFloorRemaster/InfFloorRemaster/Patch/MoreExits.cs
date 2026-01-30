using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using MTM101BaldAPI;
using System.Diagnostics;

namespace InfFloorRemaster.Patch
{
    [HarmonyPatch(typeof(Directions))]
    [HarmonyPatch("All")]
    class AllowStuff
    {
        static bool Prefix(ref List<Direction> __result)
        {

            if (InfFloorMod.currentFloorData.exitCount <= 4) return true;
            System.Reflection.MethodBase fo = new StackTrace().GetFrame(2).GetMethod(); //gets the thing that called it
            if (fo.Name == "MoveNext")
            {
                List<Direction> directions = new List<Direction>
                {
                    Direction.North,
                    Direction.East,
                    Direction.South,
                    Direction.West,
                };
                __result = new List<Direction>
                {
                    Direction.North,
                    Direction.East,
                    Direction.South,
                    Direction.West,
                };
                for (int i = 0; i < InfFloorMod.currentFloorData.exitCount; i++)
                {
                    __result.Add(directions[i % 4]); //this make it look pretty :D
                }
                return false;
            }

            return true;
        }
    }
}
