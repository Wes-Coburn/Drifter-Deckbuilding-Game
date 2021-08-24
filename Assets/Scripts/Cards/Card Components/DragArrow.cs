using UnityEngine;

public class DragArrow : MonoBehaviour
{
    public GameObject SourceCard { get; set; }
    private LineRenderer lineRend;

    private void Awake()
    {
        lineRend = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (SourceCard == null) return;
        Vector3 dragPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(dragPoint.x, dragPoint.y, -2);
        transform.SetParent(UIManager.Instance.CurrentWorldSpace.transform, true);
        lineRend.SetPosition(0, transform.position);
        lineRend.SetPosition(1, SourceCard.transform.position);
    }
}
