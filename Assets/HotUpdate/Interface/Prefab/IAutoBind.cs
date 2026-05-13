namespace HotUpdate.Interface
{
    /// <summary>
    /// 节点接口，用于标识可以自动绑定子物体的组件
    /// 使用方法：
    /// 1. 让你的脚本继承此接口
    /// 2. 在脚本中声明与子物体名称相同的字段
    /// 3. 当子物体重命名时，会自动绑定到对应的字段
    ///
    /// 示例：
    /// public class PlayerController : MonoBehaviour, IAutoBind
    /// {
    ///     [SerializeField] private Transform head;  // 当子物体命名为"head"时自动绑定
    ///     [SerializeField] private Rigidbody body;  // 当子物体命名为"body"时自动绑定
    /// }
    /// </summary>
    public interface IAutoBind
    {
    }
}
