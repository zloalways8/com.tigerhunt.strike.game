using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SonarEngine : MonoBehaviour
{
    [SerializeField] private AudioMixer _mainAudioMixer;
    [SerializeField] private Slider _bgMusicSlider; // Слайдер для фоновой музыки
    [SerializeField] private Slider _fxSoundSlider; // Слайдер для звуковых эффектов
    
    private const float DefaultVolume = 0.75f;

    private void Awake()
    {
        LoadAndApplyVolumeSettings();
        SubscribeToSliderEvents();
    }

    private void LoadAndApplyVolumeSettings()
    {
        float savedMusicVolume = PlayerPrefs.GetFloat(OmniMetrics.HARMONIC_FLOW_ENABLED, DefaultVolume);
        float savedSoundVolume = PlayerPrefs.GetFloat(OmniMetrics.SONIC_PULSE_SWITCH, DefaultVolume);

        _bgMusicSlider.value = savedMusicVolume;
        AdjustMusicVolume(savedMusicVolume);

        _fxSoundSlider.value = savedSoundVolume;
        AdjustSoundVolume(savedSoundVolume);
    }

    private void SubscribeToSliderEvents()
    {
        _bgMusicSlider.onValueChanged.AddListener(AdjustMusicVolume);
        _fxSoundSlider.onValueChanged.AddListener(AdjustSoundVolume);
    }

    public void AdjustMusicVolume(float volume)
    {
        _mainAudioMixer.SetFloat(OmniMetrics.HARMONIC_FLOW_ENABLED, volume); // Логарифмическая шкала
        PlayerPrefs.SetFloat(OmniMetrics.HARMONIC_FLOW_ENABLED, volume);
    }

    public void AdjustSoundVolume(float volume)
    {
        _mainAudioMixer.SetFloat(OmniMetrics.SONIC_PULSE_SWITCH, volume); // Логарифмическая шкала
        PlayerPrefs.SetFloat(OmniMetrics.SONIC_PULSE_SWITCH, volume);
    }
}