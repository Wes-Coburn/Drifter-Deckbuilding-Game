using UnityEngine;
using TMPro;

public class LoadingSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject chapterText;

    public string ChapterText
    {
        set
        {
            string bookend = " --- ";
            chapterText.GetComponent<TextMeshProUGUI>().SetText(bookend + value + bookend);
        }
    }
}
