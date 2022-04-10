using UnityEngine;
using TMPro;

public class AetherCellPopupDisplay : MonoBehaviour
{
    private DialogueManager dMan;
    private AnimationManager anMan;
    private PlayerManager pMan;

    private int aetherValue;

    [SerializeField] private GameObject aetherQuantity;
    [SerializeField] private GameObject totalAether;
    [SerializeField] private GameObject continueButton;

    [SerializeField] private GameObject[] hiddenZones;
    [SerializeField] private GameObject newAetherChest;

    public int AetherQuantity
    {

        set
        {
            aetherValue = value;
            aetherQuantity.GetComponent<TextMeshProUGUI>().SetText(value + "X");
        }
    }
    public int TotalAether
    {
        set
        {
            totalAether.GetComponent<TextMeshProUGUI>().SetText("Total Aether: " + value);
        }
    }

    private void Awake()
    {
        dMan = DialogueManager.Instance;
        anMan = AnimationManager.Instance;
        pMan = PlayerManager.Instance;

        newAetherChest.SetActive(true);
        continueButton.SetActive(false);
        foreach (GameObject go in hiddenZones)
            go.SetActive(false);

        GetComponent<SoundPlayer>().PlaySound(0);
        anMan.CreateParticleSystem(newAetherChest, ParticleSystemHandler.ParticlesType.NewCard, 5); // TESTING
    }

    public void NewAetherChest_OnClick()
    {
        newAetherChest.SetActive(false);
        continueButton.SetActive(true);
        foreach (GameObject go in hiddenZones)
            go.SetActive(true);
        GetComponent<SoundPlayer>().PlaySound(1);
        PlayerManager.Instance.AetherCells += aetherValue;
        anMan.CreateParticleSystem(null, ParticleSystemHandler.ParticlesType.ButtonPress, 1); // TESTING
    }

    public void ContinueButton_OnClick()
    {
        UIManager.Instance.DestroyAetherCellPopup();
        if (!SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene)) dMan.DisplayDialoguePopup();
        else if (dMan.EngagedHero.NextDialogueClip is CombatRewardClip crc)
        {
            dMan.EngagedHero.NextDialogueClip = crc.NextDialogueClip;
            SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
        }
        else Debug.LogError("NEXT CLIP IS NOT COMBAT_REWARD_CLIP!");
    }
}
