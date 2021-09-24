using System.Collections;
using UnityEngine;

public class CardContainer : MonoBehaviour
{
    private UIManager uMan;
    private Coroutine seekRoutine;
    private GameObject child;
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
        }
    }

    private void Awake()
    {
        uMan = UIManager.Instance;
    }

    public void MoveContainer(GameObject newParent)
    {
        Child.transform.SetParent(uMan.CurrentCanvas.transform);
        transform.SetParent(newParent.transform, false);
        if (Child.CompareTag(CombatManager.PLAYER_CARD))
            transform.SetAsFirstSibling();
        else transform.SetAsLastSibling();
        SeekChild();
    }

    public void DetachChild()
    {
        if (seekRoutine != null) return;
        Child.transform.SetParent(uMan.CurrentCanvas.transform);
    }

    public void SeekChild()
    {
        if (seekRoutine != null) return;
        seekRoutine = StartCoroutine(SeekChildNumerator());
    }

    private IEnumerator SeekChildNumerator()
    {
        float distance;
        float speed = 3;
        do
        {
            distance = Vector2.Distance(Child.transform.position, 
                transform.position);
            Child.transform.position = Vector2.MoveTowards(Child.transform.position, 
                transform.position, speed);
            yield return new WaitForEndOfFrame();
        }
        while (distance > 0);
        Child.transform.SetParent(gameObject.transform);
        seekRoutine = null;
    }
}
