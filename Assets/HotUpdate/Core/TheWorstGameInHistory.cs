using HotUpdate.Audio.System;
using HotUpdate.Browser;
using HotUpdate.Console;
using HotUpdate.Data.Model;
using HotUpdate.Data.Utility;
using HotUpdate.Dialogue.Model;
using HotUpdate.Dialogue.System;
using HotUpdate.Download.System;
using HotUpdate.Enemy;
using HotUpdate.SceneLoad.System;
using QFramework;

namespace HotUpdate.Core
{
    public class TheWorstGameInHistory : Architecture<TheWorstGameInHistory>
    {
        protected override void Init()
        {
            // 注册系统
            RegisterSystem(new SceneLoadSystem());
            RegisterSystem(new AudioSystem());
            RegisterSystem(new BrowserSystem());
            RegisterSystem(new ConsoleSystem());
            RegisterSystem(new DownloadSystem());
            RegisterSystem(new DialogueSystem());

            // 注册数据模型
            RegisterModel(new GameDataModel());
            RegisterModel(new DialogueModel());
            RegisterModel(new EnemyDataModel());

            // 注册存储工具
            RegisterUtility<IStorage>(new JsonStorage());
        }
    }
}