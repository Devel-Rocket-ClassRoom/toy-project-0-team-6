using System.Collections;
using TMPro;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
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
    [SerializeField] private AudioClip mainMenuSound;
    [SerializeField] private AudioClip inGameSound;
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

   
    private int damageDeal;
    private int damageTaken;
    private int attackCount;
    private int hitCount;
    private float runTimer;
    private bool timerRunning;

    private bool gameOver = false;

   

    private void Awake()
    {
        AudioManager.Instance.PlayBGM(mainMenuSound);
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

    private void OnDestroy()//
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
        if (Input.GetKeyDown(KeyCode.Escape) && gamePanel.activeSelf) OnPause();
        if (timerRunning) runTimer += Time.deltaTime;
        CheckDeath();
    }

  
    private void CheckDeath()
    {
        //캐릭터쪽은 보스와 달리 dead체크가 미완성으로 보여 UI쪽에서 체크하도록 함. 보스는 클리어 이벤트로 체크
        //추후 캐릭터쪽이 보스와 동일하게 이벤트로 바뀌면 해당 기능은 제거.
        if (gameOver) return;
        if (characterState == null) return;
        if (!gamePanel.activeSelf) return;
        if (!characterState.IsDead) return;
        

        gameOver = true;
        OnGameOver();
    }

   


    

    public void OnStartGame()
    {
        AudioManager.Instance.PlaySE(clickSound);
        AudioManager.Instance.PlayBGM(inGameSound);
        gameOver = false;
        ResetStats();
        timerRunning = true;

        startMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        if (clearPanel != null) clearPanel.SetActive(false);
        gamePanel.SetActive(true);
        settingsController.StartCount();

        Time.timeScale = 1f;
        CursorInvisible();
    }

    public void OnOpenSettings()
    {
        AudioManager.Instance.PlaySE(clickSound);
        startMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        CursorVisible();
    }

    public void OnCloseSettings()
    {
        AudioManager.Instance.PlaySE(clickSound);
        SaveManager.Instance.Save();
        settingsPanel.SetActive(false);
        startMenuPanel.SetActive(true);
        CursorVisible();
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
        
        pausePanel.SetActive(true);
        gamePanel.SetActive(false);
        Time.timeScale = 0f;
        CursorVisible();
    }

    public void OnResume()
    {
        AudioManager.Instance.PlaySE(clickSound);
        pausePanel.SetActive(false);
        gamePanel.SetActive(true);
        Time.timeScale = 1f;
        CursorInvisible();
    }

    public void OnGameOver()
    {
        timerRunning = false;
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        settingsController.StopCount();
        UpdateGameOverTexts();
        SaveManager.Instance.Save();
        CursorVisible();
        StartCoroutine(ShowGameOverLogStats());
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
        CursorVisible();
        StartCoroutine(ShowClearLogStats());
    }
    public void CursorVisible()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void CursorInvisible()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
        damageDeal += dmg;
        attackCount++;
      
    }
    private void ResetStats()
    {
        damageDeal = 0;
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
        clearDamageDealtText.text = $"{damageDeal:N0}";
        clearDamageTakenText.text = $"{damageTaken:N0}";
        clearAttackCountText.text = $"{attackCount}";
        clearHitCountText.text = $"{hitCount}";
        clearTimeText.text = FormatTime(runTimer);
    }

    private void UpdateGameOverTexts()
    {
        overDamageDealtText.text = $"{damageDeal:N0}";
        overDamageTakenText.text = $"{damageTaken:N0}";
         overAttackCountText.text = $"{attackCount}";
        overHitCountText.text = $"{hitCount}";
        
        float ratio = boss.CurrentHp / (float)boss.data.BossHp * 100f;
        overBossHpText.text = $"{ratio:F0}%";
         
    }

    //게임오버/클리어 패널의 로그 텍스트 순차적으로 띄우기

    private IEnumerator ShowClearLogStats()
    {
        float delay = 0.3f;
        
        clearDamageDealtText.gameObject.SetActive(false);
        clearDamageTakenText.gameObject.SetActive(false);
        clearAttackCountText.gameObject.SetActive(false);
        clearHitCountText.gameObject.SetActive(false);
        clearTimeText.gameObject.SetActive(false);

        yield return new WaitForSecondsRealtime(delay);
        clearDamageDealtText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(delay);
        clearDamageTakenText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(delay);
        clearAttackCountText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(delay);
        clearHitCountText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(delay);
        clearTimeText.gameObject.SetActive(true);
    }
    private IEnumerator ShowGameOverLogStats()
    {
        float delay = 0.3f;
        overDamageDealtText.gameObject.SetActive(false);
        overDamageTakenText.gameObject.SetActive(false);
        overAttackCountText.gameObject.SetActive(false);
        overHitCountText.gameObject.SetActive(false);
        overBossHpText.gameObject.SetActive(false);

        yield return new WaitForSecondsRealtime(delay);
        overDamageDealtText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(delay);
        overDamageTakenText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(delay);
        overAttackCountText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(delay);
        overHitCountText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(delay);
        overBossHpText.gameObject.SetActive(true);
        //중복 요소 줄이기 고려.
    }
}
