using System.Runtime.Serialization;
using UnityEngine;

public class CharacterState : MonoBehaviour, IDamageable
{ 
    public enum StateType
    {
        Idle,
        Move,
        Attack,
        Dodge,
        Damaged,
        Die
    }
    public enum StaminaUseType
    {
        NormalAttack = 0,       //공격 
        RestoreStamina,         //초당 회복
        Dodge,                  //회피
    }

    private CharacterMove characterMove;
    public int maxHealth = 100;
    private int currentHealth;

    private int consumablesCount;   //현재 소모품 갯수
    private int currentConsumable;  //현재 소모품 타입(임시 int)

    private int power;          //힘
    private int healthStat;     //체력스텟
    private int dexterity;      //민첩

    public float maxStamina;          //최대 스테미나
    private float currentStamina;     //현재 스테미나
    public float[] stmUseSpeed = new float[] { 20, 0.5f, 4 };   //스테미나 소모 속도[공격(1회)/달리기(초당)/회피(1회)] /임시 값
    private float restoreStmTime = 3f;  //스테미나 회복 대기시간
    private float restoreStmTimer = 0f; //대기 타이머

    public StateType currentState = StateType.Idle;

    public bool IsDead => currentHealth <= 0;     //사망 여부
    public bool IsDrained => currentStamina <= 0;      //스테미나 다떨어진 상태
    public bool CanRestoreStm => restoreStmTimer <= 0f;

    public event System.Action<int> Damaged;    //데미지 이벤트 함수
    public event System.Action<float> OnStaminaChanged; //스테미나 이벤트 함수

    private Animator anim;
    private readonly int Normal = Animator.StringToHash("Normal");
    private readonly int Hard = Animator.StringToHash("Hard");
    private readonly int VeryHard = Animator.StringToHash("VeryHard");
    private readonly int Die = Animator.StringToHash("Die");

    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
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
        characterMove = GetComponent<CharacterMove>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (restoreStmTimer > 0)
        {
            restoreStmTimer -= Time.deltaTime;

            if (restoreStmTimer < 0)
                restoreStmTimer = 0;
        }
        if (CanRestoreStm && currentStamina < maxStamina)
        {
            CurrentStamina += stmUseSpeed[1] * Time.deltaTime;
        }
    }

    public void Dead()
    {
        currentState = StateType.Die;

        anim.SetTrigger(Die);
    }

    public void Attacking()
    {
        if (currentState == StateType.Dodge ||
        currentState == StateType.Damaged ||
        currentState == StateType.Die)
            return;

        currentState = StateType.Attack;

        currentStamina -= stmUseSpeed[(int)StaminaUseType.NormalAttack];
    }

    public void GetDamage(DamageVO damageData)
    {
        switch (damageData.damageType)
        {
            case DamageVO.DamageType.noDamage:
            case DamageVO.DamageType.soft:
                break;
            case DamageVO.DamageType.normal:
                anim.SetTrigger(Normal);
                currentState = StateType.Damaged;
                break;
            case DamageVO.DamageType.hard:
                anim.SetTrigger(Hard);
                currentState = StateType.Damaged;
                break;
            case DamageVO.DamageType.veryHard:
                anim.SetTrigger(VeryHard);
                currentState = StateType.Damaged;
                break;
            case DamageVO.DamageType.instantKill:
                Dead();
                break;
        }


        CurrentHealth -= damageData.amount;

        Damaged?.Invoke(damageData.amount);
    }
}
