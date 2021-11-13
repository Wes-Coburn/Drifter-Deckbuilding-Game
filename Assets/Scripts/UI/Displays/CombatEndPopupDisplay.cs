using UnityEngine;

public class CombatEndPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject victoryText;
    [SerializeField] private GameObject defeatText;

    public GameObject VictoryText { get => victoryText; }
    public GameObject DefeatText { get => defeatText; }

    public void OnClick()
    {
        GameManager gMan = GameManager.Instance;
        DialogueManager dMan = DialogueManager.Instance;
        UIManager uMan = UIManager.Instance;

        if (gMan.IsCombatTest)
        {
            gMan.EndGame();
            return;
        }

        if (victoryText.activeSelf == true)
        {
            if (dMan.EngagedHero.NextDialogueClip is CombatRewardClip)
                uMan.CreateNewCardPopup(null, CardManager.Instance.ChooseCards()); // TESTING
            else Debug.LogError("NEXT CLIP IS NOT COMBAT REWARD CLIP!");
        }
        else SceneLoader.LoadScene(SceneLoader.Scene.CombatScene, true);
        uMan.DestroyCombatEndPopup();
    }
}
