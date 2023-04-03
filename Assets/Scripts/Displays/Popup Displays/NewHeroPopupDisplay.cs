using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewHeroPopupDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject popupTitle, newHeroChest, continueButton,
        playerHero, heroPortrait, heroName, heroDescription, heroBackstory,
        heroPowerDescription, heroUltimateDescription;

    private PlayerHero newPlayerHero;
    private HeroPower newHeroPower, newHeroUltimate;

    private void Awake() => GetComponent<SoundPlayer>().PlaySound(0);

    public void DisplayNewHeroPopup(string title, PlayerHero playerHero, HeroPower heroPower, HeroPower heroUltimate)
    {
        popupTitle.GetComponent<TextMeshProUGUI>().SetText(title);
        newPlayerHero = playerHero;
        newHeroPower = heroPower;
        newHeroUltimate = heroUltimate;

        newHeroChest.SetActive(true);
        this.playerHero.SetActive(false);
        this.heroPowerDescription.SetActive(false);
        this.heroUltimateDescription.SetActive(false);

        Managers.AN_MAN.CreateParticleSystem(newHeroChest, ParticleSystemHandler.ParticlesType.NewCard, 5);
    }

    public void NewHeroChest_OnClick()
    {
        GetComponent<SoundPlayer>().PlaySound(1);
        foreach (var s in newPlayerHero.HeroPower.PowerSounds)
            Managers.AU_MAN.StartStopSound(null, s);

        newHeroChest.SetActive(false);
        Managers.AN_MAN.CreateParticleSystem(null, ParticleSystemHandler.ParticlesType.ButtonPress);

        playerHero.SetActive(true);
        heroPortrait.GetComponent<Image>().sprite = newPlayerHero.HeroPortrait;
        Managers.U_MAN.GetPortraitPosition(newPlayerHero.HeroName, out Vector2 position, out Vector2 scale);
        heroPortrait.transform.localPosition = position;
        heroPortrait.transform.localScale = scale;
        heroName.GetComponent<TextMeshProUGUI>().SetText(newPlayerHero.HeroName);
        heroDescription.GetComponent<TextMeshProUGUI>().SetText(newPlayerHero.HeroDescription);
        heroBackstory.GetComponentInChildren<TextMeshProUGUI>().SetText(newPlayerHero.HeroBackstory);
        heroBackstory.SetActive(false);

        heroPowerDescription.SetActive(true);
        heroPowerDescription.GetComponent<HeroPowerDescriptionDisplay>()
            .DisplayHeroPower(newHeroPower, false);

        heroUltimateDescription.SetActive(true);
        heroUltimateDescription.GetComponent<HeroPowerDescriptionDisplay>()
            .DisplayHeroPower(newHeroUltimate, true);
    }

    public void ContinueButton_OnClick()
    {
        GetComponent<SoundPlayer>().PlaySound(2);
        DestroyAndContinue();
    }

    public void ShowInfoButton_OnClick() => heroBackstory.SetActive(!heroBackstory.activeSelf);

    private void DestroyAndContinue()
    {
        Managers.U_MAN.DestroyInteractablePopup(gameObject);

        /*
        if (SceneLoader.IsActiveScene(SceneLoader.Scene.HomeBaseScene))
        {
            // blank
        }
        // Dialogue Scene
        else if (SceneLoader.IsActiveScene(SceneLoader.Scene.DialogueScene)) Managers.D_MAN.DisplayDialoguePopup();
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
        else Debug.LogError("INVALID SCENE!");
        */

        Managers.U_MAN.CreateChooseRewardPopup();
    }
}
