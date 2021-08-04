using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    /* SINGELTON PATTERN */
    public static DialogueManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public NPCHero EngagedHero { get; private set; }
    private DialogueClip currentDialogueClip;
    private DialogueSceneDisplay dialogueDisplay;

    private void DisplayCurrentHeroes(NPCHero hero)
    {
        PlayerManager pm = PlayerManager.Instance;
        dialogueDisplay.PlayerHeroPortrait = pm.PlayerHero.HeroPortrait;
        dialogueDisplay.PlayerHeroName = pm.PlayerHero.HeroName;
        dialogueDisplay.OtherHeroPortrait = hero.HeroPortrait;
        dialogueDisplay.OtherHeroName = hero.HeroName;

    }
    public void StartDialogue(NPCHero hero)
    {
        dialogueDisplay = FindObjectOfType<DialogueSceneDisplay>();
        EngagedHero = hero;
        currentDialogueClip = hero.NextDialogueClip;
        DisplayCurrentHeroes(hero);
        DisplayDialoguePopup();
    }

    public void EndDialogue(DialogueClip nextClip = null)
    {
        Debug.Log(EngagedHero.HeroName + " NEXTCLIP is " + EngagedHero.NextDialogueClip.name);
        if (nextClip != null) EngagedHero.NextDialogueClip = nextClip;
        EngagedHero = null;
        currentDialogueClip = null;
        //UIManager.Instance.StopTimedText();
    }

    private void DisplayDialoguePopup()
    {
        dialogueDisplay.OtherHeroSpeech = currentDialogueClip.DialoguePrompt;
        dialogueDisplay.PlayerHeroSpeech = "...";

        if (currentDialogueClip.DialogueResponse1 == null)
            dialogueDisplay.Response_1 = "";
        else
            dialogueDisplay.Response_1 = currentDialogueClip.DialogueResponse1.ResponseText;

        if (currentDialogueClip.DialogueResponse2 == null)
            dialogueDisplay.Response_2 = "";
        else
            dialogueDisplay.Response_2 = currentDialogueClip.DialogueResponse2.ResponseText;

        if (currentDialogueClip.DialogueResponse3 == null)
            dialogueDisplay.Response_3 = "";
        else
            dialogueDisplay.Response_3 = currentDialogueClip.DialogueResponse3.ResponseText;

        if (currentDialogueClip.JournalNotes.Count > 0)
        {
            Debug.LogWarning("NEW JOURNAL NOTE!");
            //JournalManager.Instance.NewJournalNote(currentDialogueClip.JournalNotes);
        }
    }

    public void DialogueResponse(int response)
    {
        if (CardManager.Instance.NewCardPopup != null) return;
        /*
        if (UIManager.Instance.CurrentTypedTextRoutine != null)
        {
            UIManager.Instance.StopTimedText(true);
            return;
        }
        */
        DialogueResponse dr = null;
        switch (response)
        {
            case 1:
                dr = currentDialogueClip.DialogueResponse1;
                break;
            case 2:
                dr = currentDialogueClip.DialogueResponse2;
                break;
            case 3:
                dr = currentDialogueClip.DialogueResponse3;
                break;
        }
        if (dr == null) return;
        DialogueClip dc = dr.Response_NextClip;
        EngagedHero.RespectScore += dr.Response_Respect;
        if (dr.Response_IsExit)
        {
            EndDialogue(dc);
            return;
        }
        if (dr.Response_IsCombatStart)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
            return;
        }

        if (dc.NewEngagedHero != null) // TESTING
        {
            EngagedHero.NextDialogueClip = currentDialogueClip;
            EngagedHero = GameManager.Instance.GetActiveNPC(dc.NewEngagedHero);
            DisplayCurrentHeroes(EngagedHero);
        }
        if (dc.NewCard != null) // TESTING
        {
            CardManager.Instance.AddCard(dc.NewCard, GameManager.PLAYER, false);
        }

        currentDialogueClip = dc;
        if (currentDialogueClip is DialogueFork) currentDialogueClip = DialogueFork();
        DisplayDialoguePopup();
    }

    private DialogueClip DialogueFork() // Consider altering with the new format of responses
    {
        DialogueFork dialogueFork = (DialogueFork)currentDialogueClip;
        DialogueClip nextClip = null;
        if (dialogueFork.IsRespectCondition)
        {
            int respect = EngagedHero.RespectScore;
            if (respect <= dialogueFork.Response_2_Condition_Value) nextClip = dialogueFork.DialogueResponse1.Response_NextClip;
            else if (respect <= dialogueFork.Response_2_Condition_Value) nextClip = dialogueFork.DialogueResponse2.Response_NextClip;
            else nextClip = dialogueFork.DialogueResponse3.Response_NextClip;
        }
        return nextClip;
    }
}
