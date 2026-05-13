using System.Collections.Generic;
using UnityEngine;

public class AOTGenericReferences : MonoBehaviour
{
    // {{ AOT assemblies
    public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
    {
        "Newtonsoft.Json.dll",
        "QFramework.CoreKit.dll",
        "QFramework.dll",
        "System.Core.dll",
        "System.dll",
        "Unity.Addressables.dll",
        "Unity.ResourceManager.dll",
        "UnityEngine.CoreModule.dll",
        "UnityEngine.JSONSerializeModule.dll",
        "mscorlib.dll"
    };
    // }}

    // {{ constraint implement type
    // }} 

    // {{ AOT generic types
    // DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>>
    // DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
    // DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // DelegateList<float>
    // QFramework.Architecture.<>c<object>
    // QFramework.Architecture<object>
    // QFramework.BindableProperty.<>c<float>
    // QFramework.BindableProperty.<>c<object>
    // QFramework.BindableProperty.<>c__DisplayClass17_0<float>
    // QFramework.BindableProperty.<>c__DisplayClass17_0<object>
    // QFramework.BindableProperty<float>
    // QFramework.BindableProperty<object>
    // QFramework.EasyEvent.<>c<float>
    // QFramework.EasyEvent.<>c<object>
    // QFramework.EasyEvent.<>c__DisplayClass1_0<float>
    // QFramework.EasyEvent.<>c__DisplayClass1_0<object>
    // QFramework.EasyEvent.<>c__DisplayClass4_0<float>
    // QFramework.EasyEvent.<>c__DisplayClass4_0<object>
    // QFramework.EasyEvent<float>
    // QFramework.EasyEvent<object>
    // QFramework.MonoSingleton<object>
    // System.Action<HotUpdate.Console.ConsoleSystem.LogData>
    // System.Action<UnityEngine.Quaternion>
    // System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,object>
    // System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>>
    // System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
    // System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Action<UnityEngine.Vector3>
    // System.Action<float>
    // System.Action<int>
    // System.Action<object,object>
    // System.Action<object>
    // System.ByReference<ushort>
    // System.Collections.Generic.ArraySortHelper<HotUpdate.Console.ConsoleSystem.LogData>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Quaternion>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Vector3>
    // System.Collections.Generic.ArraySortHelper<int>
    // System.Collections.Generic.ArraySortHelper<object>
    // System.Collections.Generic.Comparer<HotUpdate.Console.ConsoleSystem.LogData>
    // System.Collections.Generic.Comparer<UnityEngine.Quaternion>
    // System.Collections.Generic.Comparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.Comparer<UnityEngine.Vector3>
    // System.Collections.Generic.Comparer<int>
    // System.Collections.Generic.Comparer<object>
    // System.Collections.Generic.Dictionary.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.Enumerator<object,int>
    // System.Collections.Generic.Dictionary.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.KeyCollection<int,object>
    // System.Collections.Generic.Dictionary.KeyCollection<object,int>
    // System.Collections.Generic.Dictionary.KeyCollection<object,object>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.ValueCollection<int,object>
    // System.Collections.Generic.Dictionary.ValueCollection<object,int>
    // System.Collections.Generic.Dictionary.ValueCollection<object,object>
    // System.Collections.Generic.Dictionary<int,object>
    // System.Collections.Generic.Dictionary<object,int>
    // System.Collections.Generic.Dictionary<object,object>
    // System.Collections.Generic.EqualityComparer<int>
    // System.Collections.Generic.EqualityComparer<object>
    // System.Collections.Generic.HashSet.Enumerator<object>
    // System.Collections.Generic.HashSet<object>
    // System.Collections.Generic.ICollection<HotUpdate.Console.ConsoleSystem.LogData>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.ICollection<UnityEngine.Quaternion>
    // System.Collections.Generic.ICollection<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.ICollection<UnityEngine.Vector3>
    // System.Collections.Generic.ICollection<int>
    // System.Collections.Generic.ICollection<object>
    // System.Collections.Generic.IComparer<HotUpdate.Console.ConsoleSystem.LogData>
    // System.Collections.Generic.IComparer<UnityEngine.Quaternion>
    // System.Collections.Generic.IComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.IComparer<UnityEngine.Vector3>
    // System.Collections.Generic.IComparer<int>
    // System.Collections.Generic.IComparer<object>
    // System.Collections.Generic.IEnumerable<HotUpdate.Console.ConsoleSystem.LogData>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.IEnumerable<UnityEngine.Quaternion>
    // System.Collections.Generic.IEnumerable<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.IEnumerable<UnityEngine.Vector3>
    // System.Collections.Generic.IEnumerable<int>
    // System.Collections.Generic.IEnumerable<object>
    // System.Collections.Generic.IEnumerator<HotUpdate.Console.ConsoleSystem.LogData>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.IEnumerator<UnityEngine.Quaternion>
    // System.Collections.Generic.IEnumerator<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.IEnumerator<UnityEngine.Vector3>
    // System.Collections.Generic.IEnumerator<int>
    // System.Collections.Generic.IEnumerator<object>
    // System.Collections.Generic.IEqualityComparer<int>
    // System.Collections.Generic.IEqualityComparer<object>
    // System.Collections.Generic.IList<HotUpdate.Console.ConsoleSystem.LogData>
    // System.Collections.Generic.IList<UnityEngine.Quaternion>
    // System.Collections.Generic.IList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.IList<UnityEngine.Vector3>
    // System.Collections.Generic.IList<int>
    // System.Collections.Generic.IList<object>
    // System.Collections.Generic.KeyValuePair<int,object>
    // System.Collections.Generic.KeyValuePair<object,int>
    // System.Collections.Generic.KeyValuePair<object,object>
    // System.Collections.Generic.LinkedList.Enumerator<object>
    // System.Collections.Generic.LinkedList<object>
    // System.Collections.Generic.LinkedListNode<object>
    // System.Collections.Generic.List.Enumerator<HotUpdate.Console.ConsoleSystem.LogData>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Quaternion>
    // System.Collections.Generic.List.Enumerator<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Vector3>
    // System.Collections.Generic.List.Enumerator<int>
    // System.Collections.Generic.List.Enumerator<object>
    // System.Collections.Generic.List<HotUpdate.Console.ConsoleSystem.LogData>
    // System.Collections.Generic.List<UnityEngine.Quaternion>
    // System.Collections.Generic.List<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.List<UnityEngine.Vector3>
    // System.Collections.Generic.List<int>
    // System.Collections.Generic.List<object>
    // System.Collections.Generic.ObjectComparer<HotUpdate.Console.ConsoleSystem.LogData>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Quaternion>
    // System.Collections.Generic.ObjectComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Vector3>
    // System.Collections.Generic.ObjectComparer<int>
    // System.Collections.Generic.ObjectComparer<object>
    // System.Collections.Generic.ObjectEqualityComparer<int>
    // System.Collections.Generic.ObjectEqualityComparer<object>
    // System.Collections.Generic.Queue.Enumerator<object>
    // System.Collections.Generic.Queue<object>
    // System.Collections.ObjectModel.ReadOnlyCollection<HotUpdate.Console.ConsoleSystem.LogData>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Quaternion>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector3>
    // System.Collections.ObjectModel.ReadOnlyCollection<int>
    // System.Collections.ObjectModel.ReadOnlyCollection<object>
    // System.Comparison<HotUpdate.Console.ConsoleSystem.LogData>
    // System.Comparison<UnityEngine.Quaternion>
    // System.Comparison<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Comparison<UnityEngine.Vector3>
    // System.Comparison<int>
    // System.Comparison<object>
    // System.Func<System.Threading.Tasks.VoidTaskResult>
    // System.Func<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
    // System.Func<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
    // System.Func<byte>
    // System.Func<float,float,byte>
    // System.Func<object,System.Threading.Tasks.VoidTaskResult>
    // System.Func<object,UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
    // System.Func<object,byte>
    // System.Func<object,object,byte>
    // System.Func<object,object,object>
    // System.Func<object,object>
    // System.Func<object>
    // System.IProgress<object>
    // System.Linq.Enumerable.Iterator<object>
    // System.Linq.Enumerable.WhereArrayIterator<object>
    // System.Linq.Enumerable.WhereEnumerableIterator<object>
    // System.Linq.Enumerable.WhereListIterator<object>
    // System.Nullable<byte>
    // System.Predicate<HotUpdate.Console.ConsoleSystem.LogData>
    // System.Predicate<UnityEngine.Quaternion>
    // System.Predicate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
    // System.Predicate<UnityEngine.Vector3>
    // System.Predicate<int>
    // System.Predicate<object>
    // System.ReadOnlySpan<ushort>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.Threading.Tasks.VoidTaskResult>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.Threading.Tasks.VoidTaskResult>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
    // System.Runtime.CompilerServices.TaskAwaiter<System.Threading.Tasks.VoidTaskResult>
    // System.Runtime.CompilerServices.TaskAwaiter<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
    // System.Runtime.CompilerServices.TaskAwaiter<object>
    // System.Span<ushort>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.Threading.Tasks.VoidTaskResult>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<object>
    // System.Threading.Tasks.Task<System.Threading.Tasks.VoidTaskResult>
    // System.Threading.Tasks.Task<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
    // System.Threading.Tasks.Task<object>
    // System.Threading.Tasks.TaskCompletionSource<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
    // System.Threading.Tasks.TaskCompletionSource<object>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.Threading.Tasks.VoidTaskResult>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<object>
    // System.Threading.Tasks.TaskFactory<System.Threading.Tasks.VoidTaskResult>
    // System.Threading.Tasks.TaskFactory<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
    // System.Threading.Tasks.TaskFactory<object>
    // UnityEngine.AddressableAssets.AddressablesImpl.<>c__DisplayClass79_0<object>
    // UnityEngine.AddressableAssets.AddressablesImpl.<>c__DisplayClass88_0<object>
    // UnityEngine.AddressableAssets.AddressablesImpl.<>c__DisplayClass91_0<object>
    // UnityEngine.Events.InvokableCall<byte>
    // UnityEngine.Events.InvokableCall<float>
    // UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene,int>
    // UnityEngine.Events.UnityAction<byte>
    // UnityEngine.Events.UnityAction<float>
    // UnityEngine.Events.UnityEvent<byte>
    // UnityEngine.Events.UnityEvent<float>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass60_0<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass60_0<object>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass61_0<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass61_0<object>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<object>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
    // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>
    // UnityEngine.ResourceManagement.ChainOperationTypelessDepedency<object>
    // UnityEngine.ResourceManagement.ResourceManager.<>c__DisplayClass95_0<object>
    // UnityEngine.ResourceManagement.ResourceManager.CompletedOperation<object>
    // UnityEngine.ResourceManagement.Util.GlobalLinkedListNodeCache<object>
    // UnityEngine.ResourceManagement.Util.LinkedListNodeCache<object>
    // }}

