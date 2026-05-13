using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("Flare Engine/一Saving/WorldBool")]
    public class WorldBool : WorldVariable
    {
        [SerializeField] public string variableName = "name"; // name must be unique
        [SerializeField] private bool currentValue;
        [SerializeField] private bool isScriptableObject;
        [SerializeField] private WorldBoolSO soReference;
        [SerializeField] private bool save;
        [SerializeField] private bool saveManually;
        [SerializeField] private UnityEventBool onLoadConditionTrue = new();
        [SerializeField] private UnityEventBool onLoadConditionFalse = new();
        [SerializeField] private SaveBool saveBool = new();

        private bool sOAvailable => isScriptableObject && soReference != null;

        private void Start()
        {
            Initialize();
        }

        public override void Initialize()
        {
            SetSOValue();
            if (save) RestoreValue();
            if (IsTrue())
                onLoadConditionTrue.Invoke(currentValue);
            else
                onLoadConditionFalse.Invoke(currentValue);
            initialized = true;
        }

        public override void Register()
        {
            if (sOAvailable) soReference.Register(this);
            if (initialized)
                Initialize(); // register is called from onEnable, ensure objects is always initialized on enable but only if the start method has been used
        }

        public override void Save()
        {
            if (save && !saveManually)
            {
                saveBool.value = currentValue;
                Storage.Save(saveBool, WorldManager.saveFolder, variableName);
            }
        }

        public void RestoreValue()
        {
            saveBool.value = currentValue;
            var newSavedBool = Storage.Load(saveBool, WorldManager.saveFolder, variableName);
            if (newSavedBool != null)
            {
                currentValue = newSavedBool.value;
                SetSOValue();
            }
        }

        public void SaveManually()
        {
            saveBool.value = currentValue;
            Storage.Save(saveBool, WorldManager.saveFolder, variableName);
        }

        public override void DeleteSavedData()
        {
            Storage.Delete(WorldManager.saveFolder, variableName);
        }

        public void Refresh(bool value)
        {
            currentValue = value;
        }

        public void SetValue(bool value)
        {
            currentValue = value;
            SetSOValue();
        }

        public void SetValueAndSave(bool value)
        {
            currentValue = value;
            SetSOValue();
            Save();
        }

        public void SetTrue()
        {
            currentValue = true;
            SetSOValue();
        }

        public void SetFalse()
        {
            currentValue = false;
            SetSOValue();
        }

        public bool IsTrue()
        {
            return currentValue;
        }

        public bool IsFalse()
        {
            return currentValue == false;
        }

        public bool GetValue()
        {
            return currentValue;
        }

        public void WorldBoolTrackerRegister()
        {
            if (WorldBoolTracker.get != null) WorldBoolTracker.get.Register(this);
        }

        private void SetSOValue()
        {
            if (sOAvailable) soReference.SetWorldValue(currentValue);
        }

        #region

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool foldOut;
        [SerializeField] [HideInInspector] private bool eventFoldOut;
        [SerializeField] [HideInInspector] private bool saveFoldOut;
        [SerializeField] [HideInInspector] private bool objFoldOut;
        [SerializeField] [HideInInspector] private bool loadFoldOutTrue;
        [SerializeField] [HideInInspector] private bool loadFoldOutFalse;
        [SerializeField] [HideInInspector] private bool createSO;
        [SerializeField] [HideInInspector] private bool isSceneName;
#pragma warning restore 0414
#endif

        #endregion
    }
}