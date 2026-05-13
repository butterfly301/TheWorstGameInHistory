using System;
using QFramework;
using UnityEngine;

namespace HotUpdate.Core.Condition
{
    /// <summary>
    /// 条件系统
    /// 提供条件判断的全局访问点和初始化
    /// </summary>
    public class ConditionSystem : AbstractSystem
    {
        protected override void OnInit()
        {
            Debug.Log("[ConditionSystem] 条件系统初始化完成");
            Debug.Log($"[ConditionSystem] 当前支持的条件类型: {ConditionChecker.GetRegisteredConditionTypesCount()}");
        }

        /// <summary>
        /// 快捷方法：检查单个条件
        /// </summary>
        public bool Check(ConditionData condition)
        {
            return ConditionChecker.CheckCondition(condition);
        }

        /// <summary>
        /// 快捷方法：检查条件组
        /// </summary>
        public bool CheckGroup(ConditionGroup group)
        {
            return ConditionChecker.CheckConditionGroup(group);
        }

        /// <summary>
        /// 注册自定义条件类型
        /// </summary>
        public void RegisterConditionType(string typeName, Func<ConditionData, bool> checker)
        {
            ConditionChecker.RegisterConditionType(typeName, checker);
        }

        /// <summary>
        /// 取消注册条件类型
        /// </summary>
        public void UnregisterConditionType(string typeName)
        {
            ConditionChecker.UnregisterConditionType(typeName);
        }
    }
}
