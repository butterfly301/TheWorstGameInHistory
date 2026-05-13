using System;
using System.Collections;
using System.Collections.Generic;
using TwoBitMachines.FlareEngine.AI;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using TwoBitMachines.FlareEngine.Interactables;
using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("Flare Engine/WorldManager")]
    public class WorldManager : MonoBehaviour
    {
        [NonSerialized] public static LayerMask collisionMask;
        [NonSerialized] public static LayerMask platformMask;
        [NonSerialized] public static LayerMask worldMask;
        [NonSerialized] public static int platformLayer;
        [NonSerialized] public static int playerLayer;
        [NonSerialized] public static int worldLayer;
        [NonSerialized] public static int hideLayer;
        [NonSerialized] public static int enemyLayer;

        [NonSerialized] public static List<InputButtonSO> inputs = new();
        [NonSerialized] public static WorldManager get;
        [NonSerialized] public static Camera gameCam;
        [NonSerialized] public static bool gameReset;

        public static string saveFolder;
        [SerializeField] public InputButtonSO pause;
        [SerializeField] public int levelNumber;
        [SerializeField] public bool isSaveMenu;

        [SerializeField] public UnityEvent onAwake;
        [SerializeField] public UnityEvent onStart;
        [SerializeField] public UnityEvent onPause;
        [SerializeField] public UnityEvent onUnpause;
        [SerializeField] public UnityEvent onResetAll;

        [SerializeField] public List<WorldEventSO> worldEvents = new();
        [SerializeField] public SaveOptions save = new();
        [NonSerialized] private readonly WaitForEndOfFrame endOfFrame = new();
        [SerializeField] public WorldUpdate endOfFrameUpdate;

        [NonSerialized] public bool initialized;
        [SerializeField] public WorldUpdate lateUpdate;
        [SerializeField] public NormalCallback onSave;
        [NonSerialized] private float pauseTimeScale = 1f;
        [NonSerialized] private float totalPlayTime;

        [SerializeField] public WorldUpdate update;
        [SerializeField] public WorldResetAll worldResetAll;

        private void Awake()
        {
            get = this;
            gameReset = false;
            gameCam = Camera.main;

            Time.timeScale = 1f;
            Util.InitUtil();
            RegisterInput(pause);
            ResetDifficultyVariables();
            InitializeSaveState();
            Journal.Find(transform);

            platformLayer = LayerMask.NameToLayer("Platform");
            playerLayer = LayerMask.NameToLayer("Player");
            worldLayer = LayerMask.NameToLayer("World");
            enemyLayer = LayerMask.NameToLayer("Enemy");
            hideLayer = LayerMask.NameToLayer("Hide");

            worldMask = 1 << worldLayer;
            platformMask = 1 << platformLayer;
            collisionMask = worldMask | platformMask;

            MovingPlatformDetect.Setup();
            gameObject.AddComponent<Wiggle>();

            for (var i = 0; i < worldEvents.Count; i++) worldEvents[i]?.ClearListeners();

            Screen.fullScreen = PlayerPrefs.GetInt("IsFullScreen") <= 0 ? true : false;
            onAwake.Invoke();
            initialized = true;
        }

        private void Start()
        {
            for (var i = 0; i < inputs.Count; i++) inputs[i]?.RestoreSavedValues();
            StartCoroutine(EndOfFrame());
            onStart.Invoke();
        }

        private void Update()
        {
            //* Execute the following in order
            Root.deltaTime = Time.deltaTime;
            SlowMotion.Run();
            WaterBatch.Run();
            MovingPlatformDetect.Run();
            Pathfinding.OccupiedNodes();
            PathfindingRT.OccupiedNodes();
            PathfindingBasic.OccupiedNodes();
            Player.Run();
            Character.AICharacters();
            Player.PostAIRun();
            ProjectileBase.Projectiles();
            Foliage.RunFoliage();
            update?.Invoke();
            PauseInput();
            MovingPlatformDetect.ResetQuery();
            totalPlayTime += Time.deltaTime;
        }

        private void LateUpdate()
        {
            Character.LateAICharacters();
            Foliage.LateUpdateFoliage();
            Foliage.DrawFoliage();
            lateUpdate?.Invoke();
        }

        private void OnDisable()
        {
            Save();
            WorldVariable.ClearTempChildren(); // unregister
            InventorySO.ClearTempChildren();
        }

        public void ResetDifficultyVariables()
        {
            AIDamage.difficulty = 1f;
        }

        public void InitializeSaveState()
        {
            Storage.encrypt = false; // probably not right?
            SaveOptions.Load(ref save);
            saveFolder = save.RetrieveSaveFolder();
            Storage.encrypt = save.navigate;
            //* loading of saved data has to occur in Start for other classes
        }

        public void ResetAll()
        {
            gameReset = true;
            ResetInputs();
            update?.Invoke(true);
            lateUpdate?.Invoke(true);
            endOfFrameUpdate?.Invoke(true);
            worldResetAll?.Invoke();

            Time.timeScale = 1f;
            Inventory.blockInventories = false;

            SlowMotion.Reset();
            Character.ResetMovingPlatforms();
            Player.ResetPlayers();
            Water.ResetWaves();
            Rope.ResetRopes();
            ProjectileBase.ResetProjectiles();
            Dialogue.ResetDialogue(true);
            DialogueBubble.ResetDialogue();
            WorldEffects.ResetEffects();
            WorldBoolTracker.ClearAll();
            TransformTracker.Reset();
            Tool.ResetTools();
            Teleport.ResetAll();
            WorldVariable.ResetAndClear();
            Character.ResetAllAI(); //                        reset AI second to last for checkpoint system
            CheckPoint.ResetPlayerAll();
            Safire2DCamera.Safire2DCamera.ResetCameras(); //  resets on player position
            Player.BlockAllInputs(false);
            onResetAll.Invoke();
        }

        public void Save()
        {
            WorldVariable.SaveData();
            InventorySO.SaveData();
            if (onSave != null) onSave.Invoke();
            save.Save(levelNumber, totalPlayTime, isSaveMenu);
            totalPlayTime = 0;
        }

        public void DeleteAllSavedData()
        {
            Storage.DeleteAll(save.gameName);
            Storage.DeleteAll(saveFolder);
        }

        public void Pause()
        {
            if (Time.timeScale == 0)
                return;

            Player.BlockAllInputsRemember(true);
            pauseTimeScale = Time.timeScale;
            Time.timeScale = 0;
            onPause.Invoke();
        }

        public void Unpause()
        {
            Player.BlockAllInputsRemember(false);
            Time.timeScale = pauseTimeScale;
            onUnpause.Invoke();
        }

        public void PauseNoInvoke()
        {
            if (Time.timeScale == 0)
                return;

            Player.BlockAllInputsRemember(true);
            pauseTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

        public void UnpauseNoInvoke()
        {
            Player.BlockAllInputsRemember(false);
            Time.timeScale = pauseTimeScale;
        }

        public void BlockPlayerInput(bool value)
        {
            Player.BlockAllInputs(value);
        }

        public void ClearInputs()
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                if (inputs[i] == null)
                    continue;
                inputs[i].inputPressed = false;
                inputs[i].inputReleased = false;
                inputs[i].axisReleased = false;
            }
        }

        public void ResetInputs()
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                if (inputs[i] == null)
                    continue;
                inputs[i].inputPressed = false;
                inputs[i].inputReleased = false;
                inputs[i].axisReleased = false;
                inputs[i].axisPressed = false;
                inputs[i].inputHold = false;
            }
        }

        public static void RegisterInput(InputButtonSO input)
        {
            if (input != null && !inputs.Contains(input)) inputs.Add(input);
        }

        private void PauseInput()
        {
            if (pause != null && pause.Pressed())
            {
                if (Time.timeScale > 0)
                    Pause();
                else
                    Unpause();
            }
        }

        private IEnumerator EndOfFrame()
        {
            while (true) // will only execute at the end of the frame
            {
                yield return endOfFrame;
                ClearInputs();
                endOfFrameUpdate?.Invoke();
                gameReset = false;
                Inventory.blockInventories = false;
            }
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private string createSignal = "eventName";
        [SerializeField] [HideInInspector] private string gameNameRef = "";
        [SerializeField] [HideInInspector] private bool createWorldEvent;
        [SerializeField] [HideInInspector] private bool foldOut;
        [SerializeField] [HideInInspector] private bool worldFoldOut;
        [SerializeField] [HideInInspector] private bool deleteFoldOut;
        [SerializeField] [HideInInspector] private bool onAwakeFoldOut;
        [SerializeField] [HideInInspector] private bool onStartFoldOut;
        [SerializeField] [HideInInspector] private bool onPauseFoldOut;
        [SerializeField] [HideInInspector] private bool onUnpauseFoldOut;
        [SerializeField] [HideInInspector] private bool onResetAllFoldOut;
        [SerializeField] [HideInInspector] private bool saveSlotsFoldOut;
        [SerializeField] [HideInInspector] private bool navigateRef;
        [SerializeField] [HideInInspector] public bool viewDebug;
        public static bool viewDebugger => get != null && get.viewDebug;
#pragma warning restore 0414

        private void OnDrawGizmosSelected()
        {
            if (save.gameName != gameNameRef)
            {
                gameNameRef = save.gameName;
                save.Save();
            }

            if (save.navigate != navigateRef)
            {
                navigateRef = save.navigate;
                save.Save();
            }
        }
#endif

        #endregion
    }

    public static class UserFolderPaths
    {
        public static string[] paths =
            { "TBMPlayerFolder", "TBMAINode", "TBMBlackboard", "TBMBullet", "TBMWorldEvents" };

        public static string[] pathLabel =
            { "Player Ability Folder", "AI Node Folder", "Blackboard Folder", "Bullet Folder", "World Events" };

        public static string Path(UserFolder type)
        {
            return PlayerPrefs.GetString(paths[(int)type], "");
        }

        public static string FolderName(UserFolder type)
        {
            return PlayerPrefs.GetString(paths[(int)type] + "Name", "");
        }
    }

    public enum UserFolder
    {
        Player,
        AINode,
        Blackboard,
        Bullet,
        WorldEvents
    }
}