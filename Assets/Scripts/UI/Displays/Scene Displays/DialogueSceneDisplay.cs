using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject playerHeroPortrait;
    [SerializeField] private GameObject playerHeroImage;
    [SerializeField] private GameObject playerHeroName;
    [SerializeField] private GameObject npcHeroPortrait;
    [SerializeField] private GameObject npcHeroImage;
    [SerializeField] private GameObject npcHeroName;
    [SerializeField] private GameObject npcHeroSpeech;
    [SerializeField] private GameObject response_1;
    [SerializeField] private GameObject response_2;
    [SerializeField] private GameObject response_3;

    [SerializeField] private GameObject response_Object_1;
    [SerializeField] private GameObject response_Object_2;
    [SerializeField] private GameObject response_Object_3;

    public GameObject PlayerHeroPortrait { get => playerHeroPortrait; }
    public Sprite PlayerHeroImage
    {
        set
        {
            playerHeroImage.GetComponent<Image>().sprite = value;
            UIManager.Instance.GetPortraitPosition
                (PlayerManager.Instance.PlayerHero.HeroName, 
                out Vector2 position, out Vector2 scale, SceneLoader.Scene.DialogueScene);
            playerHeroImage.transform.localPosition = position;
            playerHeroImage.transform.localScale = scale;
        }
    }
    public string PlayerHeroName
    {
        set
        {
            playerHeroName.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    public GameObject NPCHeroPortrait { get => npcHeroPortrait; }
    public Sprite NPCHeroImage
    {
        set
        {
            npcHeroImage.GetComponent<Image>().sprite = value;
        }
    }
    public string NPCHeroName
    {
        set
        {
            npcHeroName.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    public GameObject NPCHeroSpeechObject { get => npcHeroSpeech; }
    public string NPCHeroSpeech
    {
        set
        {
            npcHeroSpeech.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    public GameObject Response_1_Object { get => response_Object_1; }
    public string Response_1
    {
        set
        {
            response_1.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    public GameObject Response_2_Object { get => response_Object_2; }
    public string Response_2
    {
        set
        {
            response_2.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    public GameObject Response_3_Object { get => response_Object_3; }
    public string Response_3
    {
        set
        {
            response_3.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
}
