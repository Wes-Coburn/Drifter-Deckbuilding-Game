using UnityEngine;
using TMPro;

public class DialogueSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject playerHeroPortrait;
    [SerializeField] private GameObject playerHeroName;
    [SerializeField] private GameObject playerHeroSpeech;

    [SerializeField] private GameObject response_1;
    [SerializeField] private GameObject response_2;
    [SerializeField] private GameObject response_3;

    [SerializeField] private GameObject otherHeroPortrait;
    [SerializeField] private GameObject otherHeroName;
    [SerializeField] private GameObject otherHeroSpeech;

    public Sprite PlayerHeroPortrait
    {
        set
        {
            playerHeroPortrait.GetComponent<SpriteRenderer>().sprite = value;
        }
    }
    public string PlayerHeroName
    {
        set
        {
            playerHeroName.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    public string PlayerHeroSpeech
    {
        set
        {
            playerHeroSpeech.GetComponent<TextMeshProUGUI>().SetText(value);
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
    public Sprite OtherHeroPortrait
    {
        set
        {
            otherHeroPortrait.GetComponent<SpriteRenderer>().sprite = value;
        }
    }
    public string OtherHeroName
    {
        set
        {
            otherHeroName.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    public string OtherHeroSpeech
    {
        set
        {
            otherHeroSpeech.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
}
