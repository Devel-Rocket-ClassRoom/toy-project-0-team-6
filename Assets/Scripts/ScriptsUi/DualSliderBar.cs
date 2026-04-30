using System.Collections;
 
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class DualSliderBar : MonoBehaviour
{
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider delaySlider;

    [SerializeField] private float delaySeconds = 0.5f;
    [SerializeField] private float lerpSpeed    = 3f;

    private float _trueRatio;       // 실제 값 비율, main과 delay가 따라가는 목표값
    private float _delayTimer;     // 피격 후 delay가 main을 따라가기 전까지의 타이머

    private void Awake()
    {
        _trueRatio = 1f;
        mainSlider.value  = 1f;
        delaySlider.value = 1f;
    }

    public void SetValue(float current, float max)
    {
        float newRatio = max > 0 ? current / max : 0f;
        bool isDecreasing = newRatio < _trueRatio;
        _trueRatio = newRatio;
         


        if (isDecreasing)
        {

            if (mainSlider.value < delaySlider.value)
            {
                mainSlider.value = delaySlider.value;
                StartCoroutine(DelayedDrop(newRatio));
                _delayTimer = delaySeconds;
                return;

            }
            //  main 즉시, delay는 타이머 리셋 후 Update에서 따라감
            mainSlider.value = newRatio;
            _delayTimer = delaySeconds;
        }
        else
        {
            // delay 즉시, main은 Update에서 따라감
             
            delaySlider.value = newRatio;
            
        }
    }
    private IEnumerator DelayedDrop(float targetValue)
    {
        yield return null; // 한 프레임 대기
        mainSlider.value = targetValue;
    }

    private void Update()
    {
         
       //피격 후 타이머되면 천천히 따라감
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

        //회복 시 천천히 따라감
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