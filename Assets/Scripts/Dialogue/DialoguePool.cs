using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Pool", menuName = "Dialogue/Dialogue Pool")]
public class DialoguePool : DialogueClip
{
    [Header("DIALOGUE CLIPS")]
    [SerializeField] private List<DialogueClip> dialogueClips;
    public List<DialogueClip> DialogueClips { get => dialogueClips; }
    private int currentClip;

    private int currentBranch;
    public int CurrentBranch
    {
        get => currentBranch;
        set
        {
            currentBranch = value;
            if (currentBranch < 1 || currentBranch > 3) 
                Debug.LogError("CurrentBranch = " + currentBranch);
        }
    }
    public DialogueClip CurrentClip
    {
        get
        {
            int lastClip = dialogueClips.Count - 1;
            if (currentClip == lastClip)
                return dialogueClips[currentClip];
            else if (currentClip < lastClip)
                return dialogueClips[currentClip++];
            else
            {
                currentClip = lastClip;
                return dialogueClips[currentClip];
            }
        }
    }

    private void Awake()
    {
        dialogueClips = new List<DialogueClip>();
        currentClip = 0;
    }

    public override void LoadDialogueClip(DialogueClip dc)
    {
        base.LoadDialogueClip(dc);
        DialoguePool dp = dc as DialoguePool;

        // Don't load clips again
        if (dialogueClips.Count < 1)
        {
            foreach (DialogueClip clip in dp.DialogueClips)
                dialogueClips.Add(clip);
        }
    }

    public void LoadDialoguePool(DialoguePool dp)
    {
        developerNotes = dp.DeveloperNotes;
        journalNotes = new List<JournalNote>();
        foreach (JournalNote jn in dp.JournalNotes)
            journalNotes.Add(jn);
        dialoguePrompt = dp.DialoguePrompt;
        // Next Clips
        response_1_NextClip = dp.Response_1_NextClip;
        response_2_NextClip = dp.response_2_NextClip;
        response_3_NextClip = dp.Response_3_NextClip;
        // Responses
            // R1
        response_1 = response_1_NextClip.DialoguePoolResponse;
        response_1_Amiability = 
            response_1_NextClip.DialoguePoolResponse_Amiability;
        response_1_isExit = response_1_NextClip.DialoguePoolResponse_isExit;
            // R2
        response_2 = response_2_NextClip.DialoguePoolResponse;
        response_2_Amiability = 
            response_2_NextClip.DialoguePoolResponse_Amiability;
        response_2_isExit = response_2_NextClip.DialoguePoolResponse_isExit;
            // R3
        response_3 = response_3_NextClip.DialoguePoolResponse;
        response_3_Amiability = 
            response_3_NextClip.DialoguePoolResponse_Amiability;
        response_3_isExit = response_3_NextClip.DialoguePoolResponse_isExit;
        foreach (DialogueClip clip in dp.DialogueClips)
            dialogueClips.Add(clip);
    }

    public void LoadNewResponse(int branch, DialogueClip dc)
    {
        Debug.Log("LOAD_NEW_RESPONSE!");
        switch (branch)
        {
            case 1:
                response_1_NextClip = dc;
                response_1 = dc.DialoguePoolResponse;
                response_1_Amiability = dc.DialoguePoolResponse_Amiability;
                response_1_isExit = dc.DialoguePoolResponse_isExit;
                break;
            case 2:
                response_2_NextClip = dc;
                response_2 = dc.DialoguePoolResponse;
                response_2_Amiability = dc.DialoguePoolResponse_Amiability;
                response_2_isExit = dc.DialoguePoolResponse_isExit;
                break;
            case 3:
                response_3_NextClip = dc;
                response_3 = dc.DialoguePoolResponse;
                response_3_Amiability = dc.DialoguePoolResponse_Amiability;
                response_3_isExit = dc.DialoguePoolResponse_isExit;
                break;
            default:
                Debug.LogError("BRANCH = " + branch);
                break;
        }
    }
}
