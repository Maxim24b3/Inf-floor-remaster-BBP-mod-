using System.Collections;
using UnityEngine;

namespace InfFloorRemaster.Scripts.Game.Components
{
    public class BonkManager : Singleton<BonkManager>
    {
        private HudGauge[] gauges = new HudGauge[4];
        private MovementModifier bonkMoveMod = new MovementModifier(Vector3.zero, 3.5f);

        public void StartEffect(HudGauge gauge, int playerNumber, float time)
        {
            gauges[playerNumber] = gauge;
            StartCoroutine(Timer(time, playerNumber));
        }

        private IEnumerator Timer(float time, int player)
        {
            float currentTime = time;
            Singleton<CoreGameManager>.Instance.GetPlayer(player).plm.am.moveMods.Add(bonkMoveMod);
            Singleton<CoreGameManager>.Instance.GetPlayer(player).itm.Disable(true);
            Singleton<CoreGameManager>.Instance.GetPlayer(player).GetComponent<PlayerParametrs>().triggerOff = true;
            while (currentTime > 0)
            {
                currentTime -= 1 * Time.deltaTime;
                gauges[player].SetValue(time, currentTime);
                yield return null;
            }
            Singleton<CoreGameManager>.Instance.GetPlayer(player).itm.enabled = true;
            gauges[player].Deactivate();
            gauges[player] = null;
            Singleton<CoreGameManager>.Instance.GetPlayer(player).plm.am.moveMods.Remove(bonkMoveMod);
            Singleton<CoreGameManager>.Instance.GetPlayer(player).itm.Disable(false);
            Singleton<CoreGameManager>.Instance.GetPlayer(player).GetComponent<PlayerParametrs>().triggerOff = false;
        }
    }
}
