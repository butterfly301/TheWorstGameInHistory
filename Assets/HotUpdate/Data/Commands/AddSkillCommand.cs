using HotUpdate.Data.Model;
using HotUpdate.Enums;
using QFramework;
using UnityEngine;

namespace HotUpdate.Data.Commands
{
    public class AddSkillCommand : AbstractCommand
    {
        private CharacterName name;
        private SkillType skillName;

        public AddSkillCommand(CharacterName name, SkillType skillName)
        {
            this.name = name;
            this.skillName = skillName;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<GameDataModel>();
            var currentData = model.CurrentGameData.Value;

            // 使用 JsonUtility 进行深拷贝，确保数据不可变性并触发更新
            var json = JsonUtility.ToJson(currentData);
            var newData = JsonUtility.FromJson<GameData>(json);

            for (int i = 0; i < newData.skillData.Count; ++i)
            {
                if (newData.skillData[i].name != name) continue;
                if (!newData.skillData[i].skills.Contains(skillName))
                {
                    newData.skillData[i].skills.Add(skillName);
                    model.CurrentGameData.Value = newData;
                }
            }
        }
    }
}