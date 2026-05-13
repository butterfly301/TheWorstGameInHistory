// 重新加载当前场景命令  

using HotUpdate.SceneLoad.System;
using QFramework;

namespace HotUpdate.SceneLoad.Commands
{
    public class ReloadCurrentSceneCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var sceneSystem = this.GetSystem<SceneLoadSystem>();
            var currentSceneName = sceneSystem.GetCurrentSceneName();
            if (!string.IsNullOrEmpty(currentSceneName)) this.SendCommand(new LoadSceneCommand(currentSceneName));
        }
    }
}