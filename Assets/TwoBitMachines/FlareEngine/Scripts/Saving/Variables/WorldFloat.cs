using System;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("Flare Engine/一Saving/WorldFloat")]
    public class WorldFloat : WorldVariable
    {
        [SerializeField] public string variableName = "name"; // name must be unique
        [SerializeField] public float currentValue = 10;
        [SerializeField] public float minValue;
        [SerializeField] public float maxValue = 100;
        [SerializeField] private bool save;
        [SerializeField] private bool broadcastValue;
        [SerializeField] private bool isScriptableObject;
        [SerializeField] private bool saveManually;
        [SerializeField] public WorldFloatSO soReference;

        [SerializeField] public int characterDirectionX = 1;
        [SerializeField] public bool callDamageEffect;
        [SerializeField] public float positionOffset = 1f;
        [SerializeField] public string worldEffect = "";

        [SerializeField] public UnityEventEffect onValueChanged = new();
        [SerializeField] private UnityEventEffect onMinValue = new();
        [SerializeField] private UnityEventEffect onMaxValue = new();
        [SerializeField] private UnityEventEffect onValueIncreased = new();
        [SerializeField] private UnityEventEffect onValueDecreased = new();
        [SerializeField] private UnityEventFloat onLoadConditionTrue = new();
        [SerializeField] private UnityEventFloat onLoadConditionFalse = new();
        [SerializeField] private UnityEventFloat onSceneStart = new();
        [SerializeField] private SaveFloat saveFloat = new();
        [NonSerialized] public Collider2D colliderRef;

        [NonSerialized] private float refreshValue;
        [NonSerialized] private float tempValue;

        public bool isSaved => save;
        public bool cantIncrement { get; private set; }
        public bool register => isScriptableObject && soReference != null;

        private void Start()
        {
            Initialize();
        }

        public override void Initialize()
        {
            tempValue = 0;
            SetSOValue();
            if (save) RestoreValue();
            if (IsTrue())
                onLoadConditionTrue.Invoke(currentValue);
            else
                onLoadConditionFalse.Invoke(currentValue);
            onSceneStart.Invoke(currentValue);
            initialized = true;
        }

        public override void Register()
        {
            if (register) soReference.Register(this);
            if (initialized) Initialize();
        }

        public override bool IncrementValue(Transform attacker, float value, Vector2 direction)
        {
            if (cantIncrement)
                return false;

            var newValue = currentValue + value;
            // if (colliderRef == null)
            // {
            //         colliderRef = this.gameObject.GetComponent<Collider2D>();
            // }
            //ImpactPacket impact =  ImpactPacket.impact.Set(worldEffect, this.transform, colliderRef, this.transform.position, attacker, direction, 1, value);

            if (newValue <= minValue)
            {
                var success = currentValue > minValue;
                currentValue = minValue;
                SetSOValue();
                if (success)
                {
                    onValueDecreased.Invoke(BasicImpact(attacker, direction,
                        value)); // impact packet con potentially be changed by these events, need to reset
                    onValueChanged.Invoke(BasicImpact(attacker, direction, value));
                    onMinValue.Invoke(BasicImpact(attacker, direction, value));
                }

                return success;
            }

            if (newValue >= maxValue)
            {
                var success = currentValue < maxValue;
                currentValue = maxValue;
                SetSOValue();
                if (success)
                {
                    onValueIncreased.Invoke(BasicImpact(attacker, direction, value));
                    onValueChanged.Invoke(BasicImpact(attacker, direction, value));
                    onMaxValue.Invoke(BasicImpact(attacker, direction, value));
                }

                return success;
            }

            currentValue = newValue;
            SetSOValue();
            onValueChanged.Invoke(BasicImpact(attacker, direction, value));
            if (value > 0) onValueIncreased.Invoke(BasicImpact(attacker, direction, value));
            if (value < 0) onValueDecreased.Invoke(BasicImpact(attacker, direction, value));
            return true;
        }

        public ImpactPacket BasicImpact(Transform attacker, Vector2 direction, float value = 0)
        {
            if (colliderRef == null) colliderRef = gameObject.GetComponent<Collider2D>();
            return ImpactPacket.impact.Set(worldEffect, transform, colliderRef, transform.position, attacker, direction,
                characterDirectionX, value);
        }

        public void IncreaseTempValue(ItemEventData itemEventData)
        {
            if (cantIncrement)
                return;
            tempValue += itemEventData.genericFloat;
        }

        public void IncreaseTempValue(float value)
        {
            if (cantIncrement)
                return;
            tempValue += value;
        }

        public void Increment(float value) //                created this method so it can be used by onEvent
        {
            IncrementValue(null, value, Vector2.zero);
        }

        public void Increment(ItemEventData itemEventData) // created this method so it can be used by onEvent
        {
            itemEventData.success = IncrementValue(null, itemEventData.genericFloat, Vector2.zero);
        }

        public void CantIncrement(bool value)
        {
            cantIncrement = value;
        }

        public void IncrementValueBypass(float value)
        {
            currentValue = Mathf.Clamp(currentValue + value, minValue, maxValue);
            SetSOValue();
        }

        public void SetMaxValue(float value)
        {
            maxValue = value;
        }

        public void SetValue(float value)
        {
            currentValue = Mathf.Clamp(value, minValue, maxValue);
            SetSOValue();
        }

        public float GetValue()
        {
            return currentValue + tempValue;
        }

        public void SetTrue()
        {
            currentValue = 1f;
            SetSOValue();
        }

        public void SetFalse()
        {
            currentValue = 0;
            SetSOValue();
        }

        public bool IsTrue()
        {
            return currentValue > 0;
        }

        private void SetSOValue()
        {
            if (register) soReference.SetWorldValue(currentValue);
            BroadcastValue();
        }

        private void BroadcastValue()
        {
            if (!broadcastValue)
                return;
            for (var i = 0; i < variables.Count; i++)
            {
                var worldObj = variables[i];
                if (worldObj != this && worldObj.Name() == variableName)
                {
                    if (worldObj is WorldFloat worldFloat)
                    {
                        if (currentValue <= worldFloat.minValue)
                            worldFloat.onMinValue.Invoke(worldFloat.BasicImpact(worldFloat.transform, Vector2.zero));
                        if (currentValue != worldFloat.currentValue)
                            worldFloat.onValueChanged.Invoke(worldFloat.BasicImpact(worldFloat.transform,
                                Vector2.zero));
                    }

                    variables[i].InternalSet(currentValue);
                }
            }
        }

        public override void InternalSet(float newValue)
        {
            currentValue = newValue;
        }

        public override string Name()
        {
            return variableName;
        }

        public override void ClearTempValue()
        {
            tempValue = 0;
        }

        public void SetRefreshValue()
        {
            currentValue = refreshValue;
            SetSOValue();
        }

        public void UpdateRefreshValue()
        {
            refreshValue = currentValue;
        }

        #region Editor Variables

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool foldOut;
        [SerializeField] [HideInInspector] private bool eventFoldOut;
        [SerializeField] [HideInInspector] private bool minFoldOut;
        [SerializeField] [HideInInspector] private bool maxFoldOut;
        [SerializeField] [HideInInspector] private bool changedFoldOut;
        [SerializeField] [HideInInspector] private bool increasedFoldOut;
        [SerializeField] [HideInInspector] private bool decreasedFoldOut;
        [SerializeField] [HideInInspector] private bool changedGOFoldOut;
        [SerializeField] [HideInInspector] private bool shieldFoldOut;
        [SerializeField] [HideInInspector] private bool impactFoldOut;
        [SerializeField] [HideInInspector] private bool damageFoldOut;
        [SerializeField] [HideInInspector] private bool damageEffectFoldOut;
        [SerializeField] [HideInInspector] private bool loadFoldOutTrue;
        [SerializeField] [HideInInspector] private bool loadFoldOutFalse;
        [SerializeField] [HideInInspector] private bool sceneStartFoldOut;
        [SerializeField] [HideInInspector] private bool createSO;
#pragma warning restore 0414
#endif

        #endregion

        #region Save

        public void RestoreValue()
        {
            saveFloat.value = currentValue;
            // currentValue = Storage.Load<SaveFloat>(saveFloat, WorldManager.saveFolder, variableName).value;
            var newSavedFloat = Storage.Load(saveFloat, WorldManager.saveFolder, variableName);
            if (newSavedFloat != null)
            {
                currentValue = newSavedFloat.value;
                refreshValue = currentValue;
                SetSOValue();
            }
        }

        public override void Save()
        {
            if (save && !saveManually) SaveNow();
        }

        public void SaveManually()
        {
            SaveNow();
        }

        public void SaveTempValue()
        {
            currentValue = Mathf.Clamp(currentValue + tempValue, minValue, maxValue);
            tempValue = 0;
            SetSOValue();
            SaveManually();
        }

        public void SetValueAndSave(float value)
        {
            currentValue = Mathf.Clamp(value, minValue, maxValue);
            SetSOValue();
            SaveNow();
        }

        public override void DeleteSavedData()
        {
            Storage.Delete(WorldManager.saveFolder, variableName);
        }

        private void SaveNow()
        {
            saveFloat.value = currentValue;
            refreshValue = currentValue;
            Storage.Save(saveFloat, WorldManager.saveFolder, variableName);
        }

        #endregion
    }
}