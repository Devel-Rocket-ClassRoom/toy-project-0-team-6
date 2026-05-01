using System.Collections;
using UnityEngine;

public class PlayerHUDController : MonoBehaviour
{
    [SerializeField] private DualSliderBar hpBar;
    [SerializeField] private StaminaBar staminaBar;
    [SerializeField] private CharacterState characterState;

    private IEnumerator Start()
    {
        if (characterState == null)
        {
            Debug.LogWarning("캐릭터 연결X");
            yield break;
        }

        characterState.Damaged += OnDamaged;
        characterState.OnStaminaChanged += OnStaminaChanged;

        yield return null; // CharacterState.Start() 완료 대기

        hpBar.SetValue(characterState.CurrentHealth, characterState.maxHealth);
        staminaBar.SetValue(characterState.CurrentStamina, characterState.maxStamina);
    }

    private void OnDestroy()
    {
        if (characterState == null) return;
        characterState.Damaged -= OnDamaged;
        characterState.OnStaminaChanged -= OnStaminaChanged;
    }

    private void OnDamaged(int dmg)//이벤트 시그니처 맞춰서 int로 받음 실제 사용X.
    {
        if (characterState == null) return;
        hpBar.SetValue(characterState.CurrentHealth, characterState.maxHealth);
    }

    private void OnStaminaChanged(float current)
    {
        if (characterState == null) return;
        staminaBar.SetValue(current, characterState.maxStamina);
    }

    public void OnHealForTest(int current, int max)
    {
        hpBar.SetValue(current, max);
    }
}