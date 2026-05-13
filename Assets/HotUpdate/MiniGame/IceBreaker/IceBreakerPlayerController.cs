using System.Collections;
using System.Collections.Generic;
using HotUpdate.Interface;
using HotUpdate.Manager;
using HotUpdate.Utility;
using Tiny;
using UnityEngine;
using UnityEngine.Serialization;

namespace HotUpdate.MiniGame.IceBreaker
{
    public class IceBreakerPlayerController : MonoBehaviour,IAutoBind
    {
        public enum PlayerState
        {
            Idle,
            Playing,
            Returning,
            Shattered,
            Revive,
            Success
        }

        private static readonly int AttackTrigger = Animator.StringToHash("Attack");

        // 状态机相关
        public PlayerState currentState;

        //地面检测相关
        private readonly float groundCheckRadius = 0.25f;
        private readonly float rearrangeDuration = 1.5f;
        private readonly float returnDuration = 1.0f;
        private readonly float shardSpacing = 0.33f;
        private float acceleration;

        //动画机相关
        [SerializeField]private Transform groundCheck;
        [SerializeField]private Animator animator;
        private Material attackMaterial;
        private float explosionForce;

        private LayerMask groundLayer;
        private Material idleMaterial;
        private float initialAcceleration;
        private float initialMoveSpeed;

        //战斗相关
        private bool isDashing;

        private float jerk;

        private int jumpCount;
        private float jumpForce;

        //数值相关
        private float moveSpeed;
        private Vector3 playerReturnStartPosition;

        //移动相关
        [SerializeField]private Rigidbody2D rigidbody2d;
        private float returnTimer;
        private readonly List<Transform> shards = new();

        private Vector3 shardSize;
        private readonly List<Vector3> shardStartPositions = new();

        private IShakeStateSaved smallShakeSo;

        //显示相关
        [SerializeField]private SpriteRenderer spriteRenderer;
        private Vector3 startPosition;
        private Trail trail;

        private void Update()
        {
            switch (currentState)
            {
                case PlayerState.Playing:
                    PlayingStateUpdate();
                    break;
                case PlayerState.Shattered:
                    ShatteredStateUpdate();
                    break;
                case PlayerState.Revive:
                    ReviveStateUpdate();
                    break;
                case PlayerState.Success:
                    SuccessStateUpdate();
                    break;
            }
        }

        private void FixedUpdate()
        {
            switch (currentState)
            {
                case PlayerState.Playing:
                    PlayingStateFixUpdate();
                    break;
                case PlayerState.Returning:
                    ReturningStateFixUpdate();
                    break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (currentState == PlayerState.Playing)
                // 碰到障碍物或敌人就通知管理器
                if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    if (!isDashing)
                    {
                        TakeDamage();
                    }
                    else
                    {
                        var enemy = other.gameObject.GetComponent<IceBreakerEnemyController>();
                        if (enemy != null) enemy.TakeDamage();
                    }
                }
        }

        public void Init()
        {
            IceBreakerManager.Instance.SetIceBreakerPlayer(this);
            var data = IceBreakerManager.Instance.GetIceBreakerData();
            moveSpeed = data.playerData.moveSpeed;
            initialMoveSpeed = moveSpeed;
            acceleration = data.playerData.acceleration;
            initialAcceleration = acceleration;
            jerk = data.playerData.jerk;
            jumpForce = data.playerData.jumpForce;
            explosionForce = data.playerData.explosionForce;
            groundLayer = LayerMask.GetMask("Platform");

            AddressablesManager.Instance.LoadAssetAsync<RuntimeAnimatorController>(
                AddressableKeys.Animations.MiniGame.IceBreaker.Player.Controller.Player_Controller,
                handle =>
                {
                    animator = GetComponent<Animator>();
                    animator.runtimeAnimatorController = handle.Result;
                });

            rigidbody2d = GetComponent<Rigidbody2D>();

            AddressablesManager.Instance.LoadAssetAsync<IShakeStateSaved>(
                AddressableKeys.ScriptableObjects.Effect.Shakes_Asset,
                handle => { smallShakeSo = handle.Result; });

            spriteRenderer = GetComponent<SpriteRenderer>();
            shardSize = spriteRenderer.bounds.size / 3f;

            trail = GetComponent<Trail>();
            AddressablesManager.Instance.LoadAssetAsync<Material>(
                AddressableKeys.Art.Materials.TrailPlayerAttack_Mat,
                handle =>
                {
                    attackMaterial = handle.Result;
                    AddressablesManager.Instance.LoadAssetAsync<Material>(
                        AddressableKeys.Art.Materials.TrailPlayerIdle_Mat,
                        handle2 => { idleMaterial = handle2.Result; });
                });

            SwitchState(PlayerState.Idle);
        }

