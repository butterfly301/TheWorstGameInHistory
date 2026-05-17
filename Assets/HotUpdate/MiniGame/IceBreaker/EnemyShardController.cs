using System.Collections;
using HotUpdate.Manager;
using UnityEngine;

namespace HotUpdate.MiniGame.IceBreaker
{
    public class EnemyShardController : MonoBehaviour
    {
        [Tooltip("碎片存活时间")] [SerializeField] private float lifeTime = 2f;

private void OnEnable()
        {
            StartCoroutine(ReturnToPoolAfterDelay());
        }

private IEnumerator ReturnToPoolAfterDelay()
        {
            yield return new WaitForSeconds(lifeTime);
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
        }
    }
}