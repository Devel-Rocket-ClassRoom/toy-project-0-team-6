using UnityEngine;

public class RotateTest : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(0, 100f * Time.deltaTime, 0);
    }
}
