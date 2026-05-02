//현재는 패널 전환만 확인하는 스크립트. 인게임 내 데이터 연동은 추후 예정.
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    
    [SerializeField] private AudioClip clickSound;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnPause();
    }
    public void OnStartGame()
    {
        startMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void OnOpenSettings()
    {
        startMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void OnCloseSettings()
    {
        AudioManager.Instance.PlaySE(clickSound);
        settingsPanel.SetActive(false);
        startMenuPanel.SetActive(true);
    }
    public void OnGameRestart()
    {
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(true);  
    }
    public void OnMainMenu()
    {
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        startMenuPanel.SetActive(true);
    }
    public void OnPause()
    {
        bool isPaused = pausePanel.activeSelf;
        pausePanel.SetActive(!isPaused);
        gamePanel.SetActive(isPaused);
    }
    public void OnResume()
    {
        pausePanel.SetActive(false);
        gamePanel.SetActive(true);
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