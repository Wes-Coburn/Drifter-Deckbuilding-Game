using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroSelectSceneDisplay : MonoBehaviour
{
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

    private PlayerManager pMan;
    private CombatManager coMan;
    private UIManager uMan;
    private CardManager caMan;

    private PlayerHero[] playerHeroes;
    private int currentHero;

    private PlayerHero LoadedHero
    {
        get => playerHeroes[currentHero];
    }

    private void Start()
    {
        pMan = PlayerManager.Instance;
        coMan = CombatManager.Instance;
        uMan = UIManager.Instance;
        caMan = CardManager.Instance;
        playerHeroes = Resources.LoadAll<PlayerHero>("Heroes");
        heroBackstory.SetActive(false);
        currentHero = 2; // Start with Kili
    }

    private void LoadSelection()
    {
        PlayerHero newPH = ScriptableObject.CreateInstance<PlayerHero>();
        newPH.LoadHero(LoadedHero);
        pMan.PlayerHero = newPH;

        foreach (UnitCard uc in caMan.PlayerStartUnits)
            for (int i = 0; i < GameManager.PLAYER_START_UNITS; i++)
                caMan.AddCard(uc, GameManager.PLAYER);
    }

    public void ConfirmButton_OnClick()
    {
        LoadSelection();
        SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
    }

    public void BackButton_OnClick() =>
        GameManager.Instance.EndGame();
    
    public void SelectHero_RightArrow_OnClick()
    {
        if (++currentHero > playerHeroes.Length - 1)
            currentHero = 0;
        DisplaySelectedHero();
    }
    
    public void SelectHero_LeftArrow_OnClick()
    {
        if (--currentHero < 0)
            currentHero = playerHeroes.Length - 1;
        DisplaySelectedHero();
    }

    public void SelectedHero_OnClick() => heroBackstory.SetActive(!heroBackstory.activeSelf);

    public void DisplaySelectedHero()
    {
        foreach (Sound s in LoadedHero.HeroPower.PowerSounds)
            AudioManager.Instance.StartStopSound(null, s);

        heroBackstory.GetComponentInChildren<TextMeshProUGUI>().SetText(LoadedHero.HeroBackstory);
        heroName.GetComponent<TextMeshProUGUI>().SetText(LoadedHero.HeroName);
        heroPortrait.GetComponent<Image>().sprite = LoadedHero.HeroPortrait;
        uMan.GetPortraitPosition(LoadedHero.HeroName,
            out Vector2 position, out Vector2 scale, SceneLoader.Scene.HeroSelectScene);
        heroPortrait.transform.localPosition = position;
        heroPortrait.transform.localScale = scale;
        heroDescription.GetComponent<TextMeshProUGUI>().SetText(LoadedHero.HeroDescription);

        // POWER
        heroPower.GetComponent<PowerZoom>().LoadedPower = LoadedHero.HeroPower;
        heroPowerCost.GetComponent<TextMeshProUGUI>().SetText(LoadedHero.HeroPower.PowerCost.ToString());
        heroPowerImage.GetComponent<Image>().sprite = LoadedHero.HeroPower.PowerSprite;
        heroPowerDescription.GetComponent<TextMeshProUGUI>().SetText
            ("<b><u>" + LoadedHero.HeroPower.PowerName +
            ":</b></u> " + caMan.FilterKeywords(LoadedHero.HeroPower.PowerDescription));
        
        // ULTIMATE
        heroUltimate.GetComponent<PowerZoom>().LoadedPower = LoadedHero.HeroUltimate;
        heroUltimateCost.GetComponent<TextMeshProUGUI>().SetText(LoadedHero.HeroUltimate.PowerCost.ToString());
        heroUltimateImage.GetComponent<Image>().sprite = LoadedHero.HeroUltimate.PowerSprite;
        heroUltimateDescription.GetComponent<TextMeshProUGUI>().SetText
            ("<b><u>" + LoadedHero.HeroUltimate.PowerName +
            " (Ultimate):</b></u> " + caMan.FilterKeywords(LoadedHero.HeroUltimate.PowerDescription));
    }
}
