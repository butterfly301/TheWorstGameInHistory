using System.Collections;
using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;

namespace HotUpdate.MiniGame.IceBreaker
{
    public class LevelGenerator : MonoBehaviour
    {
        private Transform backgroundParent;
        private Color backLayerColor;
        private readonly float buildingAnimationDuration = 0.5f;
        private BuildingDrawer buildingDrawer;
        private GameObject enemyPrefab;
        private Color frontLayerColor;
        private float gameStartTime;

private float generationZoneWidth;
        private GameObject goalObj;
        private GameObject goalPrefab;
        private bool initialized;

private float levelDuration; // 游戏时长，单位秒
        private float maxEnemyDistance;
        private float maxObstacleDistance;
        private float maxZ;
        private float minEnemyDistance;
        private float minObstacleDistance;
        private float minZ;
        private float nextBackgroundSpawnX;

private float nextEnemyX;

private float nextObstacleX;

private Vector3 nextSpawnPoint;
        private GameObject obstaclePrefab;
        private GameObject platformPrefab;
        private float platformY;
        private Transform player;

private void Update()
        {
            if (initialized)
            {
                // 当玩家接近当前已生成世界的边缘时，生成新的部分
                if (player.position.x + 20f > nextSpawnPoint.x) SpawnSegment();

// 当玩家接近背景生成边缘时，生成新的楼房
                if (player.position.x > nextBackgroundSpawnX - generationZoneWidth) SpawnBackgroundBuildings();

// 检查是否到达生成终点的时间
                if (goalObj == null && Time.time - gameStartTime >= levelDuration) SpawnGoal();
            }
        }

public void Init(Transform playerTransform)
        {
            player = playerTransform;
            IceBreakerManager.Instance.SetLevelGenerator(this);
            var data = IceBreakerManager.Instance.GetIceBreakerData();
            var levelData = data.levelData;
            levelDuration = levelData.levelDuration;
#if UNITY_EDITOR
            levelDuration = 10;
#endif
            platformY = levelData.platformY;
            minObstacleDistance = levelData.minObstacleDistance;
            maxObstacleDistance = levelData.maxObstacleDistance;
            minEnemyDistance = levelData.minEnemyDistance;
            maxEnemyDistance = levelData.maxEnemyDistance;

generationZoneWidth = levelData.generationZoneWidth;
            frontLayerColor = levelData.frontLayerColor;
            backLayerColor = levelData.backLayerColor;
            minZ = levelData.minZ;
            maxZ = levelData.maxZ;

buildingDrawer = gameObject.AddComponent<BuildingDrawer>();

AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.PlatformPrefab_Prefab,
                handle =>
                {
                    platformPrefab = handle.Result;

AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                        AddressableKeys.ObstaclePrefab_Prefab,
                        handle2 =>
                        {
                            obstaclePrefab = handle2.Result;
                            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                                AddressableKeys.EnemyPrefab_Prefab,
                                handle3 =>
                                {
                                    enemyPrefab = handle3.Result;
                                    AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                                        AddressableKeys.GoalPrefab_Prefab,
                                        handle4 =>
                                        {
                                            goalPrefab = handle4.Result;

// Create pools
                                            ObjectPoolManager.Instance.CreatePool(PoolTag.Obstacle, obstaclePrefab, 10);
                                            ObjectPoolManager.Instance.CreatePool(PoolTag.Enemy, enemyPrefab, 10);

// 生成一个超长平台，避免拼接缝隙导致卡顿
                                            var bigPlatform = Instantiate(platformPrefab,
                                                new Vector3(2500, platformY, 0), Quaternion.identity);
                                            bigPlatform.transform.localScale = new Vector3(5000, 1, 1);

InitBackgroundGenerator();
                                            StartLevelGeneration();
                                        });
                                });
                        });
                });
        }

