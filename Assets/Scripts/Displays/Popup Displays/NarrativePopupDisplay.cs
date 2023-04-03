using TMPro;
using UnityEngine;

public class NarrativePopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject narrativeTitle;
    [SerializeField] private GameObject narrativeText;
    [SerializeField] private GameObject clipCounter;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject previousButton;

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
        clipCounterText = clipCounter.GetComponent<TextMeshProUGUI>();
        continueButton.SetActive(false);
    }

    private void DisplayCurrentClip(bool isFirstDisplay = false)
    {
        if (isFirstDisplay) Managers.AU_MAN.StartStopSound
                (null, loadedNarrative.NarrativeStartSound);

        int clipCount = loadedNarrative.NarrativeText.Length;
        bool showPrevious = true;
        bool showNext = true;
        bool showContinue = false;
        if (currentClip == 0)
            showPrevious = false;
        if (currentClip == clipCount - 1)
        {
            showNext = false;
            showContinue = true;
        }
        nextButton.SetActive(showNext);
        continueButton.SetActive(showContinue);
        previousButton.SetActive(showPrevious);
        clipCounterText.SetText(currentClip + 1 + "/" + clipCount);

        Managers.D_MAN.TimedText(loadedNarrative.NarrativeText[currentClip],
            narrativeText.GetComponent<TextMeshProUGUI>());
    }

    public void NextButton_OnClick()
    {
        if (Managers.D_MAN.CurrentTextRoutine != null)
        {
            Managers.D_MAN.StopTimedText(true);
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

    public void ContinueButton_OnClick()
    {
        Managers.U_MAN.DestroyNarrativePopup();

        /* Bonus Cards Feature
        if (loadedNarrative.NarrativeName.StartsWith("Part 1"))
        {
            int bonusRewards = GameManager.BONUS_START_REWARDS;
            if (bonusRewards > 0)
            {
                if (bonusRewards > 1)
                    ChooseRewardPopupDisplay.BonusRewards += bonusRewards;

                Managers.U_MAN.CreateChooseRewardPopup();
            }
        }
        */

        if (Managers.G_MAN.TutorialActive_WorldMap)
        {
            Managers.G_MAN.TutorialActive_WorldMap = false;
            Managers.U_MAN.CreateTutorialActionPopup(TutorialActionPopupDisplay.Type.WorldMap);
        }
    }
}
