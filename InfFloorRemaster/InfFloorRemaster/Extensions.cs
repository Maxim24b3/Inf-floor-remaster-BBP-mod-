using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace InfFloorRemaster
{
    public static class Extensions
    {
        public static SceneObject[] GetAllSceneObjects()
        {
            return Resources.FindObjectsOfTypeAll<SceneObject>();
        }

        public static SceneObject GetSceneObject(string name)
        {
            SceneObject[] scenes = Extensions.GetAllSceneObjects();
            return scenes.Where(x => x.levelTitle == name).First();
        }

        public static SceneObject CopyScene(SceneObject origin)
        {
            return Object.Instantiate(origin);
        }

        public static LevelObject GetLevelObjectForomScene(SceneObject sceneObject)
        {
            return sceneObject.levelObject;
        }

        public static LevelObject GetRandomLevelObjcetFromScene(SceneObject sceneObject, System.Random rng)
        {
            return WeightedSelection<LevelObject>.ControlledRandomSelection(sceneObject.randomizedLevelObject, rng);
        }

        public static LevelObject GetRandomLevelObjcetFromScene(SceneObject sceneObject, LevelType? type = null)
        {
            if (type != null)
                return sceneObject.randomizedLevelObject.First(x => x.selection.type == type).selection;

            if (sceneObject.randomizedLevelObject.Length != 0)
                return sceneObject.randomizedLevelObject[0].selection;

            return sceneObject.levelObject;
        }

        public static StructureWithParameters[] AddNewStructures(this StructureWithParameters[] ar, StructureWithParameters[] otherAr, params System.Type[] exceptTypes)
        {
            for (int i = 0; i < otherAr.Length; i++)
            {
                if (!exceptTypes.Contains(otherAr[i].prefab.GetType()) && !ar.Any(x => x.prefab.name == otherAr[i].prefab.name))
                    ar = ar.AddToArray(otherAr[i]);
            }
            return ar;
        }

        public static WeightedStructureWithParameters[] AddNewStructures(this WeightedStructureWithParameters[] ar, WeightedStructureWithParameters[] otherAr, params System.Type[] exceptTypes)
        {
            for (int i = 0; i < otherAr.Length; i++)
            {
                if (!exceptTypes.Contains(otherAr[i].selection.prefab.GetType()) && !ar.Any(x => x.selection.prefab.name == otherAr[i].selection.prefab.name))
                    ar = ar.AddToArray(otherAr[i]);
            }
            return ar;
        }

        public static WeightedRoomAsset[] AddNewSpecialRooms(this WeightedRoomAsset[] ar, WeightedRoomAsset[] otherAr)
        {
            for (int i = 0; i < otherAr.Length; i++)
            {
                if (!ar.Any(x => x.selection == otherAr[i].selection))
                    ar = ar.AddToArray(otherAr[i]);
            }
            return ar;
        }

        public static void AddNewAssetsToWeightedRoomList(this List<WeightedRoomAsset> list, RoomGroup roomGroup)
        {
            for (int i = 0; i < roomGroup.potentialRooms.Length; i++)
            {
                list.Add(roomGroup.potentialRooms[i]);
            }
        }

        public static SwapTo SwapComponent<SwapFrom, SwapTo>(this GameObject me, SwapFrom component, bool swapReferencesOnSameObject = false) where SwapFrom : MonoBehaviour where SwapTo : SwapFrom
        {
            SwapTo newComponent = me.AddComponent<SwapTo>();

            // Копируем все поля из старого компонента в новый
            FieldInfo[] fields = typeof(SwapFrom).GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance
            );

            foreach (FieldInfo field in fields)
            {
                field.SetValue(newComponent, field.GetValue(component));
            }

            // Если нужно, заменяем ссылки на тот же объект
            if (swapReferencesOnSameObject)
            {
                foreach (MonoBehaviour mono in me.GetComponents<MonoBehaviour>())
                {
                    if (mono == component)
                        continue;

                    fields = mono.GetType().GetFields(
                        BindingFlags.Public |
                        BindingFlags.NonPublic |
                        BindingFlags.Instance
                    );

                    foreach (FieldInfo field in fields)
                    {
                        if (field.FieldType == typeof(SwapFrom) && field.GetValue(mono) == component)
                        {
                            field.SetValue(mono, newComponent);
                        }
                    }
                }
            }

            Object.DestroyImmediate(component);

            return newComponent;
        }
    }
}
