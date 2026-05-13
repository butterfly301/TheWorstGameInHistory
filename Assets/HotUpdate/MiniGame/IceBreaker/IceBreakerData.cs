using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdate.MiniGame.IceBreaker
{
    [Serializable]
    public class PlayerData
    {
        public float moveSpeed;
        public float jumpForce;
        public float acceleration;
        public float jerk;
        public float explosionForce;
    }

    [Serializable]
    public class LevelData
    {
        public float levelDuration;
        public float platformY;
        public float minObstacleDistance;
        public float maxObstacleDistance;
        public float minEnemyDistance;
        public float maxEnemyDistance;
        public float initialObstacleOffset;
        public float initialEnemyOffset;
        public float explosionForce;
        public float generationZoneWidth;
        public Color frontLayerColor;
        public Color backLayerColor;
        public float minZ;
        public float maxZ;
    }

    [Serializable]
    public class EnemyData
    {
        public float speed;
        public float patrolRange;
        public Vector2 clockwiseSize;
        public float explosionForce;
    }

    [Serializable]
    public class DownloadData
    {
        public List<string> downloadTLH2Urls;
        public List<string> downloadOtherFileUrls;
        public string targetSubFolder;
    }

    [Serializable]
    public class IceBreakerData
    {
        public PlayerData playerData;
        public LevelData levelData;
        public EnemyData enemyData;
        public DownloadData downloadData;
    }
}