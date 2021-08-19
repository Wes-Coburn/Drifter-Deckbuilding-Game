using System.Collections;
using UnityEngine;
using TMPro;

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
    
    public NPCHero EngagedHero { get; set; } // PUBLIC SET FOR COMBAT TEST BUTTON
    private DialogueClip currentDialogueClip;
    private DialogueSceneDisplay dialogueDisplay;

    public Coroutine CurrentTextRoutine { get; private set; }
    private string currentTypedText;
    private TextMeshProUGUI currentTmPro;
    private float typedTextDelay;

    private void DisplayCurrentHeroes(NPCHero hero)
    {
        PlayerManager pm = PlayerManager.Instance;
        dialogueDisplay.PlayerHeroPortrait = pm.PlayerHero.HeroPortrait;
        dialogueDisplay.PlayerHeroName = pm.PlayerHero.HeroName;
        dialogueDisplay.NPCHeroPortrait = hero.HeroPortrait;
        dialogueDisplay.NPCHeroName = hero.HeroName;
    }

    public void StartDialogue(NPCHero npc)
    {
        AudioManager.Instance.StartStopSound("Soundtrack_DialogueScene", null, AudioManager.SoundType.Soundtrack);
        dialogueDisplay = FindObjectOfType<DialogueSceneDisplay>();
        EngagedHero = npc;
        currentDialogueClip = npc.NextDialogueClip;
        if (currentDialogueClip == null)
        {
            Debug.LogError("CLIP IS NULL!");
            return;
        }
        DisplayCurrentHeroes(npc);
        DisplayDialoguePopup();
    }

    public void EndDialogue(DialogueClip nextClip = null)
    {
        if (nextClip != null) EngagedHero.NextDialogueClip = nextClip;
        EngagedHero = null;
        currentDialogueClip = null;
        StopTimedText();
    }

    public void DisplayDialoguePopup()
    {
        DialoguePrompt dpr = currentDialogueClip as DialoguePrompt;
        dialogueDisplay.NPCHeroSpeech = dpr.DialoguePromptText;

        TimedText(dpr.DialoguePromptText, 
            dialogueDisplay.NPCHeroSpeechObject.GetComponent<TextMeshProUGUI>());

        // Response 1
        if (dpr.DialogueResponse1 == null)
            dialogueDisplay.Response_1 = "";
        else
            dialogueDisplay.Response_1 = dpr.DialogueResponse1.ResponseText;
        // Response 2
        if (dpr.DialogueResponse2 == null)
            dialogueDisplay.Response_2 = "";
        else
            dialogueDisplay.Response_2 = dpr.DialogueResponse2.ResponseText;
        // Response 3
        if (dpr.DialogueResponse3 == null)
            dialogueDisplay.Response_3 = "";
        else
            dialogueDisplay.Response_3 = dpr.DialogueResponse3.ResponseText;
        /*
        // Journal Notes
        if (dpr.JournalNotes.Count > 0)
        {
            Debug.LogWarning("NEW JOURNAL NOTE!");
            //JournalManager.Instance.NewJournalNote(currentDialogueClip.JournalNotes);
        }
        */
    }
    
    public void TimedText(string text, TextMeshProUGUI tmPro)
    {
        typedTextDelay = 0.05f; // TESTING
        StopTimedText();
        CurrentTextRoutine = StartCoroutine(TimedTextNumerator(text, tmPro));
    }
    public void StopTimedText(bool finishText = false)
    {
        if (CurrentTextRoutine != null)
        {
            StopCoroutine(CurrentTextRoutine);
            if (finishText) currentTmPro.SetText(currentTypedText);
            CurrentTextRoutine = null;
            currentTypedText = null;
            currentTmPro = null;
        }
    }
    private IEnumerator TimedTextNumerator(string text, TextMeshProUGUI tmPro)
    {
        currentTypedText = text;
        currentTmPro = tmPro;
        float delay = typedTextDelay;
        string fullText = text;
        string currentText;
        for (int i = 0; i < fullText.Length + 1; i++)
        {
            AudioManager.Instance.StartStopSound("SFX_Typing");
            currentText = fullText.Substring(0, i);
            tmPro.SetText(currentText);
            yield return new WaitForSeconds(delay);
        }
        CurrentTextRoutine = null;
    }

    public void DialogueResponse(int response)
    {
        if (currentDialogueClip == null) return; // TESTING

        if (CurrentTextRoutine != null)
        {
            StopTimedText(true);
            return;
        }
        DialoguePrompt dpr = currentDialogueClip as DialoguePrompt;
        DialogueResponse dr = null;
        switch (response)
        {
            case 1:
                dr = dpr.DialogueResponse1;
                break;
            case 2:
                dr = dpr.DialogueResponse2;
                break;
            case 3:
                dr = dpr.DialogueResponse3;
                break;
        }

        DialogueClip dc = dr.Response_NextClip;
        if (dc == null) return;

        EngagedHero.RespectScore += dr.Response_Respect;
        // Exit
        if (dr.Response_IsExit)
        {
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

        if (dc is DialoguePrompt)
        {
            // New Engaged Hero
            if (dpr.NewEngagedHero != null)
            {
                EngagedHero.NextDialogueClip = dc;
                EngagedHero = GameManager.Instance.GetActiveNPC(dpr.NewEngagedHero);
                DisplayCurrentHeroes(EngagedHero);
            }
            // New Card
            if (dpr.NewCard != null)
                CardManager.Instance.AddCard(dpr.NewCard, GameManager.PLAYER, false);
        }
        currentDialogueClip = dc;
        if (currentDialogueClip is DialogueFork) currentDialogueClip = DialogueFork();
        if (dpr.NewCard == null)
            DisplayDialoguePopup();
    }

    private DialogueClip DialogueFork()
    {
        DialogueFork df = currentDialogueClip as DialogueFork;
        DialogueClip nextClip = null;
        if (df.IsRespectCondition)
        {
            int respect = EngagedHero.RespectScore;
            if (respect <= df.Clip_2_Condition_Value) nextClip = df.Clip_1;
            else if (respect <= df.Clip_2_Condition_Value) nextClip = df.Clip_2;
            else nextClip = df.Clip_3;
        }
        return nextClip;
    }
}
