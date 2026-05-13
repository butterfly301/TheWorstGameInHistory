using HotUpdate.SceneLoad.System;
using QFramework;

// 卸载当前场景命令
namespace HotUpdate.SceneLoad.Commands
{
    public class UnloadCurrentSceneCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetSystem<SceneLoadSystem>().UnloadCurrentScene();
        }
    }
}