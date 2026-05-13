using HotUpdate.Data.Model;
using QFramework;
using UnityEngine;

namespace HotUpdate.Data.Commands
{
    // 删除软件的命令
    public class RemoveSoftwareCommand : AbstractCommand
    {
        private readonly SoftwareName _software;

        public RemoveSoftwareCommand(SoftwareName software)
        {
            _software = software;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<GameDataModel>();
            var currentData = model.CurrentGameData.Value;

            // 深拷贝避免直接修改引用
            var json = JsonUtility.ToJson(currentData);
            var newData = JsonUtility.FromJson<GameData>(json);

            if (newData.software != null) newData.software.Remove(_software);

            model.CurrentGameData.Value = newData;
        }
    }
}