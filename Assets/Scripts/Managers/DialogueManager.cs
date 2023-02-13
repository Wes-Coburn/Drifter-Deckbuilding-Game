using System.Collections;
using TMPro;
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

    private DialogueClip currentDialogueClip;
    private DialogueSceneDisplay dialogueDisplay;
    private string currentTypedText;
    private TextMeshProUGUI currentTmPro;
    private float typedTextDelay;
    private bool newEngagedHero;

    public DialogueSceneDisplay DialogueDisplay { get => dialogueDisplay; }
    public NPCHero EngagedHero { get; set; } // PUBLIC SET FOR COMBAT TEST BUTTON
    public Coroutine CurrentTextRoutine { get; private set; }
    public bool AllowResponse { get; set; }

    private void Start()
    {
        newEngagedHero = false;
        AllowResponse = true;
    }

    /******
     * *****
     * ****** DIALOGUE_MANAGER
     * *****
     *****/
    public void Reset_DialogueManager() => StopTimedText();

    public void DisplayCurrentHeroes()
    {
        dialogueDisplay.PlayerHeroImage = Managers.P_MAN.HeroScript.HeroPortrait;
        dialogueDisplay.PlayerHeroName = Managers.P_MAN.HeroScript.HeroName;
        dialogueDisplay.NPCHeroImage = EngagedHero.HeroPortrait;
        dialogueDisplay.NPCHeroName = EngagedHero.HeroName;
    }

    private void ChangeReputations(DialoguePrompt prompt)
    {
        if (prompt.Reputation_Mages != 0)
        {
            Managers.G_MAN.ChangeReputation
                (GameManager.ReputationType.Mages, prompt.Reputation_Mages);
        }
        if (prompt.Reputation_Mutants != 0)
        {
            Managers.G_MAN.ChangeReputation
                (GameManager.ReputationType.Mutants, prompt.Reputation_Mutants);
        }
        if (prompt.Reputation_Rogues != 0)
        {
            Managers.G_MAN.ChangeReputation
                (GameManager.ReputationType.Rogues, prompt.Reputation_Rogues);
        }
        if (prompt.Reputation_Techs != 0)
        {
            Managers.G_MAN.ChangeReputation
                (GameManager.ReputationType.Techs, prompt.Reputation_Techs);
        }
        if (prompt.Reputation_Warriors != 0)
        {
            Managers.G_MAN.ChangeReputation
                (GameManager.ReputationType.Warriors, prompt.Reputation_Warriors);
        }
    }

    public void ChangeReputations(CombatRewardClip crc)
    {
        if (crc.Reputation_Mages != 0)
        {
            Managers.G_MAN.ChangeReputation
                (GameManager.ReputationType.Mages, crc.Reputation_Mages);
        }
        if (crc.Reputation_Mutants != 0)
        {
            Managers.G_MAN.ChangeReputation
                (GameManager.ReputationType.Mutants, crc.Reputation_Mutants);
        }
        if (crc.Reputation_Rogues != 0)
        {
            Managers.G_MAN.ChangeReputation
                (GameManager.ReputationType.Rogues, crc.Reputation_Rogues);
        }
        if (crc.Reputation_Techs != 0)
        {
            Managers.G_MAN.ChangeReputation
                (GameManager.ReputationType.Techs, crc.Reputation_Techs);
        }
        if (crc.Reputation_Warriors != 0)
        {
            Managers.G_MAN.ChangeReputation
                (GameManager.ReputationType.Warriors, crc.Reputation_Warriors);
        }
    }

    public void StartDialogue()
    {
        Managers.AU_MAN.StopCurrentSoundscape();

        Managers.AU_MAN.StartStopSound(null,
            Managers.G_MAN.CurrentLocation.LocationSoundscape, AudioManager.SoundType.Soundscape);

        Managers.AU_MAN.StartStopSound("Soundtrack_Dialogue1", null,
            AudioManager.SoundType.Soundtrack);

        currentDialogueClip = EngagedHero.NextDialogueClip;
        if (currentDialogueClip == null)
        {
            Debug.LogError("CLIP IS NULL!");
            return;
        }

        dialogueDisplay = FindObjectOfType<DialogueSceneDisplay>();
        dialogueDisplay.PlayerHeroPortrait.SetActive(false);
        dialogueDisplay.NPCHeroPortrait.SetActive(false);

        var prompt = currentDialogueClip as DialoguePrompt;
        if (prompt.NewLocations != null)
        {
            float delay = 0;
            foreach (NewLocation newLoc in prompt.NewLocations)
            {
                Managers.G_MAN.GetActiveLocation(newLoc.Location, newLoc.NewNpc);
                if (newLoc.Location.IsAugmenter) continue;
                FunctionTimer.Create(() => Managers.U_MAN.CreateLocationPopup
                (Managers.G_MAN.GetActiveLocation(newLoc.Location), true), delay);
                delay += 5;
            }
        }
        else Debug.LogWarning("NEW LOCATIONS IS NULL!");

        DisplayCurrentHeroes();
        DisplayDialoguePopup();
        Managers.AN_MAN.DialogueIntro();

        AllowResponse = false;
        FunctionTimer.Create(() => ChangeReputations(prompt), 1);
        FunctionTimer.Create(() => AllowResponse = true, 1);
    }

    public void EndDialogue()
    {
        EngagedHero = null;
        currentDialogueClip = null;
        StopTimedText();
    }

    public void DisplayDialoguePopup()
    {
        var dpr = currentDialogueClip as DialoguePrompt;
        if (newEngagedHero)
        {
            dialogueDisplay.NPCHeroSpeech = "";
            newEngagedHero = false;
            AllowResponse = false;
            AnimationManager.Instance.NewEngagedHero(false);
            return;
        }

        TimedText(dpr.DialoguePromptText,
            dialogueDisplay.NPCHeroSpeechObject.GetComponent<TextMeshProUGUI>());

        // Response 1
        bool r1_Active = true;
        if (string.IsNullOrEmpty(dpr.DialogueResponse1.ResponseText))
            r1_Active = false;
        dialogueDisplay.Response_1_Object.SetActive(r1_Active);
        if (r1_Active)
            dialogueDisplay.Response_1 =
                FilterText(dpr.DialogueResponse1.ResponseText);

        // Response 2
        bool r2_Active = true;
        if (string.IsNullOrEmpty(dpr.DialogueResponse2.ResponseText))
            r2_Active = false;
        dialogueDisplay.Response_2_Object.SetActive(r2_Active);
        if (r2_Active)
            dialogueDisplay.Response_2 =
                FilterText(dpr.DialogueResponse2.ResponseText);

        // Response 3
        bool r3_Active = true;
        if (string.IsNullOrEmpty(dpr.DialogueResponse3.ResponseText))
            r3_Active = false;
        dialogueDisplay.Response_3_Object.SetActive(r3_Active);
        if (r3_Active)
            dialogueDisplay.Response_3 =
                FilterText(dpr.DialogueResponse3.ResponseText);
    }

    public string FilterText(string text)
    {
        if (Managers.P_MAN.HeroScript != null)
        {
            string shortName = Managers.P_MAN.HeroScript.HeroShortName;
            if (!string.IsNullOrEmpty(shortName))
                text = text.Replace("<HERO NAME>", shortName);
        }

        // EXPLICIT LANGUAGE FILTER
        if (!GameManager.Instance.HideExplicitLanguage) return text;
        string[] filterWords =
        {
            "fucking", "Fucking",
            "fucked", "Fucked",
            "fuckers", "Fuckers",
            "fucker", "Fucker",
            "fucks", "Fucks",
            "fuck", "Fuck",

            "bitching", "Bitching",
            "bitches", "Bitches",
            "bitch", "Bitch",

            "assholes", "Assholes",
            "asshole", "Asshole",

            "shitting", "Shitting",
            "shitters", "Shitters",
            "shitter", "Shitter",
            "shits", "Shits",
            "shit", "Shit"
        };

        foreach (string s in filterWords)
        {
            string stars = "";
            for (int i = 0; i < s.Length; i++)
                stars += "*";
            text = text.Replace(s, stars);
        }
        return text;
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
        if (SceneLoader.SceneIsLoading || !AllowResponse) return;

        if (currentDialogueClip == null)
        {
            Debug.LogError("CURRENT CLIP IS NULL!");
            return;
        }

        if (currentDialogueClip is DialoguePrompt) { }
        else
        {
            Debug.LogError("CURRENT CLIP IS NOT PROMPT!");
            return;
        }

        if (CurrentTextRoutine != null)
        {
            StopTimedText(true);
            return;
        }

        var prompt = currentDialogueClip as DialoguePrompt;
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
        if (nextClip == null)
        {
            Debug.LogError("NEXT CLIP IS NULL!");
            return;
        }

        DialoguePrompt nextPrompt = null;
        if (nextClip is DialoguePrompt dPrompt)
        {
            nextPrompt = dPrompt;
            if (nextPrompt.NewLocations != null)
            {
                float delay = 0;
                foreach (NewLocation newLoc in nextPrompt.NewLocations)
                {
                    Managers.G_MAN.GetActiveLocation(newLoc.Location, newLoc.NewNpc);
                    if (newLoc.Location.IsAugmenter) continue;
                    FunctionTimer.Create(() =>
                    Managers.U_MAN.CreateLocationPopup(Managers.G_MAN.GetActiveLocation(newLoc.Location), true), delay);
                    delay += 5;
                }
            }
            else Debug.LogWarning("NEW LOCATIONS IS NULL!");

            ChangeReputations(nextPrompt);
        }

        if (dResponse.NPC_NextClip != null)
        {
            EngagedHero.NextDialogueClip = dResponse.NPC_NextClip;
            Debug.Log("NEXT CLIP: " + dResponse.NPC_NextClip.ToString());
        }

        // Travel Location
        if (dResponse.Response_TravelLocation != null)
        {
            Location location = dResponse.Response_TravelLocation;
            Managers.U_MAN.CreateTravelPopup(Managers.G_MAN.GetActiveLocation(location));
            return;
        }
        // Combat Start
        if (dResponse.Response_IsCombatStart)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
            if (nextClip is CombatRewardClip) EngagedHero.NextDialogueClip = nextClip;
            else Debug.LogError("NEXT CLIP IS NOT COMBAT REWARD CLIP!");
            return;
        }
        // World Map Start
        if (dResponse.Response_IsWorldMapStart)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
            return;
        }
        // Recruitment
        if (dResponse.Response_IsRecruitmentStart)
        {
            Managers.U_MAN.CreateCardPage(CardPageDisplay.CardPageType.RecruitUnit);
            return;
        }
        // Action Shop
        if (dResponse.Response_IsActionShopStart)
        {
            Managers.U_MAN.CreateCardPage(CardPageDisplay.CardPageType.AcquireAction);
            return;
        }
        // Item Shop
        if (dResponse.Response_IsShopStart)
        {
            Managers.U_MAN.CreateItemPagePopup(false);
            return;
        }
        // Cloning
        if (dResponse.Response_IsCloningStart)
        {
            Managers.U_MAN.CreateCardPage(CardPageDisplay.CardPageType.CloneUnit);
            return;
        }
        // New Augment
        if (dResponse.Response_IsNewAugmentStart)
        {
            Managers.U_MAN.CreateNewAugmentPopup();
        }
        // Exit
        if (dResponse.Response_IsExit)
        {
            Managers.U_MAN.CreateGameEndPopup(); // FOR BETA ONLY
            GameLoader.SaveGame(); // FOR BETA ONLY
            return;
        }

        if (nextPrompt != null)
        {
            // New Engaged Hero
            if (nextPrompt.NewEngagedHero != null)
            {
                EngagedHero = Managers.G_MAN.GetActiveNPC(nextPrompt.NewEngagedHero);
                newEngagedHero = true;
            }
            else
            {
                if (nextPrompt.HideNPC)
                {
                    if (!prompt.HideNPC)
                    {
                        currentDialogueClip = nextClip;
                        AllowResponse = false;
                        Managers.AN_MAN.NewEngagedHero(true);
                        if (nextPrompt.NewCard == null && nextPrompt.AetherCells < 1) return;
                    }
                }
            }

            // Aether Cells
            if (nextPrompt.AetherCells > 0) Managers.P_MAN.AetherCells += nextPrompt.AetherCells;
            // New Card
            if (nextPrompt.NewCard != null) Managers.U_MAN.CreateNewCardPopup(nextPrompt.NewCard, "New Card!");
        }

        currentDialogueClip = nextClip;
        if (nextPrompt != null && nextPrompt.NewCard == null &&
            !dResponse.Response_IsNewAugmentStart) DisplayDialoguePopup();
    }
}
