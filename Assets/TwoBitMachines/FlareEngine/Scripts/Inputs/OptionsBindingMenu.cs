using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TwoBitMachines.FlareEngine
{
    public class OptionsBindingMenu : MonoBehaviour
    {
        [SerializeField] public InputActionAsset inputAction;
        [SerializeField] public AudioManager audioManager;
        [SerializeField] public Toggle fullScreen;
        [SerializeField] public Slider music;
        [SerializeField] public Slider sfx;
        [SerializeField] public Dropdown resolutions;
        [SerializeField] public Button resetAll;
        [NonSerialized] private Resolution[] res;

        private void Start()
        {
            if (fullScreen != null) fullScreen.isOn = PlayerPrefs.GetInt("IsFullScreen") <= 0 ? true : false;
            if (music != null) music.value = audioManager.musicVolume;
            if (sfx != null) sfx.value = audioManager.sfxVolume;
            fullScreen?.onValueChanged.AddListener(SetFullScreen);
            resolutions?.onValueChanged.AddListener(SetResolution);
            music?.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfx?.onValueChanged.AddListener(OnSFXVolumeChanged);
            resetAll?.onClick.AddListener(ResetAll);
            SetResolution();
        }

        public void OnEnable() // for saving new input values
        {
            var rebinds = PlayerPrefs.GetString("rebinds");
            if (!string.IsNullOrEmpty(rebinds)) inputAction.LoadBindingOverridesFromJson(rebinds);
        }

        public void OnDisable()
        {
            var rebinds = inputAction.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("rebinds", rebinds);
        }

        public void OnMusicVolumeChanged(float value)
        {
            audioManager?.MasterMusicVolume(value);
        }

        public void OnSFXVolumeChanged(float value)
        {
            audioManager?.MasterSFXVolume(value);
        }

        public void SetFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
            PlayerPrefs.SetInt("IsFullScreen", isFullScreen ? 0 : 1);
        }

        private void SetResolution()
        {
            res = Screen.resolutions;
            resolutions.ClearOptions();

            Array.Sort(res, (x, y) =>
            {
                // Sort by refresh rate first
                var refreshRateComparison = x.refreshRate.CompareTo(y.refreshRate);
                if (refreshRateComparison != 0) return refreshRateComparison;

                // If refresh rates are the same, sort by width and height
                var widthComparison = x.width.CompareTo(y.width);
                if (widthComparison != 0) return widthComparison;

                return x.height.CompareTo(y.height);
            });

            var current = 0;
            var refreshRate = PlayerPrefs.GetInt("RefreshRateTBM", Screen.currentResolution.refreshRate);
            var options = new List<string>();
            for (var i = 0; i < res.Length; i++)
            {
                var option = res[i].width + " x " + res[i].height + " @ " + res[i].refreshRate;
                options.Add(option);

                if (res[i].width == Screen.width && res[i].height == Screen.height &&
                    res[i].refreshRate == refreshRate) current = i;
            }

            resolutions.AddOptions(options);
            resolutions.value = current;
            resolutions.RefreshShownValue();
        }

        public void SetResolution(int index)
        {
            var resolution = res[index];
            Screen.SetResolution(resolution.width, resolution.height,
                PlayerPrefs.GetInt("IsFullScreen") <= 0 ? true : false, resolution.refreshRate);
            PlayerPrefs.SetInt("RefreshRateTBM", resolution.refreshRate);
        }

        public void ResetAll()
        {
            if (inputAction == null) return;
            foreach (var map in inputAction.actionMaps) map.RemoveAllBindingOverrides();
            var list = GetComponentsInChildren<RebindInputButtonSO>();
            for (var i = 0; i < list.Length; i++) list[i].ResetBinding();
        }

        public void ResetSpecific(string controlScheme)
        {
            if (inputAction == null)
            {
                Debug.LogWarning("InputAction is missing reference.");
                return;
            }

            foreach (var map in inputAction.actionMaps)
            foreach (var action in map.actions)
                action.RemoveBindingOverride(InputBinding.MaskByGroup(controlScheme));
        }
    }
}