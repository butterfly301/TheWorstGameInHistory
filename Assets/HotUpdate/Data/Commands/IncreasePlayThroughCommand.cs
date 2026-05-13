using HotUpdate.Data.Model;
using QFramework;
using UnityEngine;

namespace HotUpdate.Data.Commands
{
    // 增加周目的命令
    public class IncreasePlayThroughCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var model = this.GetModel<GameDataModel>();
            var currentData = model.CurrentGameData.Value;

            // 使用 JsonUtility 进行深拷贝，避免手动复制字段
            // 这样以后往 GameData 添加新数据时，就不需要修改这里了
            var json = JsonUtility.ToJson(currentData);
            var newData = JsonUtility.FromJson<GameData>(json);

            newData.playThrough++;

            // 更新模型，这将触发自动保存
            model.CurrentGameData.Value = newData;

            // 可以发送一个事件通知其他系统周目数已改变
            this.SendEvent(new PlayThroughChangedEvent
            {
                NewPlayThrough = newData.playThrough
            });
        }
    }

    // 周目改变事件
    public struct PlayThroughChangedEvent
    {
        public int NewPlayThrough;
    }
}