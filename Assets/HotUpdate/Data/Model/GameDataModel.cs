using HotUpdate.Data.Utility;
using QFramework;

namespace HotUpdate.Data.Model
{
    public class GameDataModel : AbstractModel
    {
        // 使用BindableProperty包装GameData，当其值变化时可被监听[3](@ref)
        public BindableProperty<GameData> CurrentGameData { get; private set; }

protected override void OnInit()
        {
            CurrentGameData = new BindableProperty<GameData>(new GameData());

// 从存档加载数据
            var storage = this.GetUtility<IStorage>();
            var loadedData = storage.Load<GameData>("TheWorstGameInHistory_Save");

if (loadedData != null)
                CurrentGameData.Value = loadedData;
            else
                storage.Save("TheWorstGameInHistory_Save", CurrentGameData);

// 注册一个监听，当CurrentGameData的值变化时自动保存
            CurrentGameData.Register(newData =>
            {
                // 保存到本地存储
                storage.Save("TheWorstGameInHistory_Save", newData);
            });
        }
    }
}