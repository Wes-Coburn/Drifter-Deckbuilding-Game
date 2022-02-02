using System;
using UnityEngine;

public class CardContainer : MonoBehaviour
{
    private GameObject child;
    readonly float minSpeed = 0.5f;
    readonly float maxSpeed = 10;
    private float distance;

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

    private void Update()
    {
        if (IsDetached || Child == null) return;

        Vector2 bufferedPosition = transform.position; // TESTING
        if (BufferDistance != null) // TESTING
        {
            float bufferedX = transform.position.x - BufferDistance.x;
            float bufferedY = transform.position.y - BufferDistance.y;
            bufferedPosition.Set(bufferedX, bufferedY);
        }
        distance = Vector2.Distance(Child.transform.position, bufferedPosition); // TESTING

        float speed = distance/10;
        if (speed < minSpeed) speed = minSpeed;
        else if (speed > maxSpeed) speed = maxSpeed;
        if (distance > 0)
        {
            Child.transform.position =
                Vector2.MoveTowards(Child.transform.position, bufferedPosition, speed);
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
