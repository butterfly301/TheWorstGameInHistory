using HotUpdate.Enemy;
using HotUpdate.World;
using UnityEngine;

namespace HotUpdate.Utility
{
    public class SpawnEnemy : MonoBehaviour
    {
        public int spawnPointIndex;

public void Spawn()
        {
            var spawnPoints = WorldConfigManager1.Instance.GetEnemySpawnPoints(spawnPointIndex);
            EnemyManager.Instance.SpawnEnemy("BugToiletVillage", spawnPoints.position, Quaternion.identity);
        }
    }
}