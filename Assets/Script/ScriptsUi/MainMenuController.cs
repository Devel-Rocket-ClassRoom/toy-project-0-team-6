//현재는 패널 전환만 확인하는 스크립트. 인게임 내 데이터 연동은 추후 예정.
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;

    [SerializeField] private AudioClip clickSound;
    [SerializeField] private SettingsController settingsController;

    private void Start()
    {
        startMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        Time.timeScale = 0f; // 게임 시작 시 일시정지 상태로 시작
        //메인메뉴상에서 보스가 움직여서 캐릭터를 공격하는 현상이 발생하여 일시정지 상태로 시작하도록 변경
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gamePanel.activeSelf)
            OnPause();
    }

    public void OnStartGame()
    {
        startMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(true);
        settingsController.StartCount();
        Time.timeScale = 1f;
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
        settingsController.StartCount();
        Time.timeScale = 1f;
    }

    public void OnMainMenu()
    {
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        startMenuPanel.SetActive(true);
        Time.timeScale = 1f;
        settingsController.StopCount();
    }

    public void OnPause()
    {
        bool isPaused = pausePanel.activeSelf;
        pausePanel.SetActive(!isPaused);
        gamePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 1f : 0f;
    }

    public void OnResume()
    {
        pausePanel.SetActive(false);
        gamePanel.SetActive(true);
        Time.timeScale = 1f;
    }
    public void OnGameOver()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        Time.timeScale = 1f;
        settingsController.StopCount();
        //캐릭터 hp 0되면 호출 예정
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