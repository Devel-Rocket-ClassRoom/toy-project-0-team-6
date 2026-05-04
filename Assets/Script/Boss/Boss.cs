using System;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour, IDamageable
{
    private enum Statement
    {
        Idle,
        Attack,
        Death,
        Move,
    }

    private static readonly int MiddleHit = Animator.StringToHash("MiddleHit");
    private static readonly int BigHit = Animator.StringToHash("BigHit");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int BigAttack = Animator.StringToHash("BigAttack");
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Phase2 = Animator.StringToHash("Phase2");
    private static readonly string PlayerTag = "Player";

    private Animator animator;
    public NormalAttackZone attackZone;
    public BossData data;
    private NavMeshAgent agent;
    private GameObject target;

    private Statement currentstatement;
    private int normalAttackCount = 0;
    private float idleTime = 0f;
    private float idleInterval = 4f;
    public float attackDistance = 0f;
    private float toTargetDistance => Vector3.Distance(transform.position, target.transform.position);
    private float attackCoolTime = 0f;
    private float attackInterval = 4f;
    private bool phase2;
    private int maxHp;
    private int currentHp;
    private int damage;
    private bool isDeath;
    private bool invincible;

    public event Action<int> OnDamage;

    private Statement CurrentStatement
    {
        get { return currentstatement; }
        set
        {
            switch (value)
            {
                case Statement.Idle:
                    currentstatement = value;
                    agent.isStopped = true;
                    break;
                case Statement.Attack:
                    agent.isStopped = true;
                    currentstatement = value;
                    break;
                case Statement.Death:
                    agent.isStopped = true;
                    currentstatement = value;
                    break;
                case Statement.Move:
                    idleTime = 0f;
                    agent.isStopped = false;
                    currentstatement = value;
                    break;
            }
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        attackZone.gameObject.SetActive(false);
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag(PlayerTag);
        
        CurrentStatement = Statement.Idle;
        phase2 = false;
        maxHp = data.BossHp;
        currentHp = maxHp;
        damage = data.Attack;
        isDeath = false;
        invincible = false;
    }

    private void Update()
    {

        if (target == null || isDeath)
        {
            return;
        }

        if(!phase2 && currentHp <= maxHp / 2)
        {
            invincible = true;
            phase2 = true;
            damage = Mathf.CeilToInt(data.Attack * 1.3f);
            attackInterval = Mathf.Floor(attackInterval * 0.9f);
            idleInterval = Mathf.Floor(idleInterval * 0.9f);
            agent.speed = Mathf.Ceil(agent.speed * 1.1f);
            agent.isStopped = true;
            animator.SetTrigger(Phase2);
        }

        attackCoolTime += Time.deltaTime;

        agent.SetDestination(target.transform.position);

        switch (CurrentStatement)
        {
            case Statement.Idle:
                idleTime += Time.deltaTime;
                if (toTargetDistance < attackDistance)
                {
                    CurrentStatement = Statement.Attack;
                }
                if (idleTime > idleInterval)
                {
                    CurrentStatement = Statement.Move;
                }
                break;
            case Statement.Attack:
                if(attackCoolTime > attackInterval)
                {
                    OnAttack();
                    attackCoolTime = 0f;
                }
                break;
            case Statement.Death:
                OnDeath();
                break;
            case Statement.Move:
                OnMove();
                if(toTargetDistance < attackDistance)
                {
                    CurrentStatement = Statement.Attack;
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        if(target == null)
        {
            return;
        }

        transform.LookAt(target.transform.position);
    }



    public void OnMiddleHit()
    {
        animator.SetTrigger(MiddleHit);
    }

    public void OnBigHit()
    {
        animator.SetTrigger(BigHit);
    }

    public void OnPhase2()
    {
        currentHp -= currentHp / 2;
    }

    public void OnDeath()
    {
        isDeath = true;
        currentstatement = Statement.Death;
        animator.SetTrigger(Death);
    }

    public void OnAttack()
    {
        if(normalAttackCount >= 2)
        {
            animator.SetTrigger(BigAttack);
            normalAttackCount = 0;
        }
        else
        {
            animator.SetTrigger(Attack);
            normalAttackCount++;
        }
    }

    public void OnBigAttack()
    {
        animator.SetTrigger(BigAttack);
    }

    public void OnMove()
    {
        animator.SetBool(Move, true);
    }

    public void ToggleAttackZone()
    {
        if(attackZone.gameObject.activeSelf == false)
        {
            attackZone.gameObject.SetActive(true);
            DamageVO attackInfo = new() { amount = damage, damageType = DamageVO.DamageType.normal };
            attackZone.SetDamage(attackInfo);
            attackZone.attackable = true;
            return;
        }
        attackZone.gameObject.SetActive(false);
    }

    public void AttackAnimationEnd()
    {
        CurrentStatement = Statement.Idle;
        animator.SetBool(Move, false);
    }

    public void GetDamage(DamageVO damageData)
    {
        if (invincible)
        {
            return;
        }

        currentHp -= damageData.amount;
        OnDamage?.Invoke(damageData.amount);

        switch (damageData.damageType)
        {   
            case DamageVO.DamageType.normal:
                OnMiddleHit();
                break;
            case DamageVO.DamageType.hard:
            case DamageVO.DamageType.veryHard:
                OnBigHit();
                break;
            case DamageVO.DamageType.instantKill:
                OnDeath();
                break;
            case DamageVO.DamageType.noDamage:
            case DamageVO.DamageType.soft:
            default:
                break;
        }
    }

    public void Phase2MotionEnd()
    {
        agent.isStopped = currentstatement == Statement.Move ? false : true;
    }

    public void ToggleInvincible()
    {
        invincible = !invincible;
    }
}
