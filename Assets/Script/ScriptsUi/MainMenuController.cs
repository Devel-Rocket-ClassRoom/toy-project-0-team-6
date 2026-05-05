using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private GameObject pausePanel;

    [Header("References")]
    [SerializeField] private CharacterState characterState;
    [SerializeField] private Boss boss;

    [Header("Audio")]
    [SerializeField] private AudioClip clickSound;

    [Header("Controllers")]
    [SerializeField] private SettingsController settingsController;

    [Header("Clear Panel Text")]
    [SerializeField] private TextMeshProUGUI clearDamageDealtText;
    [SerializeField] private TextMeshProUGUI clearDamageTakenText;
    [SerializeField] private TextMeshProUGUI clearAttackCountText;
    [SerializeField] private TextMeshProUGUI clearHitCountText;
    [SerializeField] private TextMeshProUGUI clearTimeText;

    [Header("GameOver Panel Text")]
    [SerializeField] private TextMeshProUGUI overDamageDealtText;
    [SerializeField] private TextMeshProUGUI overDamageTakenText;
    [SerializeField] private TextMeshProUGUI overAttackCountText;
    [SerializeField] private TextMeshProUGUI overHitCountText;
    [SerializeField] private TextMeshProUGUI overBossHpText;

   
    private int damageDealt;
    private int damageTaken;
    private int attackCount;
    private int hitCount;
    private float runTimer;
    private bool timerRunning;

    private bool gameOver = false;

   

    private void Awake()
    {
        startMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        clearPanel.SetActive(false);
        pausePanel.SetActive(false);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
         //캐릭터 인스턴스 탐색
        if (characterState == null || !characterState.gameObject.activeInHierarchy)
        {
            characterState = FindAnyObjectByType<CharacterState>();
        }

        if (characterState != null)
        {
            characterState.Damaged += OnPlayerDamaged;
        }

        // boss도 동일하게 비활성 참조면 씬에서 탐색
        if (boss == null || !boss.gameObject.activeInHierarchy)
        {
            boss = FindAnyObjectByType<Boss>();
        }
            

        if (boss != null)
        {
            boss.OnClear += OnClear;
            boss.OnDamage += OnBossDamaged;
        }
    }

    private void OnDestroy()
    {
        if (characterState != null)
        {
            characterState.Damaged -= OnPlayerDamaged;
        }

        if (boss != null)
        {
            boss.OnClear -= OnClear;
            boss.OnDamage -= OnBossDamaged;
        }
    }

 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gamePanel.activeSelf)
            OnPause();

        if (timerRunning)
            runTimer += Time.deltaTime;

        CheckDeath();
    }

  
    private void CheckDeath()
    {
        if (gameOver) return;
        if (characterState == null) return;
        if (!gamePanel.activeSelf) return;
        if (!characterState.IsDead) return;

        gameOver = true;
        OnGameOver();
    }

   

    private void OnPlayerDamaged(int dmg)
    {
        if (gameOver) return;
        damageTaken += dmg;
        hitCount++;
    }

    private void OnBossDamaged(int dmg)
    {
        if (gameOver) return;
        damageDealt += dmg;
        attackCount++;
        // invincible 구간엔 이벤트가 안 오므로 damageDealt는 과소 집계될 수 있음
        // 정확한 최종 피해량은 UpdateGameOverTexts/UpdateClearTexts에서 HP 차이로 보정
    }

    // boss 최대HP - 현재HP = 실제 입힌 총 피해 (invincible 무관)
    private int GetActualDamageDealt()
    {
        if (boss == null) return damageDealt;
        int fromHp = boss.data != null ? boss.data.BossHp - boss.CurrentHp : damageDealt;
        return Mathf.Max(fromHp, damageDealt); // 둘 중 더 큰 값 사용
    }
 

    private void ResetStats()
    {
        damageDealt = 0;
        damageTaken = 0;
        attackCount = 0;
        hitCount = 0;
        runTimer = 0f;
        timerRunning = false;
    }

    private string FormatTime(float seconds)
    {
        int min = (int)(seconds / 60);
        int sec = (int)(seconds % 60);
        return $"{min:D2}:{sec:D2}";
    }

    private void UpdateClearTexts()
    {
        clearDamageDealtText.text = $"{damageDealt:N0}";
        clearDamageTakenText.text = $"{damageTaken:N0}";
        clearAttackCountText.text = $"{attackCount}";
        clearHitCountText.text = $"{hitCount}";
        clearTimeText.text = FormatTime(runTimer);
    }

    private void UpdateGameOverTexts()
    {
        overDamageDealtText.text = $"{damageDealt:N0}";
        overDamageTakenText.text = $"{damageTaken:N0}";
         overAttackCountText.text = $"{attackCount}";
        overHitCountText.text = $"{hitCount}";
        
        float ratio = boss.CurrentHp / (float)boss.data.BossHp * 100f;
        overBossHpText.text = $"{ratio:F0}%";
         
    }

    

    public void OnStartGame()
    {
        AudioManager.Instance.PlaySE(clickSound);
        gameOver = false;
        ResetStats();
        timerRunning = true;

        startMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        if (clearPanel != null) clearPanel.SetActive(false);
        gamePanel.SetActive(true);
        settingsController.StartCount();

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnOpenSettings()
    {
        AudioManager.Instance.PlaySE(clickSound);
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
        AudioManager.Instance.PlaySE(clickSound);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMainMenu()
    {
        AudioManager.Instance.PlaySE(clickSound);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        timerRunning = false;
        gamePanel.SetActive(false);
        if (clearPanel != null) clearPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        settingsController.StopCount();
        UpdateGameOverTexts();
        SaveManager.Instance.Save();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnClear()
    {
        
        if (gameOver) return;
        gameOver = true;
        timerRunning = false;
        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        if (clearPanel != null) clearPanel.SetActive(true);
        Time.timeScale = 0f;
        settingsController.StopCount();
        SaveManager.Instance.CurrentData.stageClearCount++;
        UpdateClearTexts();
        SaveManager.Instance.Save();
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
