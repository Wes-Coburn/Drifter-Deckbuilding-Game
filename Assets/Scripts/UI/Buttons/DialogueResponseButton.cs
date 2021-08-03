using UnityEngine;

public class DialogueResponseButton : MonoBehaviour
{
    [SerializeField] private int response;
    public void OnClick() => DialogueManager.Instance.DialogueResponse(response);
}