    public void RefMethods()
    {
        // object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string)
        // object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string,Newtonsoft.Json.JsonSerializerSettings)
        // System.Void QFramework.Architecture<object>.RegisterModel<object>(object)
        // System.Void QFramework.Architecture<object>.RegisterSystem<object>(object)
        // System.Void QFramework.Architecture<object>.RegisterUtility<object>(object)
        // System.Void QFramework.CanSendCommandExtension.SendCommand<object>(QFramework.ICanSendCommand)
        // System.Void QFramework.CanSendCommandExtension.SendCommand<object>(QFramework.ICanSendCommand,object)
        // System.Void QFramework.CanSendEventExtension.SendEvent<HotUpdate.Data.Commands.PlayThroughChangedEvent>(QFramework.ICanSendEvent,HotUpdate.Data.Commands.PlayThroughChangedEvent)
        // System.Void QFramework.CanSendEventExtension.SendEvent<object>(QFramework.ICanSendEvent,object)
        // System.Void QFramework.IArchitecture.SendCommand<object>(object)
        // System.Void QFramework.IArchitecture.SendEvent<HotUpdate.Data.Commands.PlayThroughChangedEvent>(HotUpdate.Data.Commands.PlayThroughChangedEvent)
        // System.Void QFramework.IArchitecture.SendEvent<object>(object)
        // System.Void QFramework.IOCContainer.Register<object>(object)
        // object QFramework.UnityEngineGameObjectExtension.Hide<object>(object)
        // object System.Activator.CreateInstance<object>()
        // object[] System.Array.Empty<object>()
        // int System.Linq.Enumerable.Count<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
        // object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
        // System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,HotUpdate.DialogueSystem.DialogueModel.<LoadAllDataAsync>d__8>(System.Runtime.CompilerServices.TaskAwaiter<object>&,HotUpdate.DialogueSystem.DialogueModel.<LoadAllDataAsync>d__8&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,HotUpdate.Enemy.EnemyDataModel.<LoadDataAsync>d__6>(System.Runtime.CompilerServices.TaskAwaiter<object>&,HotUpdate.Enemy.EnemyDataModel.<LoadDataAsync>d__6&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,HotUpdate.DialogueSystem.DialogueModel.<LoadAllDataAsync>d__8>(System.Runtime.CompilerServices.TaskAwaiter<object>&,HotUpdate.DialogueSystem.DialogueModel.<LoadAllDataAsync>d__8&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,HotUpdate.Enemy.EnemyDataModel.<LoadDataAsync>d__6>(System.Runtime.CompilerServices.TaskAwaiter<object>&,HotUpdate.Enemy.EnemyDataModel.<LoadDataAsync>d__6&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,HotUpdate.Manager.AddressablesManager.<LoadAssetTaskAsync>d__6<object>>(System.Runtime.CompilerServices.TaskAwaiter<object>&,HotUpdate.Manager.AddressablesManager.<LoadAssetTaskAsync>d__6<object>&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,HotUpdate.Manager.AddressablesManager.<LoadAssetsByLabelTaskAsync>d__7<object>>(System.Runtime.CompilerServices.TaskAwaiter<object>&,HotUpdate.Manager.AddressablesManager.<LoadAssetsByLabelTaskAsync>d__7<object>&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter,HotUpdate.Download.System.DownloadSystem.<DownloadAndSaveAsync>d__2>(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter&,HotUpdate.Download.System.DownloadSystem.<DownloadAndSaveAsync>d__2&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<HotUpdate.DialogueSystem.DialogueModel.<LoadAllDataAsync>d__8>(HotUpdate.DialogueSystem.DialogueModel.<LoadAllDataAsync>d__8&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<HotUpdate.Enemy.EnemyDataModel.<LoadDataAsync>d__6>(HotUpdate.Enemy.EnemyDataModel.<LoadDataAsync>d__6&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<HotUpdate.Download.System.DownloadSystem.<DownloadAndSaveAsync>d__2>(HotUpdate.Download.System.DownloadSystem.<DownloadAndSaveAsync>d__2&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<HotUpdate.Manager.AddressablesManager.<LoadAssetTaskAsync>d__6<object>>(HotUpdate.Manager.AddressablesManager.<LoadAssetTaskAsync>d__6<object>&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<HotUpdate.Manager.AddressablesManager.<LoadAssetsByLabelTaskAsync>d__7<object>>(HotUpdate.Manager.AddressablesManager.<LoadAssetsByLabelTaskAsync>d__7<object>&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,HotUpdate.SceneLoad.System.SceneLoadSystem.<LoadSceneAsync>d__9>(System.Runtime.CompilerServices.TaskAwaiter&,HotUpdate.SceneLoad.System.SceneLoadSystem.<LoadSceneAsync>d__9&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>,HotUpdate.SceneLoad.System.SceneLoadSystem.<LoadSceneAsync>d__9>(System.Runtime.CompilerServices.TaskAwaiter<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>&,HotUpdate.SceneLoad.System.SceneLoadSystem.<LoadSceneAsync>d__9&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,HotUpdate.MiniGame.IceBreaker.IceBreakerGoalCanvas.<StartDownloadAndSaveAsync>d__11>(System.Runtime.CompilerServices.TaskAwaiter<object>&,HotUpdate.MiniGame.IceBreaker.IceBreakerGoalCanvas.<StartDownloadAndSaveAsync>d__11&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<HotUpdate.MiniGame.IceBreaker.IceBreakerGoalCanvas.<StartDownloadAndSaveAsync>d__11>(HotUpdate.MiniGame.IceBreaker.IceBreakerGoalCanvas.<StartDownloadAndSaveAsync>d__11&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<HotUpdate.SceneLoad.System.SceneLoadSystem.<LoadSceneAsync>d__9>(HotUpdate.SceneLoad.System.SceneLoadSystem.<LoadSceneAsync>d__9&)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<object>(object)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<object>> UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync<object>(object,System.Action<object>)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetAsync<object>(object)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetWithChain<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,object)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<object>> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetsAsync<object>(System.Collections.Generic.IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation>,System.Action<object>,bool)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<object>> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetsAsync<object>(object,System.Action<object>,bool)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<object>> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetsWithChain<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,System.Collections.Generic.IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation>,System.Action<object>,bool)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<object>> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetsWithChain<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,object,System.Action<object>,bool)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.TrackHandle<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>)
        // object UnityEngine.Component.GetComponent<object>()
        // object UnityEngine.Component.GetComponentInChildren<object>()
        // object UnityEngine.Component.GetComponentInParent<object>()
        // object[] UnityEngine.Component.GetComponentsInChildren<object>()
        // object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
        // bool UnityEngine.Component.TryGetComponent<object>(object&)
        // object UnityEngine.GameObject.AddComponent<object>()
        // object UnityEngine.GameObject.GetComponent<object>()
        // object UnityEngine.GameObject.GetComponentInChildren<object>()
        // object UnityEngine.GameObject.GetComponentInChildren<object>(bool)
        // object[] UnityEngine.GameObject.GetComponentsInChildren<object>()
        // object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
        // bool UnityEngine.GameObject.TryGetComponent<object>(object&)
        // object UnityEngine.JsonUtility.FromJson<object>(string)
        // object UnityEngine.Object.Instantiate<object>(object)
        // object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
        // object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
        // object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Vector3,UnityEngine.Quaternion)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Convert<object>()
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateChainOperation<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,System.Func<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateChainOperation<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,System.Func<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>,bool)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateCompletedOperation<object>(object,string)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateCompletedOperationInternal<object>(object,bool,System.Exception,bool)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateCompletedOperationWithException<object>(object,System.Exception)
        // object UnityEngine.ResourceManagement.ResourceManager.CreateOperation<object>(System.Type,int,UnityEngine.ResourceManagement.Util.IOperationCacheKey,System.Action<UnityEngine.ResourceManagement.AsyncOperations.IAsyncOperation>)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.ProvideResource<object>(UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<object>> UnityEngine.ResourceManagement.ResourceManager.ProvideResources<object>(System.Collections.Generic.IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation>,bool,System.Action<object>)
        // UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.StartOperation<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<object>,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle)
    }
}