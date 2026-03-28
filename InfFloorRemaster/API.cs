using InfFloorRemaster.Classes;
using System.Collections.Generic;
using UnityEngine;

namespace InfFloorRemaster.API
{
    public class API
    {
        // NOTE TO MODDERS: IF YOU WANT TO CREATE YOUR OWN UPGRADES AS A SEPERATE MOD, INHERIT FROM STANDARDUPGRADE AND OVERRIDE THE GETICON FUNCTION!!
        public static void AddUpgrade(StandardUpgrade upgrade)
        {
            InfFloorMod.Upgrades.Add(upgrade.id, upgrade);
        }

        public static void AddWeightedStructureWithParameters(WeightedStructureWithParameters weightedStructureWithParameters, int minFloor)
        {
            AdditionsFromOtherMods.weightedStructureWithParameters.Add(new WeightedStructureWithParametersWithMinFloor(weightedStructureWithParameters, minFloor));
        }

        public static void AddWeightedNPC(WeightedNPC npc, int minFloors)
        {
            AdditionsFromOtherMods.NPCs.Add(new WeightedNPCWithMinFloor(npc, minFloors));
        }
    }
}

// NOTE TO MODDERS: It's better not to look here
namespace InfFloorRemaster
{
    internal static class AdditionsFromOtherMods
    {
        public static List<WeightedNPCWithMinFloor> NPCs = new List<WeightedNPCWithMinFloor>();
        public static List<WeightedStructureWithParametersWithMinFloor> weightedStructureWithParameters = new List<WeightedStructureWithParametersWithMinFloor>();
    }

    internal class LevelGameObjectWithMinFloor<T> where T : class
    {
        public T selection = default(T);
        public int minFloor = 0;

        internal LevelGameObjectWithMinFloor(T selection,int minFloor)
        {
            this.selection = selection;
            this.minFloor = minFloor;
        }
    }

    internal class WeightedNPCWithMinFloor : LevelGameObjectWithMinFloor<WeightedNPC>
    {
        internal  WeightedNPCWithMinFloor(WeightedNPC weightedNPC, int minFloor) : base(weightedNPC, minFloor)
        {

        }
    }

    internal class WeightedStructureWithParametersWithMinFloor : LevelGameObjectWithMinFloor<WeightedStructureWithParameters>
    {
        internal WeightedStructureWithParametersWithMinFloor(WeightedStructureWithParameters weightedStructure, int minFloor) : base(weightedStructure, minFloor)
        {

        }
    }
}
