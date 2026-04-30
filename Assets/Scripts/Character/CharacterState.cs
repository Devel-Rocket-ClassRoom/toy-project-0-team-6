using UnityEngine;

public class CharacterState : MonoBehaviour
{
    public int maxHealth = 100;

    private int currentHealth;
    private int consumablesCount;
    private int currentConsumable;

    private int power;
    private int healthStat;
    private int dexterity;

    public bool isDead = false;

    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);

            if (currentHealth == 0)
                isDead = true;
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
    }
}
