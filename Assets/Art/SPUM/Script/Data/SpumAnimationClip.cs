using System;

[Serializable]
public class SpumAnimationClip : ICloneable
{
    public int index;
    public string Name;
    public string StateType;
    public string ClipPath;
    public bool HasData;
    public string UnitType;
    public string SubCategory;

    public object Clone()
    {
        return new SpumAnimationClip
        {
            index = index,
            Name = Name,
            StateType = StateType,
            ClipPath = ClipPath,
            HasData = HasData,
            UnitType = UnitType,
            SubCategory = SubCategory
        };
    }
}