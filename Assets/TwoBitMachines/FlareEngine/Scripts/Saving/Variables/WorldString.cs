using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("Flare Engine/一Saving/WorldString")]
    public class WorldString : WorldVariable
    {
        [SerializeField] public string variableName = "name"; // name must be unique
        [SerializeField] private string currentValue;
        [SerializeField] private bool isScriptableObject;
        [SerializeField] private WorldStringSO soReference;
        [SerializeField] private bool save;
        [SerializeField] private UnityEventString afterLoad = new();
        [SerializeField] private SaveString saveString = new();

        private bool sOAvailable => isScriptableObject && soReference != null;

        private void Start()
        {
            Initialize();
        }

        public override void Initialize()
        {
            SetSOValue();
            if (save) RestoreValue();
            afterLoad.Invoke(currentValue);
            initialized = true;
        }

        public override void Register()
        {
            if (sOAvailable) soReference.Register(this);
            if (initialized) Initialize();
        }

        public void RestoreValue()
        {
            saveString.value = currentValue;
            var newSaveString = Storage.Load(saveString, WorldManager.saveFolder, variableName);
            if (newSaveString != null)
            {
                currentValue = newSaveString.value;
                SetSOValue();
            }
        }

        public override void Save()
        {
            if (save)
            {
                saveString.value = currentValue;
                Storage.Save(saveString, WorldManager.saveFolder, variableName);
            }
        }

        public override void DeleteSavedData()
        {
            Storage.Delete(WorldManager.saveFolder, variableName);
        }

        public void Refresh(string value)
        {
            currentValue = value;
        }

        public void SetValue(string value)
        {
            currentValue = value;
            SetSOValue();
        }

        public void SetValueAndSave(string value)
        {
            currentValue = value;
            SetSOValue();
            Save();
        }

        public string GetValue()
        {
            return currentValue;
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
        [SerializeField] [HideInInspector] private bool loadFoldOut;
        [SerializeField] [HideInInspector] private bool createSO;
#pragma warning restore 0414
#endif

        #endregion
    }
}