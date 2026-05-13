using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SpumPackage : ICloneable
{
    public string Name;
    public string Path;
    public string Version;
    public string CreationDate;
    public List<SpumAnimationClip> SpumAnimationData = new();
    public List<SpumTextureData> SpumTextureData = new();

    public object Clone()
    {
        return new SpumPackage
        {
            Name = Name,
            Path = Path,
            Version = Version,
            CreationDate = CreationDate,
            SpumAnimationData = SpumAnimationData?.Select(a => (SpumAnimationClip)a.Clone()).ToList(),
            SpumTextureData = SpumTextureData?.Select(t => (SpumTextureData)t.Clone()).ToList()
        };
    }
}