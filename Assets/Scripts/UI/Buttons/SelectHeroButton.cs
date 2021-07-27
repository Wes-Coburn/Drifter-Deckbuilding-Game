using UnityEngine;

public class SelectHeroButton : MonoBehaviour
{
    [SerializeField] private GameObject NewGameSceneDisplay;
    public void OnClick()
    {
        PlayerHero ph = NewGameSceneDisplay.GetComponent<NewGameSceneDisplay>().SelectedHero;
        PlayerManager.Instance.PlayerHero = ph;
        SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
    }
}
