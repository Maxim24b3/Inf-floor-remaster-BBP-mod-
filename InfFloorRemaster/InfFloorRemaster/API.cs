using InfFloorRemaster.Classes;
using System.Collections.Generic;

namespace InfFloorRemaster.API
{
    public class API
    {
        // NOTE TO MODDERS: IF YOU WANT TO CREATE YOUR OWN UPGRADES AS A SEPERATE MOD, INHERIT FROM STANDARDUPGRADE AND OVERRIDE THE GETICON FUNCTION!!
        public static void AddUpgrade(StandardUpgrade upgrade)
        {
            InfFloorMod.Upgrades.Add(upgrade.id, upgrade);
        }

        // NOTE TO MODDERS: No Works!! i'm too lazy to make it works right now (are not generated in the game)!!
        public static void AddWeightedStructureWithParameters(WeightedStructureWithParameters weightedStructureWithParameters, int minFloor)
        {
            AdditionsFromOtherMods.weightedStructureWithParameters.Add(new WeightedStructureWithParametersWithFloorPoint(weightedStructureWithParameters, minFloor));
        }

        // NOTE TO MODDERS: No Works!! i'm too lazy to make it works right now (are not generated in the game)!!
        public static void AddWeightedNPC(WeightedNPC npc, int minFloors)
        {
            AdditionsFromOtherMods.NPCs.Add(new WeightedNPCWithFloorPoint(npc, minFloors));
        }
    }
}

namespace InfFloorRemaster
{
    public static class AdditionsFromOtherMods
    {
        public static List<WeightedNPCWithFloorPoint> NPCs = new List<WeightedNPCWithFloorPoint>();
        public static List<WeightedStructureWithParametersWithFloorPoint> weightedStructureWithParameters = new List<WeightedStructureWithParametersWithFloorPoint>();
    }

    public class WeightedNPCWithFloorPoint
    {
        public WeightedNPC npc;
        public int floorPoint;

        public WeightedNPCWithFloorPoint(WeightedNPC npc, int floorPoint)
        {
            this.npc = npc;
            this.floorPoint = floorPoint;
        }
    }

    public class WeightedStructureWithParametersWithFloorPoint
    {
        public WeightedStructureWithParameters weightedStructureWithParameters;
        public int floorPoint;

        public WeightedStructureWithParametersWithFloorPoint(WeightedStructureWithParameters weightedStructureWithParameters, int floorPoint)
        {
            this.weightedStructureWithParameters = weightedStructureWithParameters;
            this.floorPoint = floorPoint;
        }
    }
}
