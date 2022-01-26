using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroSelectSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject screenDimmer;
    [SerializeField] private GameObject confirmHeroPortrait;
    [SerializeField] private GameObject confirmHeroName;
    [SerializeField] private GameObject confirmAugmentImage;
    [SerializeField] private GameObject confirmAugmentName;
    [SerializeField] private GameObject confirmHeroBackstory;

    [SerializeField] private GameObject selectedHero;
    [SerializeField] private GameObject skillCard_1;
    [SerializeField] private GameObject skillCard_2;
    [SerializeField] private GameObject heroPortrait;
    [SerializeField] private GameObject heroName;
    [SerializeField] private GameObject heroDescription;
    [SerializeField] private GameObject heroPowerImage;
    [SerializeField] private GameObject heroPowerDescription;
    [SerializeField] private GameObject heroPowerCost;

    [SerializeField] private GameObject selectedAugment;
    [SerializeField] private GameObject augmentName;
    [SerializeField] private GameObject augmentImage;
    [SerializeField] private GameObject augmentDescription;

    private PlayerManager pMan;
    private CombatManager coMan;
    private UIManager uMan;
    private CardManager caMan;

    private PlayerHero[] playerHeroes;
    private HeroAugment[] heroAugments;
    private GameObject currentSkill_1;
    private GameObject currentSkill_2;
    private int currentHero;
    private int currentAugment;

    private PlayerHero LoadedHero
    {
        get => playerHeroes[currentHero];
    }
    private HeroAugment LoadedAugment
    {
        get => heroAugments[currentAugment];
    }

    private void Start()
    {
        pMan = PlayerManager.Instance;
        coMan = CombatManager.Instance;
        uMan = UIManager.Instance;
        caMan = CardManager.Instance;
        playerHeroes = Resources.LoadAll<PlayerHero>("Heroes");
        heroAugments = Resources.LoadAll<HeroAugment>("Hero Augments");
        DisplayConfirmSelection();
    }

    private void DisplayConfirmSelection()
    {
        selectedAugment.SetActive(false);
        selectedHero.SetActive(false);
        currentHero = 1; // Start with Kili
        currentAugment = 1; // Start with Inertial Catalyzer
        DisplayConfirmHero();
        DisplayConfirmAugment();
    }

    private void DisplayConfirmHero()
    {
        confirmHeroName.GetComponent<TextMeshProUGUI>().SetText(LoadedHero.HeroName);
        confirmHeroPortrait.GetComponent<Image>().sprite = LoadedHero.HeroPortrait;
        confirmHeroBackstory.GetComponent<TextMeshProUGUI>().SetText(LoadedHero.HeroBackstory);
        uMan.GetPortraitPosition(LoadedHero.HeroName,
            out Vector2 position, out Vector2 scale, SceneLoader.Scene.HeroSelectScene);
        confirmHeroPortrait.transform.localPosition = position;
        confirmHeroPortrait.transform.localScale = scale;
        Sound[] sounds = LoadedHero.HeroPower.PowerSounds;
        foreach (Sound s in sounds) AudioManager.Instance.StartStopSound(null, s);
        if (selectedHero.activeSelf) DisplaySelectedHero();
    }

    private void DisplayConfirmAugment()
    {
        confirmAugmentName.GetComponent<TextMeshProUGUI>().SetText(LoadedAugment.AugmentName);
        confirmAugmentImage.GetComponent<Image>().sprite = LoadedAugment.AugmentImage;
        if (selectedAugment.activeSelf) DisplaySelectedAugment();
    }

    private void LoadSelections()
    {
        PlayerHero newPH = ScriptableObject.CreateInstance<PlayerHero>();
        newPH.LoadHero(LoadedHero);
        pMan.PlayerHero = newPH;

        foreach (UnitCard uc in caMan.PlayerStartUnits)
            for (int i = 0; i < GameManager.PLAYER_START_UNITS; i++)
                caMan.AddCard(uc, GameManager.PLAYER);
        foreach (SkillCard skill in pMan.PlayerHero.HeroStartSkills)
            for (int i = 0; i < GameManager.PLAYER_START_SKILLS; i++)
                caMan.AddCard(skill, GameManager.PLAYER);

        HeroAugment ha = LoadedAugment;
        pMan.AddAugment(ha);
    }

    public void ConfirmButton_OnClick()
    {
        LoadSelections();
        SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
    }

    public void BackButton_OnClick() =>
        GameManager.Instance.EndGame();
    
    public void ScreenDimmer_OnClick()
    {
        screenDimmer.SetActive(false);
        if (selectedHero.activeSelf)
        {
            PowerZoom pz = FindObjectOfType<PowerZoom>();
            pz.DestroyPowerPopup();
            pz.DestroyAbilityPopup();
            selectedHero.SetActive(false);
        }
        else if (selectedAugment.activeSelf)
            selectedAugment.SetActive(false);
        else Debug.LogError("ERROR!");
    }
    
    public void SelectHero_RightArrow_OnClick()
    {
        if (++currentHero > playerHeroes.Length - 1)
            currentHero = 0;
        DisplayConfirmHero();
    }
    
    public void SelectHero_LeftArrow_OnClick()
    {
        if (--currentHero < 0)
            currentHero = playerHeroes.Length - 1;
        DisplayConfirmHero();
    }
    
    public void SelectAugment_RightArrow_OnClick()
    {
        if (++currentAugment > heroAugments.Length - 1)
            currentAugment = 0;
        DisplayConfirmAugment();
    }
    
    public void SelectAugment_LeftArrow_OnClick()
    {
        if (--currentAugment < 0)
            currentAugment = heroAugments.Length - 1;
        DisplayConfirmAugment();
    }

    public void SelectedAugment_OnClick()
    {
        screenDimmer.SetActive(true);
        selectedAugment.SetActive(true);
        selectedHero.SetActive(false);
        DisplaySelectedAugment();
    }
    
    private void DisplaySelectedAugment()
    {
        augmentName.GetComponent<TextMeshProUGUI>().SetText(LoadedAugment.AugmentName);
        augmentImage.GetComponent<Image>().sprite = LoadedAugment.AugmentImage;
        augmentDescription.GetComponent<TextMeshProUGUI>().SetText(LoadedAugment.AugmentDescription);
    }
    
    public void SelectedHero_OnClick()
    {
        screenDimmer.SetActive(true);
        selectedAugment.SetActive(false);
        selectedHero.SetActive(true);
        DisplaySelectedHero();
    }
    
    private void DisplaySelectedHero()
    {
        heroName.GetComponent<TextMeshProUGUI>().SetText(LoadedHero.HeroName);
        heroPowerCost.GetComponent<TextMeshProUGUI>().SetText(LoadedHero.HeroPower.PowerCost.ToString());
        heroPortrait.GetComponent<Image>().sprite = LoadedHero.HeroPortrait;
        uMan.GetPortraitPosition(LoadedHero.HeroName,
            out Vector2 position, out Vector2 scale, SceneLoader.Scene.HeroSelectScene);
        heroPortrait.transform.localPosition = position;
        heroPortrait.transform.localScale = scale;
        heroDescription.GetComponent<TextMeshProUGUI>().SetText(LoadedHero.HeroDescription);
        heroPowerImage.GetComponent<Image>().sprite = LoadedHero.HeroPower.PowerSprite;
        heroPowerImage.GetComponentInParent<PowerZoom>().LoadedPower = LoadedHero.HeroPower;
        heroPowerDescription.GetComponent<TextMeshProUGUI>().SetText
            ("<b><u>" + LoadedHero.HeroPower.PowerName +
            ":</b></u> " + LoadedHero.HeroPower.PowerDescription);

        if (currentSkill_1 != null)
        {
            Destroy(currentSkill_1);
            currentSkill_1 = null;
        }
        if (currentSkill_2 != null)
        {
            Destroy(currentSkill_2);
            currentSkill_2 = null;
        }
        Vector2 vec2 = new Vector2();
        currentSkill_1 = coMan.ShowCard(LoadedHero.HeroStartSkills[0], vec2, CombatManager.DisplayType.HeroSelect);
        currentSkill_2 = coMan.ShowCard(LoadedHero.HeroStartSkills[1], vec2, CombatManager.DisplayType.HeroSelect);
        currentSkill_1.GetComponent<CardDisplay>().DisableVisuals();
        currentSkill_2.GetComponent<CardDisplay>().DisableVisuals();
        currentSkill_1.transform.SetParent(skillCard_1.transform, false);
        currentSkill_2.transform.SetParent(skillCard_2.transform, false);
        Vector2 scaleVec = new Vector2(4, 4);
        currentSkill_1.transform.localScale = scaleVec;
        currentSkill_2.transform.localScale = scaleVec;
    }
}
