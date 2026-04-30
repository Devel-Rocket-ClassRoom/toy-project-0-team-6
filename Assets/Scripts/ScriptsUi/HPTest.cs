using UnityEngine;

public class HPTest : MonoBehaviour
{
    [SerializeField] private DualSliderBar hpBar;
    [SerializeField] private ConsumableSlotUI consumableSlot;

    private float _hp = 100f;
    private float damage = 20f;
    private int _count = 5;

    private void Start()
    {
        hpBar.SetValue(_hp, 100f);
        consumableSlot.SetCount(_count);
    }

    private void Update()
    {
        // Q 누르면 데미지
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _hp = Mathf.Max(0f, _hp - damage);
            hpBar.SetValue(_hp, 100f);
        }

        // E 누르면 회복
        if (Input.GetKeyDown(KeyCode.E))
        {
            _hp = Mathf.Min(100f, _hp + damage);
            hpBar.SetValue(_hp, 100f);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_count > 0)
            {
                _count--;
                consumableSlot.SetCount(_count);
            }
        }
    }
}