        public void SwitchState(PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Idle:
                    moveSpeed = initialMoveSpeed;
                    acceleration = initialAcceleration;
                    isDashing = false;
                    spriteRenderer.enabled = true;
                    rigidbody2d.velocity = Vector2.zero;
                    rigidbody2d.bodyType = RigidbodyType2D.Static;
                    currentState = PlayerState.Idle;
                    break;
                case PlayerState.Playing:
                    // 进入游戏状态的逻辑
                    jumpCount = 0;
                    spriteRenderer.enabled = true;
                    trail.enabled = true;
                    rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
                    currentState = PlayerState.Playing;
                    break;
                case PlayerState.Returning:
                    // 进入返回状态的逻辑
                    rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
                    returnTimer = 0f;
                    playerReturnStartPosition = transform.position;
                    shardStartPositions.Clear();
                    foreach (var shard in shards) shardStartPositions.Add(shard.position);

                    currentState = PlayerState.Returning;
                    break;
                case PlayerState.Shattered:
                    // 进入碎裂状态的逻辑
                    Shatter();
                    trail.enabled = false;
                    rigidbody2d.bodyType = RigidbodyType2D.Static;
                    currentState = PlayerState.Shattered;
                    break;
                case PlayerState.Revive:
                    moveSpeed = initialMoveSpeed;
                    acceleration = initialAcceleration;
                    isDashing = false;
                    spriteRenderer.enabled = true;
                    rigidbody2d.velocity = Vector2.zero;
                    rigidbody2d.bodyType = RigidbodyType2D.Static;
                    foreach (var shard in shards) ObjectPoolManager.Instance.ReturnToPool(shard.gameObject);

                    currentState = PlayerState.Revive;
                    break;
                case PlayerState.Success:
                    Shatter();
                    trail.enabled = false;
                    rigidbody2d.bodyType = RigidbodyType2D.Static;
                    currentState = PlayerState.Success;
                    break;
            }
        }

        private void PlayingStateUpdate()
        {
            if (IceBreakerManager.Instance.GetCurrentGameState() != IceBreakerManager.GameState.Playing)
                return;
            acceleration += jerk * Time.deltaTime;
            moveSpeed += acceleration * Time.deltaTime;

            bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            if (isGrounded) jumpCount = 0;
#if PLATFORM_STANDALONE_WIN
            // 跳跃 - 鼠标右键
            if (Input.GetMouseButtonDown(1)) Jump();

            // 攻击 - 鼠标左键
            if (Input.GetMouseButtonDown(0)) Attack();
#endif
        }

        private void ShatteredStateUpdate()
        {
            // 碎裂状态下的逻辑
#if PLATFORM_STANDALONE_WIN
            if (Input.GetMouseButtonDown(0)) IceBreakerManager.Instance.RestartGame();
#endif
        }

        private void ReviveStateUpdate()
        {
#if PLATFORM_STANDALONE_WIN
            if (Input.GetMouseButtonDown(0)) SwitchState(PlayerState.Playing);
#endif
        }

        private void SuccessStateUpdate()
        {
            // 成功状态下的逻辑
        }

        private void PlayingStateFixUpdate()
        {
            // 持续向右移动
            rigidbody2d.velocity = new Vector2(moveSpeed, rigidbody2d.velocity.y);
        }

        private void ReturningStateFixUpdate()
        {
            returnTimer += Time.fixedDeltaTime;
            var t = Mathf.Clamp01(returnTimer / returnDuration);

            // 在 returnDuration 时间内将玩家主体插值到其起始位置
            transform.position = Vector3.Lerp(playerReturnStartPosition, startPosition, t);

            if (shards.Count == 0)
            {
                // 如果没有碎片，检查主体位置后直接重启
                if (t >= 1.0f)
                {
                    transform.position = startPosition;
                    IceBreakerManager.Instance.RestartGame();
                }

                return;
            }

            for (var i = 0; i < shards.Count; i++)
            {
                // 计算每个碎片在3x3网格中的目标位置
                var x = (i % 3 - 1) * shardSize.x;
                var y = (i / 3 - 1) * shardSize.y;
                var returnPos = startPosition + new Vector3(x, y, 0);

                // 在 returnDuration 时间内将碎片从其起始位置插值到目标位置
                if (i < shardStartPositions.Count)
                    shards[i].position = Vector3.Lerp(shardStartPositions[i], returnPos, t);
            }

            // 如果计时器完成
            if (t >= 1.0f)
            {
                // 到达后，重置并切换到 Revive 状态
                transform.position = startPosition;
                SwitchState(PlayerState.Revive);
            }
        }

