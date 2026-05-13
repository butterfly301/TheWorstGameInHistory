using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HybridCLR;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public static class HotUpdateReflectionUtility
{
    private static Assembly hotUpdateAss;
    private static readonly Dictionary<string, MethodInfo> methodCache = new();
    private static readonly Dictionary<string, PropertyInfo> propertyCache = new();
    private static readonly Dictionary<string, Delegate> delegateCache = new();

    private static bool isInitialized;
    private static bool aotMetadataLoaded;

    // Addressable标签配置
    private const string HotUpdateDllLabel = "HotUpdateDll";
    private const string AotDllLabel = "AotDll";

    /// <summary>
    ///     加载AOT元数据（必须在加载热更新程序集前调用）
    /// </summary>
    public static async Task LoadAOTMetadataAsync()
    {
        if (aotMetadataLoaded)
        {
            Debug.Log("AOT元数据已加载，跳过重复加载");
            return;
        }

        try
        {
            Debug.Log("开始从Addressable加载AOT元数据...");

            // 从Addressable加载AOT元数据DLL
            var aotDlls = await LoadAOTDllsFromAddressable();

            var successCount = 0;
            var failCount = 0;

            foreach (var aotDll in aotDlls)
            {
                // 为AOT程序集补充元数据
                var error = RuntimeApi.LoadMetadataForAOTAssembly(
                    aotDll.Value,
                    HomologousImageMode.SuperSet
                );

                if (error == LoadImageErrorCode.OK)
                {
                    successCount++;
                    Debug.Log($"✅ AOT元数据加载成功: {aotDll.Key}");
                }
                else
                {
                    failCount++;
                    Debug.LogError($"❌ AOT元数据加载失败: {aotDll.Key}, 错误: {error}");
                }
            }

            aotMetadataLoaded = true;
            Debug.Log($"AOT元数据加载完成 - 成功: {successCount}, 失败: {failCount}");

            // 验证AOT元数据
            ValidateAOTMetadata();
        }
        catch (Exception ex)
        {
            Debug.LogError($"加载AOT元数据异常: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private static async Task<Dictionary<string, byte[]>> LoadAOTDllsFromAddressable()
    {
        var aotDlls = new Dictionary<string, byte[]>();

        try
        {
            Debug.Log($"开始从Addressable加载标签为 '{AotDllLabel}' 的AOT元数据DLL");

            // 从Addressable加载标记为AotDllLabel的资源
            var locations = await Addressables.LoadResourceLocationsAsync(AotDllLabel).Task;
            Debug.Log($"找到 {locations.Count} 个AOT元数据DLL资源位置");

            var loadTasks = new List<Task>();

            foreach (var location in locations)
            {
                var task = LoadSingleAOTDll(location, aotDlls);
                loadTasks.Add(task);
            }

            // 等待所有DLL加载完成
            await Task.WhenAll(loadTasks);
        }
        catch (Exception ex)
        {
            Debug.LogError($"加载AOT元数据位置信息失败: {ex.Message}");
        }

        Debug.Log($"AOT元数据DLL加载完成，共加载 {aotDlls.Count} 个DLL");
        return aotDlls;
    }

    private static async Task LoadSingleAOTDll(IResourceLocation location, Dictionary<string, byte[]> aotDlls)
    {
        try
        {
            var handle = Addressables.LoadAssetAsync<TextAsset>(location);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            {
                var dllAsset = handle.Result;
                var dllName = Path.GetFileNameWithoutExtension(location.PrimaryKey);

                if (!aotDlls.ContainsKey(dllName))
                {
                    aotDlls[dllName] = dllAsset.bytes;
                    Debug.Log($"✅ 加载AOT DLL: {dllName}, 大小: {dllAsset.bytes.Length} 字节");
                }
                else
                {
                    Debug.LogWarning($"跳过重复的AOT DLL: {dllName}");
                }
            }
            else
            {
                Debug.LogError($"❌ 加载AOT DLL失败: {location.PrimaryKey}, 状态: {handle.Status}");
            }

            Addressables.Release(handle);
        }
        catch (Exception ex)
        {
            Debug.LogError($"加载AOT DLL失败 {location.PrimaryKey}: {ex.Message}");
        }
    }

    /// <summary>
    ///     手动初始化热更程序集（必须调用！）
    /// </summary>
    public static async Task InitializeHotUpdateAssemblyAsync()
    {
        if (isInitialized)
        {
            Debug.Log("热更程序集已初始化，跳过重复初始化");
            return;
        }

        try
        {
            Debug.Log("开始从Addressable加载热更程序集...");

            if (!aotMetadataLoaded) Debug.LogWarning("AOT元数据未加载，热更新可能无法正常工作");

#if !UNITY_EDITOR
            // 打包版本：从Addressable加载热更新DLL
            await LoadHotUpdateDllFromAddressableAsync();
#else
            // 编辑器版本：直接查找
            hotUpdateAss = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "HotUpdate");

            if (hotUpdateAss != null)
            {
                Debug.Log($"✅ 热更程序集加载成功: {hotUpdateAss.FullName}");
                Debug.Log($"包含类型: {string.Join(", ", hotUpdateAss.GetTypes().Select(t => t.Name))}");
                isInitialized = true;
            }
            else
            {
                Debug.LogError("❌ 热更程序集加载失败 - 在编辑器中未找到HotUpdate程序集");
            }
#endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"热更程序集初始化异常: {ex.Message}\n{ex.StackTrace}");
        }
    }

#if !UNITY_EDITOR
    private static async Task LoadHotUpdateDllFromAddressableAsync()
    {
        try
        {
            Debug.Log($"开始从Addressable加载标签为 '{HotUpdateDllLabel}' 的热更新DLL");
            
            // 从Addressable加载热更新DLL
            var locations = await Addressables.LoadResourceLocationsAsync(HotUpdateDllLabel).Task;
            Debug.Log($"找到 {locations.Count} 个热更新DLL资源位置");
            
            if (locations.Count == 0)
            {
                Debug.LogError($"❌ 未找到标签为 '{HotUpdateDllLabel}' 的热更新DLL资源");
                return;
            }
            
            // 加载第一个热更新DLL（通常只有一个）
            var location = locations[0];
            var handle = Addressables.LoadAssetAsync<TextAsset>(location);
            await handle.Task;
            
            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            {
                TextAsset dllAsset = handle.Result;
                Debug.Log($"✅ 热更新DLL加载成功: {location.PrimaryKey}, 大小: {dllAsset.bytes.Length} 字节");
                
                hotUpdateAss = Assembly.Load(dllAsset.bytes);
                
                if (hotUpdateAss != null)
                {
                    Debug.Log($"✅ 热更程序集加载成功: {hotUpdateAss.FullName}");
                    Debug.Log($"包含类型: {string.Join(", ", hotUpdateAss.GetTypes().Select(t => t.Name))}");
                    isInitialized = true;
                }
                else
                {
                    Debug.LogError("❌ Assembly.Load加载热更新DLL失败");
                }
            }
            else
            {
                Debug.LogError($"❌ 从Addressable加载热更新DLL失败: {location.PrimaryKey}");
            }
            
            Addressables.Release(handle);
        }
        catch (Exception ex)
        {
            Debug.LogError($"从Addressable加载热更新DLL失败: {ex.Message}");
        }
    }
#endif

    /// <summary>
    ///     完整的HybridCLR初始化流程（推荐使用）
    /// </summary>
    public static async Task FullInitializeAsync()
    {
        if (IsFullyInitialized())
        {
            Debug.Log("HybridCLR已完全初始化，跳过重复初始化");
            return;
        }

        Debug.Log("开始完整HybridCLR初始化流程...");

        // 1. 先加载AOT元数据
        await LoadAOTMetadataAsync();

        // 2. 再加载热更新程序集
        await InitializeHotUpdateAssemblyAsync();

        if (IsFullyInitialized())
            Debug.Log("✅ HybridCLR完整初始化成功");
        else
            Debug.LogError("❌ HybridCLR初始化失败");
    }

    /// <summary>
    ///     验证AOT元数据是否完整
    /// </summary>
    public static void ValidateAOTMetadata()
    {
        try
        {
            // 测试一些常见的泛型类型
            var list = new List<int>();
            var dict = new Dictionary<string, object>();
            var taskType = typeof(Task<int>);

            Debug.Log("✅ AOT元数据验证通过 - 基础泛型类型可用");
        }
        catch (TypeInitializationException ex)
        {
            Debug.LogError($"❌ AOT元数据缺失: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ AOT元数据验证异常: {ex.Message}");
        }
    }

    /// <summary>
    ///     打开对应的热更新入口
    /// </summary>
    public static void Open(string index)
    {
        if (!IsFullyInitialized())
        {
            Debug.LogError($"❌ HybridCLR未完全初始化，无法执行Open({index})");
            return;
        }

        if (hotUpdateAss == null)
        {
            Debug.LogError("❌ 热更程序集未加载，无法执行Open方法");
            return;
        }

        var typeName = "Open" + index;
        var openType = hotUpdateAss.GetType(typeName);

        if (openType != null)
        {
            var go = new GameObject(typeName);
            go.AddComponent(openType);
            Debug.Log($"✅ 创建热更新组件: {typeName}");
        }
        else
        {
            Debug.LogError($"❌ 未找到热更新类型: {typeName}");
            Debug.Log($"可用的类型: {string.Join(", ", hotUpdateAss.GetTypes().Select(t => t.Name))}");
        }
    }

    // 确保在调用任何方法前都检查初始化
    private static void CheckInitialization()
    {
        if (!isInitialized) Debug.LogWarning("热更程序集未初始化，请先调用FullInitializeAsync()");
    }

    /// <summary>
    ///     调用热更新组件的方法
    /// </summary>
    public static object CallMethod(GameObject targetObject, string componentName, string methodName,
        params object[] parameters)
    {
        try
        {
            if (!IsFullyInitialized())
            {
                Debug.LogError("❌ HybridCLR未完全初始化，无法调用方法");
                return null;
            }

            if (hotUpdateAss == null)
            {
                Debug.LogError("❌ 热更程序集未加载成功，无法调用方法");
                return null;
            }

            if (targetObject == null)
            {
                Debug.LogError("\u274c 目标游戏对象为null");
                return null;
            }

            // 先获取类型，再通过类型获取组件
            var componentType = hotUpdateAss.GetType(componentName);
            if (componentType == null)
            {
                Debug.LogError($"❌ 未找到热更组件类型: {componentName}");
                Debug.Log($"可用的类型: {string.Join(", ", hotUpdateAss.GetTypes().Select(t => t.Name))}");
                return null;
            }

            var component = targetObject.GetComponent(componentType);
            if (component == null)
            {
                Debug.LogError($"❌ 游戏对象上未找到组件: {componentName}");
                return null;
            }

            var cacheKey = $"{componentName}.{methodName}";

            // 从缓存获取或查找方法
            if (!methodCache.TryGetValue(cacheKey, out var method))
            {
                method = componentType.GetMethod(methodName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                if (method != null) methodCache[cacheKey] = method;
            }

            if (method == null)
            {
                Debug.LogWarning($"⚠️ 方法 {methodName} 在组件 {componentName} 中不存在");
                return null;
            }

            var result = method.Invoke(component, parameters);
            Debug.Log($"✅ 成功调用 {componentName}.{methodName}()");
            return result;
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ 调用方法 {methodName} 时出错: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    ///     获取热更新组件的属性值
    /// </summary>
    public static T GetProperty<T>(GameObject targetObject, string componentName, string propertyName)
    {
        try
        {
            if (!IsFullyInitialized())
            {
                Debug.LogError("❌ HybridCLR未完全初始化");
                return default;
            }

            if (targetObject == null)
            {
                Debug.LogError("\u274c 目标游戏对象为null");
                return default;
            }

            var componentType = hotUpdateAss.GetType(componentName);
            if (componentType == null)
            {
                Debug.LogError($"❌ 未找到热更组件类型: {componentName}");
                return default;
            }

            var component = targetObject.GetComponent(componentType);
            if (component == null)
            {
                Debug.LogError($"❌ 游戏对象上未找到组件: {componentName}");
                return default;
            }

            var cacheKey = $"{componentName}.{propertyName}";

            // 从缓存获取或查找属性
            if (!propertyCache.TryGetValue(cacheKey, out var property))
            {
                property = componentType.GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                if (property != null) propertyCache[cacheKey] = property;
            }

            if (property == null)
            {
                Debug.LogWarning($"⚠️ 属性 {propertyName} 在组件 {componentName} 中不存在");
                return default;
            }

            return (T)property.GetValue(component);
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ 获取属性 {propertyName} 时出错: {ex.Message}");
            return default;
        }
    }

    /// <summary>
    ///     设置热更新组件的属性值
    /// </summary>
    public static void SetProperty<T>(GameObject targetObject, string componentName, string propertyName, T value)
    {
        try
        {
            if (targetObject == null)
            {
                Debug.LogError("目标游戏对象为 null");
                return;
            }

            var componentType = hotUpdateAss.GetType(componentName);
            if (componentType == null)
            {
                Debug.LogError($"未找到热更组件类型: {componentName}");
                return;
            }

            var component = targetObject.GetComponent(componentType);
            if (component == null)
            {
                Debug.LogError($"游戏对象上未找到组件: {componentName}");
                return;
            }

            var cacheKey = $"{componentName}.{propertyName}";

            // 从缓存获取或查找属性
            if (!propertyCache.TryGetValue(cacheKey, out var property))
            {
                property = componentType.GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                if (property != null) propertyCache[cacheKey] = property;
            }

            if (property == null)
            {
                Debug.LogWarning($"属性 {propertyName} 在组件 {componentName} 中不存在");
                return;
            }

            property.SetValue(component, value);
            Debug.Log($"成功设置 {componentName}.{propertyName} = {value}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"设置属性 {propertyName} 时出错: {ex.Message}");
        }
    }

    /// <summary>
    ///     创建并缓存委托以提高性能（适用于无参数方法）
    /// </summary>
    public static void CreateAndInvokeDelegate(GameObject targetObject, string componentName, string methodName)
    {
        try
        {
            if (targetObject == null)
            {
                Debug.LogError("目标游戏对象为 null");
                return;
            }

            var componentType = hotUpdateAss.GetType(componentName);
            if (componentType == null)
            {
                Debug.LogError($"未找到热更组件类型: {componentName}");
                return;
            }

            var component = targetObject.GetComponent(componentType);
            if (component == null)
            {
                Debug.LogError($"游戏对象上未找到组件: {componentName}");
                return;
            }

            var delegateKey = $"{componentName}.{methodName}";
            if (!delegateCache.TryGetValue(delegateKey, out var cachedDelegate))
            {
                var method = componentType.GetMethod(methodName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                if (method == null)
                {
                    Debug.LogWarning($"方法 {methodName} 在组件 {componentName} 中不存在");
                    return;
                }

                // 创建Action委托（适用于无参数方法）
                cachedDelegate = Delegate.CreateDelegate(typeof(Action), component, method);
                delegateCache[delegateKey] = cachedDelegate;
            }

            ((Action)cachedDelegate).Invoke();
            Debug.Log($"通过委托调用 {componentName}.{methodName}()");
        }
        catch (Exception ex)
        {
            Debug.LogError($"委托调用 {methodName} 时出错: {ex.Message}");
        }
    }

    /// <summary>
    ///     检查热更组件是否存在
    /// </summary>
    public static bool HasComponent(GameObject targetObject, string componentName)
    {
        try
        {
            if (targetObject == null) return false;

            var componentType = hotUpdateAss.GetType(componentName);
            if (componentType == null) return false;

            return targetObject.GetComponent(componentType) != null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     添加热更组件到游戏对象
    /// </summary>
    public static Component AddComponent(GameObject targetObject, string componentName)
    {
        try
        {
            if (targetObject == null)
            {
                Debug.LogError("目标游戏对象为 null");
                return null;
            }

            var componentType = hotUpdateAss.GetType(componentName);
            if (componentType == null)
            {
                Debug.LogError($"未找到热更组件类型: {componentName}");
                return null;
            }

            return targetObject.AddComponent(componentType);
        }
        catch (Exception ex)
        {
            Debug.LogError($"添加热更组件失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    ///     为热更新组件的事件添加回调
    /// </summary>
    public static void AddEventHandler(GameObject targetObject, string componentName, string eventName,
        Delegate handler)
    {
        try
        {
            CheckInitialization();

            if (targetObject == null)
            {
                Debug.LogError("目标游戏对象为 null");
                return;
            }

            var componentType = hotUpdateAss.GetType(componentName);
            if (componentType == null)
            {
                Debug.LogError($"未找到热更组件类型: {componentName}");
                return;
            }

            var component = targetObject.GetComponent(componentType);
            if (component == null)
            {
                Debug.LogError($"游戏对象上未找到组件: {componentName}");
                return;
            }

            // 获取事件字段
            var eventField = componentType.GetField(eventName,
                BindingFlags.Public | BindingFlags.Instance);

            if (eventField == null)
            {
                Debug.LogError($"未找到事件: {eventName}");
                return;
            }

            // 获取当前的事件委托
            var currentDelegate = eventField.GetValue(component) as Delegate;

            // 合并委托
            var newDelegate = Delegate.Combine(currentDelegate, handler);

            // 设置回事件字段
            eventField.SetValue(component, newDelegate);

            Debug.Log($"成功为 {componentName}.{eventName} 添加事件处理程序");
        }
        catch (Exception ex)
        {
            Debug.LogError($"添加事件处理程序失败: {ex.Message}");
        }
    }

    /// <summary>
    ///     移除热更新组件的事件回调
    /// </summary>
    public static void DeleteEventHandler(GameObject targetObject, string componentName, string eventName,
        Delegate handler)
    {
        try
        {
            CheckInitialization();

            if (targetObject == null)
            {
                Debug.LogError("目标游戏对象为 null");
                return;
            }

            var componentType = hotUpdateAss.GetType(componentName);
            if (componentType == null)
            {
                Debug.LogError($"未找到热更组件类型: {componentName}");
                return;
            }

            var component = targetObject.GetComponent(componentType);
            if (component == null)
            {
                Debug.LogError($"游戏对象上未找到组件: {componentName}");
                return;
            }

            // 获取事件字段
            var eventField = componentType.GetField(eventName,
                BindingFlags.Public | BindingFlags.Instance);

            if (eventField == null)
            {
                Debug.LogError($"未找到事件: {eventName}");
                return;
            }

            // 获取当前的事件委托
            var currentDelegate = eventField.GetValue(component) as Delegate;

            if (currentDelegate == null)
            {
                Debug.LogWarning($"事件 {eventName} 当前没有任何委托");
                return;
            }

            // 移除指定的委托
            var newDelegate = Delegate.Remove(currentDelegate, handler);

            // 设置回事件字段
            eventField.SetValue(component, newDelegate);

            Debug.Log($"成功从 {componentName}.{eventName} 移除事件处理程序");
        }
        catch (Exception ex)
        {
            Debug.LogError($"移除事件处理程序失败: {ex.Message}");
        }
    }

    /// <summary>
    ///     检查HybridCLR是否完全初始化
    /// </summary>
    public static bool IsFullyInitialized()
    {
        return aotMetadataLoaded && isInitialized;
    }

    /// <summary>
    ///     获取加载状态信息
    /// </summary>
    public static string GetStatusInfo()
    {
        return
            $"AOT元数据加载: {aotMetadataLoaded}, 热更新程序集加载: {isInitialized}, 程序集: {(hotUpdateAss != null ? hotUpdateAss.GetName().Name : "null")}";
    }

    /// <summary>
    ///     清空缓存（在热更新重新加载时调用）
    /// </summary>
    public static void ClearCache()
    {
        methodCache.Clear();
        propertyCache.Clear();
        delegateCache.Clear();
        Debug.Log("✅ 热更新反射缓存已清空");
    }
}