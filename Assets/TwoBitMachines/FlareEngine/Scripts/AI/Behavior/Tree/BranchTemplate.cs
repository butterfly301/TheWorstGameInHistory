using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    //////[CreateAssetMenu ( )]
    public class BranchTemplate : ScriptableObject
    {
        public List<TemplateKey> template = new();
    }

    [Serializable]
    public class TemplateKey
    {
        public TemplateNode node = new();
        public string templateKey = "";
    }

    [Serializable]
    public class TemplateNode
    {
        public string nameType;
        public Vector2 position;
        [SerializeReference] public List<TemplateNode> children = new();
    }
}