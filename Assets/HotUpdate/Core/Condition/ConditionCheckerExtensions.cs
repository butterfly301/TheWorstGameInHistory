using System.Collections.Generic;
using System.Linq;

namespace HotUpdate.Core.Condition
{
    /// <summary>
    /// ConditionChecker 的扩展方法
    /// </summary>
    public static class ConditionCheckerExtensions
    {
        /// <summary>
        /// 获取已注册的条件类型数量
        /// </summary>
        public static int GetRegisteredConditionTypesCount()
        {
            return ConditionChecker.GetRegisteredConditionTypesCount();
        }

/// <summary>
        /// 检查所有条件是否都满足（AND关系）
        /// </summary>
        public static bool CheckAll(this IEnumerable<ConditionData> conditions)
        {
            if (conditions == null) return true;

foreach (var condition in conditions)
            {
                if (!ConditionChecker.CheckCondition(condition))
                {
                    return false;
                }
            }

return true;
        }

/// <summary>
        /// 检查是否满足任意一个条件（OR关系）
        /// </summary>
        public static bool CheckAny(this IEnumerable<ConditionData> conditions)
        {
            if (conditions == null) return false;

return conditions.Any(ConditionChecker.CheckCondition);
        }
    }
}
