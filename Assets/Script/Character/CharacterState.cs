using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterState : MonoBehaviour, IDamageable
{ 
    public enum StateType
    {
        Idle,
        Move,
        Attack,
        UsingConsumable,
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

    public enum ConsumableItem
    {
        Heal,

    }

    private CharacterMove characterMove;
    public CharacterAttackZone AttackZone;
    private GameObject boss;
    public int maxHealth = 100;
    private int currentHealth;

    private int consumablesCount;   //현재 소모품 갯수
    private ConsumableItem currentConsumable;  //현재 소모품 타입(임시 int)

    public int StartPower = 5;
    public int StartHealthStat = 5;
    public int StartDexterity = 5;

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
    private readonly string BossTag = "Boss";
    public int attackCount = 0;

    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            float prev = currentHealth;

            currentHealth = Mathf.Clamp(value, 0, maxHealth);

            if (prev != currentHealth)
            {

                if (vibration != null)
                {
                    StopCoroutine(vibration);
                    Gamepad.current.SetMotorSpeeds(0f, 0f);
                }

                vibration = StartCoroutine(HitVibration());

                Damaged?.Invoke(currentHealth);
            }
        }
    }

    Coroutine vibration=null;
    public IEnumerator HitVibration()
    {
        Gamepad.current.SetMotorSpeeds(0.75f, 0.75f);
        yield return new WaitForSeconds(0.2f);
        Gamepad.current.SetMotorSpeeds(0f, 0f);
        vibration = null;
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

    public ConsumableItem CurrentConsumable
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

        power = StartPower;
        healthStat = StartHealthStat;
        dexterity = StartDexterity;

        characterMove = GetComponent<CharacterMove>();
        boss = GameObject.FindGameObjectWithTag(BossTag);
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
        DamageVO damage = new DamageVO();
        damage.amount = Power;
        damage.damageType = DamageVO.DamageType.soft;

        AttackZone.SetDamage(damage);

        CurrentStamina -= stmUseSpeed[(int)StaminaUseType.NormalAttack];
    }

    public void HardAttacking()
    {
        if (currentState == StateType.Dodge ||
            currentState == StateType.Damaged ||
            currentState == StateType.Die)
            return;

        currentState = StateType.Attack;

        DamageVO damage = new DamageVO();
        damage.amount = (int)((float)Power * 1.5);
        damage.damageType = DamageVO.DamageType.soft;

        AttackZone.SetDamage(damage);

        CurrentStamina -= (int)(stmUseSpeed[(int)StaminaUseType.NormalAttack]* 1.5);
    }

    public void Dodging()
    {
        if (currentState == StateType.Attack ||
            currentState == StateType.Damaged ||
            currentState == StateType.Die)
            return;

        currentState = StateType.Dodge;

        currentStamina -= stmUseSpeed[(int)StaminaUseType.Dodge];
        characterMove.commandQueue.Clear();
        DisableAttack();
    }

    public void UsingConsumable()
    {
        if (currentState == StateType.Attack ||
            currentState == StateType.Damaged ||
            currentState == StateType.Die)
            return;

        currentState = StateType.UsingConsumable;

        CurrentHealth += 20;
    }

    public void StopHealParticle()
    {
        characterMove.HealParticle.Stop();
    }

    public bool IsInvincible()
    {
        return currentState == StateType.Dodge ||
               currentState == StateType.Die;
    }


    public void GetDamage(DamageVO damageData)
    {
        if (IsInvincible())
            return;

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
    }

    public void EnableAttack()
    {
        characterMove.commandQueue.Clear();
        AttackZone.Attackable = true;
        attackCount++;
    }

    public void DisableAttack()
    {
        AttackZone.Attackable = false;
        if(characterMove.commandQueue.Count == 0)
        {
            characterMove.EndAttack();
        }
    }
}
