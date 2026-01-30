using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfFloorRemaster.Scripts
{
    public class GeneratorData
    {
        public int id;
        public string name;
        public int additionalNPCs;
        public List<WeightedNPC> potentialNPCs = new List<WeightedNPC>();
        public List<NPC> forcedNPCs = new List<NPC>();
        public List<WeightedItemObject> items = new List<WeightedItemObject>();
        public WeightedRoomAsset[] potentialSpecialRooms = new WeightedRoomAsset[0];
        public ObjectBuilder[] forcedSpecialHallBuilders = new ObjectBuilder[0];
        public WeightedObjectBuilder[] specialHallBuilders = new WeightedObjectBuilder[0];
        public StructureWithParameters[] forcedStructures = new StructureWithParameters[0];
        public WeightedStructureWithParameters[] potentialStructures = new WeightedStructureWithParameters[0];
        public Dictionary<RoomCategory, List<WeightedRoomAsset>> roomAssets = new Dictionary<RoomCategory, List<WeightedRoomAsset>>();
        public List<WeightedRoomAsset> classRoomAssets => roomAssets[RoomCategory.Class];
        public List<WeightedRoomAsset> facultyRoomAssets => roomAssets[RoomCategory.Faculty];
        public List<WeightedRoomAsset> specialRoomAssets => roomAssets[RoomCategory.Special];
        public List<WeightedRoomAsset> officeRoomAssets => roomAssets[RoomCategory.Office];
        public List<WeightedRoomAsset> hallInsertions => roomAssets[RoomCategory.Hall];

        public GeneratorData()
        {
            roomAssets.Add(RoomCategory.Class, new List<WeightedRoomAsset>());
            roomAssets.Add(RoomCategory.Faculty, new List<WeightedRoomAsset>());
            roomAssets.Add(RoomCategory.Hall, new List<WeightedRoomAsset>());
            roomAssets.Add(RoomCategory.Office, new List<WeightedRoomAsset>());
            roomAssets.Add(RoomCategory.Special, new List<WeightedRoomAsset>());
        }
    }
}
