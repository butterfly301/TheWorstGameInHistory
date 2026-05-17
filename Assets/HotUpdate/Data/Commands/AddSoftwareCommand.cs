using HotUpdate.Data.Model;
using QFramework;
using UnityEngine;

namespace HotUpdate.Data.Commands
{
    public class AddSoftwareCommand : AbstractCommand
    {
        private readonly SoftwareName softwareName;

public AddSoftwareCommand(SoftwareName softwareName)
        {
            this.softwareName = softwareName;
        }

protected override void OnExecute()
        {
            var model = this.GetModel<GameDataModel>();
            var currentData = model.CurrentGameData.Value;

// 使用 JsonUtility 进行深拷贝，确保数据不可变性并触发更新
            var json = JsonUtility.ToJson(currentData);
            var newData = JsonUtility.FromJson<GameData>(json);

if (!newData.software.Contains(softwareName))
            {
                newData.software.Add(softwareName);
                model.CurrentGameData.Value = newData;
            }
        }
    }
}