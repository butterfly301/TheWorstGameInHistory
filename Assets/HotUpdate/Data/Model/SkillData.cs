using System;
using System.Collections.Generic;
using HotUpdate.Enums;
using UnityEngine.Serialization;

namespace  HotUpdate.Data.Model
{
    [Serializable]
    public class SkillData
    {
        public CharacterName name;
        public List<SkillType> skills;
    }
}