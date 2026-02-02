using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using SeawispHunter.RolePlay.Attributes;

public class SettingManager : MonoBehaviour
{
    // Inisiasi Objek
    [Header("Video")]
    [SerializeField] private Slider _brightnessSlider;
    [SerializeField] private Toggle _fullScreenToggle;
    [SerializeField] private Image _brightnessOverlay;

    [Header("Audio")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("Control")]
    [SerializeField] private TMP_InputField _upInput;
    [SerializeField] private TMP_InputField _downInput;
    [SerializeField] private TMP_InputField _leftInput;
    [SerializeField] private TMP_InputField _rightInput;

    [Header("Panel")]
    [SerializeField] private GameObject _videoPanel;
    [SerializeField] private GameObject _audioPanel;
    [SerializeField] private GameObject _controlPanel;

    [Header("SettingCanvas")]
    [SerializeField] private GameObject _settingsCanvas;

    // Set Objek
    void Start()
    {
        InitializeControl();
        InitializeAudio();

        OpenVideoPanel();
    }

    // Inisiasi Control
    void InitializeControl()
    {
        _upInput.text = PlayerPrefs.GetString("W");
        _downInput.text = PlayerPrefs.GetString("S");
        _leftInput.text = PlayerPrefs.GetString("A");
        _rightInput.text = PlayerPrefs.GetString("D");
    }

    // Inisiasi Audio
    void InitializeAudio()
    {
        _bgmSlider.value = PlayerPrefs.GetFloat("BGMVol", 1f);
        _sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 1f);
    }

    // Buka Panel Video
    public void OpenVideoPanel()
    {
        _videoPanel.SetActive(true);
        _audioPanel.SetActive(false);
        _controlPanel.SetActive(false);
    }

    // Buka Panel Audio
    public void OpenAudioPanel()
    {
        _videoPanel.SetActive(false);
        _audioPanel.SetActive(true);
        _controlPanel.SetActive(false);
    }

    // Buka Panel Control
    public void OpenControlPanel()
    {
        _videoPanel.SetActive(false);
        _audioPanel.SetActive(false);
        _controlPanel.SetActive(true);
    }

    // Nutup Canvas
    public void CloseSettings()
    {
        _settingsCanvas.SetActive(false);
    }

    // Set untuk fullscreen
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    // Set untuk brightness
    public void SetBrightness(float value)
    {
        float opacityValue = 1f - value;
        Color temperatureColor = _brightnessOverlay.color;
        temperatureColor.a = opacityValue;
        _brightnessOverlay.color = temperatureColor;
    }

    // Set untuk volume BGM
    public void SetBGMVolume(float volume)
    {
        float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        _audioMixer.SetFloat("BGMVolume", dbVolume);
        PlayerPrefs.SetFloat("BGMVol", volume);
    }

    // Set untuk volume SFX
    public void SetSFXVolume(float volume)
    {
        float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        _audioMixer.SetFloat("SFXVolume", dbVolume);
        PlayerPrefs.SetFloat("SFXVol", volume);
    }

    // Keybinding
    public void Keybinding(string key)
    {
        string newKey = "";
        TMP_InputField target = null;

        switch (key)
        {
            case "Up": target = _upInput; break;
            case "Down": target = _downInput; break;
            case "Left": target = _leftInput; break;
            case "Right": target = _rightInput; break;
        }

        if (target == null)
        {
            newKey = target.text.ToUpper();
            PlayerPrefs.SetString("Key" + key, newKey);
            PlayerPrefs.Save();
        }
    }
}


