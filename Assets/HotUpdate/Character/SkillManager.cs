using System;
using System.Collections.Generic;
using System.IO;
using HotUpdate.Core;
using HotUpdate.Data.Commands;
using HotUpdate.Data.Model;
using HotUpdate.Enums;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;

namespace HotUpdate.Manager
{
    public class SkillManager : IController
    {
        public static SkillManager Instance;

        static SkillManager()
        {
            Instance = new SkillManager();
        }

        public bool IsSkillUnlocked(CharacterName name, SkillType type)
        {
            var data = this.GetModel<GameDataModel>().CurrentGameData.Value;
            for (int i = 0; i < data.skillData.Count; ++i)
            {
                if (data.skillData[i].name != name) continue;
                if (data.skillData[i].skills.Contains(type)) return true;
            }

            return false;
        }

        public void UnlockSkill(CharacterName name, SkillType type)
        {
            var data = this.GetModel<GameDataModel>().CurrentGameData.Value;
            Debug.Log(JsonUtility.ToJson(data));
            for (int i = 0; i < data.skillData.Count; ++i)
            {
                if (data.skillData[i].name != name) continue;
                this.SendCommand(new AddSkillCommand(name, type));
                Debug.Log(JsonUtility.ToJson(data));
                return;
            }

            SkillData newData = new SkillData { name = name, skills = new List<SkillType>() };
            newData.skills.Add(type);
            data.skillData.Add(newData);
        }

        public IArchitecture GetArchitecture()
        {
            return TheWorstGameInHistory.Interface;
        }
    }
}