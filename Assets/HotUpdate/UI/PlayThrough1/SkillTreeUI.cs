using System.Collections.Generic;
using HotUpdate.Enums;
using UnityEngine;

namespace HotUpdate.UI
{
    /// <summary>
    /// 技能树UI管理器
    /// 负责管理技能树面板、技能点显示等技能系统相关的UI
    /// </summary>
    public class SkillTreeUI
    {
        
        // TODO: 添加技能树相关的字段
        // private GameObject skillTreePanelPrefab;
        // private GameObject skillTreePanelObj;

        public SkillTreeUI(Transform layerTrans)
        {
            
        }

        /// <summary>
        /// 初始化技能树UI系统
        /// </summary>
        public void Init()
        {
            // TODO: 加载技能树面板Prefab
            // AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            //     AddressableKeys.Prefabs.UI.Playthrough1.SkillTree_Prefab,
            //     handle => { ... }
            // );
        }

        /// <summary>
        /// 打开技能树面板
        /// </summary>
        public void OpenSkillTreePanel()
        {
            // TODO: 实现打开技能树面板逻辑
        }

        /// <summary>
        /// 关闭技能树面板
        /// </summary>
        public void CloseSkillTreePanel()
        {
            // TODO: 实现关闭技能树面板逻辑
        }

        // TODO: 添加其他技能树相关方法...
    }
}
