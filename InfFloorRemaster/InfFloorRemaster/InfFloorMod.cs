using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using InfFloorRemaster.Classes;
using InfFloorRemaster.Patch;
using InfFloorRemaster.Scripts;
using InfFloorRemaster.Scripts.Game.Builders;
using InfFloorRemaster.Scripts.Game.Components;
using InfFloorRemaster.Scripts.Game.Objects;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace InfFloorRemaster
{
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    [BepInPlugin(ModInfos.GUID, ModInfos.Name, ModInfos.Ver)]
    public class InfFloorMod : BaseUnityPlugin
    {
        public static InfFloorMod Instance { get; private set; }
        public static Dictionary<PluginInfo, Action<GeneratorData>> genActions = new Dictionary<PluginInfo, Action<GeneratorData>>();
        public static SceneObject currentSceneObject;
        public static EndlessSave save = new EndlessSave();
        public static FloorData currentFloorData => save.myFloorData;
        public static SceneObject refScene;
        public static SceneObject refF3Scene;
        public AssetManager assetManager = new AssetManager();
        public Dictionary<string ,Sprite> UpgradeIcons = new Dictionary<string, Sprite>();
        public static Dictionary<string, StandardUpgrade> Upgrades = new Dictionary<string, StandardUpgrade>();

        public static void AddGeneratorAction(PluginInfo info, Action<GeneratorData> data)
        {
            if (genActions.ContainsKey(info))
            {
                throw new Exception("Can't add already existing generator action!");
            }
            genActions.Add(info, data);
        }

        private void Awake()
        {
            Log(0, $"InfFloors mod loaded. Version: {ModInfos.Ver}");
            Harmony harmony = new Harmony(ModInfos.GUID);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Instance = this;
            StartCoroutine(WaitTilAllLoaded(harmony));
            MTM101BaldAPI.SaveSystem.ModdedSaveGame.AddSaveHandler(new EndlessFloorsSaveHandler());
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterImportant, LoadingEventOrder.Pre);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "MainMenu")
            {
                GameObject[] rootObjects = scene.GetRootGameObjects();
                GameObject pickModeMenu = rootObjects.Where(x => x.name == "PickMode").First();
                GameObject hideSeekMenuObject = rootObjects.Where(x => x.name == "HideSeekMenu").First();
                pickModeMenu.AddComponent<EndlessTitleUI>();
                HideSeekMenuPatch hideSeekMenu = hideSeekMenuObject.AddComponent<HideSeekMenuPatch>();
                hideSeekMenu.gl = rootObjects.Where(x => x.name == "GameLoader").First().GetComponent<GameLoader>();
            }
        }

        private IEnumerator WaitTilAllLoaded(Harmony harmony)
        {
            FieldInfo loaded = AccessTools.Field(typeof(Chainloader), "_loaded");

            while (!(bool)loaded.GetValue(loaded))
            {
                yield return null;
            }
            harmony.PatchAllConditionals();
        }

        private void RegisterImportant()
        {
            SceneObject[] scenes = Extensions.GetAllSceneObjects();
            refF3Scene = Extensions.CopyScene(scenes.Where(x => x.levelTitle == "F3").First());
            currentSceneObject = scenes.Where(x => x.levelTitle == "F3").First();
            refScene = scenes.Where(x => x.levelTitle == "F5").First();
            currentSceneObject.nextLevel = currentSceneObject;
            currentSceneObject.levelTitle = "F1";
            currentSceneObject.levelNo = 0;
            assetManager.AddRange<HappyBaldi>(Resources.FindObjectsOfTypeAll<HappyBaldi>(), (bald) =>
            {
                if (bald.name == "HappyBaldi") return "HappyBaldi1";
                return bald.name;
            });
            GameObject levelSkiper = new GameObject();
            levelSkiper.AddComponent<Cheats>();
            LevelAsset pit = Extensions.GetSceneObject("PIT").levelAsset;
            Structure_PitStopLevelUpgradeMachiene upgradeMachineBuilder = new GameObject().AddComponent<Structure_PitStopLevelUpgradeMachiene>();
            upgradeMachineBuilder.gameObject.ConvertToPrefab(true);
            pit.structures.Add(new StructureBuilderData
            {
                prefab = upgradeMachineBuilder
            });
            assetManager.Add<Sprite>("UpgradesMachineSprite", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "UpgradesMachine.png"), 100));
            assetManager.Add<Sprite>("AutoTag", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, Path.Combine("Upgrades", "AutoTag.png")), 100));
            assetManager.Add<Sprite>("AutoTag2", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, Path.Combine("Upgrades", "AutoTag2.png")), 100));
            assetManager.Add<Sprite>("Bank1", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, Path.Combine("Upgrades", "Bank1.png")), 100));
            assetManager.Add<Sprite>("Bank2", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, Path.Combine("Upgrades", "Bank2.png")), 100));
            assetManager.Add<Sprite>("Bank3", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, Path.Combine("Upgrades", "Bank3.png")), 100));
            assetManager.Add<Sprite>("Bank5", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, Path.Combine("Upgrades", "Bank5.png")), 100));
            assetManager.Add<Sprite>("Bank6", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, Path.Combine("Upgrades", "Bank6.png")), 100));
            assetManager.Add<Sprite>("Bank10", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, Path.Combine("Upgrades", "Bank10.png")), 100));
            assetManager.Add<Sprite>("Reroll", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, Path.Combine("Upgrades", "Reroll.png")), 100));
            assetManager.Add<Sprite>("Glue1", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, Path.Combine("Upgrades", "Glue1.png")), 100));
            assetManager.Add<Sprite>("Glue2", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, Path.Combine("Upgrades", "Glue2.png")), 100));
            assetManager.Add<Sprite>("Glue3", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, Path.Combine("Upgrades", "Glue3.png")), 100));
            assetManager.Add<Sprite>("Glue4", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, Path.Combine("Upgrades", "Glue4.png")), 100));
            UpgradeIcons.Add("AutoTag", assetManager.Get<Sprite>("AutoTag"));
            UpgradeIcons.Add("AutoTag2", assetManager.Get<Sprite>("AutoTag2"));
            UpgradeIcons.Add("Bank1", assetManager.Get<Sprite>("Bank1"));
            UpgradeIcons.Add("Bank2", assetManager.Get<Sprite>("Bank2"));
            UpgradeIcons.Add("Bank3", assetManager.Get<Sprite>("Bank3"));
            UpgradeIcons.Add("Bank5", assetManager.Get<Sprite>("Bank5"));
            UpgradeIcons.Add("Bank6", assetManager.Get<Sprite>("Bank6"));
            UpgradeIcons.Add("Bank10", assetManager.Get<Sprite>("Bank10"));
            UpgradeIcons.Add("Reroll", assetManager.Get<Sprite>("Reroll"));
            UpgradeIcons.Add("Glue1", assetManager.Get<Sprite>("Glue1"));
            UpgradeIcons.Add("Glue2", assetManager.Get<Sprite>("Glue2"));
            UpgradeIcons.Add("Glue3", assetManager.Get<Sprite>("Glue3"));
            UpgradeIcons.Add("Glue4", assetManager.Get<Sprite>("Glue4"));
            UpgradeRegister.RegisterDefaults();
            RegisterCanvases();
            RegisterObjects();
        }

        private void RegisterCanvases()
        {
            #region upgradeCanvas
            Canvas upgradeCanvas = UIHelpers.CreateBlankUIScreen("Upgarde menu", false);
            GameObject bgObject = new GameObject("BG");
            bgObject.transform.SetParent(upgradeCanvas.transform);
            Image BG = bgObject.AddComponent<Image>();
            BG.rectTransform.anchorMin = new Vector2(-25f, -25f);
            BG.rectTransform.anchorMax = Vector2.one * 25;
            BG.rectTransform.offsetMin = Vector2.zero;
            BG.rectTransform.offsetMax = Vector2.zero;
            TextMeshProUGUI backText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans12, "back", upgradeCanvas.transform, new Vector3(-150, 150, 0), false);
            backText.color = Color.black;
            backText.rectTransform.sizeDelta = new Vector2(50, 30);
            StandardMenuButton backButton = UIExtensions.ConvertToButton<StandardMenuButton>(backText.gameObject, true);
            backButton.name = "BackButton";
            backButton.underlineOnHigh = true;
            backText.raycastTarget = true;
            UpgradesMenuMnager umm = upgradeCanvas.gameObject.AddComponent<UpgradesMenuMnager>();

            StandardMenuButton upgradeButton = null;

            #region Upgrade buttons prefab creation
            GameObject upgradeButtonPre = new GameObject("Button");
            upgradeButtonPre.transform.SetParent(upgradeCanvas.transform);
            Image upgradeButtonImage = upgradeButtonPre.AddComponent<Image>();
            upgradeButtonImage.raycastTarget = true;
            GameObject upgradeButtonIconPre = new GameObject("Icon");
            upgradeButtonIconPre.transform.SetParent(upgradeButtonPre.transform);
            Image upgradeButtonIcon = upgradeButtonIconPre.AddComponent<Image>();
            upgradeButtonIcon.raycastTarget = false;
            upgradeButtonIcon.sprite = UpgradeIcons["AutoTag"];
            upgradeButtonPre.layer = LayerMask.NameToLayer("UI");
            upgradeButtonIconPre.layer = LayerMask.NameToLayer("UI");
            upgradeButton = upgradeButtonPre.ConvertToButton<StandardMenuButton>();
            upgradeButtonPre.transform.localScale = upgradeButtonIconPre.transform.localScale / 10;
            upgradeButtonPre.ConvertToPrefab(true);
            #endregion
            #region Create upgrade buttons
            float x1 = -150;
            float x2 = -100;
            float x3 = -50;
            float y1 = 0;
            float y2 = -50;
            float y3 = -100;
            StandardMenuButton currentButton = Instantiate(upgradeButton, upgradeCanvas.transform);
            currentButton.transform.localPosition = new Vector3(x1, y1, 0);
            umm.purchaseUpgradeButtons[0] = currentButton;

            currentButton = Instantiate(upgradeButton, upgradeCanvas.transform);
            currentButton.transform.localPosition = new Vector3(x2, y1, 0);
            umm.purchaseUpgradeButtons[1] = currentButton;

            currentButton = Instantiate(upgradeButton, upgradeCanvas.transform);
            currentButton.transform.localPosition = new Vector3(x3, y1, 0);
            umm.purchaseUpgradeButtons[2] = currentButton;

            currentButton = Instantiate(upgradeButton, upgradeCanvas.transform);
            currentButton.transform.localPosition = new Vector3(x1, y2, 0);
            umm.purchaseUpgradeButtons[3] = currentButton;

            currentButton = Instantiate(upgradeButton, upgradeCanvas.transform);
            currentButton.transform.localPosition = new Vector3(x2, y2, 0);
            umm.purchaseUpgradeButtons[4] = currentButton;

            currentButton = Instantiate(upgradeButton, upgradeCanvas.transform);
            currentButton.transform.localPosition = new Vector3(x3, y2, 0);
            umm.purchaseUpgradeButtons[5] = currentButton;

            currentButton = Instantiate(upgradeButton, upgradeCanvas.transform);
            currentButton.transform.localPosition = new Vector3(x1, y3, 0);
            umm.purchaseUpgradeButtons[6] = currentButton;

            currentButton = Instantiate(upgradeButton, upgradeCanvas.transform);
            currentButton.transform.localPosition = new Vector3(x2, y3, 0);
            umm.purchaseUpgradeButtons[7] = currentButton;

            currentButton = Instantiate(upgradeButton, upgradeCanvas.transform);
            currentButton.transform.localPosition = new Vector3(x3, y3, 0);
            umm.rerollButton = currentButton;

            currentButton = Instantiate(upgradeButton, upgradeCanvas.transform);
            currentButton.transform.localPosition = new Vector3(-100, 150, 0);
            umm.inventorySlotsUpgradeButtons[0] = currentButton;

            currentButton = Instantiate(upgradeButton, upgradeCanvas.transform);
            currentButton.transform.localPosition = new Vector3(-50, 150, 0);
            umm.inventorySlotsUpgradeButtons[1] = currentButton;

            currentButton = Instantiate(upgradeButton, upgradeCanvas.transform);
            currentButton.transform.localPosition = new Vector3(0, 150, 0);
            umm.inventorySlotsUpgradeButtons[2] = currentButton;

            currentButton = Instantiate(upgradeButton, upgradeCanvas.transform);
            currentButton.transform.localPosition = new Vector3(50, 150, 0);
            umm.inventorySlotsUpgradeButtons[3] = currentButton;

            currentButton = Instantiate(upgradeButton, upgradeCanvas.transform);
            currentButton.transform.localPosition = new Vector3(100, 150, 0);
            umm.inventorySlotsUpgradeButtons[4] = currentButton;
            #endregion
            TextMeshProUGUI priceText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, "Price:", upgradeCanvas.transform, new Vector3(100, 50, 0), false);
            priceText.color = Color.black;
            umm.priceText = priceText;
            TextMeshProUGUI nameText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, "", upgradeCanvas.transform, new Vector3(100, 25, 0), false);
            nameText.color = Color.black;
            umm.nameText = nameText;
            TextMeshProUGUI descText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, "", upgradeCanvas.transform, new Vector3(100, 0, 0), false);
            descText.color = Color.black;
            umm.descText = descText;
            UIHelpers.AddBordersToCanvas(upgradeCanvas);
            var cursorController = Resources.FindObjectsOfTypeAll<CursorController>().FirstOrDefault(x => x.name == "CursorOrigin");
            UIHelpers.AddCursorInitiatorToCanvas(upgradeCanvas, new Vector2(480, 360), cursorController);
            upgradeCanvas.gameObject.ConvertToPrefab(true);
            assetManager.Add("UpgradeMenu", upgradeCanvas);
            #endregion
        }

        private void RegisterObjects()
        {
            #region upgradeMachine
            GameObject upgradesMachine = new GameObject("Upgrade machine");
            upgradesMachine.AddComponent<UpgradeMachine>();
            BoxCollider collider = upgradesMachine.AddComponent<BoxCollider>();
            collider.size = new Vector3(2, 50, 2);
            collider.isTrigger = true;
            SpriteRenderer upgradesMachineSprite = new GameObject("Sprite").AddComponent<SpriteRenderer>();
            upgradesMachineSprite.sprite = assetManager.Get<Sprite>("UpgradesMachineSprite");
            upgradesMachineSprite.gameObject.AddComponent<BillboardUpdater>();
            upgradesMachineSprite.transform.parent = upgradesMachine.transform;
            upgradesMachineSprite.transform.localScale *= 5;
            upgradesMachineSprite.transform.position = new Vector3(0, 2.5f, 0);
            upgradesMachineSprite.sharedMaterials = new Material[1]
            {
                Resources.FindObjectsOfTypeAll<Material>().FirstOrDefault(m => m.name == "SpriteStandard_Billboard")
            };
            upgradesMachine.gameObject.ConvertToPrefab(true);
            assetManager.Add("UpgradesMachine", upgradesMachine.GetComponent<UpgradeMachine>());
            #endregion
        }

        internal static void Log(int id, object obj)
        {
            switch (id)
            {
                case 0:
                    Debug.Log($"InfFloors: {obj}");
                    break;
                case 1:
                    Debug.LogWarning($"InfFloors: {obj}");
                    break;
                case 2:
                    Debug.LogError($"InfFloors: {obj}");
                    break;
                default:
                    Debug.Log($"InfFloors: {obj}");
                    break;
            }

        }

        internal void UpdateData(ref SceneObject sceneObject)
        {
            sceneObject = currentSceneObject;
            sceneObject.levelNo = save.currentFloor;
            sceneObject.nextLevel = currentSceneObject;
            sceneObject.levelTitle = "F" + save.currentFloor;
            Singleton<CoreGameManager>.Instance.levelMapHasBeenPurchasedFor = null;
            Singleton<CoreGameManager>.Instance.tripAvailable = true;
            System.Random random = new System.Random(save.currentFloor + Singleton<CoreGameManager>.Instance.Seed());
            random.Next();
            sceneObject.manager.ReflectionSetVariable("happyBaldiPre", assetManager[typeof(HappyBaldi), "HappyBaldi" + random.Next(1, 3)]);
        }

        internal static void ExtendGenData(GeneratorData genData)
        {
            ItemMetaStorage items = new ItemMetaStorage();
            NPCMetaStorage npcs = new NPCMetaStorage();
            //NPCs
            genData.additionalNPCs = 8;
            genData.potentialNPCs = refScene.potentialNPCs;

            foreach (var fn in refScene.forcedNpcs)
            {
                genData.forcedNPCs.Add(fn);
            }
            //Items
            WeightedItemObject[] addItems = Extensions.GetRandomLevelObjcetFromScene(refScene, LevelType.Maintenance).potentialItems;
            foreach (WeightedItemObject i in addItems)
            {
                genData.items.Add(i);
            }
            List<WeightedRoomAsset> weightedRoomAssets = new List<WeightedRoomAsset>();
            //Class rooms
            weightedRoomAssets.Clear();
            weightedRoomAssets.AddNewAssetsToWeightedRoomList(Extensions.GetSceneObject("F1").levelObject.roomGroup.First(x => x.name == "Class"));
            weightedRoomAssets.AddNewAssetsToWeightedRoomList(Extensions.GetSceneObject("F2").levelObject.roomGroup.First(x => x.name == "Class"));
            genData.classRoomAssets.AddRange(weightedRoomAssets.ToArray());
            //Faculty rooms
            weightedRoomAssets.Clear();
            weightedRoomAssets.AddNewAssetsToWeightedRoomList(Extensions.GetSceneObject("F1").levelObject.roomGroup.First(x => x.name == "Faculty"));
            weightedRoomAssets.AddNewAssetsToWeightedRoomList(Extensions.GetSceneObject("F2").levelObject.roomGroup.First(x => x.name == "Faculty"));
            genData.facultyRoomAssets.AddRange(weightedRoomAssets.ToArray());
            //Office rooms
            weightedRoomAssets.Clear();
            weightedRoomAssets.AddNewAssetsToWeightedRoomList(Extensions.GetRandomLevelObjcetFromScene(refScene, LevelType.Factory).roomGroup.First(x => x.name == "Office"));
            genData.officeRoomAssets.AddRange(weightedRoomAssets.ToArray());
            //Structures
            genData.forcedStructures = genData.forcedStructures.AddNewStructures(Extensions.GetSceneObject("F2").levelObject.forcedStructures);
            genData.potentialStructures = genData.potentialStructures.AddNewStructures(Extensions.GetSceneObject("F2").levelObject.potentialStructures);

            List<WeightedStructureWithParameters> list = new List<WeightedStructureWithParameters>();

            list.AddRange(genData.potentialStructures);

            for(int i = 0; i < list.Count; i ++)
            {
                if (list[i].selection.prefab.name == "PowerLeverConstructor")
                {
                    list.RemoveAt(i);
                }
            }

            genData.potentialStructures = new WeightedStructureWithParameters[0];
            genData.potentialStructures = list.ToArray();

            StructureWithParameters vent = new StructureWithParameters()
            {
                prefab = Resources.FindObjectsOfTypeAll<Structure_Vent>().First(x => x.GetInstanceID() > 0),
                parameters = new StructureParameters()
                {
                    minMax = new IntVector2[]
                    {
                            new IntVector2(1, 35), // z defines how many vent iterations will spawn
							new IntVector2(4, 6), // Min max to tell how many corners the vent will have in its path (like, 5 segments before going straight).
							new IntVector2(15, 0) // uses x axis to tell how far the vent's exit needs to be from the entrance
                    }
                }
            };

            WeightedStructureWithParameters[] customSturctures = new WeightedStructureWithParameters[]
            {
                new WeightedStructureWithParameters()
                {
                    selection = vent,
                    weight = 51
                }
            };

            genData.potentialStructures = genData.potentialStructures.AddNewStructures(customSturctures);
        }
    }

    internal static class ModInfos
    {
        public const string Name = "Inf floors remaster";
        public const string GUID = "maximski24.IFR";
        public const string Ver = "1.0.0";
    }
}
