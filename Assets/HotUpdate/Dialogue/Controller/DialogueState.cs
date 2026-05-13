namespace HotUpdate.Dialogue.Controller
{
    /// <summary>
    ///     对话状态枚举
    /// </summary>
    public enum DialogueState
    {
        Idle, // 空闲状态，没有对话进行
        Loading, // 正在加载对话数据
        Typing, // 正在打字显示文本
        Waiting, // 等待玩家按键继续
        ShowingChoices, // 显示选项中，等待玩家选择
        Paused, // 对话暂停（如打开菜单时）
        Ended // 对话结束
    }
}