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

        if (victoryText.activeSelf)
        {
            if (Managers.D_MAN.EngagedHero.NextDialogueClip is CombatRewardClip crc)
            {
                Managers.U_MAN.CreateChooseRewardPopup();

                if (crc.NewNarrative != null) Managers.G_MAN.CurrentNarrative = crc.NewNarrative;

                if (crc.NewLocations != null)
                {
                    float delay = 0;
                    foreach (NewLocation newLoc in crc.NewLocations)
                    {
                        Managers.G_MAN.GetActiveLocation(newLoc.Location, newLoc.NewNpc);
                        if (newLoc.Location.IsAugmenter) continue;
                        FunctionTimer.Create(() => Managers.U_MAN.CreateLocationPopup
                        (Managers.G_MAN.GetActiveLocation(newLoc.Location), true), delay);
                        delay += 3;
                    }
                }
                else Debug.LogWarning("NEW LOCATIONS IS NULL!");

                Managers.D_MAN.ChangeReputations(crc);
            }
            else Debug.LogError("NEXT CLIP IS NOT COMBAT REWARD CLIP!");
        }
        else
        {
            SceneLoader.LoadAction += () => GameLoader.LoadGame();
            SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene, true);
        }
        Managers.U_MAN.DestroyInteractablePopup(gameObject);
    }
}
