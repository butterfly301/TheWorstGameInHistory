using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
//using System.IO;

public class SPUM_AnimationManager : MonoBehaviour
{
    [Header("Animation Play Controller")] [SerializeField]
    private Slider timeLineSlider;

    [SerializeField] private Slider playSpeedSlider;
    [SerializeField] private Text slidertimeLineInfo;
    [SerializeField] private Text timeLineText;
    [SerializeField] private Text playSpeedText;
    public Transform StatePanel;
    public Button StateButtonPrefab;

    public string SelectedType;
    public SPUM_AnimationControllerPanel AnimationControllerPanel;
    public SPUM_AnimationStatePanel spumAnimationStatePanel;
    public SPUM_AnimationPackagePanel spumAnimationPackagePanel;
    public string CurrentPlayClip;

    public RectTransform rectTransform;

    [Header("Animation Preset")] public SPUM_AnimationPreset PresetPrefab;

    public Transform PresetContent;
    public Button AddPresetButton;
    public SPUM_PresetData SPUM_PresetData;
    public Toggle PresetTogle;
    public Dropdown presetDropdown;

    [Header("Manager")] public SPUM_Manager SPUM_Manager;

    private AnimatorOverrideController animatorOverrideController;

    public SPUM_Prefabs unit => SPUM_Manager.PreviewPrefab;

#if UNITY_EDITOR
    public void ScrollContentReset()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void PlayAnimation(SpumAnimationClip currentPlayClip)
    {
        var animator = unit._anim;
        var PlayState = $"{currentPlayClip.StateType}";
        animator.Rebind();
        animator.Update(0f);
        animatorOverrideController[PlayState] = LoadAnimationClip(currentPlayClip.ClipPath);

        animator.SetBool("1_Move", PlayState.Contains("MOVE"));
        animator.SetBool("5_Debuff", PlayState.Contains("DEBUFF"));
        animator.SetBool("isDeath", PlayState.Contains("DEATH"));
        animator.Play(PlayState, 0, 0);
        AnimationControllerPanel.RefreshSlier(currentPlayClip.ClipPath);
    }

    private AnimationClip LoadAnimationClip(string clipPath)
    {
        var clip = Resources.Load<AnimationClip>(clipPath.Replace(".anim", ""));

        if (clip == null) Debug.LogWarning($"Failed to load animation clip '{clipPath}'.");

        return clip;
    }

    public void CloseAnimationPanels()
    {
        spumAnimationStatePanel.gameObject.SetActive(false);
        spumAnimationPackagePanel.gameObject.SetActive(false);
    }

    private void InitAnimator()
    {
        var animator = unit._anim;
        animatorOverrideController = new AnimatorOverrideController();
        animatorOverrideController.runtimeAnimatorController = animator.runtimeAnimatorController;

        var clips = animator.runtimeAnimatorController.animationClips;

        foreach (var clip in clips) animatorOverrideController[clip.name] = clip;

        animator.runtimeAnimatorController = animatorOverrideController;
    }

    public void InitializeDropdown()
    {
        presetDropdown.ClearOptions();

        var options = new List<string>();
        foreach (var preset in SPUM_PresetData.Presets) options.Add($"{preset.UnitType} - {preset.PresetName}");

        presetDropdown.AddOptions(options);
    }

    private void Start()
    {
        InitAnimator();
        CreateSpumAnimationTypeButton();
        AnimationControllerPanel.Init(unit);

        spumAnimationStatePanel.CloseButton.onClick.AddListener(() =>
        {
            spumAnimationStatePanel.gameObject.SetActive(false);
        });
        spumAnimationPackagePanel.CloseButton.onClick.AddListener(() =>
        {
            spumAnimationStatePanel.CreateStateButton(this);
            spumAnimationPackagePanel.gameObject.SetActive(false);
        });
        spumAnimationStatePanel.ResetButton.onClick.AddListener(() =>
        {
            //AllResetIndex();
            ResetSelectedStateTypeIndex();
            // 새로 패널 그리기
            RefreshStatePanel();
        });
        InitPreviewUnitPackage();
        LoadPresetLst();
        AddPresetButton.onClick.AddListener(() =>
        {
            Debug.Log("PresetAdd");
            AddPreset();
        });
        PresetTogle.onValueChanged.AddListener(On =>
        {
            if (On) LoadPresetLst();
        });
        InitializeDropdown();
    }

    [ContextMenu("TESE")]
    public void LoadPresetLst()
    {
        foreach (Transform element in PresetContent) Destroy(element.gameObject);
        if (SPUM_PresetData == null) return;
        var filteredPresets = FilterPresetsByUnitType(unit.UnitType);

        foreach (var presetData in filteredPresets)
        {
            var Preset = Instantiate(PresetPrefab, PresetContent);
            Preset.Init(presetData, this);
        }
    }

    public List<SPUM_Preset> FilterPresetsByName(string presetName)
    {
        var filteredPresets = SPUM_PresetData.Presets.Where(p => p.PresetName == presetName).ToList();
        return filteredPresets;
    }

    public List<SPUM_Preset> FilterPresetsByUnitType(string unitType)
    {
        var filteredPresets = SPUM_PresetData.Presets.Where(p => p.UnitType == unitType).ToList();
        return filteredPresets;
    }

    public void AddPreset()
    {
        var Preset = Instantiate(PresetPrefab, PresetContent);
        var presetData = new SPUM_Preset
        {
            UnitType = unit.UnitType,
            PresetName = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
            Packages = unit.spumPackages?.Select(p => (SpumPackage)p.Clone()).ToList()
        };

        SavePresetData(presetData);
    }

