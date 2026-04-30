using System;
using UnityEngine;

public class Boss : MonoBehaviour, IDamageable
{
    private enum Statement
    {
        Idle,
        Attack,
        Death,
        Hit,
        BigHit,
        Move,
    }

    private static readonly string MiddleHit = "MiddleHit";
    private static readonly string BigHit = "BigHit";
    private static readonly string Death = "Death";
    private static readonly string Attack = "Attack";

    private Animator animator;
    public NormalAttackZone attackZone;
    public BossData data;

    private Statement currentstatement;

    public Action<int> OnDamage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        attackZone.gameObject.SetActive(false);
    }

    public void OnMiddleHit()
    {
        currentstatement = Statement.Hit;
        animator.SetTrigger(MiddleHit);
    }

    public void OnBigHit()
    {
        currentstatement = Statement.BigHit;
        animator.SetTrigger(BigHit);
    }

    public void OnDeath()
    {
        currentstatement = Statement.Death;
        animator.SetTrigger(Death);
    }

    public void OnAttack()
    {
        currentstatement = Statement.Attack;
        animator.SetTrigger(Attack);
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
