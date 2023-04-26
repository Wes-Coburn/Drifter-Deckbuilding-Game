using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject heroPortrait, heroName, heroDescription,
        heroBackstory, heroPowerDescription, heroUltimateDescription,
        lockedHeroIcon, lockedPowerIcon, lockedUltimateIcon, confirmHeroButton, relatedCardsContainer;

    private List<PlayerHero> playerHeroes;
    private int currentHero, currentPower, currentUltimate;

    private PlayerHero LoadedHero => playerHeroes[currentHero];
    private HeroPower LoadedPower => currentPower == 0 ?
        LoadedHero.CurrentHeroPower : LoadedHero.AltHeroPowers[currentPower - 1];
    private HeroPower LoadedUltimate => currentUltimate == 0 ?
        LoadedHero.CurrentHeroUltimate : LoadedHero.AltHeroUltimates[currentUltimate - 1];
    private bool HeroIsUnlocked =>
        Managers.G_MAN.UnlockedHeroes.Contains(LoadedHero.HeroName);
    private bool PowerIsUnlocked => currentPower == 0 ||
        Managers.G_MAN.UnlockedPowers.Contains(LoadedPower.PowerName);
    private bool UltimateIsUnlocked => currentUltimate == 0 ||
        Managers.G_MAN.UnlockedPowers.Contains(LoadedUltimate.PowerName);

    private void Start()
    {
        var allHeroes = Resources.LoadAll<PlayerHero>("Heroes");
        var startingHeroes = new List<PlayerHero>();
        var unlockedHeroes = new List<PlayerHero>();
        var lockedHeroes = new List<PlayerHero>();

        foreach (var hero in allHeroes)
        {
            var newHero = ScriptableObject.CreateInstance<PlayerHero>();
            newHero.LoadHero(hero);

            if (Managers.G_MAN.StartingHeroes.Contains(newHero.HeroName)) startingHeroes.Add(newHero);
            else if (Managers.G_MAN.UnlockedHeroes.Contains(newHero.HeroName)) unlockedHeroes.Add(newHero);
            else lockedHeroes.Add(newHero);
        }

        (startingHeroes[1], startingHeroes[0]) = (startingHeroes[0], startingHeroes[1]);

        playerHeroes = startingHeroes.Concat(unlockedHeroes).Concat(lockedHeroes).ToList();
        heroBackstory.SetActive(false);
        currentPower = 0;
        currentUltimate = 0;

        DisplaySelectedHero();
    }

    private void LoadSelection()
    {
        var newPH = ScriptableObject.CreateInstance<PlayerHero>();
        newPH.LoadHero(LoadedHero);
        newPH.CurrentHeroPower = LoadedPower;
        newPH.CurrentHeroUltimate = LoadedUltimate;
        Managers.P_MAN.HeroScript = newPH;
    }

    private void DisplaySelectedHero()
    {

        bool unlocked = HeroIsUnlocked;
        lockedHeroIcon.SetActive(!unlocked);

        foreach (var s in LoadedHero.CurrentHeroPower.PowerSounds)
            Managers.AU_MAN.StartStopSound(null, s);

        if (unlocked) heroBackstory.GetComponentInChildren<TextMeshProUGUI>().SetText(LoadedHero.HeroBackstory);
        else heroBackstory.SetActive(false);
        heroName.GetComponent<TextMeshProUGUI>().SetText(unlocked ? LoadedHero.HeroName : UIManager.LOCK_TEXT);
        heroPortrait.GetComponent<Image>().sprite = LoadedHero.HeroPortrait; // Keep image revealed
        Managers.U_MAN.GetPortraitPosition(LoadedHero.HeroName, out Vector2 position, out Vector2 scale);
        heroPortrait.transform.localPosition = position;
        heroPortrait.transform.localScale = scale;
        heroDescription.GetComponent<TextMeshProUGUI>().SetText(unlocked ? LoadedHero.HeroDescription : UIManager.LOCK_TEXT);

        currentPower = 0;
        currentUltimate = 0;
        DisplaySelectedPower(false);
        DisplaySelectedUltimate();
    }
    
    public void DisplaySelectedPower(bool setRelatedCards = true)
    {
        bool unlocked = HeroIsUnlocked && PowerIsUnlocked;
        lockedPowerIcon.SetActive(!unlocked);
        bool canContinue = unlocked && UltimateIsUnlocked;
        confirmHeroButton.SetActive(canContinue);

        heroPowerDescription.GetComponent<HeroPowerDescriptionDisplay>()
            .DisplayHeroPower(LoadedPower, false, unlocked);

        if (setRelatedCards) DisplayRelatedCards();
    }

    public void DisplaySelectedUltimate()
    {
        bool unlocked = HeroIsUnlocked && UltimateIsUnlocked;
        lockedUltimateIcon.SetActive(!unlocked);
        bool canContinue = unlocked && PowerIsUnlocked;
        confirmHeroButton.SetActive(canContinue);

        heroUltimateDescription.GetComponent<HeroPowerDescriptionDisplay>()
            .DisplayHeroPower(LoadedUltimate, true, unlocked);

        DisplayRelatedCards();
    }

    private void DisplayRelatedCards()
    {
        foreach (Transform tran in relatedCardsContainer.transform)
            Destroy(tran.gameObject);

        if (!HeroIsUnlocked) return;

        List<Card> relatedCards = new();
        if (PowerIsUnlocked) AddSingles(LoadedPower.RelatedCards);
        if (UltimateIsUnlocked) AddSingles(LoadedUltimate.RelatedCards);

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

    public void ConfirmButton_OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        LoadSelection();
        SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
    }

    public void BackButton_OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        Managers.G_MAN.EndGame();
    }

    public void SelectHero_RightArrow_OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        if (++currentHero > playerHeroes.Count - 1) currentHero = 0;
        DisplaySelectedHero();
    }

    public void SelectHero_LeftArrow_OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        if (--currentHero < 0) currentHero = playerHeroes.Count - 1;
        DisplaySelectedHero();
    }

    public void ShowInfoButton_OnClick()
    {
        if (SceneLoader.SceneIsLoading || lockedHeroIcon.activeInHierarchy) return;
        heroBackstory.SetActive(!heroBackstory.activeSelf);
    }

    public void SelectPower_RightArrow_OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        if (++currentPower > LoadedHero.AltHeroPowers.Length) currentPower = 0;
        DisplaySelectedPower();
    }

    public void SelectPower_LeftArrow_OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        if (--currentPower < 0) currentPower = LoadedHero.AltHeroPowers.Length;
        DisplaySelectedPower();
    }

    public void SelectUltimate_RightArrow_OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        if (++currentUltimate > LoadedHero.AltHeroUltimates.Length) currentUltimate = 0;
        DisplaySelectedUltimate();
    }

    public void SelectUltimate_LeftArrow_OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        if (--currentUltimate < 0) currentUltimate = LoadedHero.AltHeroUltimates.Length;
        DisplaySelectedUltimate();
    }
}
