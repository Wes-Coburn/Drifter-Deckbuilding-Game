using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewGameSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject selectedHero;
    [SerializeField] private GameObject skillCard_1;
    [SerializeField] private GameObject skillCard_2;
    [SerializeField] private GameObject heroPortrait;
    [SerializeField] private GameObject heroName;
    [SerializeField] private GameObject heroDescription;
    [SerializeField] private GameObject heroPowerImage;
    [SerializeField] private GameObject heroPowerDescription;

    [SerializeField] private GameObject selectedAugment;
    [SerializeField] private GameObject augmentName;
    [SerializeField] private GameObject augmentImage;
    [SerializeField] private GameObject augmentDescription;

    private GameObject currentSkill_1;
    private GameObject currentSkill_2;

    [SerializeField] private PlayerHero[] playerHeroes;
    [SerializeField] private HeroAugment[] heroAugments;
    private int currentSelection;
    private bool heroSelected;
    public PlayerHero SelectedHero { get => playerHeroes[currentSelection]; }
    public HeroAugment SelectedAugment { get => heroAugments[currentSelection]; }

    private void Start()
    {
        currentSelection = 0;
        heroSelected = false;
        DisplaySelectedHero();
    }

    public void SelectRightArrow() => NextSelection(RightOrLeft.Right);
    public void SelectLeftArrow() => NextSelection(RightOrLeft.Left);
    public void ConfirmSelection()
    {
        PlayerManager pm = PlayerManager.Instance;
        if (!heroSelected)
        {
            PlayerHero ph = SelectedHero;
            pm.PlayerHero = ph;
            heroSelected = true;
            currentSelection = 0;
            DisplaySelectedAugment();
        }
        else
        {
            HeroAugment ha = SelectedAugment;
            pm.HeroAugments.Add(ha);
            SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
        }
    }

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
        selectedAugment.SetActive(true);
        selectedHero.SetActive(false);
        skillCard_1.SetActive(false);
        skillCard_2.SetActive(false);

        augmentName.GetComponent<TextMeshProUGUI>().SetText(SelectedAugment.AugmentName);
        augmentImage.GetComponent<Image>().sprite = SelectedAugment.AugmentImage;
        augmentDescription.GetComponent<TextMeshProUGUI>().SetText(SelectedAugment.AugmentDescription);
    }

    private void DisplaySelectedHero()
    {
        selectedHero.SetActive(true);
        selectedAugment.SetActive(false);

        heroName.GetComponent<TextMeshProUGUI>().SetText(SelectedHero.HeroName);
        heroPortrait.GetComponent<Image>().sprite = SelectedHero.HeroPortrait;
        heroDescription.GetComponent<TextMeshProUGUI>().SetText(SelectedHero.HeroDescription);
        heroPowerImage.GetComponent<Image>().sprite = SelectedHero.HeroPower.PowerSprite;

        int cost = SelectedHero.HeroPower.PowerCost;
        string actions;
        if (cost > 1) actions = "actions";
        else actions = "action";
        string description = " (" + cost + " " + actions + ", 1/turn): ";

        heroPowerDescription.GetComponent<TextMeshProUGUI>().SetText(SelectedHero.HeroPower.PowerName +
            description + SelectedHero.HeroPower.PowerDescription);

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

        currentSkill_1 = CardManager.Instance.ShowCard(SelectedHero.HeroSkills[0]);
        currentSkill_2 = CardManager.Instance.ShowCard(SelectedHero.HeroSkills[1]);
        currentSkill_1.transform.SetParent(skillCard_1.transform, false);
        currentSkill_2.transform.SetParent(skillCard_2.transform, false);
        Vector2 vec2 = new Vector2(4, 4);
        currentSkill_1.transform.localScale = vec2;
        currentSkill_2.transform.localScale = vec2;
    }
}
