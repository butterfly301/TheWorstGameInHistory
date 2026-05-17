using System.Collections;
using System.Collections.Generic;
using HotUpdate.Audio.Commands;
using HotUpdate.Core;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HotUpdate.MiniGame.IceBreaker
{
    public class IceBreakerGoalController : MonoBehaviour, IController
    {
        [HideInInspector] public List<Rigidbody2D> goalShards;
        private readonly float rearrangeDuration = 1.5f; // 重新排列动画的持续时间
        private readonly float shardSpacing = 1f; // 碎片之间的间距
        private Vector3 cameraPosition;
        private Transform canvasTransform;
        private float explosionForce;
        private IceBreakerGoalCanvas iceBreakerGoalCanvas;
        private bool isFirstTimeTrigger = true;
        private IceBreakerPlayerController player;

        private void Start()
        {
            foreach (Transform child in transform)
                if (child.name.Contains("GoalShard"))
                    goalShards.Add(child.GetComponent<Rigidbody2D>());

            canvasTransform = transform.Find("Canvas");
            iceBreakerGoalCanvas = canvasTransform.GetComponent<IceBreakerGoalCanvas>();
            iceBreakerGoalCanvas.Init(this);
            var data = IceBreakerManager.Instance.GetIceBreakerData();
            explosionForce = data.levelData.explosionForce;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<PlayerBody>(out _))
                if (isFirstTimeTrigger)
                {
                    isFirstTimeTrigger = false;
                    Shatter();
                    iceBreakerGoalCanvas.SetResultButtonInteractable(true);
                    player = other.GetComponent<IceBreakerPlayerController>();
                    player.SwitchState(IceBreakerPlayerController.PlayerState.Success);
                    this.SendCommand(new PlayMusicCommand(AddressableKeys.Hate__Birth_and_Humanity_Mp3));
                }
        }

        public IArchitecture GetArchitecture()
        {
            return TheWorstGameInHistory.Interface;
        }

        private void Shatter()
        {
            foreach (var t in goalShards)
                if (t != null)
                {
                    t.bodyType = RigidbodyType2D.Dynamic;
                    Vector2 direction = (t.transform.position - transform.position).normalized;
                    if (direction.sqrMagnitude < 0.01f) // 中心碎片
                        direction = Random.insideUnitCircle.normalized;

                    t.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                }
        }

        public void StartRearranging()
        {
            if (Camera.main != null)
                cameraPosition = new Vector3
                    (Camera.main.transform.position.x, Camera.main.transform.position.y, -5);
            StartCoroutine(RearrangeShards());
        }

        private IEnumerator RearrangeShards()
        {
            var rows = 6;
            var cols = 8;
            var totalWidth = (cols - 1) * shardSpacing;
            var totalHeight = (rows - 1) * shardSpacing;

            var center = cameraPosition;
            var startOffset = new Vector3(-totalWidth / 2, totalHeight / 2, 0);

            var startPositions = new List<Vector3>();
            var targetPositions = new List<Vector3>();

            StartCoroutine(iceBreakerGoalCanvas.MoveCanvas(cameraPosition, rearrangeDuration));

            for (var i = 0; i < goalShards.Count; i++)
            {
                if (goalShards[i] == null) continue;

                // 禁用物理效果以进行平滑移动
                goalShards[i].velocity = Vector2.zero;
                goalShards[i].angularVelocity = 0;
                goalShards[i].bodyType = RigidbodyType2D.Kinematic;

                startPositions.Add(goalShards[i].transform.position);

                var row = i / cols;
                var col = i % cols;

                var targetPos = center + startOffset + new Vector3(col * shardSpacing, -row * shardSpacing, 0);
                targetPositions.Add(targetPos);
            }

            var elapsedTime = 0f;
            while (elapsedTime < rearrangeDuration)
            {
                elapsedTime += Time.deltaTime;
                var t = Mathf.Clamp01(elapsedTime / rearrangeDuration);
                // 使用 SmoothStep 获得更平滑的缓动效果
                t = t * t * (3f - 2f * t);

                for (var i = 0; i < goalShards.Count; i++)
                    if (goalShards[i] != null)
                    {
                        goalShards[i].transform.position = Vector3.Lerp(startPositions[i], targetPositions[i], t);
                        goalShards[i].transform.rotation =
                            Quaternion.Lerp(goalShards[i].transform.rotation, Quaternion.identity, t);
                    }

                yield return null;
            }

            // 确保所有碎片都在最终位置并锁定
            for (var i = 0; i < goalShards.Count; i++)
                if (goalShards[i] != null)
                {
                    goalShards[i].transform.position = targetPositions[i];
                    goalShards[i].transform.rotation = Quaternion.identity;
                }
        }

        public Vector3 GetCameraPosition()
        {
            return cameraPosition;
        }

        public IceBreakerPlayerController GetPlayer()
        {
            return player;
        }
    }
}
