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
        AudioManager.Instance.StartStopSound("Soundtrack_DialogueScene", null, AudioManager.SoundType.Soundtrack);
        dialogueDisplay = FindObjectOfType<DialogueSceneDisplay>();
        EngagedHero = hero;
        currentDialogueClip = hero.NextDialogueClip;
        DisplayCurrentHeroes(hero);
        DisplayDialoguePopup();
    }

    public void EndDialogue(DialogueClip nextClip = null)
    {
        if (nextClip != null) EngagedHero.NextDialogueClip = nextClip;
        EngagedHero = null;
        currentDialogueClip = null;
        //UIManager.Instance.StopTimedText();
    }

    private void DisplayDialoguePopup()
    {
        dialogueDisplay.OtherHeroSpeech = currentDialogueClip.DialoguePrompt;
        // Response 1
        if (currentDialogueClip.DialogueResponse1 == null)
            dialogueDisplay.Response_1 = "";
        else
            dialogueDisplay.Response_1 = currentDialogueClip.DialogueResponse1.ResponseText;
        // Response 2
        if (currentDialogueClip.DialogueResponse2 == null)
            dialogueDisplay.Response_2 = "";
        else
            dialogueDisplay.Response_2 = currentDialogueClip.DialogueResponse2.ResponseText;
        // Response 3
        if (currentDialogueClip.DialogueResponse3 == null)
            dialogueDisplay.Response_3 = "";
        else
            dialogueDisplay.Response_3 = currentDialogueClip.DialogueResponse3.ResponseText;
        // Journal Notes
        if (currentDialogueClip.JournalNotes.Count > 0)
        {
            Debug.LogWarning("NEW JOURNAL NOTE!");
            //JournalManager.Instance.NewJournalNote(currentDialogueClip.JournalNotes);
        }
    }

    public void DialogueResponse(int response)
    {
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
        // Exit
        if (dr.Response_IsExit)
        {
            EndDialogue(dc);
            GameManager.Instance.EndGame(); // FOR TESTING ONLY
            return;
        }
        // Combat Start
        if (dr.Response_IsCombatStart)
        {
            EngagedHero.NextDialogueClip = dr.Response_NextClip;
            SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
            return;
        }
        // New Engaged Hero
        if (dc.NewEngagedHero != null)
        {
            EngagedHero.NextDialogueClip = dc;
            EngagedHero = GameManager.Instance.GetActiveNPC(dc.NewEngagedHero);
            DisplayCurrentHeroes(EngagedHero);
        }
        // New Card
        if (dc.NewCard != null)
        {
            CardManager.Instance.AddCard(dc.NewCard, GameManager.PLAYER, false);
        }

        currentDialogueClip = dc;
        if (currentDialogueClip is DialogueFork) currentDialogueClip = DialogueFork();
        DisplayDialoguePopup();
    }

    private DialogueClip DialogueFork()
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
