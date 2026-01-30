using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InfFloorRemaster.Patch
{
    public class EndlessTitleUI : MonoBehaviour
    {
        void Start()
        {
            Destroy(gameObject.transform.Find("Endless").gameObject);
            Destroy(gameObject.transform.Find("Challenge").gameObject);
            Destroy(gameObject.transform.Find("FieldTrips").gameObject);
            Destroy(gameObject.transform.Find("Tutorial").gameObject);
            Transform theMain = gameObject.transform.Find("Main");
            theMain.gameObject.SetActive(true);
            Transform modeText = gameObject.transform.Find("ModeText");
            Transform seedInput = gameObject.transform.Find("SeedInput");
            theMain.localPosition -= new Vector3(0f, 48f, 0f);
            TMP_Text mainText = theMain.GetComponentInChildren<TMP_Text>();
            if (mainText == null)
            {
                mainText = theMain.GetComponent<TMP_Text>();
            }
            mainText.text = "inf floors";

            StandardMenuButton mainB = theMain.GetComponent<StandardMenuButton>();
            mainB.OnHighlight.RemoveAllListeners();

            mainB.OnHighlight.AddListener(() => modeText.gameObject.GetComponent<TextLocalizer>().GetLocalizedText("Classic infinty floors mode (in development!)"));
        }
    }

    public class HideSeekMenuPatch : MonoBehaviour
    {
        public GameLoader gl;

        private void Start()
        {
            Transform theMain = gameObject.transform.Find("MainNew");
            StandardMenuButton mainB = theMain.GetComponent<StandardMenuButton>();
            mainB.OnPress.RemoveAllListeners();
            mainB.OnPress.AddListener(() => {
                InfFloorMod.save.Reset();
                gl.LoadLevel(InfFloorMod.currentSceneObject);
            });
        }
    }
}
