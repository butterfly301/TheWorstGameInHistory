using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PlayerState
{
    IDLE,
    MOVE,
    ATTACK,
    DAMAGED,
    DEBUFF,
    DEATH,
    OTHER
}

public class SPUM_Prefabs : MonoBehaviour
{
    public float _version;
    public bool EditChk;
    public string _code;
    public Animator _anim;

    public string UnitType;
    public List<SpumPackage> spumPackages = new();
    public List<PreviewMatchingElement> ImageElement = new();
    public List<SPUM_AnimationData> SpumAnimationData = new();
    public List<AnimationClip> IDLE_List = new();
    public List<AnimationClip> MOVE_List = new();
    public List<AnimationClip> ATTACK_List = new();
    public List<AnimationClip> DAMAGED_List = new();
    public List<AnimationClip> DEBUFF_List = new();
    public List<AnimationClip> DEATH_List = new();
    public List<AnimationClip> OTHER_List = new();
    private AnimatorOverrideController OverrideController;
    public Dictionary<string, List<AnimationClip>> StateAnimationPairs = new();

    public void OverrideControllerInit()
    {
        var animator = _anim;
        OverrideController = new AnimatorOverrideController();
        OverrideController.runtimeAnimatorController = animator.runtimeAnimatorController;

        // 모든 애니메이션 클립을 가져옵니다
        var clips = animator.runtimeAnimatorController.animationClips;

        foreach (var clip in clips)
            // 복제된 클립으로 오버라이드합니다
            OverrideController[clip.name] = clip;

        animator.runtimeAnimatorController = OverrideController;
        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            var stateText = state.ToString();
            StateAnimationPairs[stateText] = new List<AnimationClip>();
            switch (stateText)
            {
                case "IDLE":
                    StateAnimationPairs[stateText] = IDLE_List;
                    break;
                case "MOVE":
                    StateAnimationPairs[stateText] = MOVE_List;
                    break;
                case "ATTACK":
                    StateAnimationPairs[stateText] = ATTACK_List;
                    break;
                case "DAMAGED":
                    StateAnimationPairs[stateText] = DAMAGED_List;
                    break;
                case "DEBUFF":
                    StateAnimationPairs[stateText] = DEBUFF_List;
                    break;
                case "DEATH":
                    StateAnimationPairs[stateText] = DEATH_List;
                    break;
                case "OTHER":
                    StateAnimationPairs[stateText] = OTHER_List;
                    break;
            }
        }
    }

    public bool allListsHaveItemsExist()
    {
        var allLists = new List<List<AnimationClip>>
        {
            IDLE_List, MOVE_List, ATTACK_List, DAMAGED_List, DEBUFF_List, DEATH_List, OTHER_List
        };

        return allLists.All(list => list.Count > 0);
    }

    [ContextMenu("PopulateAnimationLists")]
    public void PopulateAnimationLists()
    {
        IDLE_List = new List<AnimationClip>();
        MOVE_List = new List<AnimationClip>();
        ATTACK_List = new List<AnimationClip>();
        DAMAGED_List = new List<AnimationClip>();
        DEBUFF_List = new List<AnimationClip>();
        DEATH_List = new List<AnimationClip>();
        OTHER_List = new List<AnimationClip>();

        var groupedClips = spumPackages
            .SelectMany(package => package.SpumAnimationData)
            .Where(spumClip => spumClip.HasData &&
                               spumClip.UnitType.Equals(UnitType) &&
                               spumClip.index > -1)
            .GroupBy(spumClip => spumClip.StateType)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(clip => clip.index).ToList()
            );
        // foreach (var item in groupedClips)
        // {
        //     foreach (var clip in item.Value)
        //     {
        //         Debug.Log(clip.ClipPath);
        //     }
        // }
        foreach (var kvp in groupedClips)
        {
            var stateType = kvp.Key;
            var orderedClips = kvp.Value;
            switch (stateType)
            {
                case "IDLE":
                    IDLE_List.AddRange(orderedClips.Select(clip => LoadAnimationClip(clip.ClipPath)));
                    //StateAnimationPairs[stateType] = IDLE_List;
                    break;
                case "MOVE":
                    MOVE_List.AddRange(orderedClips.Select(clip => LoadAnimationClip(clip.ClipPath)));
                    //StateAnimationPairs[stateType] = MOVE_List;
                    break;
                case "ATTACK":
                    ATTACK_List.AddRange(orderedClips.Select(clip => LoadAnimationClip(clip.ClipPath)));
                    //StateAnimationPairs[stateType] = ATTACK_List;
                    break;
                case "DAMAGED":
                    DAMAGED_List.AddRange(orderedClips.Select(clip => LoadAnimationClip(clip.ClipPath)));
                    //StateAnimationPairs[stateType] = DAMAGED_List;
                    break;
                case "DEBUFF":
                    DEBUFF_List.AddRange(orderedClips.Select(clip => LoadAnimationClip(clip.ClipPath)));
                    //StateAnimationPairs[stateType] = DEBUFF_List;
                    break;
                case "DEATH":
                    DEATH_List.AddRange(orderedClips.Select(clip => LoadAnimationClip(clip.ClipPath)));
                    //StateAnimationPairs[stateType] = DEATH_List;
                    break;
                case "OTHER":
                    OTHER_List.AddRange(orderedClips.Select(clip => LoadAnimationClip(clip.ClipPath)));
                    //StateAnimationPairs[stateType] = OTHER_List;
                    break;
            }
        }
    }

    public void PlayAnimation(PlayerState PlayState, int index)
    {
        var animator = _anim;
        //Debug.Log(PlayState.ToString());
        var animations = StateAnimationPairs[PlayState.ToString()];
        //Debug.Log(OverrideController[PlayState.ToString()].name);
        OverrideController[PlayState.ToString()] = animations[index];
        //Debug.Log( OverrideController[PlayState.ToString()].name);
        var StateStr = PlayState.ToString();

        var isMove = StateStr.Contains("MOVE");
        var isDebuff = StateStr.Contains("DEBUFF");
        var isDeath = StateStr.Contains("DEATH");
        animator.SetBool("1_Move", isMove);
        animator.SetBool("5_Debuff", isDebuff);
        animator.SetBool("isDeath", isDeath);
        if (!isMove && !isDebuff)
        {
            var parameters = animator.parameters;
            foreach (var parameter in parameters)
                // if(parameter.type == AnimatorControllerParameterType.Bool){
                //     bool isBool = StateStr.ToUpper().Contains(parameter.name.ToUpper());
                //     animator.SetBool(parameter.name, isBool);
                // }
                if (parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    var isTrigger = parameter.name.ToUpper().Contains(StateStr.ToUpper());
                    if (isTrigger)
                    {
                        Debug.Log($"Parameter: {parameter.name}, Type: {parameter.type}");
                        animator.SetTrigger(parameter.name);
                    }
                }
        }
    }

    private AnimationClip LoadAnimationClip(string clipPath)
    {
        // "Animations" 폴더에서 애니메이션 클립 로드
        var clip = Resources.Load<AnimationClip>(clipPath.Replace(".anim", ""));

        if (clip == null) Debug.LogWarning($"Failed to load animation clip '{clipPath}'.");

        return clip;
    }
}