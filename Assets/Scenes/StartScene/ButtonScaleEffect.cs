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
        // ��ư ������ ���� scale �۾�����
        target.localScale = originalScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // �� �� �� ���� ũ��� ����
        target.localScale = originalScale;
    }
}
