using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroSelectSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject sceneTitle;
    [SerializeField] private GameObject selectedHero;
    [SerializeField] private GameObject heroSkills;
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

    [SerializeField] private GameObject confirmSelection;
    [SerializeField] private GameObject confirmHeroPortrait;
    [SerializeField] private GameObject confirmHeroName;
    [SerializeField] private GameObject confirmAugmentImage;
    [SerializeField] private GameObject confirmAugmentName;

    private PlayerHero[] playerHeroes;
    private HeroAugment[] heroAugments;

    private PlayerManager pMan;
    private CombatManager coMan;
    private UIManager uMan;
    private CardManager caMan;
    private GameObject currentSkill_1;
    private GameObject currentSkill_2;
    private int startSelection;
    private int currentSelection;
    private bool heroSelected;
    private bool augmentSelected;

    private PlayerHero loadedHero;
    private HeroAugment loadedAugment;

    private string SceneTitle
    {
        set
        {
            sceneTitle.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    private void Start()
    {
        pMan = PlayerManager.Instance;
        coMan = CombatManager.Instance;
        uMan = UIManager.Instance;
        caMan = CardManager.Instance;
        startSelection = 1; // Start with Kili
        currentSelection = startSelection;
        heroSelected = false;
        augmentSelected = false;
        playerHeroes = Resources.LoadAll<PlayerHero>("Heroes");
        heroAugments = Resources.LoadAll<HeroAugment>("Hero Augments");
    }

    private void LoadSelections()
    {
        PlayerHero newPH = ScriptableObject.CreateInstance<PlayerHero>();
        newPH.LoadHero(loadedHero);
        pMan.PlayerHero = newPH;

        foreach (UnitCard uc in caMan.PlayerStartUnits)
            for (int i = 0; i < GameManager.PLAYER_START_UNITS; i++)
                caMan.AddCard(uc, GameManager.PLAYER);
        foreach (SkillCard skill in pMan.PlayerHero.HeroStartSkills)
            for (int i = 0; i < GameManager.PLAYER_START_SKILLS; i++)
                caMan.AddCard(skill, GameManager.PLAYER);

        HeroAugment ha = loadedAugment;
        pMan.AddAugment(ha);
    }

    public void ConfirmButton_OnClick()
    {
        if (!heroSelected)
        {
            heroSelected = true;
            currentSelection = startSelection;
            DisplaySelectedAugment();
        }
        else if (!augmentSelected)
        {
            augmentSelected = true;
            DisplayConfirmSelection();
        }
        else
        {
            LoadSelections();
            GameManager.Instance.NextNarrative = pMan.PlayerHero.HeroBackstory;
            SceneLoader.LoadScene(SceneLoader.Scene.NarrativeScene);
        }
    }
    public void BackButton_OnClick()
    {
        if (augmentSelected)
        {
            augmentSelected = false;
            currentSelection = startSelection;
            DisplaySelectedAugment();
        }
        else if (heroSelected)
        {
            heroSelected = false;
            currentSelection = startSelection;
            DisplaySelectedHero();
        }
        else GameManager.Instance.EndGame();
    }
    public void RightArrow_OnClick() => NextSelection(RightOrLeft.Right);
    public void LeftArrow_OnClick() => NextSelection(RightOrLeft.Left);
    public enum RightOrLeft { Right, Left }
    private void NextSelection(RightOrLeft rol)
    {
        int lastSelection;
        if (!heroSelected) lastSelection = playerHeroes.Length - 1;
        else lastSelection = heroAugments.Length - 1;
        
        if (rol == RightOrLeft.Right)
        { if (++currentSelection > lastSelection) currentSelection = 0; }
        else
        { if (--currentSelection < 0) currentSelection = lastSelection; }
        
        if (!heroSelected) DisplaySelectedHero();
        else DisplaySelectedAugment();
    }

    private void DisplaySelectedAugment()
    {
        SceneTitle = "Choose Your Augment!";
        loadedAugment = heroAugments[currentSelection];
        confirmSelection.SetActive(false);
        selectedAugment.SetActive(true);
        selectedHero.SetActive(false);
        heroSkills.SetActive(false);
        augmentName.GetComponent<TextMeshProUGUI>().SetText(loadedAugment.AugmentName);
        augmentImage.GetComponent<Image>().sprite = loadedAugment.AugmentImage;
        augmentDescription.GetComponent<TextMeshProUGUI>().SetText(loadedAugment.AugmentDescription);
    }

    public void DisplaySelectedHero()
    {
        SceneTitle = "Choose Your Hero!";
        loadedHero = playerHeroes[currentSelection];
        confirmSelection.SetActive(false);
        selectedAugment.SetActive(false);
        selectedHero.SetActive(true);
        heroSkills.SetActive(true);
        heroName.GetComponent<TextMeshProUGUI>().SetText(loadedHero.HeroName);
        heroPortrait.GetComponent<Image>().sprite = loadedHero.HeroPortrait;
        uMan.GetPortraitPosition(loadedHero.HeroName, 
            out Vector2 position, out Vector2 scale, SceneLoader.Scene.HeroSelectScene);
        heroPortrait.transform.localPosition = position;
        heroPortrait.transform.localScale = scale;

        heroDescription.GetComponent<TextMeshProUGUI>().SetText(loadedHero.HeroDescription);
        heroPowerImage.GetComponent<Image>().sprite = loadedHero.HeroPower.PowerSprite;
        heroPowerImage.GetComponentInParent<PowerZoom>().LoadedPower = loadedHero.HeroPower;

        Sound[] sounds = loadedHero.HeroPower.PowerSounds;
        foreach (Sound s in sounds) AudioManager.Instance.StartStopSound(null, s);
        
        heroPowerDescription.GetComponent<TextMeshProUGUI>().SetText
            ("<b><u>" + loadedHero.HeroPower.PowerName + 
            ":</b></u> " + loadedHero.HeroPower.PowerDescription);

        heroPowerCost.GetComponent<TextMeshProUGUI>().SetText(loadedHero.HeroPower.PowerCost.ToString()); // TESTING

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
        currentSkill_1 = coMan.ShowCard(loadedHero.HeroStartSkills[0], vec2);
        currentSkill_2 = coMan.ShowCard(loadedHero.HeroStartSkills[1], vec2);
        currentSkill_1.GetComponent<CardDisplay>().DisableVisuals();
        currentSkill_2.GetComponent<CardDisplay>().DisableVisuals();

        currentSkill_1.transform.SetParent(skillCard_1.transform, false);
        currentSkill_2.transform.SetParent(skillCard_2.transform, false);
        Vector2 scaleVec = new Vector2(4, 4);
        currentSkill_1.transform.localScale = scaleVec;
        currentSkill_2.transform.localScale = scaleVec;
    }

    private void DisplayConfirmSelection()
    {
        SceneTitle = "Confirm Your Selection!";
        confirmSelection.SetActive(true);
        selectedAugment.SetActive(false);
        selectedHero.SetActive(false);
        heroSkills.SetActive(false);

        confirmHeroName.GetComponent<TextMeshProUGUI>().SetText(loadedHero.HeroName);
        confirmHeroPortrait.GetComponent<Image>().sprite = loadedHero.HeroPortrait;
        uMan.GetPortraitPosition(loadedHero.HeroName,
            out Vector2 position, out Vector2 scale, SceneLoader.Scene.HeroSelectScene);
        confirmHeroPortrait.transform.localPosition = position;
        confirmHeroPortrait.transform.localScale = scale;

        confirmAugmentName.GetComponent<TextMeshProUGUI>().SetText(loadedAugment.AugmentName);
        confirmAugmentImage.GetComponent<Image>().sprite = loadedAugment.AugmentImage;
    }
}
