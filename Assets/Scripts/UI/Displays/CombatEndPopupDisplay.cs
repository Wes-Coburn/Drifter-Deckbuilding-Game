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
                uMan.CreateNewCardPopup(null, CardManager.Instance.ChooseCards());
            else Debug.LogError("NEXT CLIP IS NOT COMBAT REWARD CLIP!");
        }
        else
        {
            gMan.LoadGame();
            SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene, true);
        }
        uMan.DestroyCombatEndPopup();
    }
}
