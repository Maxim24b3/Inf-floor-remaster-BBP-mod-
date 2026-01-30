using UnityEngine;

namespace InfFloorRemaster.Patch
{
    public class Cheats : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    Singleton<BaseGameManager>.Instance.LoadNextLevel();
                }

                if (Input.GetKeyDown(KeyCode.O))
                {
                    InfFloorMod.currentFloorData.FloorID++;
                }

                if (Input.GetKeyDown(KeyCode.I))
                {
                    Singleton<CoreGameManager>.Instance.AddPoints(1000, 0, false);
                }
            }
        }
    }
}
