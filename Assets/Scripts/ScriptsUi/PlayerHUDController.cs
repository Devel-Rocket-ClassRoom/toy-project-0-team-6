using UnityEngine;

public class PlayerHUDController : MonoBehaviour
{
    [SerializeField] private DualSliderBar hpBar;
    [SerializeField] private DualSliderBar staminaBar;
    [SerializeField] private ConsumableSlotUI consumableSlot;

    private float _maxHp      = 100f;
    private float _currentHp  = 100f;
    private float _maxStamina = 100f;
    private float _currentStamina = 100f;

    private void Start()
    {
        RefreshHP();
        RefreshStamina();
    }

    public void SetHP(float current, float max)
    {
        _currentHp = current;
        _maxHp     = max;
        RefreshHP();
    }

    public void SetStamina(float current, float max)
    {
        _currentStamina = current;
        _maxStamina     = max;
        RefreshStamina();
    }

    public void SetConsumableCount(int count) => consumableSlot.SetCount(count);

    private void RefreshHP()      => hpBar?.SetValue(_currentHp, _maxHp);
    private void RefreshStamina() => staminaBar?.SetValue(_currentStamina, _maxStamina);
}