        private void Jump()
        {
            if (jumpCount < 1)
            {
                rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, jumpForce);
                jumpCount++;
            }
        }

        private void Attack()
        {
            if (!isDashing) animator.SetTrigger(AttackTrigger);
        }

        public void TakeDamage()
        {
            SwitchState(PlayerState.Shattered);
        }

        private void Shatter()
        {
            smallShakeSo.Shake("SmallShake");
            // 获取敌人自身的尺寸，用于计算碎片位置
            shards.Clear();
            for (var i = 0; i < 9; i++)
            {
                // 计算3x3网格中的位置
                var x = (i % 3 - 1) * shardSize.x;
                var y = (i / 3 - 1) * shardSize.y;
                var spawnPos = transform.position + new Vector3(x, y, 0);
                var shard =
                    ObjectPoolManager.Instance.SpawnFromPool(PoolTag.PlayerShard, spawnPos, Quaternion.identity);
                shards.Add(shard.transform);
                var shardRb = shard.GetComponent<Rigidbody2D>();

                if (shardRb != null)
                {
                    Vector2 direction = (shard.transform.position - transform.position).normalized;
                    if (direction.sqrMagnitude < 0.01f) // 中心碎片
                        direction = Random.insideUnitCircle.normalized;

                    shardRb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                    spriteRenderer.enabled = false;
                }
            }
        }

        public void ResetPlayer()
        {
            moveSpeed = initialMoveSpeed;
            acceleration = initialAcceleration;
            SwitchState(PlayerState.Returning);
        }

        public void RearrangeShards(Vector3 centerPosition)
        {
            if (currentState == PlayerState.Success) StartCoroutine(RearrangeShardsCoroutine(centerPosition));
        }

        private IEnumerator RearrangeShardsCoroutine(Vector3 centerPosition)
        {
            var totalWidth = (shards.Count - 1) * shardSpacing;
            var startOffset = new Vector3(-totalWidth / 2, 0, 0);

            var startPositions = new List<Vector3>();
            var startRotations = new List<Quaternion>();
            var targetPositions = new List<Vector3>();

            for (var i = 0; i < shards.Count; i++)
            {
                if (shards[i] == null) continue;

                var shardRb = shards[i].GetComponent<Rigidbody2D>();
                if (shardRb != null)
                {
                    shardRb.velocity = Vector2.zero;
                    shardRb.angularVelocity = 0;
                    shardRb.bodyType = RigidbodyType2D.Kinematic;
                }

                var shardRenderer = shards[i].GetComponent<SpriteRenderer>();
                if (shardRenderer != null) shardRenderer.sortingOrder = 5;

                startPositions.Add(shards[i].position);
                startRotations.Add(shards[i].rotation);

                var targetPos = centerPosition + startOffset + new Vector3(i * shardSpacing, 0, 0);
                targetPositions.Add(targetPos);
            }

            var elapsedTime = 0f;
            while (elapsedTime < rearrangeDuration)
            {
                elapsedTime += Time.deltaTime;
                var t = Mathf.Clamp01(elapsedTime / rearrangeDuration);
                t = t * t * (3f - 2f * t); // SmoothStep

                for (var i = 0; i < shards.Count; i++)
                    if (shards[i] != null)
                    {
                        shards[i].position = Vector3.Lerp(startPositions[i], targetPositions[i], t);
                        shards[i].rotation = Quaternion.Lerp(startRotations[i], Quaternion.identity, t);
                    }

                yield return null;
            }

            for (var i = 0; i < shards.Count; i++)
                if (shards[i] != null)
                {
                    shards[i].position = targetPositions[i];
                    shards[i].rotation = Quaternion.identity;
                }
        }

        public void SetIsDashing(bool value)
        {
            isDashing = value;
            trail?.SetMaterial(isDashing ? attackMaterial : idleMaterial);
        }

        public void SetStartPosition(Vector3 startPos)
        {
            transform.position = startPos;
            startPosition = startPos;
        }

        public PlayerState GetCurrentState()
        {
            return currentState;
        }
    }
}