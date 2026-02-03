using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using InfFloorRemaster.Classes;

namespace InfFloorRemaster.Scripts.Game.Components
{
    public class UpgradesMenuMnager : MonoBehaviour
    {
        private StandardUpgrade[] slots = new StandardUpgrade[8];
        private StandardUpgrade rerollSlot = new StandardUpgrade();
        public StandardMenuButton[] purchaseUpgradeButtons = new StandardMenuButton[8];
        public StandardMenuButton[] inventorySlotsUpgradeButtons = new StandardMenuButton[5];
        public StandardMenuButton rerollButton = null;
        public TextMeshProUGUI descText;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI priceText;

        private void OnEnable()
        {
            CursorInitiator cursorInitiator = GetComponent<CursorInitiator>();
            cursorInitiator.useRawPosition = true;
            Initialize();
        }

        public void Initialize()
        {
            StandardMenuButton backButton = transform.Find("BackButton").GetComponent<StandardMenuButton>();
            backButton.OnPress.RemoveAllListeners();
            backButton.OnPress.AddListener(() => Destroy(gameObject));
            backButton.OnPress.AddListener(() => Singleton<CoreGameManager>.Instance.Pause(false));
            for (int i = 0; i < purchaseUpgradeButtons.Length; i ++)
            {
                int currentIndex = i;
                purchaseUpgradeButtons[i].OnPress.AddListener(() => PressButton(currentIndex));
                purchaseUpgradeButtons[i].OnHighlight.AddListener(() => GetInfo(currentIndex));
                purchaseUpgradeButtons[i].eventOnHigh = true;
            }
            for (int i = 0; i < inventorySlotsUpgradeButtons.Length; i ++)
            {
                int currentIndex = i;
                inventorySlotsUpgradeButtons[currentIndex].OnPress.AddListener(() => InfFloorMod.save.SellUpgrade(InfFloorMod.save.Upgrades[currentIndex].id));
                inventorySlotsUpgradeButtons[currentIndex].OnPress.AddListener(() => UpdateItems());
            }
            rerollButton.OnPress.AddListener(() => PressButton(9));
            rerollButton.transform.Find("Icon").GetComponent<Image>().sprite = InfFloorMod.Instance.assetManager.Get<Sprite>("Reroll");
            Reroll();
        }

        public void Reroll()
        {
            for (int i = 0; i < slots.Length; i ++)
            {
                slots[i] = GetRandomValidUpgrade(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            }

            UpdateItems();
        }

        private void UpdateIcons()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (purchaseUpgradeButtons[i].gameObject.activeSelf == false) continue;
                Transform iconTransform = purchaseUpgradeButtons[i].transform.Find("Icon");
                if (slots[i] != null)
                {
                    iconTransform.GetComponent<Image>().sprite = slots[i].GetIcon(InfFloorMod.save.GetUpgradeCount(slots[i].id));
                }
                else
                {
                    iconTransform.GetComponent<Image>().sprite = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(x => x.name == "Transparent");
                }
            }
            for (int i = 0; i < inventorySlotsUpgradeButtons.Length; i++)
            {
                if (inventorySlotsUpgradeButtons[i].gameObject.activeSelf == false) continue;
                Transform iconTransform = inventorySlotsUpgradeButtons[i].transform.Find("Icon");
                if (InfFloorMod.save.Upgrades[i].id != "none" && InfFloorMod.save.Upgrades[i].count > 0)
                {
                    iconTransform.GetComponent<Image>().sprite = InfFloorMod.Upgrades[InfFloorMod.save.Upgrades[i].id].GetIcon(InfFloorMod.save.Upgrades[i].count - 1);
                }
                else
                {
                    iconTransform.GetComponent<Image>().sprite = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(x => x.name == "Transparent");
                }
            }
        }

        private void UpdateItems()
        {
            for (int i = 0; i < slots.Length; i ++)
            {
                if (slots[i] != null && slots[i].ShouldAppear(InfFloorMod.save.GetUpgradeCount(slots[i].id)) == false)
                {
                    slots[i] = null;
                }

                if (slots[i] == null)
                {
                    purchaseUpgradeButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    purchaseUpgradeButtons[i].gameObject.SetActive(true);
                }
            }
            for (int i = 0; i < inventorySlotsUpgradeButtons.Length; i++)
            {
                if (InfFloorMod.save.Upgrades[i].id == "none" || InfFloorMod.save.Upgrades[i].count <= 0)
                {
                    inventorySlotsUpgradeButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    inventorySlotsUpgradeButtons[i].gameObject.SetActive(true);
                }
            }
            UpdateIcons();
        }

        public void PurchaseUpgrade(int slot)
        {
            if (slots[slot] == null) return;
            if (InfFloorMod.save.CanPurchaseUpgrade(slots[slot], slots[slot].behavior) && Singleton<CoreGameManager>.Instance.GetPoints(0) >= slots[slot].GetCost(InfFloorMod.save.GetUpgradeCount(slots[slot].id)))
            {
                InfFloorMod.save.PurchaseUpgrade(slots[slot], slots[slot].behavior);
                Singleton<CoreGameManager>.Instance.AddPoints(0 - slots[slot].GetCost(InfFloorMod.save.GetUpgradeCount(slots[slot].id)) , 0, true);
                slots[slot] = null;
                UpdateItems();
            }
        }

        public void GetInfo(int slot)
        {
            if (slots[slot] == null)
            {
                priceText.text = "Price: N/A";
                nameText.text = "N/A";
                descText.text = "N/A";
                return;
            }

            priceText.text = $"Price: {slots[slot].GetCost(InfFloorMod.save.GetUpgradeCount(slots[slot].id))}";
            nameText.text = $"{slots[slot].id}";
            descText.text = $"{slots[slot].GetLoca(InfFloorMod.save.GetUpgradeCount(slots[slot].id))}";
        }

        public void PressButton(int id)
        {
            Debug.Log("Button id: " + id.ToString());
            if (id < 8)
            {
                PurchaseUpgrade(id);
                return;
            }
            Reroll();
        }

        public static StandardUpgrade GetRandomValidUpgrade(int seed)
        {
            List<WeightedSelection<StandardUpgrade>> upgradesTemp = new List<WeightedSelection<StandardUpgrade>>();
            InfFloorMod.Upgrades.Do(x =>
            {
                if (x.Key == "none") return;
                if (x.Value.weight == 0) return;
                if (x.Key == "reroll") return;
                if (!x.Value.ShouldAppear(InfFloorMod.save.GetUpgradeCount(x.Value.id))) return;
                upgradesTemp.Add(new WeightedSelection<StandardUpgrade>()
                {
                    selection = x.Value,
                    weight = x.Value.weight
                });
            });
            WeightedSelection<StandardUpgrade>[] weightedUpgrades = upgradesTemp.ToArray();
            if (weightedUpgrades.Length == 0) return null;
            var value = WeightedSelection<StandardUpgrade>.ControlledRandomSelection(weightedUpgrades, new System.Random(seed));
            Debug.Log(value.id);
            return value;
        }
    }
}
