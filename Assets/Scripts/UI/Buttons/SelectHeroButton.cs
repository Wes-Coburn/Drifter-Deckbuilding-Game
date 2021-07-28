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
        PlayerHero ph = NewGameSceneDisplay.GetComponent<NewGameSceneDisplay>().SelectedHero;
        PlayerManager.Instance.PlayerHero = ph;
        SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
    }
}
