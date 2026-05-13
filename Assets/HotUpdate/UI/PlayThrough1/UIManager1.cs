using System.Collections;
using System.Collections.Generic;
using HotUpdate.Dialogue.View;
using HotUpdate.Enums;
using HotUpdate.Interface;
using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace HotUpdate.UI
{
    public class UIManager1 : UIManager
    {
        private readonly Dictionary<UILayer, Transform> uiLayerTrans = new();
        [SerializeField]private Transform narratorUITrans;
        [SerializeField]private Transform traditionDialogueUITrans;
        [SerializeField]private Transform bubbleUITrans;
        [SerializeField]private Transform skillTreeUITrans;
        [SerializeField]private Transform pauseUITrans;
        [SerializeField]private Transform mapPanelTrans;
        [SerializeField]private Transform glitchWindowTrans;
        [SerializeField]private Transform glitchEffectTrans;
        [SerializeField]private Transform touchControlTrans;

        // 子管理器（组合模式）
        public NarratorDialogueUI NarratorDialogue { get; private set; }
        public TraditionalDialogueUI TraditionalDialogue { get; private set; }
        public BubbleDialogueUI BubbleDialogue { get; private set; }
        public SkillTreeUI SkillTree { get; private set; }
        public PauseUI PauseUI { get; private set; }
        public MapUI MapUI { get; private set; }
        public GlitchEffectUI GlitchEffect { get; private set; }
        public GlitchWindowUI GlitchWindow { get; private set; }
        public TouchControlUI TouchControl { get; private set; }

        public void Init()
        {
            uiLayerTrans.Add(UILayer.NarratorUI, narratorUITrans);
            uiLayerTrans.Add(UILayer.traditionDialogueUI, traditionDialogueUITrans);
            uiLayerTrans.Add(UILayer.bubbleUI, bubbleUITrans);
            uiLayerTrans.Add(UILayer.SkillTreeUI, skillTreeUITrans);
            uiLayerTrans.Add(UILayer.PauseUI, pauseUITrans);
            uiLayerTrans.Add(UILayer.MapPanel, mapPanelTrans);
            uiLayerTrans.Add(UILayer.GlitchWindow, glitchWindowTrans);
            uiLayerTrans.Add(UILayer.GlitchEffect, glitchEffectTrans);
            uiLayerTrans.Add(UILayer.TouchControl, touchControlTrans);

            // 初始化所有子管理器
            NarratorDialogue = new NarratorDialogueUI(uiLayerTrans[UILayer.NarratorUI]);
            TraditionalDialogue = new TraditionalDialogueUI(uiLayerTrans[UILayer.traditionDialogueUI]);
            BubbleDialogue = new BubbleDialogueUI(uiLayerTrans[UILayer.bubbleUI]);
            SkillTree = new SkillTreeUI(uiLayerTrans[UILayer.SkillTreeUI]);
            PauseUI = new PauseUI(uiLayerTrans[UILayer.PauseUI]);
            MapUI = new MapUI(uiLayerTrans[UILayer.MapPanel]);
            GlitchEffect = new GlitchEffectUI(uiLayerTrans[UILayer.GlitchEffect]);
            GlitchWindow = new GlitchWindowUI(uiLayerTrans[UILayer.GlitchWindow]);
            TouchControl = new TouchControlUI(uiLayerTrans[UILayer.TouchControl]);

            // 初始化各子系统的UI
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.Prefabs.UI.Playthrough1.Choice_Prefab,
                handle =>
                {
                    var choiceButtonPrefab = handle.Result;
                    NarratorDialogue.Init(choiceButtonPrefab);
                    TraditionalDialogue.Init(choiceButtonPrefab);
                    BubbleDialogue.Init(choiceButtonPrefab);
                }
            );
            SkillTree.Init();
            PauseUI.Init();
            MapUI.Init();
            GlitchEffect.Init();
            GlitchWindow.Init();
            TouchControl.Init();
        }

        #region 委托给子管理器

        public override DialogueViewBase ShowDialogueView(DialogueViewType viewType)
        {
            switch (viewType)
            {
                case DialogueViewType.Bubble:
                    return BubbleDialogue.GetDialogueView();
                case DialogueViewType.Narrator:
                    return NarratorDialogue.GetDialogueView();
                case DialogueViewType.Traditional:
                    return TraditionalDialogue.GetDialogueView();
                default:
                    return null;
            }
        }

        public override void OpenPausePanel()
        {
            PauseUI.OpenPausePanel();
        }

        public override void ClosePausePanel()
        {
            PauseUI.ClosePausePanel();
        }

        public override List<IInventory> GetInventory()
        {
            return MapUI.GetInventory();
        }

        public override void OpenMapPanel()
        {
            MapUI.OpenMapPanel();
        }

        public override void CloseMapPanel()
        {
            MapUI.CloseMapPanel();
        }

        public override void AdjustGlitchEffect(float changeValue)
        {
            GlitchEffect.AdjustGlitchEffect(changeValue);
        }

        public override void OpenGlitchWindow()
        {
            GlitchWindow.OpenGlitchWindow();
        }

        #endregion
    }
}

public enum UILayer
{
    NarratorUI,
    traditionDialogueUI,
    bubbleUI,
    SkillTreeUI,
    PauseUI,
    MapPanel,
    GlitchWindow,
    GlitchEffect,
    TouchControl
}