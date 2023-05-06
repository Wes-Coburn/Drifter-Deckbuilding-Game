using TMPro;
using UnityEngine;

public class TutorialActionPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject tipText, tipCounter,
        nextTipButton, previousTipButton, continueButton;

    private int currentTip, totalTips;
    private TextMeshProUGUI tipTextMesh, tipCountMesh;

    public enum Type
    {
        Tutorial,
        WorldMap
    }

    private Type tutorialType;
    public Type TutorialType
    {
        set
        {
            tutorialType = value;
            switch(tutorialType)
            {
                case Type.Tutorial:
                    totalTips = 10;
                    break;
                case Type.WorldMap:
                    totalTips = 5;
                    break;
            }
            SetCurrentTip();
        }
    }

    private void Awake()
    {
        tipTextMesh = tipText.GetComponent<TextMeshProUGUI>();
        tipCountMesh = tipCounter.GetComponent<TextMeshProUGUI>();
        currentTip = 1;
    }

    public void NextTipButton_OnClick()
    {

        currentTip++;
        SetCurrentTip();
    }

    public void PreviousTipButton_OnClick()
    {
        currentTip--;
        SetCurrentTip();
    }

    public void ContinueButton_OnClick()
    {
        Managers.U_MAN.DestroyTutorialActionPopup();
        Managers.EV_MAN.PauseDelayedActions(false);
    }

    private void SetCurrentTip()
    {
        string text;
        Vector2 position = new();

        bool tipCounter_Active = true;
        bool continueButton_Active = false;
        bool previousButton_Active = true;
        bool nextButton_Active = true;

        switch (tutorialType, currentTip)
        {
            // Tutorial
            case (Type.Tutorial, 1):
                FirstCase();
                text = $"This is your {TextFilter.Clrz_ylw("Hero")}.";
                position.Set(470, 0);
                break;
            case (Type.Tutorial, 2):
                text = $"This is the {TextFilter.Clrz_ylw("Enemy Hero")}.";
                position.Set(-270, 230);
                break;
            case (Type.Tutorial, 3):
                text = $"Your hero has {TextFilter.Clrz_ylw("Health")}, " +
                    $"{TextFilter.Clrz_ylw("Energy")}, a {TextFilter.Clrz_ylw("Power")}, " +
                    $"and an {TextFilter.Clrz_ylw("Ultimate")}. " +
                    "\nHover over each component for more details.";
                position.Set(470, 0);
                break;
            case (Type.Tutorial, 4):
                text = $"Damage dealt to your hero reduces their {TextFilter.Clrz_ylw("Health")}. " +
                    "If your hero's health reaches 0, you lose.";
                position.Set(470, 0);
                break;
            case (Type.Tutorial, 5):
                text = $"{TextFilter.Clrz_ylw("Energy")} is used to play cards and use hero powers. " +
                    $"You gain 1 energy at the start of your turn, increased by 1 each turn (max {GameManager.MAXIMUM_ENERGY}).";
                position.Set(470, 0);
                break;
            case (Type.Tutorial, 6):
                text = $"You can use your {TextFilter.Clrz_ylw("Hero Power")} once each turn. " +
                    $"Each time you use your hero power {GameManager.HERO_ULTMATE_GOAL}+ times, you can use " +
                    $"your {TextFilter.Clrz_ylw("Hero Ultimate")} once.";
                position.Set(470, 0);
                break;
            case (Type.Tutorial, 7):
                text = $"You draw cards from your {TextFilter.Clrz_ylw("Deck")}. When it runs " +
                    $"out of cards, you'll shuffle your discard pile back into it.";
                position.Set(-500, -150);
                break;
            case (Type.Tutorial, 8):
                text = $"You draw {GameManager.START_HAND_SIZE} cards at the beginning of each combat, and 1 each turn after that.";
                position.Set(-500, -150);
                break;
            case (Type.Tutorial, 9):
                text = "After you draw your first hand, you can replace any number of cards " +
                    $"with new ones from your deck. This is known as a {TextFilter.Clrz_ylw("Mulligan")}.";
                position.Set(-500, -150);
                break;
            case (Type.Tutorial, 10): // Tutorial - Last Case
                text = "Now you know the basics. Let's play.";
                LastCase();
                break;
            case (Type.Tutorial, 11): // Tutorial - Catch Case
                currentTip = 10;
                return;

            // World Map
            case (Type.WorldMap, 1):
                FirstCase();
                text = $"This is the {TextFilter.Clrz_ylw("World Map")}.";
                break;
            case (Type.WorldMap, 2):
                text = $"This is the {TextFilter.Clrz_ylw("time of day")}. Each day starts at {TextFilter.Clrz_ylw("morning")}, " +
                    $"and when it reaches {TextFilter.Clrz_ylw("night")}, you must return to your ship to rest.";
                position.Set(500, 170);
                break;
            case (Type.WorldMap, 3):
                text = $"This is {TextFilter.Clrz_ylw("Your Ship")}. Rest at night, sell cards and items, and change your powers.";
                position.Set(380, -270);
                break;
            case (Type.WorldMap, 4):
                text = $"Look for the large diamond icon to find the current {TextFilter.Clrz_ylw("Priority Location")}. " +
                    $"Visiting these locations will move the game forward, though others may provide valuable rewards...";
                position.Set(145, 100);
                break;
            case (Type.WorldMap, 5): // World Map - Last Case
                LastCase();
                text = $"It's night now, time to rest at {TextFilter.Clrz_ylw("Your Ship")}.";
                break;
            case (Type.WorldMap, 6): // World Map - Catch Case
                currentTip = 5;
                return;

            // Universal
            default:
                Debug.LogError("INVALID TIP NUMBER!");
                return;
        }

        tipCounter.SetActive(tipCounter_Active);
        continueButton.SetActive(continueButton_Active);
        previousTipButton.SetActive(previousButton_Active);
        nextTipButton.SetActive(nextButton_Active);
        DisplayCurrentTip(text, position);

        void FirstCase() => previousButton_Active = false;
        void LastCase()
        {
            tipCounter_Active = false;
            continueButton_Active = true;
            nextButton_Active = false;
        }
    }

    private void DisplayCurrentTip(string text, Vector2 position)
    {
        tipCountMesh.SetText(currentTip + "/" + totalTips);
        tipTextMesh.SetText(text);
        transform.position = position;
    }
}
