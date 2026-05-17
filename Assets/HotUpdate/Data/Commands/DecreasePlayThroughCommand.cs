using HotUpdate.Data.Model;
using QFramework;
using UnityEngine;

namespace HotUpdate.Data.Commands
{
    // 减少周目的命令
    public class DecreasePlayThroughCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var model = this.GetModel<GameDataModel>();
            var currentData = model.CurrentGameData.Value;

// 深拷贝避免直接修改引用
            var json = JsonUtility.ToJson(currentData);
            var newData = JsonUtility.FromJson<GameData>(json);

newData.playThrough = Mathf.Max(0, newData.playThrough - 1);

model.CurrentGameData.Value = newData;

this.SendEvent(new PlayThroughChangedEvent
            {
                NewPlayThrough = newData.playThrough
            });
        }
    }
}