private void StartLevelGeneration()
        {
            var data = IceBreakerManager.Instance.GetIceBreakerData();
            // 初始化生成点位置为固定高度
            nextSpawnPoint = new Vector3(0, platformY, 0);
            nextObstacleX = data.levelData.initialObstacleOffset;
            nextEnemyX = data.levelData.initialEnemyOffset;

// 生成安全的初始平台
            SpawnSegment(true);
            // 将玩家放置在第一个平台的安全位置（同一高度）
            var startPosition = new Vector3(5, platformY + 1, 0);
            player.gameObject.GetComponent<IceBreakerPlayerController>().SetStartPosition(startPosition);
            SpawnSegment(true);

// 生成剩余的初始平台
            for (var i = 0; i < 3; i++) SpawnSegment();

//记录起始时间
            gameStartTime = Time.time;
            initialized = true;
        }

private void SpawnSegment(bool isSafe = false)
        {
            // 固定同一高度
            nextSpawnPoint.y = platformY;

// 平台长度可以继续随机，用来分段生成障碍物
            var platformLength = Random.Range(5f, 15f);
            var startX = nextSpawnPoint.x;
            var endX = startX + platformLength;

if (isSafe)
            {
                // 如果是安全区域，推迟生成点
                if (nextObstacleX < endX) nextObstacleX = endX + Random.Range(minObstacleDistance, maxObstacleDistance);

if (nextEnemyX < endX) nextEnemyX = endX + Random.Range(minEnemyDistance, maxEnemyDistance);
            }
            else
            {
                while (nextObstacleX < endX)
                {
                    var randomScaleY = Random.Range(1f, 2f);
                    var randomScaleX = Random.Range(1f, 3f);
                    var obstaclePosition = new Vector3(nextObstacleX, platformY + randomScaleY / 2f, 0);
                    var obstacle =
                        ObjectPoolManager.Instance.SpawnFromPool(PoolTag.Obstacle, obstaclePosition,
                            Quaternion.identity);
                    if (obstacle != null) obstacle.transform.localScale = new Vector3(randomScaleX, randomScaleY, 1);

nextObstacleX += Random.Range(minObstacleDistance, maxObstacleDistance);
                }

while (nextEnemyX < endX)
                {
                    var randomScaleY = Random.Range(0f, 4f);
                    var enemyPosition = new Vector3(nextEnemyX, platformY + randomScaleY, 0);
                    ObjectPoolManager.Instance.SpawnFromPool(PoolTag.Enemy, enemyPosition, Quaternion.identity);
                    nextEnemyX += Random.Range(minEnemyDistance, maxEnemyDistance);
                }
            }

// 更新下一个生成点的位置
            nextSpawnPoint.x += platformLength;
        }

private void InitBackgroundGenerator()
        {
            backgroundParent = new GameObject("BackgroundBuildings").transform;

nextBackgroundSpawnX = player.position.x - generationZoneWidth / 2;

// Initial generation to fill the screen
            while (nextBackgroundSpawnX < player.position.x + generationZoneWidth) SpawnBackgroundBuildings();
        }

private void SpawnBackgroundBuildings()
        {
            // 为当前位置生成一个楼房的随机尺寸
            var numRows = Random.Range(4, 13);
            var numCols = Random.Range(2, 6);
            var scale = 2.0f; // 缩放比例
            var padding = 0.3f * scale;
            var windowWidth = 0.5f * scale;
            var windowHeight = 0.7f * scale;
            var buildingWidth = numCols * windowWidth + (numCols + 1) * padding;
            var buildingHeight = numRows * windowHeight + (numRows + 1) * padding;

// 动态创建一个楼房
            var building = CreateBuilding(numRows, numCols, padding, windowWidth, windowHeight, buildingWidth,
                buildingHeight);

// 将楼房放置在背景父物体下
            building.transform.SetParent(backgroundParent);

// 为楼房设置一个随机的Z深度
            var randomZ = Random.Range(minZ, maxZ);

// The building's base should be at the top of the platform.
            building.transform.position = new Vector3(nextBackgroundSpawnX, platformY - 7f, randomZ);

// 根据Z位置调整颜色和细节
            ConfigureBuildingAppearance(building, randomZ);

// Animate the building drawing
            StartCoroutine(buildingDrawer.AnimateDrawing(building, buildingAnimationDuration));

// 更新下一个生成点
            nextBackgroundSpawnX += buildingWidth * Random.Range(0.5f, 2.0f); // Use smaller, more frequent spacing
        }

