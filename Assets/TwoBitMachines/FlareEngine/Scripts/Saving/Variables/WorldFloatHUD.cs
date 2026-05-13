using System;
using System.Collections.Generic;
using TMPro;
using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TwoBitMachines.FlareEngine
{
    public class WorldFloatHUD : MonoBehaviour
    {
        [SerializeField] public FloatHUDType type;
        [SerializeField] public UnityEvent onValueChanged;
        [SerializeField] public WorldFloat worldFloat;
        [SerializeField] public WorldFloatSO worldFloatSO;
        [SerializeField] public Projectile projectile;
        [SerializeField] public Tool tool;
        [SerializeField] public Firearms fireArm;

        //DiscreteItems
        [SerializeField] public Sprite iconFull;
        [SerializeField] public Sprite iconEmpty;
        [SerializeField] public List<Image> icons = new();
        [SerializeField] public int startValue = 3;
        [SerializeField] public string discreteSaveKey = "";
        [SerializeField] public bool canIncrease;
        [SerializeField] public bool saveDiscreteManually;

        [SerializeField] private SaveFloat saveFloat = new();

        //Continuous
        [SerializeField] public Image bar;
        [SerializeField] public float maxValue;

        //Numbers
        [SerializeField] public TextMeshProUGUI textNumbers;

        [SerializeField] public bool eventFoldOut;
        [NonSerialized] private float previousValue = -1;
        [NonSerialized] private int tempStartValue = 3;

        public void Update()
        {
            if (IsNull() || previousValue == Value()) return;

            var value = Value();
            previousValue = value;

            if (type == FloatHUDType.DiscreteItems)
            {
                var items = (int)value;
                for (var i = 0; i < icons.Count; i++)
                {
                    if (icons[i] == null) continue;
                    if (canIncrease && i + 1 > tempStartValue)
                    {
                        icons[i].gameObject.SetActive(false);
                        continue;
                    }

                    var active = i + 1 <= items;
                    icons[i].sprite = active ? iconFull : iconEmpty;

                    if (!icons[i].gameObject.activeInHierarchy) icons[i].gameObject.SetActive(true);
                }
            }

            if (type == FloatHUDType.Continuous)
            {
                if (bar == null) return;
                bar.fillAmount = value / maxValue;
            }

            if (type == FloatHUDType.Numbers)
            {
                if (textNumbers == null) return;
                var items = (int)Mathf.Clamp(value, 0, float.MaxValue);
                textNumbers.text = items < 10 ? "0" + items : items.ToString();
            }

            onValueChanged.Invoke();
        }

        public void OnEnable()
        {
            if (canIncrease) Restore();
            previousValue = -1;
            Update();
            WorldManager.get.worldResetAll -= ResetTempValue;
            WorldManager.get.worldResetAll += ResetTempValue;
        }

        private void Restore()
        {
            saveFloat.value = startValue;
            startValue = (int)Storage.Load(saveFloat, WorldManager.saveFolder, discreteSaveKey).value;
            tempStartValue = startValue;
        }

        private float Value()
        {
            if (worldFloat != null) return worldFloat.GetValue();
            if (worldFloatSO != null) return worldFloatSO.GetValue();
            if (projectile != null) return projectile.projectile.ammunition.ammunition;
            if (fireArm != null) return fireArm.Ammunition();
            if (tool != null) return tool.ToolValue();
            return 0;
        }

        private bool IsNull()
        {
            return worldFloat == null && worldFloatSO == null && projectile == null && fireArm == null && tool == null;
        }

        public void IncreaseDiscreteValue()
        {
            tempStartValue++;
            previousValue = -1;
            if (!saveDiscreteManually) SaveDiscrete();
        }

        public void ResetTempValue()
        {
            if (saveDiscreteManually)
            {
                Restore();
                previousValue = -1;
            }
        }

        public void SaveDiscrete()
        {
            saveFloat.value = tempStartValue;
            Storage.Save(saveFloat, WorldManager.saveFolder, discreteSaveKey);
        }

        public void IncreaseWorldFloat(int value = 1)
        {
            if (Value() + value <= tempStartValue)
            {
                if (worldFloat != null)
                    worldFloat.Increment(value);
                else if (worldFloatSO != null) worldFloatSO.IncrementValue(value);
            }
        }
    }

    public enum FloatHUDType
    {
        DiscreteItems,
        Continuous,
        Numbers
    }
}