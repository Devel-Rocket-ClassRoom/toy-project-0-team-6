using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gamePanel;

    public void OnStartGame()
    {
        startMenuPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void OnOpenSettings()
    {
        startMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void OnCloseSettings()
    {
        settingsPanel.SetActive(false);
        startMenuPanel.SetActive(true);
    }

    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}