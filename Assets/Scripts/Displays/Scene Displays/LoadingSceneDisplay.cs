using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject tipText;
    [SerializeField] private GameObject chapterText;
    [SerializeField] private GameObject loadingBar;

    private Slider loadingBarSlider;
    private void Start()
    {
        loadingBarSlider = loadingBar.GetComponent<Slider>();
        loadingBar.SetActive(false);
    }
    private void Update()
    {
        if (SceneLoader.LoadingProgress >= 0)
        {
            if (!loadingBar.activeInHierarchy) loadingBar.SetActive(true);
            loadingBarSlider.SetValueWithoutNotify(SceneLoader.LoadingProgress);
        }
    }

    public string TipText
    {
        set
        {
            tipText.GetComponent<TextMeshProUGUI>().SetText(Managers.CA_MAN.FilterKeywords(value));
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
