using HarmonyLib;
using MTM101BaldAPI.SaveSystem;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
            nnB.OnHighlight.RemoveAllListeners();

            mainB.OnHighlight.AddListener(() => modeText.gameObject.GetComponent<TextLocalizer>().GetLocalizedText("Btn_InfFloors_Desc"));
            mainB.OnHighlight.AddListener(() => mode = "main");
            nnB.OnHighlight.AddListener(() => modeText.gameObject.GetComponent<TextLocalizer>().GetLocalizedText("Btn_NNChallenge_Desc"));
            nnB.OnHighlight.AddListener(() => mode = "nine nine");

            hideSeekMenuModed = hideSeekMenu.gameObject.AddComponent<HideSeekMenuModed>();
            hideSeekMenuModed.titleUI = this;

            StandardMenuButton startGameButton = hideSeekMenu.Find("MainNew").GetComponent<StandardMenuButton>();
            startGameButton.OnPress.RemoveAllListeners();
            startGameButton.OnPress.AddListener(() => StartGame());
        }

        private void StartGame()
        {
            gl.gameObject.SetActive(true);
            gl.CheckSeed();

            if (mode == "main")
            {
                InfFloorMod.save = new Classes.EndlessSave();
                InfFloorMod.save.currentFloor = 1;
                InfFloorMod.currentSceneObject = InfFloorMod.refF3Scene;
                InfFloorMod.Instance.UpdateData(ref InfFloorMod.currentSceneObject);

                gl.Initialize(2);
                gl.SetMode((int)Mode.Main);

                ElevatorScreen evl = GameObject.FindObjectOfType<ElevatorScreen>();
                gl.AssignElevatorScreen(evl);
                evl.gameObject.SetActive(true);
                gl.LoadLevel(InfFloorMod.currentSceneObject);
                evl.Initialize();
                gl.SetSave(true);
            }
            else if (mode == "nine nine")
            {
                gl.Initialize(0);
                gl.SetMode((int)Mode.Main);
                InfFloorMod.save = new Classes.EndlessSave();
                InfFloorMod.save.currentFloor = 99;
                Singleton<CoreGameManager>.Instance.AddPoints(99999, 0, false);
                InfFloorMod.currentSceneObject.mapPrice = InfFloorMod.save.myFloorData.mapPrice;
                InfFloorMod.Instance.UpdateData(ref InfFloorMod.currentSceneObject);
                ElevatorScreen evl = GameObject.FindObjectOfType<ElevatorScreen>();
                gl.AssignElevatorScreen(evl);
                evl.gameObject.SetActive(true);
                gl.LoadLevel(InfFloorMod.pitScene);
                evl.Initialize();
                Singleton<CoreGameManager>.Instance.nextLevel = InfFloorMod.currentSceneObject;
                Singleton<CoreGameManager>.Instance.lifeMode = LifeMode.Intense;
                Singleton<CoreGameManager>.Instance.SetLives(0, true);
                Singleton<CoreGameManager>.Instance.currentMode = InfFloorMod.NNFloorMode;
                Singleton<CoreGameManager>.Instance.currentMode = Mode.Main;

                gl.SetSave(false);
            }
        }
        
        public void StartGameSave()
        {
            gl.gameObject.SetActive(true);
            gl.CheckSeed();
            gl.SetMode((int)Mode.Main);
            Singleton<CursorManager>.Instance.LockCursor();
            ElevatorScreen evl = GameObject.FindObjectOfType<ElevatorScreen>();
            gl.AssignElevatorScreen(evl);
            evl.gameObject.SetActive(true);
            gl.LoadSavedGame();
            evl.Initialize();
            Singleton<ModdedFileManager>.Instance.DeleteSavedGame();
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
            StandardMenuButton continueGameButton = transform.Find("MainContinue").GetComponent<StandardMenuButton>();
            continueGameButton.transform.position = new Vector3(continueGameButton.transform.position.x, 200f, continueGameButton.transform.position.z);
            continueGameButton.OnPress.RemoveAllListeners();
            continueGameButton.OnPress.AddListener(() => titleUI.StartGameSave());
        }
    }
}