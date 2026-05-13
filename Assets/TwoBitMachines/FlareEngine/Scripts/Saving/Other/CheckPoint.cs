using System;
using System.Collections.Generic;
using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("Flare Engine/一Saving/CheckPoint")]
    public class CheckPoint : MonoBehaviour
    {
        public enum CheckPointSaveType
        {
            Automatic,
            OnButtonPress,
            SaveManually
        }

        public enum CheckPointType
        {
            Priority,
            Any
        }

        public static List<CheckPoint> checkPoint = new();
        [SerializeField] public string checkPointName = "CheckPoint";
        [SerializeField] public bool hasDefault;
        [SerializeField] public int defaultIndex;
        [SerializeField] public InputButtonSO input;
        [SerializeField] public UnityEvent onReset;
        [SerializeField] public UnityEvent onSave;
        [SerializeField] public CheckPointType type;
        [SerializeField] public List<Checks> checkPoints = new();

        [SerializeField] private bool saveManually;
        [SerializeField] private SaveFloat saveFloat = new();

        private void Awake()
        {
            WorldManager.RegisterInput(input);
        }

        private void Start()
        {
            for (var i = 0; i < checkPoints.Count; i++) checkPoints[i].Initialize();
            ResetPlayer();
        }

        public void Update()
        {
            Vector2 player = Player.PlayerTransform().position + Vector3.up * 0.1f;
            for (var i = 0; i < checkPoints.Count; i++) checkPoints[i].Execute(this, player, saveManually);
            saveManually = false;
        }

        private void OnEnable()
        {
            if (!checkPoint.Contains(this)) checkPoint.Add(this);
        }

        private void OnDisable()
        {
            if (checkPoint.Contains(this)) checkPoint.Remove(this);
        }

        public static void ResetPlayerAll()
        {
            for (var i = 0; i < checkPoint.Count; i++) checkPoint[i].ResetPlayer();
        }

        public void ResetPlayer()
        {
            saveFloat.value = hasDefault ? defaultIndex : -1f;
            var currentIndex = (int)Storage.Load(saveFloat, WorldManager.saveFolder, checkPointName).value;
            for (var i = 0; i < checkPoints.Count; i++)
                if (checkPoints[i].index == currentIndex)
                {
                    var player = Player.PlayerTransform();
                    if (player != null)
                    {
                        if (checkPoints[i].playerDirection != Checks.PlayerDirection.LeaveAsIs)
                            Player.SetPlayerDirection((int)checkPoints[i].playerDirection);
                        player.transform.position = checkPoints[i].bounds.bottomCenter + Vector2.up * 0.01f;
                        Safire2DCamera.Safire2DCamera.ResetCameras();
                        checkPoints[i].onReset.Invoke();
                        onReset.Invoke();
                    }

                    return;
                }
        }

        public void SaveManually()
        {
            saveManually = true;
        }

        public void ResetCheckPoint()
        {
            Storage.Delete(WorldManager.saveFolder, checkPointName);
        }

        [Serializable]
        public class Checks
        {
            public enum PlayerDirection
            {
                PlayerFacesRight = 1,
                PlayerFacesLeft = -1,
                LeaveAsIs = 2
            }

            [SerializeField] public int index;
            [SerializeField] public UnityEvent onReset;
            [SerializeField] public UnityEvent onSave;
            [SerializeField] public CheckPointSaveType saveType;
            [SerializeField] public PlayerDirection playerDirection = PlayerDirection.PlayerFacesRight;
            [SerializeField] public SimpleBounds bounds = new();
            [NonSerialized] private bool inside;

            public void Initialize()
            {
                bounds.Initialize();
                inside = false;
            }

            public void Execute(CheckPoint check, Vector2 player, bool saveManually)
            {
                if (bounds.Contains(player))
                {
                    if (saveType == CheckPointSaveType.Automatic)
                    {
                        if (!inside)
                        {
                            inside = true;
                            SaveCheckPoint(check);
                        }
                    }
                    else if (saveType == CheckPointSaveType.OnButtonPress)
                    {
                        if (check.input != null && check.input.Pressed()) SaveCheckPoint(check);
                    }
                    else
                    {
                        if (saveManually) SaveCheckPoint(check);
                    }
                }
                else
                {
                    inside = false;
                }
            }

            private void SaveCheckPoint(CheckPoint check)
            {
                if (check.type == CheckPointType.Priority)
                {
                    var currentIndex = GetCurrentIndex(check);
                    if (index >= currentIndex) Save(check);
                }
                else
                {
                    Save(check);
                }
            }

            private int GetCurrentIndex(CheckPoint check)
            {
                check.saveFloat.value = -1;
                return (int)Storage.Load(check.saveFloat, WorldManager.saveFolder, check.checkPointName).value;
            }

            public void Save(CheckPoint check)
            {
                check.saveFloat.value = index;
                Storage.Save(check.saveFloat, WorldManager.saveFolder, check.checkPointName);
                onSave.Invoke();
                check.onSave.Invoke();
            }

            #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
            [SerializeField] [HideInInspector] private bool delete;
            [SerializeField] [HideInInspector] private bool foldOut;
            [SerializeField] [HideInInspector] private bool eventsFoldOut;
            [SerializeField] [HideInInspector] private bool resetFoldOut;
            [SerializeField] [HideInInspector] private bool saveFoldOut;
#pragma warning restore 0414
#endif

            #endregion
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private int signalIndex = -1;
        [SerializeField] [HideInInspector] private bool add;
        [SerializeField] [HideInInspector] private bool active;
        [SerializeField] [HideInInspector] private bool foldOut;
        [SerializeField] [HideInInspector] private bool eventsFoldOut;
        [SerializeField] [HideInInspector] private bool resetFoldOut;
        [SerializeField] [HideInInspector] private bool saveFoldOut;
#pragma warning restore 0414
#endif

        #endregion
    }
}