//현재는 패널 전환만 확인하는 스크립트. 인게임 내 데이터 연동은 추후 예정.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;

    [SerializeField] private AudioClip clickSound;
    [SerializeField] private SettingsController settingsController;


    private void Awake()
    {
        startMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        Time.timeScale = 0f; // 게임 시작 시 일시정지 상태로 시작
        Cursor.lockState = CursorLockMode.None; // 커서 보이도록 설정
        Cursor.visible = true;
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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnOpenSettings()
    {
        startMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnCloseSettings()
    {
        AudioManager.Instance.PlaySE(clickSound);
        //SaveManager.Instance.Save();
        settingsPanel.SetActive(false);
        startMenuPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnGameRestart()
    {
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(true);
        settingsController.StartCount();
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMainMenu()
    {
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        startMenuPanel.SetActive(true);
        Time.timeScale = 1f;
        settingsController.StopCount();
 

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
 
    }

    public void OnPause()
    {
        
        bool isPaused = pausePanel.activeSelf;
        pausePanel.SetActive(!isPaused);
        gamePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 1f : 0f;
 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
 
    }

    public void OnResume()
    {
        pausePanel.SetActive(false);
        gamePanel.SetActive(true);
        Time.timeScale = 1f;
 

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
 
 
    }
    public void OnGameOver()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        Time.timeScale = 1f;
        settingsController.StopCount();
        //캐릭터 hp 0되면 호출 예정


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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