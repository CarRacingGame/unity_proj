using UnityEngine;

public class PulsatingImage : MonoBehaviour
{
    public float scaleAmount = 0.05f; // 얼마나 커졌다 작아질지
    public float speed = 10f;         // 애니메이션 속도
    private Vector3 originalScale;
    private float timer = 0f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        timer += Time.deltaTime * speed;
        float scaleFactor = 1 + Mathf.Sin(timer) * scaleAmount; // -0.05 ~ +0.05
        transform.localScale = originalScale * scaleFactor;
    }
}
