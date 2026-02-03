using UnityEngine;

namespace InfFloorRemaster.Scripts.Game.Objects
{
    public class UpgradeMachine : MonoBehaviour, IClickable<int>
    {
        public void Clicked(int pm)
        {
            Singleton<CoreGameManager>.Instance.Pause(false);
            Instantiate(InfFloorMod.Instance.assetManager.Get<Canvas>("UpgradeMenu"));
        }

        public void ClickableSighted(int pm)
        {

        }

        public void ClickableUnsighted(int pm)
        {

        }

        public bool ClickableHidden()
        {
            return false;
        }

        public bool ClickableRequiresNormalHeight()
        {
            return true;
        }
    }
}
