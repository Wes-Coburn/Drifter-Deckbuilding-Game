using UnityEngine;

public class SelectHeroButton : MonoBehaviour
{
    [SerializeField] private GameObject NewGameSceneDisplay;
    public void OnClick()
    {
        if (CardZoom.ZoomCardIsCentered)
        {
            UIManager.Instance.DestroyZoomObjects();
            return;
        }
        gameObject.GetComponent<SoundPlayer>().PlaySound(0);
        NewGameSceneDisplay.GetComponent<NewGameSceneDisplay>().ConfirmSelection();
    }
}
