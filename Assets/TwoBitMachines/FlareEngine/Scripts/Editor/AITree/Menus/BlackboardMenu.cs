using System.Collections.Generic;
using System.Text;
using TwoBitMachines.Editors;
using TwoBitMachines.FlareEngine.AI;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    public class BlackboardMenu
    {
        public static AIBase ai;
        public static StringBuilder itemNameBuilder = new();

        public static void Open(AIBase newAI, List<string> dataList, int index)
        {
            ai = newAI;
            var menu = new GenericMenu();
            var prefix = index == 2 ? "Target/" : index == 3 ? "Territory/" : "Variable/";

            for (var i = 0; i < dataList.Count; i++)
            {
                var itemName = dataList[i];
                if (itemName.StartsWith(prefix))
                {
                    itemNameBuilder.Clear();
                    itemNameBuilder.Append(itemName, prefix.Length, itemName.Length - prefix.Length);
                    menu.AddItem(new GUIContent(itemNameBuilder.ToString()), false, OnAddBlackboardTreeItem, itemName);
                }
            }

            menu.ShowAsContext();
        }

        private static void OnAddBlackboardTreeItem(object obj)
        {
            if (ai == null)
                return;
            var path = (string)obj;
            var nameType = path.Split('/');
            CreateBlackboard(ai.data, nameType[0], nameType[nameType.Length - 1]);
        }

        public static Blackboard CreateBlackboard(List<Blackboard> children, string typeName, string nameType)
        {
            if (ai == null)
                return null;

            Blackboard node = null;
            var actionType = "TwoBitMachines.FlareEngine.AI.BlackboardData." + nameType;
            if (EditorTools.RetrieveType(actionType, out var type))
            {
                node = ai.gameObject.gameObject.AddComponent(type) as Blackboard;
                children.Add(node);
                node.hideFlags = HideFlags.HideInInspector;
                if (typeName == "Target")
                    node.blackboardType = BlackboardType.Target;
                else if (typeName == "Territory")
                    node.blackboardType = BlackboardType.Territory;
                else
                    node.blackboardType = BlackboardType.Variable;
            }

            return node;
        }
    }
}