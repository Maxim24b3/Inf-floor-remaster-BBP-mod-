using HarmonyLib;
using MTM101BaldAPI.SaveSystem;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InfFloorRemaster.Patch
{
    public class EndlessTitleUI : MonoBehaviour
    {
        public GameLoader gl;
        public Transform hideSeekMenu;
        private bool has_incre = false;
        private TextLocalizer f99Local;
        public string mode;
        private HideSeekMenuModed hideSeekMenuModed;

        private void OnEnable()
        {
            if (f99Local == null) return;
            f99Local.GetLocalizedText("Btn_NNChallenge");
        }

        void Start()
        {
            Destroy(gameObject.transform.Find("Endless").gameObject);
            Destroy(gameObject.transform.Find("Challenge").gameObject);
            Destroy(gameObject.transform.Find("FieldTrips").gameObject);
            Destroy(gameObject.transform.Find("Tutorial").gameObject);
            Transform theMain = gameObject.transform.Find("Main");
            theMain.gameObject.SetActive(true);
            Transform theNNChallenge = Instantiate(theMain, theMain.parent);
            theNNChallenge.transform.SetSiblingIndex(1);
            theNNChallenge.name = "99Challenge";
            Transform modeText = gameObject.transform.Find("ModeText");
            theMain.localPosition -= new Vector3(0f, 48f, 0f);
            theNNChallenge.localPosition -= new Vector3(0f, 96f, 0f);
            TMP_Text mainText = theMain.GetComponent<TMP_Text>();
            TMP_Text nnText = theNNChallenge.GetComponent<TMP_Text>();
            mainText.GetComponent<TextLocalizer>().GetLocalizedText("Btn_InfFloors");
            f99Local = nnText.GetComponent<TextLocalizer>();
            f99Local.GetLocalizedText("Btn_NNChallenge");
            nnText.text = "99";
            nnText.color = Color.red;

            StandardMenuButton mainB = theMain.GetComponent<StandardMenuButton>();
            StandardMenuButton nnB = theNNChallenge.GetComponent<StandardMenuButton>();
            mainB.OnHighlight.RemoveAllListeners();
            nnB.OffHighlight.RemoveAllListeners();

            mainB.OnHighlight.AddListener(() => modeText.gameObject.GetComponent<TextLocalizer>().GetLocalizedText("Btn_InfFloors_Desc"));
            mainB.OnHighlight.AddListener(() => mode = "main");
            nnB.OnHighlight.AddListener(() => modeText.gameObject.GetComponent<TextLocalizer>().GetLocalizedText("Btn_NNChallenge_Desc"));
            nnB.OnHighlight.AddListener(() => mode = "nine nine");

            hideSeekMenuModed = hideSeekMenu.gameObject.AddComponent<HideSeekMenuModed>();
            hideSeekMenuModed.titleUI = this;
            StandardMenuButton startGameButton = hideSeekMenu.Find("MainNew").GetComponent<StandardMenuButton>();
            startGameButton.OnPress.RemoveAllListeners();
            startGameButton.OnPress.AddListener(() => {
                if (mode == "main")
                {
                    InfFloorMod.save = new Classes.EndlessSave();
                    gl.LoadLevel(InfFloorMod.currentSceneObject);
                    InfFloorMod.Instance.UpdateData(ref InfFloorMod.currentSceneObject);
                    Singleton<CoreGameManager>.Instance.currentMode = Mode.Main;
                }
                else if (mode == "nine nine")
                {
                    InfFloorMod.save = new Classes.EndlessSave();
                    InfFloorMod.save.currentFloor = 99;
                    Singleton<CoreGameManager>.Instance.AddPoints(99999, 0, false);
                    InfFloorMod.currentSceneObject.mapPrice = InfFloorMod.save.myFloorData.mapPrice;
                    InfFloorMod.Instance.UpdateData(ref InfFloorMod.currentSceneObject);
                    gl.LoadLevel(InfFloorMod.pitScene);
                    Singleton<CoreGameManager>.Instance.nextLevel = InfFloorMod.currentSceneObject;
                    Singleton<CoreGameManager>.Instance.lifeMode = LifeMode.Intense;
                    Singleton<CoreGameManager>.Instance.SetLives(0, true);
                    Singleton<CoreGameManager>.Instance.currentMode = InfFloorMod.NNFloorMode;
                }
            });
        }

        void Update()
        {
            if (!has_incre)
            {
                f99Local.GetLocalizedText("99");
                has_incre = true;
            }
        }
    }

    public class HideSeekMenuModed : MonoBehaviour
    {
        public EndlessTitleUI titleUI;

        void Update()
        {
            if (titleUI.mode == "main")
            {
                if (Singleton<PlayerFileManager>.Instance.savedGameData.saveAvailable)
                {
                    StandardMenuButton continueGameButton = transform.Find("MainContinue").GetComponent<StandardMenuButton>();

                    continueGameButton.gameObject.SetActive(true);
                }

            }
            else
            {
                StandardMenuButton continueGameButton = transform.Find("MainContinue").GetComponent<StandardMenuButton>();

                continueGameButton.gameObject.SetActive(false);
            }
        }

        void OnEnable()
        {
            Transform conButt = transform.Find("MainContinue");
            conButt.position = new Vector3(conButt.position.x, 200f, conButt.position.z);
        }
    }

    public class FloorChanger : MonoBehaviour
    {

    }
}
