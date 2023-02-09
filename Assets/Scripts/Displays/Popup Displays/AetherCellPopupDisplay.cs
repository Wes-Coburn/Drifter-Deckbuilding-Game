using TMPro;
using UnityEngine;

public class AetherCellPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject aetherQuantity;
    [SerializeField] private GameObject aetherQuantity_Additional;
    [SerializeField] private GameObject totalAether;
    [SerializeField] private GameObject totalAether_Additional;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject[] hiddenZones;
    [SerializeField] private GameObject newAetherChest;

    public int AetherQuantity { private get; set; }
    public GameObject AetherQuantityObject { get => aetherQuantity; }
    public TextMeshProUGUI AetherQuantity_Additional { get => aetherQuantity_Additional.GetComponent<TextMeshProUGUI>(); }
    public GameObject TotalAetherObject { get => totalAether; }
    public TextMeshProUGUI TotalAether_Additional { get => totalAether_Additional.GetComponent<TextMeshProUGUI>(); }

    private void Awake()
    {
        newAetherChest.SetActive(true);
        continueButton.SetActive(false);
        foreach (GameObject go in hiddenZones) go.SetActive(false);
        aetherQuantity.GetComponent<TextMeshProUGUI>().SetText(0 + "");
        totalAether.GetComponent<TextMeshProUGUI>().SetText(Managers.P_MAN.AetherCells + "");
        GetComponent<SoundPlayer>().PlaySound(0);
        Managers.AN_MAN.CreateParticleSystem(newAetherChest, ParticleSystemHandler.ParticlesType.NewCard, 5);
    }

    public void NewAetherChest_OnClick()
    {
        newAetherChest.SetActive(false);
        continueButton.SetActive(true);
        foreach (GameObject go in hiddenZones) go.SetActive(true);
        GetComponent<SoundPlayer>().PlaySound(1);
        Managers.P_MAN.AetherCells += AetherQuantity;
        Managers.AN_MAN.CreateParticleSystem(null, ParticleSystemHandler.ParticlesType.ButtonPress, 1);
    }

    public void ContinueButton_OnClick()
    {
        Managers.U_MAN.DestroyInteractablePopup(gameObject);
        if (SceneLoader.IsActiveScene(SceneLoader.Scene.HomeBaseScene)) return;

        if (!SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene)) Managers.D_MAN.DisplayDialoguePopup();
        else if (Managers.D_MAN.EngagedHero.NextDialogueClip is CombatRewardClip crc)
        {
            Managers.D_MAN.EngagedHero.NextDialogueClip = crc.NextDialogueClip;
            SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
        }
        else Debug.LogError("NEXT CLIP IS NOT COMBAT_REWARD_CLIP!");
    }
}
