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
        [SerializeField] private Transform narratorUITrans;
        [SerializeField] private Transform traditionDialogueUITrans;
        [SerializeField] private Transform bubbleUITrans;
        [SerializeField] private Transform skillTreeUITrans;
        [SerializeField] private Transform pauseUITrans;
        [SerializeField] private Transform mapPanelTrans;
        [SerializeField] private Transform glitchWindowTrans;
        [SerializeField] private Transform glitchEffectTrans;
        [SerializeField] private Transform touchControlTrans;

        public NarratorDialogueUI NarratorDialogue { get; private set; }
        public TraditionalDialogueUI TraditionalDialogue { get; private set; }
        public BubbleDialogueUI BubbleDialogue { get; private set; }
        public SkillTreeUI SkillTree { get; private set; }
        public PauseUI PauseUI { get; private set; }
        public MapUI MapUI { get; private set; }
        public GlitchEffectUI GlitchEffect { get; private set; }
        public GlitchWindowUI GlitchWindow { get; private set; }
        public TouchControlUI TouchControl { get; private set; }

        public override void Init()
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

            NarratorDialogue = new NarratorDialogueUI(uiLayerTrans[UILayer.NarratorUI]);
            TraditionalDialogue = new TraditionalDialogueUI(uiLayerTrans[UILayer.traditionDialogueUI]);
            BubbleDialogue = new BubbleDialogueUI(uiLayerTrans[UILayer.bubbleUI]);
            SkillTree = new SkillTreeUI(uiLayerTrans[UILayer.SkillTreeUI]);
            PauseUI = new PauseUI(uiLayerTrans[UILayer.PauseUI]);
            MapUI = new MapUI(uiLayerTrans[UILayer.MapPanel]);
            GlitchEffect = new GlitchEffectUI(uiLayerTrans[UILayer.GlitchEffect]);
            GlitchWindow = new GlitchWindowUI(uiLayerTrans[UILayer.GlitchWindow]);
            TouchControl = new TouchControlUI(uiLayerTrans[UILayer.TouchControl]);

            NarratorDialogue.Init();
            TraditionalDialogue.Init();
            BubbleDialogue.Init();
            SkillTree.Init();
            PauseUI.Init();
            MapUI.Init();
            GlitchEffect.Init();
            GlitchWindow.Init();
            TouchControl.Init();
        }
        private enum UILayer
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
    }
}