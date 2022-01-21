using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NarrativeSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject narrativeText;
    [SerializeField] private GameObject clipCounter;
    [SerializeField] private GameObject continueButton;

    private DialogueManager dMan;
    private Narrative narrative;
    private int currentClip;
    private TextMeshProUGUI clipCounterText;

    public Narrative CurrentNarrative
    {
        get => narrative;
        set
        {
            narrative = value;
            currentClip = 0;
            DisplayCurrentClip();
        }
    }

    private void Awake()
    {
        dMan = DialogueManager.Instance;
        clipCounterText = clipCounter.GetComponent<TextMeshProUGUI>();
        continueButton.SetActive(false);
    }

    private void DisplayCurrentClip()
    {
        clipCounterText.SetText(currentClip + 1 + "/" + 
            narrative.NarrativeText.Length);
        background.GetComponent<Image>().sprite = CurrentNarrative.NarrativeBackground;
        dMan.TimedText(narrative.NarrativeText[currentClip], 
            narrativeText.GetComponent<TextMeshProUGUI>());
    }
    
    public void NextButton_OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        GetComponent<SoundPlayer>().PlaySound(0);

        if (dMan.CurrentTextRoutine != null)
        {
            dMan.StopTimedText(true);
            return;
        }
        int lastClip = narrative.NarrativeText.Length - 1;
        if (++currentClip < lastClip)
            DisplayCurrentClip();
        else if (currentClip == lastClip)
        {
            DisplayCurrentClip();
            continueButton.SetActive(true);
        }
        else currentClip--;
    }
    public void PreviousButton_OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        GetComponent<SoundPlayer>().PlaySound(0);

        if (--currentClip < 0) currentClip = 0;
        else DisplayCurrentClip();
        continueButton.SetActive(false);
    }
    public void ContinueButton_OnClick() =>
        GameManager.Instance.EndNarrative();
}
