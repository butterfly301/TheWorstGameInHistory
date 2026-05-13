using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    public class QuestUIComplete : QuestUI
    {
        [SerializeField] public TextMeshProUGUI title;
        [SerializeField] public InputButtonSO exitButton;
        [SerializeField] public float timeOut = 5;
        [SerializeField] public List<QuestRewardsUI> rewards = new();
        [NonSerialized] private float counter;

        private void Update()
        {
            if ((exitButton == null && Clock.TimerExpiredUnscaled(ref counter, timeOut)) ||
                (exitButton != null && exitButton.Pressed())) gameObject.SetActive(false);
        }

        public override void OpenQuestWindow(QuestSO questSO)
        {
            if (questSO == null) return;

            counter = 0;
            gameObject.SetActive(true);
            if (title != null) title.SetText(questSO.title);
            EnableRewards(questSO.rewards, rewards);
        }
    }
}