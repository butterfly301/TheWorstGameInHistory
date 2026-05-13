using HotUpdate.Manager;
using UnityEngine;

public class EffectBase : MonoBehaviour
{
    [Tooltip("If true, the object will be returned to the ObjectPoolManager. If false, it will be destroyed.")]
    public bool isPooled = true;

    /// <summary>
    ///     Finishes the effect by either pooling or destroying the GameObject.
    /// </summary>
    public void FinishEffect()
    {
        if (isPooled)
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }
}