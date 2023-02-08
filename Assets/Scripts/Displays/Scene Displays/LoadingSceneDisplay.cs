using TMPro;
using UnityEngine;

public class LoadingSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject tipText;
    [SerializeField] private GameObject chapterText;

    public string TipText
    {
        set
        {
            tipText.GetComponent<TextMeshProUGUI>().SetText(ManagerHandler.CA_MAN.FilterKeywords(value));
        }
    }
    public string ChapterText
    {
        set
        {
            string bookend = " --- ";
            chapterText.GetComponent<TextMeshProUGUI>().SetText(bookend + value + bookend);
        }
    }
}