private GameObject CreateBuilding(int numRows, int numCols, float padding, float windowWidth,
            float windowHeight, float totalWidth, float totalHeight)
        {
            // 1. 创建根对象
            var buildingGo = new GameObject("LineBuilding");
            var lineRenderer = buildingGo.AddComponent<LineRenderer>();

// 2. 配置外部轮廓的 LineRenderer
            lineRenderer.useWorldSpace = false;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.numCornerVertices = 4;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

DrawRectangle(lineRenderer, totalWidth, totalHeight);

// 4. 创建窗户
            for (var r = 0; r < numRows; r++)
            for (var c = 0; c < numCols; c++)
            {
                var windowGo = new GameObject($"Window_{r}_{c}");
                windowGo.transform.SetParent(buildingGo.transform);

var x = padding + c * (windowWidth + padding);
                var y = padding + r * (windowHeight + padding);
                windowGo.transform.localPosition = new Vector3(x, y, 0);

var windowLr = windowGo.AddComponent<LineRenderer>();
                windowLr.useWorldSpace = false;
                windowLr.startWidth = 0.05f;
                windowLr.endWidth = 0.05f;
                windowLr.numCornerVertices = 4;
                windowLr.material = new Material(Shader.Find("Sprites/Default"));
                DrawRectangle(windowLr, windowWidth, windowHeight);
            }

return buildingGo;
        }

private void ConfigureBuildingAppearance(GameObject buildingInstance, float zPos)
        {
            var t = Mathf.InverseLerp(minZ, maxZ, zPos);
            var buildingColor = Color.Lerp(frontLayerColor, backLayerColor, t);

// 应用颜色到楼房的所有 LineRenderer (包括窗户)
            var allRenderers = buildingInstance.GetComponentsInChildren<LineRenderer>();
            foreach (var lr in allRenderers)
            {
                lr.startColor = buildingColor;
                lr.endColor = buildingColor;
            }

// (可选) 隐藏远处图层的窗户以提高性能
            // 如果楼房很远（例如，在Z范围的后半部分），则隐藏窗户
            if (t > 0.5f)
                foreach (Transform child in buildingInstance.transform)
                    if (child.name.StartsWith("Window"))
                        child.gameObject.SetActive(false);
        }

private void DrawRectangle(LineRenderer lineRenderer, float width, float height)
        {
            lineRenderer.positionCount = 5;
            var points = new[]
            {
                new Vector3(0, 0, 0),
                new Vector3(width, 0, 0),
                new Vector3(width, height, 0),
                new Vector3(0, height, 0),
                new Vector3(0, 0, 0)
            };
            lineRenderer.SetPositions(points);
        }

private void SpawnGoal()
        {
            // 在路径末端生成终点（同一高度略微抬高）
            var goalPosition = new Vector3(nextSpawnPoint.x + 10f, platformY, 0);
            goalObj = Instantiate(goalPrefab, goalPosition, Quaternion.identity);
        }

public void ResetLevel()
        {
            ObjectPoolManager.Instance.ReturnToPool(PoolTag.Obstacle);
            ObjectPoolManager.Instance.ReturnToPool(PoolTag.Enemy);
            ObjectPoolManager.Instance.ReturnToPool(PoolTag.EnemyShard);
            Destroy(goalObj);
            // 停止所有正在运行的协程，特别是建筑绘制动画
            StopAllCoroutines();

StartCoroutine(ResetBackgroundAndLevel());
        }

private IEnumerator ResetBackgroundAndLevel()
        {
            if (backgroundParent != null && backgroundParent.childCount > 0)
            {
                var buildingCount = backgroundParent.childCount;
                var completedCount = 0;

foreach (Transform building in backgroundParent)
                    StartCoroutine(buildingDrawer.AnimateErasing(building.gameObject, buildingAnimationDuration,
                        () => { completedCount++; }));

yield return new WaitUntil(() => completedCount >= buildingCount);

Destroy(backgroundParent.gameObject);
            }

// 重新初始化背景生成器
            InitBackgroundGenerator();

// 重置关卡生成相关的变量
            nextSpawnPoint = new Vector3(0, platformY, 0);
            var data = IceBreakerManager.Instance.GetIceBreakerData();
            nextObstacleX = data.levelData.initialObstacleOffset;
            nextEnemyX = data.levelData.initialEnemyOffset;

StartLevelGeneration();
        }
    }
}