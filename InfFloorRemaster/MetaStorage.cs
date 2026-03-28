using MTM101BaldAPI;
using MTM101BaldAPI.Registers;

namespace InfFloorRemaster
{
    internal static class MetaStorage
    {
        static ItemMetaStorage items = MTM101BaldiDevAPI.itemMetadata;
        static NPCMetaStorage npcs = MTM101BaldiDevAPI.npcMetadata;

        public static ItemObject GetItem(Items itemTypes)
        {
            return items.FindByEnum(itemTypes).value;
        }

        public static NPC GetNpc(Character character)
        {
            return npcs.Get(character).value;
        }
    }
}
