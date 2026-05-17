using HotUpdate.SceneLoad.System;
using QFramework;

// 加载场景命令
namespace HotUpdate.SceneLoad.Commands
{
    public class LoadSceneCommand : AbstractCommand
    {
        private readonly float _extraWaitSeconds;
        private readonly LoadingScreenType _loadingScreenType;
        private readonly string _sceneAddress;
        private readonly bool _withFade;

public LoadSceneCommand(string sceneAddress, bool withFade = true,
            LoadingScreenType loadingScreenType = LoadingScreenType.Default, float extraWaitSeconds = 3f)
        {
            _sceneAddress = sceneAddress;
            _withFade = withFade;
            _loadingScreenType = loadingScreenType;
            _extraWaitSeconds = extraWaitSeconds;
        }

protected override void OnExecute()
        {
            var sceneSystem = this.GetSystem<SceneLoadSystem>();
            sceneSystem.LoadSceneAsync(_sceneAddress, _withFade, _loadingScreenType, _extraWaitSeconds);
        }
    }
}
