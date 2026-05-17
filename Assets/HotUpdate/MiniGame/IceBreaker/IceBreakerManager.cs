using HotUpdate.Core;
using HotUpdate.Interface;
using HotUpdate.Manager;
using HotUpdate.SceneLoad.Commands;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.MiniGame.IceBreaker
{
    public class IceBreakerManager : MonoSingleton<IceBreakerManager>, IController,IAutoBind
    {
        public enum GameState
        {
            Playing,
            Paused
        }

        private GameState currentState = GameState.Playing;
        private IceBreakerData data;
        private GameObject enemyShardPrefab;
        private Button exitButton;

        //UI相关
        [SerializeField]private Transform iceBreakerCanvas;
        private bool isPaused;
        private LevelGenerator levelGenerator;
        [SerializeField]private Transform pausePanel;
        private IceBreakerPlayerController playerController;
        private GameObject playerShardPrefab;

        private void Awake()
        {
            // 初始化
            Time.timeScale = 1f;
            LoadGameAssets();
            exitButton = pausePanel.Find("ExitButton").GetComponent<Button>();
            exitButton.onClick.AddListener(() =>
            {
                this.SendCommand(new LoadSceneCommand(AddressableKeys.MainMenu_Unity, false));
            });
            pausePanel.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) TogglePauseGame();
        }

        public IArchitecture GetArchitecture()
        {
            return TheWorstGameInHistory.Interface;
        }

        private void LoadGameAssets()
        {
            AddressablesManager.Instance.LoadAssetAsync<TextAsset>(
                AddressableKeys.IceBreakerData_Json,
                handle =>
                {
                    var json = handle.Result.text;
                    data = JsonUtility.FromJson<IceBreakerData>(json);
                });

            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.PlayerShard_Prefab,
                handle =>
                {
                    playerShardPrefab = handle.Result;
                    ObjectPoolManager.Instance.CreatePool(PoolTag.PlayerShard, playerShardPrefab, 27);
                });

            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.EnemyShard_Prefab,
                handle =>
                {
                    enemyShardPrefab = handle.Result;
                    ObjectPoolManager.Instance.CreatePool(PoolTag.EnemyShard, enemyShardPrefab, 27);
                });
        }

        /// <summary>
        ///     开始游戏，这个方法由UI按钮调用
        /// </summary>
        public void StartGame()
        {
            if (playerController != null) playerController.SwitchState(IceBreakerPlayerController.PlayerState.Playing);
        }

        /// <summary>
        ///     重新开始游戏
        /// </summary>
        public void RestartGame()
        {
            // 确保时间恢复正常
            Time.timeScale = 1f;
            if (playerController != null) playerController.ResetPlayer();

            if (levelGenerator != null) levelGenerator.ResetLevel();
        }

        /// <summary>
        ///     切换游戏暂停状态
        /// </summary>
        private void TogglePauseGame()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
            pausePanel.gameObject.SetActive(isPaused);
            currentState = isPaused ? GameState.Paused : GameState.Playing;
        }

        public void SetIceBreakerPlayer(IceBreakerPlayerController iceBreakerPlayerController)
        {
            playerController = iceBreakerPlayerController;
        }

        public void SetLevelGenerator(LevelGenerator iceBreakerLevelGenerator)
        {
            levelGenerator = iceBreakerLevelGenerator;
        }

        public IceBreakerData GetIceBreakerData()
        {
            return data;
        }

        public GameState GetCurrentGameState()
        {
            return currentState;
        }
    }
}