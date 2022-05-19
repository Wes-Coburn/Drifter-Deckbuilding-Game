using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroSelectSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject skillCard_1;
    [SerializeField] private GameObject skillCard_2;
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
    private GameObject currentSkill_1;
    private GameObject currentSkill_2;
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
        currentHero = 1; // Start with Kili
    }

    private void LoadSelection()
    {
        PlayerHero newPH = ScriptableObject.CreateInstance<PlayerHero>();
        newPH.LoadHero(LoadedHero);
        pMan.PlayerHero = newPH;

        foreach (UnitCard uc in caMan.PlayerStartUnits)
            for (int i = 0; i < GameManager.PLAYER_START_UNITS; i++)
                caMan.AddCard(uc, GameManager.PLAYER);
        foreach (SkillCard skill in pMan.PlayerHero.HeroSkills)
            for (int i = 0; i < GameManager.PLAYER_START_SKILLS; i++)
                caMan.AddCard(skill, GameManager.PLAYER);
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

        List<SkillCard> skills = new List<SkillCard>();
        foreach (SkillCard sc in LoadedHero.HeroSkills)
            skills.Add(sc);

        int skill_1 = Random.Range(0, skills.Count);
        SkillCard card_1 = skills[skill_1];
        skills.RemoveAt(skill_1);
        int skill_2 = Random.Range(0, skills.Count);
        SkillCard card_2 = skills[skill_2];

        currentSkill_1 = coMan.ShowCard(card_1, vec2, CombatManager.DisplayType.HeroSelect);
        currentSkill_2 = coMan.ShowCard(card_2, vec2, CombatManager.DisplayType.HeroSelect);
        currentSkill_1.GetComponent<CardDisplay>().DisableVisuals();
        currentSkill_2.GetComponent<CardDisplay>().DisableVisuals();
        currentSkill_1.transform.SetParent(skillCard_1.transform, false);
        currentSkill_2.transform.SetParent(skillCard_2.transform, false);
        Vector2 scaleVec = new Vector2(4, 4);
        currentSkill_1.transform.localScale = scaleVec;
        currentSkill_2.transform.localScale = scaleVec;
    }
}
