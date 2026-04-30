using UnityEngine;

public class Boss : MonoBehaviour, IDamageable
{
    private static readonly string MiddleHit = "MiddleHit";
    private static readonly string BigHit = "BigHit";
    private static readonly string Death = "Death";
    private static readonly string Attack = "Attack";

    private Animator animator;
    public AttackZone attackZone;

    public DamageVO state;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        attackZone.gameObject.SetActive(false);
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
        animator.SetTrigger(Death);
    }

    public void OnAttack()
    {
        state.amount = 100;
        state.damageType = DamageVO.DamageType.normal;
        animator.SetTrigger(Attack);
    }

    public void ToggleAttackZone()
    {
        if(attackZone.gameObject.activeSelf == false)
        {
            attackZone.gameObject.SetActive(true);
            attackZone.attackable = true;
            return;
        }
        attackZone.gameObject.SetActive(false);
    }

    public void GetDamage(DamageVO damageData)
    {
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
                break;
            case DamageVO.DamageType.noDamage:
            case DamageVO.DamageType.soft:
            default:
                break;
        }
    }
}
