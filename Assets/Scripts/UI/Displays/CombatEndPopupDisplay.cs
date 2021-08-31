using UnityEngine;

public class CombatEndPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject victoryText;
    [SerializeField] private GameObject defeatText;

    public GameObject VictoryText { get => victoryText; }
    public GameObject DefeatText { get => defeatText; }

    public void OnClick()
    {
        GameManager gm = GameManager.Instance;
        if (victoryText.activeSelf == true)
        {
            if (gm.IsCombatTest) gm.EndGame();
            else
            {
                // Show rewards, new card popup
                SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
            }
        }
        else gm.EndGame();
    }
}
