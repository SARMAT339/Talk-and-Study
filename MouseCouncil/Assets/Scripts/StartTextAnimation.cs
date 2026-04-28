using UnityEngine;
using TMPro;

public class StartTextAnimation : MonoBehaviour
{
    public float pulseSpeed = 2f;     // скорость "дыхания"
    public float scaleAmount = 0.1f;  // насколько увеличивается
    public float minAlpha = 0.4f;     // минимальная прозрачность
    public float maxAlpha = 1f;       // максимальная

    private TextMeshProUGUI text;
    private Vector3 baseScale;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        baseScale = transform.localScale;
    }

    void Update()
    {
        float t = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;

        // Пульсация размера
        float scale = 1f + t * scaleAmount;
        transform.localScale = baseScale * scale;

        // Пульсация прозрачности
        Color color = text.color;
        color.a = Mathf.Lerp(minAlpha, maxAlpha, t);
        text.color = color;
    }
}