using UnityEngine;

[CreateAssetMenu(menuName = "FlareEngine/ShakesSO")]
public abstract class IShakeStateSaved : ScriptableObject
{
    public abstract void Shake(string shakeName);
}