    public void EditPresetData(string PreviousName, string ChangedName)
    {
        var presetToRemove = SPUM_PresetData.Presets.Find(p => p.PresetName == PreviousName);
        if (presetToRemove != null)
        {
            presetToRemove.PresetName = ChangedName;
            LoadPresetLst();
            Debug.Log("Changed: " + ChangedName);
        }
        else
        {
            Debug.LogWarning("Preset not found: " + PreviousName);
        }
    }

    public void SavePresetData(SPUM_Preset presetData)
    {
#if UNITY_EDITOR
        SPUM_PresetData.Presets.Add(presetData);
        EditorUtility.SetDirty(SPUM_PresetData);
        AssetDatabase.SaveAssets();
        LoadPresetLst();
        Debug.Log("Preset saved: " + presetData.PresetName);
#endif
    }

    public void DeletePresetData(string name)
    {
        // 프리셋 데이터를 삭제하는 로직을 추가합니다.
        var presetToRemove = SPUM_PresetData.Presets.Find(p => p.PresetName == name);
        if (presetToRemove != null)
        {
            SPUM_PresetData.Presets.Remove(presetToRemove);
            Debug.Log("Preset deleted: " + presetToRemove.PresetName);
            LoadPresetLst();
        }
        else
        {
            Debug.LogWarning("Preset not found: " + unit._code);
        }
    }

    public void ApplyPreset(SPUM_Preset preset)
    {
        unit.spumPackages = preset.Packages;
        PresetTogle.isOn = false;
        PlayFirstAnimation();
    }

    public void PlayFirstAnimation()
    {
        var clip = unit.spumPackages
            .SelectMany(package => package.SpumAnimationData)
            .FirstOrDefault(data =>
                data.StateType.Equals("IDLE", StringComparison.OrdinalIgnoreCase) &&
                data.HasData &&
                data.index == 0 &&
                data.UnitType.Equals(unit.UnitType));
        if (clip == null)
        {
            Debug.LogWarning("package data error");
            unit.spumPackages = SPUM_Manager.GetSpumLegacyData();
            return;
        }

        PlayAnimation(clip);
    }


    private void CreateSpumAnimationTypeButton()
    {
        var UnitStateData = unit.spumPackages;

        spumAnimationStatePanel.AddClipButton.onClick.RemoveAllListeners();

        foreach (var state in SPUM_Manager.StateList)
        {
            var StateButton = Instantiate(StateButtonPrefab, StatePanel);
            StateButton.GetComponentInChildren<Text>().text = state;

            var stateType = state;
            StateButton.onClick.AddListener(() =>
            {
                SelectedType = stateType;
                spumAnimationPackagePanel.gameObject.SetActive(false);
                spumAnimationStatePanel.gameObject.SetActive(true);
                spumAnimationStatePanel.CreateStateButton(this);
            });
        }

        // 클립 추가 버튼 데이터 할당
        spumAnimationStatePanel.AddClipButton.onClick.AddListener(() =>
        {
            spumAnimationPackagePanel.gameObject.SetActive(true);
            spumAnimationPackagePanel.CreateSpumAnimationPackagePanel(this);
        });
    }

    public void RefreshStatePanel()
    {
        spumAnimationStatePanel.CreateStateButton(this);
    }

    public void InitPreviewUnitPackage()
    {
        unit.spumPackages = SPUM_Manager.GetSpumLegacyData();
        ResetSelectedStateTypeIndex();
        PlayFirstAnimation();
    }

    public void ResetSelectedStateTypeIndex()
    {
        unit.spumPackages = SPUM_Manager.spumPackages;
        var UnitPackagesData = unit.spumPackages;
        var SelectState = SelectedType;
        var UnitType = unit.UnitType;

        foreach (var package in UnitPackagesData)
        foreach (var clipData in package.SpumAnimationData)
            if (clipData.StateType.Equals(SelectedType) && clipData.UnitType.Equals(unit.UnitType))
            {
                clipData.index = -1;
                clipData.HasData = false;
            }

        SpumPackage legacyPackage = null;

        foreach (var package in UnitPackagesData)
            if (package.Name.Trim().Equals("Legacy", StringComparison.OrdinalIgnoreCase))
            {
                legacyPackage = package;
                break;
            }

//var legacyPackage = UnitPackagesData.Where(p => p.Name.Trim().Equals("Legacy", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        if (legacyPackage != null)
        {
            var relevantClips = legacyPackage.SpumAnimationData
                .Where(clipData =>
                    clipData.StateType.Equals(SelectedType) &&
                    clipData.UnitType.Equals(unit.UnitType))
                .ToList();
            for (var i = 0; i < relevantClips.Count; i++)
            {
                //Debug.Log(relevantClips[i].Name);
                relevantClips[i].index = i;
                relevantClips[i].HasData = true;
            }
        }
    }

    public void IndexSawp(int clipindex, int dir)
    {
        if (clipindex == 0 && dir < 0) return;

        var UnitPackagesData = unit.spumPackages;
        var filteredClips = UnitPackagesData
            .SelectMany(package => package.SpumAnimationData)
            .Where(clip => clip.StateType.Equals(SelectedType) && clip.HasData && clip.UnitType.Equals(unit.UnitType))
            .OrderBy(clip => clip.index).ToList();


        var currentIndex = clipindex;
        var swapTargetIndex = clipindex + dir;
        if (swapTargetIndex == filteredClips.Count) return;
        //Debug.Log($"{currentIndex}==>{swapTargetIndex} {filteredClips.Count}");
        filteredClips[currentIndex].index = swapTargetIndex;
        filteredClips[swapTargetIndex].index = currentIndex;
        RefreshStatePanel();
    }
#endif
}