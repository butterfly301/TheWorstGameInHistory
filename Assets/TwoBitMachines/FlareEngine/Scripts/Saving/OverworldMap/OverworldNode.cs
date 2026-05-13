using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.FlareEngine
{
    public class OverworldNode : MonoBehaviour
    {
        public static List<Texture2D> icon;
        [SerializeField] public OverworldNodeType type;
        [SerializeField] public string sceneName = "SceneName";
        [SerializeField] public string blockSaveKey;
        [SerializeField] public string signal = "";
        [SerializeField] public string unlockKey = "";
        [SerializeField] public string nodeName = "";
        [SerializeField] public float signalTime = 1f;
        [SerializeField] public SetNextNodeType setNextNodeType;
        [SerializeField] public Sprite imageLocked;
        [SerializeField] public Sprite imageUnlocked;

        [SerializeField] public UnityEvent onUnlock;
        [SerializeField] public UnityEvent onTeleport;
        [SerializeField] public UnityEvent isBlocked;
        [SerializeField] public UnityEvent isUnblocked;
        [SerializeField] public UnityEvent signalComplete;
        [SerializeField] public UnityEventOverworldNode onEnterNode;
        [SerializeField] public UnityEventOverworldNode onExitNode;

        [SerializeField] public OverworldNode teleportToNode;
        [SerializeField] public OverworldNode nextNode;

        [SerializeField] public bool unlocked;
        [SerializeField] private SpriteRenderer rendererRef;
        [SerializeField] private SaveBool saveBool = new();
        [SerializeField] public List<OverworldNode> path = new();
        [SerializeField] public List<OverworldNode> blockPath = new();

        public bool isLevel => type == OverworldNodeType.Level;
        public bool isBlock => type == OverworldNodeType.Block;
        public bool isTeleport => type == OverworldNodeType.Teleport;
        public bool canBeLocked => isLevel || isBlock;
        public Vector2 position => transform.position;

        public float G { get; set; }
        public float H { get; set; }
        public float F => G + H;
        public OverworldNode Parent { get; set; }

        public void Awake()
        {
            if (rendererRef == null) rendererRef = gameObject.GetComponent<SpriteRenderer>();
        }

        public void Start()
        {
            if (canBeLocked)
            {
                if (isLevel)
                {
                    saveBool.value = false;
                    unlocked = Storage.Load(saveBool, WorldManager.saveFolder, sceneName).value;
                }

                if (isBlock)
                {
                    saveBool.value = false;
                    unlocked = Storage.Load(saveBool, WorldManager.saveFolder, blockSaveKey).value;
                    if (unlocked)
                        isUnblocked.Invoke();
                    else
                        isBlocked.Invoke();
                }

                SetImage();
            }
        }

        public void UnlockBlock()
        {
            if (isBlock)
            {
                unlocked = true;
                saveBool.value = true;
                Storage.Save(saveBool, WorldManager.saveFolder, blockSaveKey);
                onUnlock.Invoke();
                SetImage();
            }
        }

        private void SetImage()
        {
            if (rendererRef != null && canBeLocked) rendererRef.sprite = IsLocked() ? imageLocked : imageUnlocked;
        }

        public bool IsLocked()
        {
            return canBeLocked ? !unlocked : false;
        }

        public bool PathBlocked(OverworldNode otherNode)
        {
            return otherNode.IsLocked() || blockPath.Contains(otherNode);
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] public bool foldOut;
        [SerializeField] public bool enterFoldOut;
        [SerializeField] public bool onEnterFoldOut;
        [SerializeField] public bool onExitFoldOut;
        [SerializeField] public bool unlockFoldOut;
        [SerializeField] public bool isBlockedFoldOut;
        [SerializeField] public bool isUnblockedFoldOut;
        [SerializeField] public bool signalFoldOut;
        [SerializeField] public bool teleportFoldOut;
        [SerializeField] public float timeStamp = 2f;

        public void OnDrawGizmos()
        {
            if (icon == null || icon.Count == 0) // || Application.isPlaying)
                return;
            Texture2D iconTexture = null; //

            if (isLevel)
                iconTexture = icon.GetIcon("LockRed");
            else if (isBlock)
                iconTexture = icon.GetIcon("Stop");
            else if (isTeleport)
                iconTexture = icon.GetIcon("TeleportYellow");
            else
                return;

            var iconSize = 1f;
            var position = this.position + Vector2.up * -1f;
            var iconRect = new Rect(position.x, position.y, iconSize, -iconSize);
            Gizmos.DrawGUITexture(iconRect, iconTexture);
        }
#pragma warning restore 0414
#endif

        #endregion
    }

    public enum OverworldNodeType
    {
        Basic,
        Level,
        Block,
        Teleport,
        HasItem,
        Start
    }

    public enum SetNextNodeType
    {
        No,
        TeleportTo,
        MoveTo
    }

    [Serializable]
    public class UnityEventOverworldNode : UnityEvent<OverworldNode>
    {
    }
}