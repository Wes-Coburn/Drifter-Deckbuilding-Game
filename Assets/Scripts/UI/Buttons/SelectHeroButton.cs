using UnityEngine;

public class SelectHeroButton : MonoBehaviour
{
    public void OnClick()
    {
        if (CardZoom.ZoomCardIsCentered)
        {
            UIManager.Instance.DestroyZoomObjects();
            return;
        }
        gameObject.GetComponent<SoundPlayer>().PlaySound(0);
        FindObjectOfType<HeroSelectSceneDisplay>().ConfirmSelection();
    }
}
