using QFramework;
using UnityEngine;

public class WorldConfigManager2 : MonoSingleton<WorldConfigManager2>
{
    public Transform PlayerSpawnPoint { get; private set; }

private void Awake()
    {
        PlayerSpawnPoint = transform.Find("PlayerSpawnPoint");
    }
}