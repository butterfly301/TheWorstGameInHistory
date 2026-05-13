using System;
using System.Collections.Generic;

namespace HotUpdate.Data.Model
{
    [Serializable]
    public class GameData
    {
        public int playThrough;
        public List<SoftwareName> software = new();
        public List<SkillData> skillData = new();
    }

    public enum SoftwareName
    {
        IceBreaker,
        TLH_1793
    }
}