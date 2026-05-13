using UnityEngine;

public class InitializationIndex : MonoBehaviour
{
    public string index;

    private async void Start()
    {
        // 初始化HybridCLR
        await HotUpdateReflectionUtility.FullInitializeAsync();

        if (HotUpdateReflectionUtility.IsFullyInitialized()) HotUpdateReflectionUtility.Open(index);
    }
}