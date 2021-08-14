using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject playerHeroPortrait;
    [SerializeField] private GameObject playerHeroName;
    [SerializeField] private GameObject npcHeroPortrait;
    [SerializeField] private GameObject npcHeroName;
    [SerializeField] private GameObject npcHeroSpeech;
    [SerializeField] private GameObject response_1;
    [SerializeField] private GameObject response_2;
    [SerializeField] private GameObject response_3;

    public Sprite PlayerHeroPortrait
    {
        set
        {
            playerHeroPortrait.GetComponent<Image>().sprite = value;
        }
    }
    public string PlayerHeroName
    {
        set
        {
            playerHeroName.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    public Sprite NPCHeroPortrait
    {
        set
        {
            npcHeroPortrait.GetComponent<Image>().sprite = value;
        }
    }
    public string NPCHeroName
    {
        set
        {
            npcHeroName.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    public string NPCHeroSpeech
    {
        set
        {
            npcHeroSpeech.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    public string Response_1
    {
        set
        {
            response_1.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    public string Response_2
    {
        set
        {
            response_2.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    public string Response_3
    {
        set
        {
            response_3.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
}
