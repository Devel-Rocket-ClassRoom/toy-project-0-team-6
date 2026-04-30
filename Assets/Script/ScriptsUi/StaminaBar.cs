using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Slider staminaSlider;

    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float regenPerSecond = 10f;

    private float _currentStamina;

    private void Start()
    {
        _currentStamina = maxStamina;
        Refresh();
    }

    private void Update()
    {
        if (_currentStamina < maxStamina)
        {
            _currentStamina = Mathf.Min(maxStamina, _currentStamina + regenPerSecond * Time.deltaTime);
            Refresh();
        }
    }

    public void Use(float amount)
    {
        _currentStamina = Mathf.Max(0f, _currentStamina - amount);
        Refresh();
    }

    private void Refresh() => staminaSlider.value = _currentStamina / maxStamina;
}