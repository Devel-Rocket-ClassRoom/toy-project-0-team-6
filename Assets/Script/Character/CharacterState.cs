using UnityEngine;

public class CharacterState : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    private int currentHealth;

    private int consumablesCount;   //현재 소모품 갯수
    private int currentConsumable;  //현재 소모품 타입(임시 int)

    private int power;          //힘
    private int healthStat;     //체력스텟
    private int dexterity;      //민첩

    public float maxStamina;          //최대 스테미나
    private float currentStamina;     //현재 스테미나
    public float[] stmUseSpeed = new float[3] { 2, 0.5f, 4 };   //스테미나 소모 속도[공격(1회)/달리기(초당)/회피(1회)] /임시 값
    private float restoreStmTime = 3f;  //스테미나 회복 대기시간
    private float restoreStmTimer = 0f; //대기 타이머

    public bool isDead;     //사망 여부
    public bool isDrained;      //스테미나 다떨어진 상태
    public bool CanRestoreStm => restoreStmTimer <= 0f;

    public event System.Action<int> Damaged;    //데미지 이벤트 함수
    public event System.Action<float> OnStaminaChanged;

    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);

            if (currentHealth > 0)
            {
                isDead = false;
            }
            else
            {
                isDead = true;
            }

        }
    }

    public float CurrentStamina
    {
        get { return currentStamina; }
        set
        {
            if (currentStamina > value)
            {
                restoreStmTimer = restoreStmTime;
            }

            currentStamina = Mathf.Clamp(value, 0, maxStamina);

            if (currentStamina <= 0)
                isDrained = true;
            else
                isDrained = false;

            float prev = currentStamina;

            currentStamina = Mathf.Clamp(value, 0, maxStamina);

            if (prev != currentStamina)
            {
                OnStaminaChanged?.Invoke(currentStamina);
            }
        }
    }

    public int ConsumablesCount
    {
        get { return consumablesCount; }
        set { consumablesCount = value; }
    }

    public int CurrentConsumable        //소모품 타입이 아직 없어 int로 대체
    {
        get { return currentConsumable; }
        set { currentConsumable = value; }
    }

    public int Power => power;
    public int HealthStat => healthStat;
    public int Dexterity => dexterity;

    void Start()
    {
        CurrentHealth = maxHealth;
        CurrentStamina = maxStamina;
    }

    private void Update()
    {
        if (restoreStmTimer > 0)
        {
            restoreStmTimer -= Time.deltaTime;
        }
        if (CanRestoreStm && currentStamina < maxStamina)
        {
            CurrentStamina += stmUseSpeed[1] * Time.deltaTime;
        }
    }

    public void GetDamage(DamageVO damageData)
    {
        CurrentHealth -= damageData.amount;

        Damaged?.Invoke(damageData.amount);
    }
}
