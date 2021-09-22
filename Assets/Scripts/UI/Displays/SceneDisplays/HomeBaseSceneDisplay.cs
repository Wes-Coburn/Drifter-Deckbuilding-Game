using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeBaseSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject heroPower;
    [SerializeField] private GameObject heroName;
    [SerializeField] private GameObject heroImage;
    [SerializeField] private GameObject heroDescription;
    [SerializeField] private GameObject heroPowerDescription;
    [SerializeField] private GameObject heroPowerImage;
    
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
        }
    }
    private string HeroDescription
    {
        set
        {
            heroDescription.GetComponent<TextMeshProUGUI>().SetText(value);
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
        PlayerHero ph = PlayerManager.Instance.PlayerHero;
        HeroPower = ph.HeroPower;
        HeroName = ph.HeroName;
        HeroSprite = ph.HeroPortrait;
        HeroDescription = ph.HeroDescription;
        HeroPowerDescription = ph.HeroPower.PowerDescription;
        HeroPowerSprite = ph.HeroPower.PowerSprite;
    }

    public void CardPageButton()
    {
        UIManager.Instance.CreateCardPagePopup();
    }

    public void BackButton_OnClick()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
    }
}
