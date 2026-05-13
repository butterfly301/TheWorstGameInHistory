using HotUpdate.SceneLoad.System;
using QFramework;

namespace HotUpdate.SceneLoad.Commands
{
    public class HideLoadingScreenCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var sceneSystem = this.GetSystem<SceneLoadSystem>();
            sceneSystem.HideLoadingScreen();
        }
    }
}