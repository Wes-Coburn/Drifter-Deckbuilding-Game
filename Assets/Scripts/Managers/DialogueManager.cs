using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Start()
    {
        ActiveDialoguePools = new List<DialoguePool>();
    }
    public GameObject EngagedCharacterObject { get; private set; }
    public NPCHero EngagedHero { get; private set; }
    public List<DialoguePool> ActiveDialoguePools;
    private DialogueClip currentDialogueClip;
    private DialogueSceneDisplay dialogueDisplay;

    public void StartDialogue(GameObject characterObject, NPCHero hero)
    {
        dialogueDisplay = GameObject.FindObjectOfType<DialogueSceneDisplay>(); // TESTING
        EngagedHero = hero;
        EngagedCharacterObject = characterObject;
        currentDialogueClip = hero.NextDialogueClip;
        if (currentDialogueClip is DialoguePool dp)
            currentDialogueClip = DialoguePool(dp);
        DisplayDialoguePopup();
        EngagedCharacterObject.GetComponent<Button>().enabled = false;
    }

    public void EndDialogue(DialogueClip nextClip = null)
    {
        Debug.Log(EngagedHero.HeroName + " NEXTCLIP is " + EngagedHero.NextDialogueClip.name);
        if (nextClip != null) EngagedHero.NextDialogueClip = nextClip;
        EngagedCharacterObject.GetComponent<Button>().enabled = true;
        EngagedHero = null;
        currentDialogueClip = null;
        //UIManager.Instance.StopTimedText();
    }

    private void DisplayDialoguePopup()
    {   // TESTING
        dialogueDisplay.OtherHeroSpeech = currentDialogueClip.DialoguePrompt;
        dialogueDisplay.PlayerHeroSpeech = "...";
        dialogueDisplay.Response_1 = currentDialogueClip.Response_1;
        dialogueDisplay.Response_2 = currentDialogueClip.Response_2;
        dialogueDisplay.Response_3 = currentDialogueClip.Response_3;

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

        if (currentDialogueClip is DialoguePool dp)
            dp.CurrentBranch = response;

        switch (response)
        {
            case 1:
                EngagedHero.RespectScore += currentDialogueClip.Response_1_Respect;
                DialogueClip dc1 = currentDialogueClip.Response_1_NextClip;
                if (currentDialogueClip.Response_1_isExit)
                {
                    EndDialogue(dc1);
                    return;
                }
                currentDialogueClip = dc1;
                break;
            case 2:
                EngagedHero.RespectScore += currentDialogueClip.Response_2_Respect;
                DialogueClip dc2 = currentDialogueClip.Response_2_NextClip;
                if (currentDialogueClip.Response_2_isExit)
                {
                    EndDialogue(dc2);
                    return;
                }
                currentDialogueClip = dc2;
                break;
            case 3:
                EngagedHero.RespectScore += currentDialogueClip.Response_3_Amiability;
                DialogueClip dc3 = currentDialogueClip.Response_3_NextClip;
                if (currentDialogueClip.Response_3_isExit)
                {
                    EndDialogue(dc3);
                    return;
                }
                currentDialogueClip = dc3;
                break;
        }
        if (currentDialogueClip is DialogueFork) currentDialogueClip = DialogueFork();
        else if (currentDialogueClip is DialoguePool dPool) currentDialogueClip = DialoguePool(dPool);
        DisplayDialoguePopup();
    }

    private DialogueClip DialogueFork()
    {
        DialogueFork dialogueFork = (DialogueFork)currentDialogueClip;
        DialogueClip nextClip = null;
        if (dialogueFork.IsRespectCondition)
        {
            int amiabilityScore = EngagedHero.RespectScore;
            if (amiabilityScore <= dialogueFork.Response_1_Condition_Value) nextClip = dialogueFork.Response_1_NextClip;
            else if (amiabilityScore <= dialogueFork.Response_2_Condition_Value) nextClip = dialogueFork.Response_2_NextClip;
            else nextClip = dialogueFork.Response_3_NextClip;
        }
        return nextClip;
    }
    
    private DialogueClip DialoguePool(DialoguePool pool)
    {
        int poolIndex = ActiveDialoguePools.FindIndex(x => x.DialoguePrompt == pool.DialoguePrompt);
        Debug.Log("<" + pool.ToString() + "> RETURNED <" + poolIndex + ">");
        DialoguePool dp = null;
        if (poolIndex == -1)
        {
            Debug.Log("CREATING NEW DIALOGUE POOL INSTANCE!");
            dp = ScriptableObject.CreateInstance<DialoguePool>();
            dp.LoadDialoguePool(pool);
            ActiveDialoguePools.Add(dp);
        }
        else
        {
            Debug.Log("DIALOGUE POOL FOUND ... ACTIVE POOLS = " + ActiveDialoguePools.Count);
            dp = ActiveDialoguePools[poolIndex];
            dp.LoadNewResponse(dp.CurrentBranch, dp.CurrentClip);
        }
        return dp;
    }
}
