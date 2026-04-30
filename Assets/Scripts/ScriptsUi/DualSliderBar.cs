using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DualSliderBar : MonoBehaviour
{
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider delaySlider;

    [SerializeField] private float delaySeconds = 0.5f;
    [SerializeField] private float lerpSpeed    = 3f;

    private Coroutine _delayCoroutine;

    public void SetValue(float current, float max)
    {
        float ratio = max > 0 ? current / max : 0f;

        mainSlider.value = ratio;

        if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);
        _delayCoroutine = StartCoroutine(LerpDelay(ratio));
    }

    private IEnumerator LerpDelay(float target)
    {
        yield return new WaitForSeconds(delaySeconds);

        while (!Mathf.Approximately(delaySlider.value, target))
        {
            delaySlider.value = Mathf.Lerp(delaySlider.value, target, Time.deltaTime * lerpSpeed);
            yield return null;
        }

        delaySlider.value = target;
    }
}
