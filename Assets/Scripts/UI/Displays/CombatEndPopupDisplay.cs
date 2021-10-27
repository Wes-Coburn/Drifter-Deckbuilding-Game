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
        if (victoryText.activeSelf == true)
        {
            if (gMan.IsCombatTest) gMan.EndGame();
            else
            {
                if (dMan.EngagedHero.NextDialogueClip is CombatRewardClip crc)
                {
                    if (crc.NewCard != null)
                        CardManager.Instance.AddCard(crc.NewCard, GameManager.PLAYER, false);
                    else if (crc.AetherCells > 0)
                    {
                        int newAether = crc.AetherCells;
                        int newTotal = newAether + PlayerManager.Instance.AetherCells;
                        UIManager.Instance.CreateAetherCellPopup(newAether, newTotal);
                    }
                    else
                    {
                        dMan.EngagedHero.NextDialogueClip = crc.NextDialogueClip;
                        SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
                        CombatManager.Instance.IsInCombat = false;
                    }
                }
                else Debug.LogError("NEXT CLIP IS NOT COMBAT_REWARD_CLIP!");
            }
        }
        else SceneLoader.LoadScene(SceneLoader.Scene.CombatScene, true); // TESTING
        UIManager.Instance.DestroyCombatEndPopup();
    }
}
