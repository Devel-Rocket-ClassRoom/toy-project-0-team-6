using System;
using UnityEngine;

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

    private Animator animator;
    public NormalAttackZone attackZone;
    public BossData data;

    private Statement currentstatement;
    private int normalAttackCount = 0;
    private float idleTime = 0f;
    public float targetDistance = 0f;
    private float attackCoolTime = 0f;

    public Action<int> OnDamage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        attackZone.gameObject.SetActive(false);
    }

    private void Update()
    {
        attackCoolTime += Time.deltaTime;

        Debug.Log(currentstatement);

        switch (currentstatement)
        {
            case Statement.Idle:
                idleTime += Time.deltaTime;
                if(idleTime > 4f)
                {
                    currentstatement = Statement.Move;
                    idleTime = 0f;
                }
                break;
            case Statement.Attack:
                if(attackCoolTime > 4f)
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
                if(targetDistance < 3f)
                {
                    currentstatement = Statement.Attack;
                }
                break;
        }
    }



    public void OnMiddleHit()
    {
        animator.SetTrigger(MiddleHit);
    }

    public void OnBigHit()
    {
        animator.SetTrigger(BigHit);
    }

    public void OnDeath()
    {
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
        currentstatement = Statement.Attack;
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
            DamageVO attackInfo = new() { amount = 100, damageType = DamageVO.DamageType.normal };
            attackZone.SetDamage(attackInfo);
            attackZone.attackable = true;
            return;
        }
        attackZone.gameObject.SetActive(false);
    }

    public void AttackAnimationEnd()
    {
        currentstatement = Statement.Idle;
        animator.SetBool(Move, false);
    }

    public void GetDamage(DamageVO damageData)
    {
        data.BossHp -= damageData.amount;
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
}
