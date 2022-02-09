using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeBaseSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject heroPower;
    [SerializeField] private GameObject heroName;
    [SerializeField] private GameObject heroImage;
    [SerializeField] private GameObject heroDescription;
    [SerializeField] private GameObject heroBackstory;
    [SerializeField] private GameObject heroPowerDescription;
    [SerializeField] private GameObject heroPowerImage;
    [SerializeField] private GameObject confirmSpendPopup;

    private PlayerManager pMan;
    private UIManager uMan;

    public HeroPower HeroPower
    {
        set
        {
            heroPower.GetComponent<PowerZoom>().LoadedPower = value;
        }
    }
    private string HeroName
    {
        set
        {
            heroName.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    private Sprite HeroSprite
    {
        set
        {
            heroImage.GetComponent<Image>().sprite = value;
            uMan.GetPortraitPosition(pMan.PlayerHero.HeroName, out Vector2 position,
                out Vector2 scale, SceneLoader.Scene.HeroSelectScene);
            heroImage.transform.localPosition = position;
            heroImage.transform.localScale = scale;
        }
    }
    private string HeroDescription
    {
        set
        {
            heroDescription.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    private string HeroBackstory
    {
        set
        {
            heroBackstory.GetComponentInChildren<TextMeshProUGUI>().SetText(value);
        }
    }
    private string HeroPowerDescription
    {
        set
        {
            heroPowerDescription.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    private Sprite HeroPowerSprite
    {
        set
        {
            heroPowerImage.GetComponent<Image>().sprite = value;
        }
    }

    private void Start()
    {
        pMan = PlayerManager.Instance;
        uMan = UIManager.Instance;
        PlayerHero ph = pMan.PlayerHero;
        HeroPower = ph.HeroPower;
        HeroName = ph.HeroName;
        HeroSprite = ph.HeroPortrait;
        HeroDescription = ph.HeroDescription;
        HeroBackstory = ph.HeroBackstory;
        HeroPowerDescription = "<b><u>" + ph.HeroPower.PowerName +
            ":</b></u> " + ph.HeroPower.PowerDescription;
        HeroPowerSprite = ph.HeroPower.PowerSprite;
        heroBackstory.SetActive(false);
    }

    public void ShowInfoButton_OnClick() =>
        heroBackstory.SetActive(!heroBackstory.activeSelf);

    public void LearnSkillButton_OnClick(bool playSound = true) =>
        uMan.CreateCardPagePopup(CardPageDisplay.CardPageType.LearnSkill, true, playSound); // TESTING

    public void RemoveCardButton_OnClick(bool playSound = true) =>
        uMan.CreateCardPagePopup(CardPageDisplay.CardPageType.RemoveCard, true, playSound); // TESTING

    public void BackButton_OnClick() => 
        SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
}
