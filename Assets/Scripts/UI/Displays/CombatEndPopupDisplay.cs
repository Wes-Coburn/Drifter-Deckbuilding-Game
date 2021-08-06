using UnityEngine;

public class CombatEndPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject victoryText;
    [SerializeField] private GameObject defeatText;

    public GameObject VictoryText { get => victoryText; }
    public GameObject DefeatText { get => defeatText; }

    public void OnClick()
    {
        if (victoryText.activeSelf) SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
        else SceneLoader.LoadScene(SceneLoader.Scene.TitleScene);
    }
}
