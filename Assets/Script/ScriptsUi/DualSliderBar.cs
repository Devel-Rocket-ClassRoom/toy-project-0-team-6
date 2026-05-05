using UnityEngine;
using UnityEngine.UI;

public class DualSliderBar : MonoBehaviour
{
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider delaySlider;

    [SerializeField] private float delaySeconds = 0.5f;
    [SerializeField] private float lerpSpeed = 3f;

    private float _trueRatio;
    private float _delayTimer;

    private void Awake()
    {
        _trueRatio = 1f;
        mainSlider.value = 1f;
        delaySlider.value = 1f;
    }

    public void SetValue(float current, float max)
    {
        float newRatio = max > 0f ? current / max : 0f;
        bool isDecreasing = newRatio < _trueRatio;
        _trueRatio = newRatio;

        if (isDecreasing)
        {
             
            mainSlider.value = newRatio;
            _delayTimer = delaySeconds; // delay 슬라이더 대기 타이머 리셋
        }
        else
        {
            // 회복: delay 즉시, main은 Update에서 천천히 따라감
            delaySlider.value = newRatio;
        }
    }

    private void Update()
    {
        // 감소: 타이머 이후 delay가 main(실제값)을 따라감
        if (_delayTimer > 0f)
        {
            _delayTimer -= Time.deltaTime;
        }
        else
        {
            delaySlider.value = Mathf.MoveTowards(
                delaySlider.value,
                _trueRatio,
                lerpSpeed * Time.deltaTime
            );
        }

        // 회복: main이 trueRatio를 천천히 따라감
        if (mainSlider.value < _trueRatio)
        {
            mainSlider.value = Mathf.MoveTowards(
                mainSlider.value,
                _trueRatio,
                lerpSpeed * Time.deltaTime
            );
        }
    }
}
