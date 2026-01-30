using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InfFloorRemaster.Scripts;
using InfFloorRemaster.Patch;
using InfFloorRemaster.Classes;

namespace InfFloorRemaster.Patch
{
    [HarmonyPatch(typeof(MainGameManager))]
    [HarmonyPatch("LoadNextLevel")]
    class GenerateNext
    {
        static bool Prefix()
        {
            SceneObject endlessSceneObject = InfFloorMod.currentSceneObject;

            InfFloorMod.Instance.UpdateData(ref endlessSceneObject);
            InfFloorMod.save.currentFloor += 1;
            InfFloorMod.Instance.UpdateData(ref endlessSceneObject);
            GluePatch.EndFloor();
            return true;
        }
    }

    [HarmonyPatch(typeof(GameLoader))]
    [HarmonyPatch("LoadLevel")]
    class LoadLevel
    {
        static void Prefix(ref SceneObject sceneObject)
        {
            if (sceneObject.levelObject == null) return;
            InfFloorMod.Instance.UpdateData(ref sceneObject);
        }
    }

    [HarmonyPatch(typeof(EnvironmentController))]
    [HarmonyPatch("SetTimeLimit")]
    class TimeLimitChanger
    {
        static void Postfix(EnvironmentController __instance)
        {
            var timeLimitFieldInfo = AccessTools.Field(typeof(EnvironmentController), "timeLimit");
            var timeLimitFieldValue = timeLimitFieldInfo.GetValue(__instance);
            timeLimitFieldInfo.SetValue(__instance, (float)timeLimitFieldValue + InfFloorMod.currentFloorData.timeLimit);
        }
    }

    [HarmonyPatch(typeof(LevelGenerator))]
    [HarmonyPatch("StartGenerate")]
    class GenerateBegin
    {
        static List<T2> CreateWeightedShuffledListWithCount<T, T2>(List<T> list, int count, System.Random rng) where T : WeightedSelection<T2>
        {
            List<T2> newL = new List<T2>();
            List<T> selections = list.ToList();
            for (int i = 0; i < count; i++)
            {
                T2 selectedValue = (T2)AccessTools.Method(typeof(T), "ControlledRandomSelection").Invoke(null, new object[] { selections.ToArray(), rng });
                selections.RemoveAll(x => Equals(x.selection, selectedValue));
                newL.Add(selectedValue);
            }
            return newL;
        }

        static List<T> CreateShuffledListWithCount<T>(List<T> list, int count, System.Random rng)
        {
            count = Math.Min(list.Count, count);
            List<T> newList = new List<T>();
            List<T> copiedList = list.ToList();
            for (int i = 0; i < count; i++)
            {
                int selectedIndex = rng.Next(0, copiedList.Count);
                newList.Add(copiedList[selectedIndex]);
                copiedList.RemoveAt(selectedIndex);
            }
            return newList;
        }

