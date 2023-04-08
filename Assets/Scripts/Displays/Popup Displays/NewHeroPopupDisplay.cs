using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewHeroPopupDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject popupTitle, newHeroChest, continueButton,
        playerHero, heroPortrait, heroName, heroDescription, heroBackstory,
        heroPowerDescription, heroUltimateDescription, relatedCardsContainer;

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
        heroPowerDescription.SetActive(false);
        heroUltimateDescription.SetActive(false);

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

        DisplayRelatedCards();
    }

    private void DisplayRelatedCards()
    {
        List<Card> relatedCards = new();
        AddSingles(newHeroPower.RelatedCards);
        AddSingles(newHeroUltimate.RelatedCards);

        foreach (var card in relatedCards)
        {
            var cardPageCard = Managers.CA_MAN.ShowCard(card, new Vector2(), CardManager.DisplayType.Cardpage);
            var cd = cardPageCard.GetComponent<CardDisplay>();
            cd.DisableVisuals();
            cardPageCard.transform.localScale = new Vector2(1.5f, 1.5f);
            cardPageCard.transform.SetParent(relatedCardsContainer.transform, false);
        }

        Vector2 relatedCardsPos = relatedCardsContainer.transform.localPosition;
        if (relatedCards.Count > 4) relatedCardsContainer.transform.localPosition =
                new Vector2(relatedCardsPos.x, -115);

        void AddSingles(List<Card> cards)
        {
            foreach (var card in cards) AddSingle(card);
        }
        void AddSingle(Card card)
        {
            if (relatedCards.FindIndex(x => card.CardName == x.CardName) == -1)
                relatedCards.Add(card);
        }
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
