using UnityEngine;

public class CharacterState : MonoBehaviour
{
    public int maxHealth = 100;

    private int currentHealth;
    private int consumablesCount;   //현재 소모품 갯수
    private int currentConsumable;  //현재 소모품 타입(임시 int)

    private int power;          //힘
    private int healthStat;     //체력스텟
    private int dexterity;      //민첩

    public bool isDead = false; //사망 여부

    public event System.Action<int> Damaged;        //데미지 이벤트 함수

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

    public void OnDamage(int damage)
    {
        CurrentHealth -= damage;

        Damaged?.Invoke(damage);
    }
}
