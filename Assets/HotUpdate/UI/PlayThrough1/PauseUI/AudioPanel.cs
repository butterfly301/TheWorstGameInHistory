using HotUpdate.Interface;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class AudioPanel : MonoBehaviour, OptionPanelChildren,IAutoBind
{
    [SerializeField]private Slider musicSlider;
    [SerializeField]private Slider soundSlider;

public void Init()
    {
        AudioKit.Settings.MusicVolume.RegisterWithInitValue(v => musicSlider.value = v);
        musicSlider.onValueChanged.AddListener(v => { AudioKit.Settings.MusicVolume.Value = v; });
        AudioKit.Settings.SoundVolume.RegisterWithInitValue(v => soundSlider.value = v);
        soundSlider.onValueChanged.AddListener(v => { AudioKit.Settings.SoundVolume.Value = v; });
    }
}