using UnityEngine;

public class DialogueResponseButton : MonoBehaviour
{
    [SerializeField] private int response;
    public void OnClick()
    {
        if (CardManager.Instance.NewCardPopup != null)
        {
            UIManager.Instance.DestroyZoomObjects();
            return;
        }
        DialogueManager.Instance.DialogueResponse(response);
        GetComponentInParent<SoundPlayer>().PlaySound(0);
    }
}
