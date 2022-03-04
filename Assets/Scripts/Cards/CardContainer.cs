using System;
using UnityEngine;

public class CardContainer : MonoBehaviour
{
    private GameObject child;
    private float distance;

    readonly float minSpeed = 10;
    readonly float maxSpeed = 150;

    public bool IsDetached { get; set; }
    public Action OnAttachAction { get; set; }
    public GameObject Child
    {
        get => child;
        set
        {
            if (child != null)
            {
                Debug.LogError("CHILD ALREADY EXISTS!");
                return;
            }
            child = value;
            IsDetached = false;
        }
    }

    public Vector2 BufferDistance { get; set; }

    private void FixedUpdate()
    {
        if (IsDetached || Child == null) return;

        Vector2 bufferedPosition = transform.position;
        if (BufferDistance != null)
        {
            float bufferedX = transform.position.x - BufferDistance.x;
            float bufferedY = transform.position.y - BufferDistance.y;
            bufferedPosition.Set(bufferedX, bufferedY);
        }

        distance = Vector2.Distance(Child.transform.position, bufferedPosition);
        float speed = distance/5; // TESTING
        if (speed < minSpeed) speed = minSpeed;
        else if (speed > maxSpeed) speed = maxSpeed;
        if (distance > 0)
        {
            Child.transform.position =
                Vector3.MoveTowards(Child.transform.position, bufferedPosition, speed);
        }
        else OnAttach();
    }

    public void MoveContainer(GameObject newParent) =>
        transform.SetParent(newParent.transform, false);

    private void OnAttach()
    {
        if (OnAttachAction != null)
        {
            OnAttachAction();
            OnAttachAction = null;
        }
    }
}
