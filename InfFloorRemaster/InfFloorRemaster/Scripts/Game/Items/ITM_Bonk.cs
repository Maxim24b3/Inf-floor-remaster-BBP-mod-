using InfFloorRemaster.Scripts.Game.Components;
using UnityEngine;

namespace InfFloorRemaster.Scripts.Game.Items
{
    public class ITM_Bonk : Item
    {
        private HudGauge hudGauge;

        public override bool Use(PlayerManager pm)
        {
            hudGauge = Singleton<CoreGameManager>.Instance.GetHud(pm.playerNumber).gaugeManager.ActivateNewGauge(InfFloorMod.Instance.assetManager.Get<Sprite>("BonkGauge"), 25);
            Singleton<BonkManager>.Instance.StartEffect(hudGauge, pm.playerNumber, 25);
            return true;
        }
    }
}
