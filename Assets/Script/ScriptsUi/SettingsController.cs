using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Toggle frame30;
    [SerializeField] private Toggle frame60;
    [SerializeField] private Toggle frame120;
    [SerializeField] private Toggle frameunlim;

    [SerializeField] private TextMeshProUGUI playTimeTextSettings;
    [SerializeField] private TextMeshProUGUI playTimeTextPause;

    private float totalPlayTime;
    private bool isPlaying;

    private void Update()
    {
        if(!isPlaying) return;
        totalPlayTime += Time.deltaTime;
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
        int hours = Mathf.FloorToInt(time / 3600);
        int minutes = Mathf.FloorToInt((time % 3600) / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{hours:00}:{minutes:00}:{seconds:00}";
    }

    public void SetFrameRate30()
    {
        Application.targetFrameRate = 30;
    }
    public void SetFrameRate60()
    {
        Application.targetFrameRate = 60;
    }
    public void SetFrameRate120()
    {
        Application.targetFrameRate = 120;
    }
    public void SetFrameRateUnlim()
    {
        Application.targetFrameRate = -1;
    }   

}
