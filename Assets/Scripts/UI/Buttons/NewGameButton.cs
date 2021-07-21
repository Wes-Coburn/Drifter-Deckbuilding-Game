using UnityEngine;
public class NewGameButton : MonoBehaviour
{
    public void OnClick() => SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
}
