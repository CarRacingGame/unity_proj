using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScaleEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Transform target;
    public float pressedScale = 0.9f;
    private Vector3 originalScale;

    void Start()
    {
        if (target == null) target = transform;
        originalScale = target.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 버튼 누르는 순간 scale 작아지게
        target.localScale = originalScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 손 뗄 때 원래 크기로 복원
        target.localScale = originalScale;
    }
}
