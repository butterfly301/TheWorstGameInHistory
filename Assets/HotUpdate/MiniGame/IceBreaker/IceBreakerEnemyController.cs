using System;
using System.Collections.Generic;
using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HotUpdate.MiniGame.IceBreaker
{
    public class IceBreakerEnemyController : MonoBehaviour
    {
        private Vector2 clockwiseSize; // For Clockwise
        private int currentWaypointIndex;
        private Coroutine dieCoroutine;
        private float explosionForce; // 碎片爆炸力
        private bool isHit;
        private bool isShattering; // 防止重复执行碎裂
        private float patrolRange; // For Vertical/Horizontal

[Header("Patrol Settings")] private PatrolType patrolType = PatrolType.Vertical;
        private Rigidbody2D rb;

private IShakeStateSaved smallShakeSo;
        private float speed;

private Vector3 startPos;
        private readonly List<Vector3> waypoints = new();

private void Awake()
        {
            rb = GetComponentInChildren<Rigidbody2D>();
            var data = IceBreakerManager.Instance.GetIceBreakerData().enemyData;
            speed = data.speed;
            patrolRange = data.patrolRange;
            clockwiseSize = data.clockwiseSize;
            explosionForce = data.explosionForce;
        }

private void Start()
        {
            AddressablesManager.Instance.LoadAssetAsync<IShakeStateSaved>(
                AddressableKeys.Shakes_Asset,
                handle => { smallShakeSo = handle.Result; });

rb.isKinematic = true;
        }

private void Update()
        {
            if (!isHit) Patrol();
        }

private void OnEnable()
        {
            // 随机化巡逻类型
            var patrolTypeValues = Enum.GetValues(typeof(PatrolType));
            patrolType = (PatrolType)patrolTypeValues.GetValue(Random.Range(0, patrolTypeValues.Length));

startPos = transform.position;
            InitializeWaypoints();
            currentWaypointIndex = 0;
            isHit = false;
            isShattering = false;

if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector2.zero;
            }
        }

private void InitializeWaypoints()
        {
            waypoints.Clear();
            switch (patrolType)
            {
                case PatrolType.Vertical:
                    waypoints.Add(startPos + Vector3.up * patrolRange / 2);
                    waypoints.Add(startPos - Vector3.up * patrolRange / 2);
                    break;
                case PatrolType.Horizontal:
                    waypoints.Add(startPos + Vector3.left * patrolRange / 2);
                    waypoints.Add(startPos + Vector3.right * patrolRange / 2);
                    break;
                case PatrolType.Clockwise:
                    var w = clockwiseSize.x / 2;
                    var h = clockwiseSize.y / 2;
                    waypoints.Add(startPos + new Vector3(-w, h, 0));
                    waypoints.Add(startPos + new Vector3(w, h, 0));
                    waypoints.Add(startPos + new Vector3(w, -h, 0));
                    waypoints.Add(startPos + new Vector3(-w, -h, 0));
                    break;
            }
        }

private void Patrol()
        {
            if (waypoints.Count == 0) return;

var target = waypoints[currentWaypointIndex];
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

if (Vector3.Distance(transform.position, target) < 0.01f)
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }

private void Shatter()
        {
            if (isShattering) return;
            isShattering = true;

// 获取敌人自身的尺寸，用于计算碎片位置
            var shardSize = GetComponentInChildren<SpriteRenderer>().bounds.size / 3f;

for (var i = 0; i < 9; i++)
            {
                // 计算3x3网格中的位置
                var x = (i % 3 - 1) * shardSize.x;
                var y = (i / 3 - 1) * shardSize.y;
                var spawnPos = transform.position + new Vector3(x, y, 0);

var shard =
                    ObjectPoolManager.Instance.SpawnFromPool(PoolTag.EnemyShard, spawnPos, Quaternion.identity);
                var shardRb = shard.GetComponent<Rigidbody2D>();

if (shardRb != null)
                {
                    Vector2 direction = (shard.transform.position - transform.position).normalized;
                    if (direction.sqrMagnitude < 0.01f) // 中心碎片
                        direction = Random.insideUnitCircle.normalized;

shardRb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                }
            }

Die(); // 回收原敌人
        }

public void TakeDamage()
        {
            if (isShattering) return; // 如果正在碎裂，则不再执行

smallShakeSo.Shake("LittleShake");

Shatter();
        }

private void Die()
        {
            if (!gameObject.activeSelf) return;
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
        }

private enum PatrolType
        {
            Vertical,
            Horizontal,
            Clockwise
        }
    }
}
