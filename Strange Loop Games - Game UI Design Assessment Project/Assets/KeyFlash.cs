using UnityEngine;
using UnityEngine.UI;

public class KeyFlash : MonoBehaviour
{
    private Image key;
    void Update()
    {
        Flash();
    }
    private void Flash()
    {
        if (key == null) key = GetComponent<Image>();
        if (!key.enabled) return;
        float pulse = Mathf.Sin(Time.time * 8) * 0.5f + 0.5f;
        key.color = new Color(1, 1, 1, pulse);
    }
}
