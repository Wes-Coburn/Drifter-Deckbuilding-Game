using UnityEngine;
using TMPro;

public class AetherCellPopupDisplay : MonoBehaviour
{
    private DialogueManager dMan;
    private AnimationManager anMan;
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
            aetherQuantity.GetComponent<TextMeshProUGUI>().SetText(value + "x");
        }
    }
    public int TotalAether
    {
        set
        {
            totalAether.GetComponent<TextMeshProUGUI>().SetText("<u>Total Aether</u>\n" + value);
        }
    }

    private void Awake()
    {
        dMan = DialogueManager.Instance;
        anMan = AnimationManager.Instance;

        newAetherChest.SetActive(true);
        continueButton.SetActive(false);
        foreach (GameObject go in hiddenZones)
            go.SetActive(false);

        GetComponent<SoundPlayer>().PlaySound(0);
        anMan.CreateParticleSystem(newAetherChest, ParticleSystemHandler.ParticlesType.NewCard, 5);
    }

    public void NewAetherChest_OnClick()
    {
        newAetherChest.SetActive(false);
        continueButton.SetActive(true);
        foreach (GameObject go in hiddenZones)
            go.SetActive(true);
        GetComponent<SoundPlayer>().PlaySound(1);
        PlayerManager.Instance.AetherCells += aetherValue;
        anMan.CreateParticleSystem(null, ParticleSystemHandler.ParticlesType.ButtonPress, 1);
    }

    public void ContinueButton_OnClick()
    {
        UIManager.Instance.DestroyInteractablePopup(gameObject);

        if (SceneLoader.IsActiveScene(SceneLoader.Scene.HomeBaseScene)) return;
        
        if (!SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene)) dMan.DisplayDialoguePopup();
        else if (dMan.EngagedHero.NextDialogueClip is CombatRewardClip crc)
        {
            dMan.EngagedHero.NextDialogueClip = crc.NextDialogueClip;
            SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
        }
        else Debug.LogError("NEXT CLIP IS NOT COMBAT_REWARD_CLIP!");
    }
}
