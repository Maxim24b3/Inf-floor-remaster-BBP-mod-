using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.IO;

namespace InfFloorRemaster.Classes
{
    public struct UpgradeSaveData
    {
        public string id { private set; get; } //to ensure people have to create a new upgradedata if they want to change the id
        public byte count;

        public UpgradeSaveData(string id, byte count)
        {
            this.id = id;
            this.count = count;
        }
    }

    public class EndlessSave
    {
        public FloorData myFloorData = new FloorData();
        public UpgradeSaveData[] Upgrades = new UpgradeSaveData[5] { new UpgradeSaveData("none", 0), new UpgradeSaveData("none", 0), new UpgradeSaveData("none", 0), new UpgradeSaveData("none", 0), new UpgradeSaveData("none", 0) };
        public Dictionary<string, byte> Counters = new Dictionary<string, byte>();
        public int startingFloor = 1;

        public int currentFloor 
        {
            get
            {
                return myFloorData.FloorID;
            }
            set
            {
                myFloorData.FloorID = value;
            }
        }

        public void Reset()
        {
            currentFloor = 1;
            myFloorData.FloorID = 1;
            Counters.Clear();
            Upgrades = new UpgradeSaveData[5] { new UpgradeSaveData("none", 0), new UpgradeSaveData("none", 0), new UpgradeSaveData("none", 0), new UpgradeSaveData("none", 0), new UpgradeSaveData("none", 0) };
        }

        public bool HasUpgrade(string type) => GetUpgradeCount(type) > 0;

        public int GetUpgradeCount(string type)
        {
            if (Counters.ContainsKey(type)) return Counters[type];
            for (int i = 0; i < Upgrades.Length; i++)
            {
                if (Upgrades[i].id == type)
                {
                    return Upgrades[i].count;
                }
            }
            return 0;
        }

        public void SellUpgrade(string id)
        {
            for (int i = 0; i < Upgrades.Length; i++)
            {
                if (Upgrades[i].id == id)
                {
                    if (Upgrades[i].count == 1)
                    {
                        Upgrades[i] = new UpgradeSaveData("none", 0);
                        return;
                    }
                    Upgrades[i].count--;
                    return;
                }
            }
        }

        public bool CanPurchaseUpgrade(StandardUpgrade upgrade, UpgradePurchaseBehavior behavior)
        {
            switch (behavior)
            {
                case UpgradePurchaseBehavior.IncrementCounter:
                    if (!Counters.ContainsKey(upgrade.id))
                    {
                        return true;
                    }
                    if (Counters[upgrade.id] == byte.MaxValue)
                    {
                        return false;
                    }
                    return true;
                case UpgradePurchaseBehavior.Nothing:
                    return true;
                case UpgradePurchaseBehavior.FillUpgradeSlot:
                    for (int i = 0; i < Upgrades.Length; i++)
                    {
                        if (Upgrades[i].id == upgrade.id)
                        {
                            return true;
                        }
                    }
                    for (int i = 0; i < Upgrades.Length; i++)
                    {
                        if (Upgrades[i].id == "none")
                        {
                            return true;
                        }
                    }
                    return false;
            }
            throw new NotImplementedException("Not Implemented:" + behavior.ToString());
        }

        public bool PurchaseUpgrade(StandardUpgrade upgrade, UpgradePurchaseBehavior behavior)
        {
            switch (behavior)
            {
                case UpgradePurchaseBehavior.IncrementCounter:
                    if (!Counters.ContainsKey(upgrade.id))
                    {
                        Counters.Add(upgrade.id, 0);
                    }
                    if (Counters[upgrade.id] == byte.MaxValue)
                    {
                        return false;
                    }
                    Counters[upgrade.id]++;
                    return true;
                case UpgradePurchaseBehavior.Nothing:
                    return true;
                case UpgradePurchaseBehavior.FillUpgradeSlot:
                    for (int i = 0; i < Upgrades.Length; i++)
                    {
                        if (Upgrades[i].id == upgrade.id)
                        {
                            Upgrades[i].count++;
                            return true;
                        }
                    }
                    for (int i = 0; i < Upgrades.Length; i++)
                    {
                        if (Upgrades[i].id == "none")
                        {
                            Upgrades[i] = new UpgradeSaveData(upgrade.id, 1);
                            return true;
                        }
                    }
                    return false;
            }
            throw new NotImplementedException("Not Implemented:" + behavior.ToString());
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write((byte)1); //format version
            writer.Write(currentFloor);
            writer.Write(startingFloor);
            writer.Write((byte)Upgrades.Length);
            for (int i = 0; i < Upgrades.Length; i++)
            {
                writer.Write(Upgrades[i].id);
                writer.Write(Upgrades[i].count);
            }
            writer.Write(Counters.Count);
            foreach (KeyValuePair<string, byte> item in Counters)
            {
                writer.Write(item.Key);
                writer.Write(item.Value);
            }
            //writer.Write(claimedTicketCurrentFloor);
        }

        public static EndlessSave Load(BinaryReader reader)
        {
            EndlessSave data = new EndlessSave();
            data.Counters.Clear();
            byte version = reader.ReadByte();
            data.currentFloor = reader.ReadInt32();
            data.startingFloor = reader.ReadInt32();
            int upgradeLength = reader.ReadByte();
            for (int i = 0; i < upgradeLength; i++)
            {
                data.Upgrades[i] = new UpgradeSaveData(reader.ReadString(), reader.ReadByte());
            }
            int counterCount = reader.ReadInt32();
            for (int i = 0; i < counterCount; i++)
            {
                data.Counters.Add(reader.ReadString(), reader.ReadByte());
            }
            if (version == 0) return data;
            //data.claimedTicketCurrentFloor = reader.ReadBoolean();
            return data;
        }
    }
}
