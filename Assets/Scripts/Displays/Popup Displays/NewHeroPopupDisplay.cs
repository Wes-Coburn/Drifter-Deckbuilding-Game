using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewHeroPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupTitle;
    [SerializeField] private GameObject newHeroChest;
    [SerializeField] private GameObject continueButton;

    [SerializeField] private GameObject playerHero;
    [SerializeField] private GameObject heroPortrait;
    [SerializeField] private GameObject heroName;
    [SerializeField] private GameObject heroDescription;
    [SerializeField] private GameObject heroBackstory;

    [SerializeField] private GameObject heroPower;
    [SerializeField] private GameObject heroPowerImage;
    [SerializeField] private GameObject heroPowerDescription;
    [SerializeField] private GameObject heroPowerCost;

    [SerializeField] private GameObject heroUltimate;
    [SerializeField] private GameObject heroUltimateImage;
    [SerializeField] private GameObject heroUltimateDescription;
    [SerializeField] private GameObject heroUltimateCost;

    private PlayerHero newPlayerHero;
    private HeroPower newHeroPower;
    private HeroPower newHeroUltimate;

    private void Awake()
    {
        GetComponent<SoundPlayer>().PlaySound(0);
    }

    public void DisplayNewHeroPopup(string title, PlayerHero playerHero, HeroPower heroPower, HeroPower heroUltimate)
    {
        popupTitle.GetComponent<TextMeshProUGUI>().SetText(title);
        newPlayerHero = playerHero;
        newHeroPower = heroPower;
        newHeroUltimate = heroUltimate;

        newHeroChest.SetActive(true);
        this.playerHero.SetActive(false);
        this.heroPower.SetActive(false);
        this.heroUltimate.SetActive(false);

        Managers.AN_MAN.CreateParticleSystem(newHeroChest, ParticleSystemHandler.ParticlesType.NewCard, 5);
    }

    public void NewHeroChest_OnClick()
    {
        GetComponent<SoundPlayer>().PlaySound(1);
        newHeroChest.SetActive(false);
        Managers.AN_MAN.CreateParticleSystem(null, ParticleSystemHandler.ParticlesType.ButtonPress);

        playerHero.SetActive(true);
        heroPortrait.GetComponent<Image>().sprite = newPlayerHero.HeroPortrait;
        heroName.GetComponent<TextMeshProUGUI>().SetText(newPlayerHero.HeroName);
        heroDescription.GetComponent<TextMeshProUGUI>().SetText(newPlayerHero.HeroDescription);
        heroBackstory.GetComponentInChildren<TextMeshProUGUI>().SetText(newPlayerHero.HeroBackstory);

        if (newHeroPower != null)
        {
            heroPower.SetActive(true);

            heroPower.GetComponent<PowerZoom>().LoadedPower = newHeroPower;
            heroPowerCost.GetComponent<TextMeshProUGUI>().SetText(newHeroPower.PowerCost.ToString());
            heroPowerImage.GetComponent<Image>().sprite = newHeroPower.PowerSprite;
        }
        if (newHeroUltimate != null)
        {
            heroUltimate.SetActive(true);

            heroUltimate.GetComponent<PowerZoom>().LoadedPower = newHeroPower;
            heroUltimate.GetComponent<TextMeshProUGUI>().SetText(newHeroPower.PowerCost.ToString());
            heroUltimate.GetComponent<Image>().sprite = newHeroPower.PowerSprite;
        }
    }

    public void ContinueButton_OnClick()
    {
        GetComponent<SoundPlayer>().PlaySound(2);
        DestroyAndContinue();
    }

    private void DestroyAndContinue()
    {
        Managers.U_MAN.DestroyInteractablePopup(gameObject);

        if (SceneLoader.IsActiveScene(SceneLoader.Scene.HomeBaseScene))
        {
            // blank
        }
        // Dialogue Scene
        else if (SceneLoader.IsActiveScene(SceneLoader.Scene.DialogueScene)) Managers.D_MAN.DisplayDialoguePopup();
        /*
        // Combat Scene
        else if (SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene))
        {
            if (Managers.D_MAN.EngagedHero.NextDialogueClip is CombatRewardClip crc)
            {
                Managers.D_MAN.EngagedHero.NextDialogueClip = crc.NextDialogueClip;
                SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
            }
            else Debug.LogError("NEXT CLIP IS NOT COMBAT_REWARD_CLIP!");
        }
        */
        else Debug.LogError("INVALID SCENE!");
    }
}
