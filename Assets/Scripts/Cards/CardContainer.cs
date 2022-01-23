using UnityEngine;

public class CardContainer : MonoBehaviour
{
    private GameObject child;
    readonly float maxSpeed = 10;
    readonly float minSpeed = 0.5f;
    private float distance;
    public bool IsDetached { get; set; }
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

    private void Update()
    {
        if (IsDetached) return;
        distance = Vector2.Distance(Child.transform.position, transform.position);
        float speed = distance/10;
        if (speed < minSpeed) speed = minSpeed;
        else if (speed > maxSpeed) speed = maxSpeed;
        if (distance > 0)
        {
            Child.transform.position =
                Vector2.MoveTowards(Child.transform.position, transform.position, speed);
        }
    }

    public void MoveContainer(GameObject newParent) =>
        transform.SetParent(newParent.transform, false);
}
