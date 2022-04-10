using UnityEngine;
using TMPro;

public class NarrativePopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject narrativeTitle;
    [SerializeField] private GameObject narrativeText;
    [SerializeField] private GameObject clipCounter;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject previousButton;

    private UIManager uMan;
    private DialogueManager dMan;
    private Narrative loadedNarrative;
    private int currentClip;
    private TextMeshProUGUI clipCounterText;

    public Narrative LoadedNarrative
    {
        set
        {
            loadedNarrative = value;
            currentClip = 0;
            narrativeTitle.GetComponent
                <TextMeshProUGUI>().SetText(loadedNarrative.NarrativeName);
            DisplayCurrentClip(true);
        }
    }

    private void Awake()
    {
        uMan = UIManager.Instance;
        dMan = DialogueManager.Instance;
        clipCounterText = clipCounter.GetComponent<TextMeshProUGUI>();
        continueButton.SetActive(false);
    }

    private void DisplayCurrentClip(bool isFirstDisplay = false)
    {
        if (isFirstDisplay) AudioManager.Instance.StartStopSound
                (null, loadedNarrative.NarrativeStartSound); // TESTING

        int clipCount = loadedNarrative.NarrativeText.Length;
        bool showPrevious = true;
        bool showNext = true;
        if (currentClip == 0)
            showPrevious = false;
        if (currentClip == clipCount - 1)
            showNext = false;
        nextButton.SetActive(showNext);
        previousButton.SetActive(showPrevious);

        clipCounterText.SetText(currentClip + 1 + "/" + clipCount);
        dMan.TimedText(loadedNarrative.NarrativeText[currentClip],
            narrativeText.GetComponent<TextMeshProUGUI>());
    }

    public void NextButton_OnClick()
    {
        if (dMan.CurrentTextRoutine != null)
        {
            dMan.StopTimedText(true);
            return;
        }
        int lastClip = loadedNarrative.NarrativeText.Length - 1;
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
        if (--currentClip < 0) currentClip = 0;
        else DisplayCurrentClip();
        continueButton.SetActive(false);
    }

    public void ContinueButton_OnClick() =>
        uMan.DestroyNarrativePopup();
}
