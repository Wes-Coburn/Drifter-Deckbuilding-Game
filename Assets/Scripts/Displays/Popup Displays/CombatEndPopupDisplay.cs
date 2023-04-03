using UnityEngine;

public class CombatEndPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject victoryText;
    [SerializeField] private GameObject defeatText;

    public GameObject VictoryText { get => victoryText; }
    public GameObject DefeatText { get => defeatText; }
    
    private void Start()
    {
        GetComponent<SoundPlayer>().PlaySound(0);
        if (Managers.G_MAN.IsTutorial || Managers.G_MAN.IsCombatTest) return;

        // VICTORY
        if (victoryText.activeSelf)
        {
            if (Managers.D_MAN.EngagedHero.NextDialogueClip is CombatRewardClip crc)
            {
                Managers.G_MAN.GiveReputationRewards(crc);

                if (crc.NewNarrative != null) Managers.G_MAN.CurrentNarrative = crc.NewNarrative;

                if (crc.NewLocations != null)
                {
                    float delay = 0;
                    foreach (var newLoc in crc.NewLocations)
                    {
                        Managers.G_MAN.GetActiveLocation(newLoc.Location, newLoc.NewNpc);
                        if (newLoc.Location.IsAugmenter) continue;
                        FunctionTimer.Create(() => Managers.U_MAN.CreateLocationPopup
                        (Managers.G_MAN.GetActiveLocation(newLoc.Location), true), delay);
                        delay += 3;
                    }
                }
                else Debug.LogWarning("NEW LOCATIONS IS NULL!");
            }
            else Debug.LogError("NEXT CLIP IS NOT COMBAT REWARD CLIP!");
        }
    }

    public void ContinueButton_OnClick()
    {
        if (Managers.G_MAN.IsCombatTest)
        {
            Managers.G_MAN.EndGame();
            return;
        }

        if (Managers.G_MAN.IsTutorial)
        {
            if (victoryText.activeSelf) Managers.G_MAN.NewGame();
            else Managers.G_MAN.StartTutorialScene();
            return;
        }

        // VICTORY
        if (victoryText.activeSelf)
        {
            // First try to unlock new powers, then try to unlock a new hero
            if (Managers.G_MAN.UnlockNewPowers() || Managers.G_MAN.UnlockNewHero()) { }
            else Managers.U_MAN.CreateChooseRewardPopup();
        }
        // DEFEAT
        else SceneLoader.LoadScene(SceneLoader.Scene.TitleScene, true);

        Managers.U_MAN.DestroyInteractablePopup(gameObject);
    }
}
