using BepInEx;
using InfFloorRemaster.Classes;
using MTM101BaldAPI.SaveSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfFloorRemaster
{
    internal class EndlessFloorsSaveHandler : ModdedSaveGameIOBinary
    {
        public override PluginInfo pluginInfo => InfFloorMod.Instance.Info;

        public override void Load(BinaryReader reader)
        {
            InfFloorMod.save = EndlessSave.Load(reader);
        }

        public override void Reset()
        {
            InfFloorMod.save = new EndlessSave();
        }

        public override void Save(BinaryWriter writer)
        {
            InfFloorMod.save.Save(writer);
        }
    }
}
