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
    
    private DialogueClip currentDialogueClip;
    private DialogueSceneDisplay dialogueDisplay;
    private string currentTypedText;
    private TextMeshProUGUI currentTmPro;
    private float typedTextDelay;

    public DialogueSceneDisplay DialogueDisplay { get => dialogueDisplay; }
    public NPCHero EngagedHero { get; set; } // PUBLIC SET FOR COMBAT TEST BUTTON
    public Coroutine CurrentTextRoutine { get; private set; }

    private void DisplayCurrentHeroes(NPCHero hero)
    {
        PlayerManager pm = PlayerManager.Instance;
        dialogueDisplay.PlayerHeroImage = pm.PlayerHero.HeroPortrait;
        dialogueDisplay.PlayerHeroName = pm.PlayerHero.HeroName;
        dialogueDisplay.NPCHeroImage = hero.HeroPortrait;
        dialogueDisplay.NPCHeroName = hero.HeroName;
    }

    public void StartDialogue(NPCHero npc)
    {
        AudioManager.Instance.StartStopSound("Soundtrack_Dialogue1", null, AudioManager.SoundType.Soundtrack);
        dialogueDisplay = FindObjectOfType<DialogueSceneDisplay>();
        dialogueDisplay.PlayerHeroPortrait.SetActive(false);
        dialogueDisplay.NPCHeroPortrait.SetActive(false);

        currentDialogueClip = npc.NextDialogueClip;
        if (currentDialogueClip == null)
        {
            Debug.LogError("CLIP IS NULL!");
            return;
        }
        DisplayCurrentHeroes(npc);
        DisplayDialoguePopup();
        AnimationManager.Instance.DialogueIntro();
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
            dialogueDisplay.Response_1 = 
                FilterText(dpr.DialogueResponse1.ResponseText);
        // Response 2
        if (dpr.DialogueResponse2 == null)
            dialogueDisplay.Response_2 = "";
        else
            dialogueDisplay.Response_2 = 
                FilterText(dpr.DialogueResponse2.ResponseText);
        // Response 3
        if (dpr.DialogueResponse3 == null)
            dialogueDisplay.Response_3 = "";
        else
            dialogueDisplay.Response_3 = 
                FilterText(dpr.DialogueResponse3.ResponseText);
        /*
        // Journal Notes
        if (dpr.JournalNotes.Count > 0)
        {
            Debug.LogWarning("NEW JOURNAL NOTE!");
            //JournalManager.Instance.NewJournalNote(currentDialogueClip.JournalNotes);
        }
        */
    }
    
    private string FilterText(string text)
    {
        if (!GameManager.Instance.HideExplicitLanguage) 
            return text;
        string[] filterWords =
        {
            "fucking", "Fucking",
            "fucked", "Fucked",
            "fucks", "Fucks",
            "fuck", "Fuck",

            "bitching", "Bitching",
            "bitches", "Bitches",
            "bitch", "Bitch",

            "assholes", "Assholes",
            "asshole", "Asshole",

            "shitting", "Shitting",
            "shits", "Shits",
            "shit", "Shit"
        };

        string newText = text;
        foreach (string s in filterWords)
        {
            string stars = "";
            for (int i = 0; i < s.Length; i++) 
                stars += "*";
            newText = newText.Replace(s, stars);
        }
        return newText;
    }

    public void TimedText(string text, TextMeshProUGUI tmPro)
    {
        typedTextDelay = 0.05f;
        StopTimedText();
        string filteredText = FilterText(text);
        CurrentTextRoutine = 
            StartCoroutine(TimedTextNumerator(filteredText, tmPro));
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
        DialoguePrompt prompt = currentDialogueClip as DialoguePrompt;
        DialogueResponse dResponse = null;
        switch (response)
        {
            case 1:
                dResponse = prompt.DialogueResponse1;
                break;
            case 2:
                dResponse = prompt.DialogueResponse2;
                break;
            case 3:
                dResponse = prompt.DialogueResponse3;
                break;
        }
        DialogueClip nextClip = dResponse.Response_NextClip;
        if (nextClip == null) return;
        DialoguePrompt nextPrompt = nextClip as DialoguePrompt;
        EngagedHero.NextDialogueClip = nextClip;
        EngagedHero.RespectScore += dResponse.Response_Respect;

        // Exit
        if (dResponse.Response_IsExit)
        {
            GameManager.Instance.EndGame(); // FOR TESTING ONLY
            return;
        }
        // Combat Start
        if (dResponse.Response_IsCombatStart)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
            return;
        }
        // World Map Start
        if (dResponse.Response_IsWorldMapStart)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
            return;
        }
        if (nextClip is DialoguePrompt)
        {
            // New Engaged Hero
            if (nextPrompt.NewEngagedHero != null)
            {
                EngagedHero = GameManager.Instance.GetActiveNPC(nextPrompt.NewEngagedHero);
                DisplayCurrentHeroes(EngagedHero);
            }
            // New Card
            if (nextPrompt.NewCard != null)
                CardManager.Instance.AddCard(nextPrompt.NewCard, GameManager.PLAYER, false);
            else if (nextPrompt.AetherCells > 0)
            {
                int newAether = nextPrompt.AetherCells;
                int newTotal = newAether + PlayerManager.Instance.AetherCells;
                UIManager.Instance.CreateAetherCellPopup(newAether, newTotal);
            }
        }
        // Next Prompt
        currentDialogueClip = nextClip;
        if (currentDialogueClip is DialogueFork) currentDialogueClip = DialogueFork();
        if (nextPrompt.NewCard == null && nextPrompt.AetherCells < 1) DisplayDialoguePopup();
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
