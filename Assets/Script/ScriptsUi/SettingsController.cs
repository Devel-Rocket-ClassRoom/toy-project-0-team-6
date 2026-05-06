using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("Frame Rate Settings")]
    [SerializeField] private Toggle frame30;
    [SerializeField] private Toggle frame60;
    [SerializeField] private Toggle frame120;
    [SerializeField] private Toggle frameunlim;

    [Header("Mouse Settings")]
    [SerializeField] private Slider mouseSensivitySlider;

    [Header("Audio Settings")]
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider seVolumeSlider;

    [Header("References")]
    [SerializeField] private CharacterMove characterMove;

    [Header("Play Time Display")]
    [SerializeField] private TextMeshProUGUI playTimeTextSettings;
    [SerializeField] private TextMeshProUGUI playTimeTextPause;

    private float totalPlayTime;
    private bool isPlaying;
    private bool isInitialized = true;

    private void Start()
    {
        ApplySettingData();
    }
    private void Update()
    {
        if(!isPlaying) return;
        if (SaveManager.Instance == null) return;
        totalPlayTime += Time.deltaTime;
        SaveManager.Instance.CurrentData.totalPlayTime = totalPlayTime;
        UpdateTimeText();

    }
    
    public void StartCount()
    {
        isPlaying = true;
       

    }
    public void StopCount()
    {
        isPlaying = false;
    }

    private void UpdateTimeText()
    {
        string formattedTime = FormatTime(totalPlayTime);
        playTimeTextSettings.text = formattedTime;
        playTimeTextPause.text = formattedTime;
    }
    private string FormatTime(float time)
    {
        int hours = (int)(time / 3600);
        int minutes = (int)((time % 3600) / 60);
        int seconds = (int)(time % 60);
        return $"{hours:00}:{minutes:00}:{seconds:00}";
    }
    public void SetMouseSensivity(float value)
    {
        if(isInitialized) return;
        characterMove.rotateSpeed = value;
        SaveManager.Instance.CurrentData.mouseSensitivity = value;
    }
    public void SetFrameRate30()
    {
        if(isInitialized) return;
        Application.targetFrameRate = 30;
        if (SaveManager.Instance == null) return;
        SaveManager.Instance.CurrentData.targetFPS = 30;
    }
    public void SetFrameRate60()
    {
         
        if (isInitialized) return;
        Application.targetFrameRate = 60;
        if (SaveManager.Instance == null) return;
        SaveManager.Instance.CurrentData.targetFPS = 60;
    }
    public void SetFrameRate120()
    {
        if(isInitialized) return;
        Application.targetFrameRate = 120;
        if (SaveManager.Instance == null) return;
        SaveManager.Instance.CurrentData.targetFPS = 120;
    }
    public void SetFrameRateUnlim()
    {
    
        if (isInitialized) return;
        Application.targetFrameRate = -1;
        if (SaveManager.Instance == null) return;
        SaveManager.Instance.CurrentData.targetFPS = -1;
    }   

    public void SetBGMVolume(float value) 
    {
        if(isInitialized) return;
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.SetBGMVolume(value);
         
        SaveManager.Instance.CurrentData.bgmVolume = value;
    }
    public void SetSEVolume(float value)
    {
        if (isInitialized) return;
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.SetSEVolume(value);
        SaveManager.Instance.CurrentData.seVolume = value;
    }
    public void ResetSettings()
    {
        if (SaveManager.Instance == null) return;

        SaveManager.Instance.ResetData();
        ApplySettingData();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetBGMVolume(SaveManager.Instance.CurrentData.bgmVolume);
            AudioManager.Instance.SetSEVolume(SaveManager.Instance.CurrentData.seVolume);
        }
    }
    public void ApplySettingData()
    {
        if (SaveManager.Instance == null) return;
        SaveData data = SaveManager.Instance.CurrentData;

        isInitialized = true;
        switch (data.targetFPS)
        {

            case 30:
                frame30.isOn = true;
                break;
            case 60:
                frame60.isOn = true;
                break;
            case 120:
                frame120.isOn = true;
                break;
            case -1:
                frameunlim.isOn = true;
                break;
        }
        bgmVolumeSlider.value = data.bgmVolume;
        seVolumeSlider.value = data.seVolume;



        characterMove.rotateSpeed = data.mouseSensitivity;
        mouseSensivitySlider.value = characterMove.rotateSpeed;

        totalPlayTime = data.totalPlayTime;
        UpdateTimeText();

        isInitialized = false;

        Application.targetFrameRate = data.targetFPS;// 
    }

}
