using UnityEngine;
using TMPro;

public class LoadingSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject chapterText;

    public string ChapterText
    {
        set
        {
            chapterText.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
}
