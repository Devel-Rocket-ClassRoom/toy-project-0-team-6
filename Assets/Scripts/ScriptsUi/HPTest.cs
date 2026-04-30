using UnityEngine;

public class HPTest : MonoBehaviour
{
    [SerializeField] private DualSliderBar playerHpBar;
    [SerializeField] private DualSliderBar enemyHpBar;
    [SerializeField] private ConsumableSlotUI consumableSlot;
    [SerializeField] private ConsumableSlotUI weaponSlot;

    [SerializeField] private ItemData ItemSlot;
    [SerializeField] private ItemData WeaponSlot;

    private float _playerHp = 100f;
    private float _enemyHp = 1000f;
    private float heal = 20f;   
    private int _count;

    private void Start()
    {
        playerHpBar.SetValue(_playerHp, 100f);
        enemyHpBar.SetValue(_enemyHp, 1000f);
        consumableSlot.SetItem(ItemSlot, ItemSlot.itemCount);
        weaponSlot.SetItem(WeaponSlot, WeaponSlot.itemCount);
    }

    private void Update()
    {
        // Q: 플레이어 피격
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _playerHp = Mathf.Max(0f, _playerHp - 20f);
            playerHpBar.SetValue(_playerHp, 100f);
        }
        // W: 플레이어 회복
        if (Input.GetKeyDown(KeyCode.E))
        {
            _playerHp = Mathf.Min(100f, _playerHp + heal);

            playerHpBar.SetValue(_playerHp, 100f);
        }
        // E: 보스 피격
        if (Input.GetKeyDown(KeyCode.E))
        {
            _enemyHp = Mathf.Max(0f, _enemyHp - 100f);
            enemyHpBar.SetValue(_enemyHp, 1000f);
        }

        // F: 소모품 사용
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_count > 0)
            {
                _playerHp = Mathf.Min(100f, _playerHp + 30f);
                _count--;
                playerHpBar.SetValue(_playerHp, 100f);
                consumableSlot.SetCount(_count);
            }
        }
    }
}