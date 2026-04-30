using UnityEngine;

public class Boss : MonoBehaviour, IDamageable
{
    private static readonly string MiddleHit = "MiddleHit";
    private static readonly string BigHit = "BigHit";
    private static readonly string Death = "Death";
    private static readonly string Attack = "Attack";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