        static void Prefix(LevelGenerator __instance)
        {
            FloorData currentFD = InfFloorMod.currentFloorData;
            GeneratorData genData = new GeneratorData();
            InfFloorMod.ExtendGenData(genData);

            SceneObject sceneObject = __instance.scene;
            LevelGenerationParameters lvlObj = __instance.ld;

            lvlObj.potentialItems = new WeightedItemObject[0];
            lvlObj.forcedNpcs = new NPC[0];
            sceneObject.potentialNPCs = new List<WeightedNPC>();

            __instance.seedOffset = currentFD.FloorID;

            System.Random rng = new System.Random(Singleton<CoreGameManager>.Instance.Seed() + __instance.seedOffset + currentFD.FloorID);

            lvlObj.minSize = new IntVector2(currentFD.minSize, currentFD.minSize);
            lvlObj.maxSize = new IntVector2(currentFD.maxSize, currentFD.maxSize);

            InfFloorMod.Log(0, $"Level min size set to {lvlObj.minSize.x}, {lvlObj.minSize.z}. Level max size {lvlObj.maxSize.x}, {lvlObj.maxSize.z}");

            lvlObj.potentialItems = genData.items.ToArray();
            lvlObj.forcedNpcs = genData.forcedNPCs.ToArray();
            sceneObject.potentialNPCs = new List<WeightedNPC>(genData.potentialNPCs);

            Color warmColor = new Color(180f / 255f, 150f / 255f, 100f / 255f);
            Color coldColor = new Color(80f / 255f, 100f / 255f, 150f / 255f);

            System.Random stableRng = new System.Random(Singleton<CoreGameManager>.Instance.Seed() + currentFD.FloorID);
            stableRng.Next();

            float coldLight = Mathf.Max(Mathf.Sin(currentFD.FloorID / (1f + (float)(rng.NextDouble() * 15f)) + stableRng.Next(-50, 50)), 0f);
            float warmLight = Mathf.Max(Mathf.Sin(currentFD.FloorID / (1f + (float)(rng.NextDouble() * 15f)) + stableRng.Next(-50, 50)), 0f);

            lvlObj.standardLightColor = Color.Lerp(Color.Lerp(Color.white, coldColor, coldLight), warmColor, warmLight);

            if (currentFD.FloorID % 99 == 0)
            {
                lvlObj.standardLightColor = Color.red;
            }

            float rgb = Mathf.Max(10f, 200f - currentFD.FloorID * 3f);
            lvlObj.standardDarkLevel = new Color(rgb / 255f, rgb / 255f, rgb / 255f);

            lvlObj.exitCount = currentFD.exitCount;

            lvlObj.standardLightStrength = Mathf.Max(Mathf.RoundToInt(3f / (currentFD.FloorID / 20f)), 2);

            lvlObj.maxLightDistance = Mathf.Max(rng.Next(4, Mathf.Clamp(Mathf.FloorToInt(currentFD.FloorID / 2), 4, 12)), InfFloorMod.refF3Scene.levelObject.maxLightDistance);

            List<WeightedRoomAsset> wra = CreateShuffledListWithCount(genData.classRoomAssets, 3 + Mathf.FloorToInt(currentFD.FloorID / 3), rng);

            wra.Do((x) =>
            {
                if (x.selection.hasActivity)
                {
                    x.weight = (int)Math.Ceiling(x.weight * (currentFD.FloorID * 0.1));
                }
            });

            RoomGroup classRoomGroup = lvlObj.roomGroup.First(x => x.name == "Class");
            classRoomGroup.potentialRooms = wra.ToArray();
            classRoomGroup.minRooms = currentFD.classRoomCount;
            classRoomGroup.maxRooms = currentFD.classRoomCount;

            RoomGroup facultyRoomGroup = lvlObj.roomGroup.First(x => x.name == "Faculty");
            facultyRoomGroup.minRooms = currentFD.minFacultyRoomCount;
            facultyRoomGroup.maxRooms = currentFD.maxFacultyRoomCount;
            facultyRoomGroup.potentialRooms = CreateShuffledListWithCount(genData.facultyRoomAssets, 4 + Mathf.FloorToInt(currentFD.FloorID / 4), rng).ToArray();

            RoomGroup officeRoomGroup = lvlObj.roomGroup.First(x => x.name == "Office");
            officeRoomGroup.maxRooms = Mathf.Max(currentFD.maxOffices, 1);
            officeRoomGroup.minRooms = 1;

            lvlObj.potentialStructures = new WeightedStructureWithParameters[0];
            lvlObj.potentialStructures = genData.potentialStructures;
            lvlObj.forcedStructures = new StructureWithParameters[0];
            lvlObj.forcedStructures = genData.forcedStructures;
            lvlObj.minSpecialBuilders = currentFD.minSpecialBuilders;
            lvlObj.maxSpecialBuilders = currentFD.maxSpecialBuilders;

            foreach(var i in lvlObj.potentialStructures)
            {
                InfFloorMod.Log(0, i.selection.prefab.name);
            }

            InfFloorMod.Log(0, $"min special builders {currentFD.minSpecialBuilders}. max special builders {currentFD.maxSpecialBuilders}");

            sceneObject.mapPrice = currentFD.mapPrice;
            lvlObj.maxItemValue = currentFD.maxItemValue;
            
            lvlObj.specialRoomsStickToEdge = currentFD.FloorID < 22 || currentFD.FloorID % 24 == 0;

            GeneratorManagement.Invoke("INF", currentFD.FloorID, sceneObject);
        }
    }
}
