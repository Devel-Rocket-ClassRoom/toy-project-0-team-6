using UnityEngine;

public class AttackZone : MonoBehaviour
{
    public Boss Boss;
    private BoxCollider attackZone;
    private IDamageable player;
    public bool attackable;

    private void OnEnable()
    {
        attackZone = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject.GetComponent<IDamageable>();
        }

        if (player != null && attackable)
        {
            attackable = false;
            player.GetDamage(Boss.state);
        }
    }

    private void OnDisable()
    {
        attackable = false;
    }
}
