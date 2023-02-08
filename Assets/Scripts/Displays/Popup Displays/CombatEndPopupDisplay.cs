using UnityEngine;

public class CombatEndPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject victoryText;
    [SerializeField] private GameObject defeatText;

    public GameObject VictoryText { get => victoryText; }
    public GameObject DefeatText { get => defeatText; }

    private void Awake() => GetComponent<SoundPlayer>().PlaySound(0);

    public void OnClick()
    {
        if (ManagerHandler.G_MAN.IsCombatTest)
        {
            ManagerHandler.G_MAN.EndGame();
            return;
        }

        if (ManagerHandler.G_MAN.IsTutorial)
        {
            if (victoryText.activeSelf) ManagerHandler.G_MAN.NewGame();
            else ManagerHandler.G_MAN.PlayTutorial();
            return;
        }

        if (victoryText.activeSelf)
        {
            if (ManagerHandler.D_MAN.EngagedHero.NextDialogueClip is CombatRewardClip crc)
            {
                ManagerHandler.U_MAN.CreateChooseRewardPopup();

                if (crc.NewNarrative != null) ManagerHandler.G_MAN.CurrentNarrative = crc.NewNarrative;

                if (crc.NewLocations != null)
                {
                    float delay = 0;
                    foreach (NewLocation newLoc in crc.NewLocations)
                    {
                        ManagerHandler.G_MAN.GetActiveLocation(newLoc.Location, newLoc.NewNpc);
                        if (newLoc.Location.IsAugmenter) continue;
                        FunctionTimer.Create(() => ManagerHandler.U_MAN.CreateLocationPopup
                        (ManagerHandler.G_MAN.GetActiveLocation(newLoc.Location), true), delay);
                        delay += 3;
                    }
                }
                else Debug.LogWarning("NEW LOCATIONS IS NULL!");

                ManagerHandler.D_MAN.ChangeReputations(crc);
            }
            else Debug.LogError("NEXT CLIP IS NOT COMBAT REWARD CLIP!");
        }
        else
        {
            SceneLoader.LoadAction += () => ManagerHandler.G_MAN.LoadGame();
            SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene, true);
        }

        ManagerHandler.U_MAN.DestroyInteractablePopup(gameObject);
    }
}
