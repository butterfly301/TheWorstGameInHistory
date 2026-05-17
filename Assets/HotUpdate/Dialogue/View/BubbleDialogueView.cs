using UnityEngine;

namespace HotUpdate.Dialogue.View
{
    /// <summary>
    ///     泡泡对话框视图
    ///     在角色头顶显示泡泡对话框
    /// </summary>
    public class BubbleDialogueView : DialogueViewBase
    {
        [Header("泡泡对话框特有配置")] [SerializeField] private Transform targetCharacter;

[SerializeField] private Vector3 offset = new(0, 2, 0);
        [SerializeField] private bool followCharacter = true;

protected void Update()
        {
            // 如果需要跟随角色移动
            if (followCharacter && targetCharacter != null) transform.position = targetCharacter.position + offset;
        }

/// <summary>
        ///     设置目标角色
        /// </summary>
        public void SetTargetCharacter(Transform character)
        {
            targetCharacter = character;
        }
    }
}