using UnityEngine;
using UnityEngine.UI;

public class DualSliderBar : MonoBehaviour
{
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider delaySlider;

    [SerializeField] private float delaySeconds = 0.5f;
    [SerializeField] private float lerpSpeed = 3f;

    private float trueRatio;
    private float delayTimer;

    private void Awake()
    {
        trueRatio = 1f;
        mainSlider.value = 1f;
        delaySlider.value = 1f;
    }

    public void SetValue(float current, float max)
    {
        float newRatio = current / max;
        bool isDecreasing = newRatio < trueRatio;
        trueRatio = newRatio;

        if (isDecreasing)
        {
             
            mainSlider.value = newRatio;
            delayTimer = delaySeconds; // delay 슬라이더 대기 타이머 리셋
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
        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
        }
        else
        {
            delaySlider.value = Mathf.MoveTowards(
                delaySlider.value,
                trueRatio,
                lerpSpeed * Time.deltaTime
            );
        }

        // 회복: main이 trueRatio를 천천히 따라감
        if (mainSlider.value < trueRatio)
        {
            mainSlider.value = Mathf.MoveTowards(
                mainSlider.value,
                trueRatio,
                lerpSpeed * Time.deltaTime
            );
        }
    }
}
