using UnityEngine;
using System.Collections.Generic;
using InfFloorRemaster.Scripts.Game.Objects;

namespace InfFloorRemaster.Scripts.Game.Builders
{
    public class Structure_PitStopLevelUpgradeMachiene : StructureBuilder
    {
        public override void Load(List<StructureData> data)
        {
            base.Load(data);
            StoreRoomFunction storeFunc = FindObjectOfType<StoreRoomFunction>();
            if (ec.CellFromPosition(28, 10).Null)
            {
                ec.CreateCell(12, new IntVector2(28, 10), storeFunc.Room);
                ec.CreateCell(9, new IntVector2(28, 11), storeFunc.Room);
                ec.CreateCell(6, new IntVector2(29, 10), storeFunc.Room);
                ec.CreateCell(3, new IntVector2(29, 11), storeFunc.Room);
                ec.ConnectCells(new IntVector2(29, 10), Direction.East);
            }

            UpgradeMachine upgradeMachine = Instantiate(InfFloorMod.Instance.assetManager.Get<UpgradeMachine>("UpgradesMachine"));
            upgradeMachine.transform.position = new Vector3(285f, 0.5f, 105f);
        }
    }
}
