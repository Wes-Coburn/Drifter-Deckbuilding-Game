using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeBaseSceneDisplay : MonoBehaviour
{
    [Header("HERO INFO")]
    [SerializeField] private GameObject heroName;
    [SerializeField] private GameObject heroDescription;
    [SerializeField] private GameObject heroBackstory;
    [SerializeField] private GameObject heroImage;

    [Header("HERO POWER")]
    [SerializeField] private GameObject heroPower;
    [SerializeField] private GameObject heroPowerCost;
    [SerializeField] private GameObject heroPowerDescription;
    [SerializeField] private GameObject heroPowerImage;

    [Header("HERO ULTIMATE")]
    [SerializeField] private GameObject heroUltimate;
    [SerializeField] private GameObject heroUltimateCost;
    [SerializeField] private GameObject heroUltimateDescription;
    [SerializeField] private GameObject heroUltimateImage;

    [Header("CLAIM REWARD BUTTON")]
    [SerializeField] private GameObject claimRewardButton;

    private PlayerManager pMan;
    private UIManager uMan;

    private string HeroName
    {
        set
        {
            heroName.GetComponent<TextMeshProUGUI>().SetText(value);
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
    private HeroPower HeroPower
    {
        set
        {
            heroPower.GetComponent<PowerZoom>().LoadedPower = value;
        }
    }
    private int HeroPowerCost
    {
        set
        {
            heroPowerCost.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
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
    private HeroPower HeroUltimate
    {
        set
        {
            heroUltimate.GetComponent<PowerZoom>().LoadedPower = value;
        }
    }
    private int HeroUltimateCost
    {
        set
        {
            heroUltimateCost.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
        }
    }
    private string HeroUltimateDescription
    {
        set
        {
            heroUltimateDescription.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    private Sprite HeroUltimateSprite
    {
        set
        {
            heroUltimateImage.GetComponent<Image>().sprite = value;
        }
    }

    public GameObject ClaimRewardButton => claimRewardButton;

    private void Start()
    {
        pMan = PlayerManager.Instance;
        uMan = UIManager.Instance;
        PlayerHero ph = pMan.PlayerHero;
        HeroName = ph.HeroName;
        HeroDescription = ph.HeroDescription;
        HeroBackstory = ph.HeroBackstory;
        HeroSprite = ph.HeroPortrait;
        heroBackstory.SetActive(false);
        // POWER
        HeroPower = ph.HeroPower;
        HeroPowerCost = ph.HeroPower.PowerCost;
        HeroPowerDescription = "<b><u>" + ph.HeroPower.PowerName +
            ":</b></u> " + ph.HeroPower.PowerDescription;
        HeroPowerSprite = ph.HeroPower.PowerSprite;
        // ULTIMATE
        HeroUltimate = ph.HeroUltimate;
        HeroUltimateCost = ph.HeroUltimate.PowerCost;
        HeroUltimateDescription = "<b><u>" + ph.HeroUltimate.PowerName +
            " (Ultimate):</b></u> " + ph.HeroUltimate.PowerDescription;
        HeroUltimateSprite = ph.HeroUltimate.PowerSprite;
    }

    public void ShowInfoButton_OnClick() =>
        heroBackstory.SetActive(!heroBackstory.activeSelf);
    
    public void RemoveCardButton_OnClick(bool playSound = true) =>
        uMan.CreateCardPagePopup(CardPageDisplay.CardPageType.RemoveCard, playSound);

    public void ClaimRewardButton_OnClick()
    {
        claimRewardButton.GetComponent<TooltipPopupDisplay>().DestroyToolTip();
        claimRewardButton.SetActive(false);
        uMan.CreateChooseRewardPopup();
    }

    public void BackButton_OnClick()
    {
        if (claimRewardButton.activeSelf)
        {
            uMan.CreateFleetingInfoPopup("Claim Your Reward!");
            return;
        }
        
        SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
    }
}
