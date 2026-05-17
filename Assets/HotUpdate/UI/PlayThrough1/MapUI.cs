using System.Collections.Generic;
using HotUpdate.Enums;
using HotUpdate.Interface;
using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;

namespace HotUpdate.UI
{
    /// <summary>
    /// 地图面板管理器
    /// 负责管理地图面板的显示和隐藏
    /// </summary>
    public class MapUI
    {
        private readonly Transform parentTransform;
        private GameObject mapPanelPrefab;
        private GameObject mapPanelObj;

public List<IInventory> Inventory { get; set; }

public MapUI(Transform parent)
        {
            parentTransform = parent;
        }

/// <summary>
        /// 初始化地图面板系统
        /// </summary>
        public void Init()
        {
            // 初始化背包列表
            if (Inventory == null)
            {
                Inventory = new List<IInventory>();
            }
        }

/// <summary>
        /// 加载指定索引的地图面板Prefab
        /// </summary>
        public void LoadMapPanelPrefab(string index)
        {
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.GetPrefabs_UI_Playthrough1_Maps(index),
                handle => { mapPanelPrefab = handle.Result; }
            );
        }

/// <summary>
        /// 打开地图面板
        /// </summary>
        public void OpenMapPanel()
        {
            if (mapPanelPrefab != null)
                mapPanelObj = Object.Instantiate(mapPanelPrefab, parentTransform);
        }

/// <summary>
        /// 关闭地图面板
        /// </summary>
        public void CloseMapPanel()
        {
            if (mapPanelObj != null)
            {
                mapPanelObj.GetComponent<DestroyAfterDelay>()?.DestroyMyself();
                mapPanelObj = null;
            }
        }

/// <summary>
        /// 获取背包物品列表
        /// </summary>
        public List<IInventory> GetInventory()
        {
            return Inventory;
        }
    }
}
