using System.Collections;
using HotUpdate.Manager;
using UnityEngine;
using UnityEngine.Events;

namespace HotUpdate.Utility
{
    public class DestroyAfterDelay : MonoBehaviour
    {
        public UnityEvent onSpawn;
        public UnityEvent onDespawn;

public string[] assetNames;

private void Start()
        {
            onSpawn.Invoke();
        }

private void OnDestroy()
        {
            onDespawn.Invoke();
        }

public void DestroyMyself(float seconds = 0f)
        {
            StartCoroutine(DestroyAfterDelayCoroutine(seconds));
        }

private IEnumerator DestroyAfterDelayCoroutine(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (assetNames.Length > 0)
                Release();
            else
                Destroy(gameObject);
        }

public void Release()
        {
            foreach (var assetName in assetNames) AddressablesManager.Instance.Release<GameObject>(assetName);
        }
    }
}