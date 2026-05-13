using QFramework;
using UnityEngine.Events;

public class WorldManagerBase : MonoSingleton<WorldManagerBase>
{
    public virtual void RegisterEvent(string eventName, UnityAction func)
    {
    }

    public virtual void Unpause()
    {
    }

    public virtual void DeleteAllSavedData()
    {
    }
}