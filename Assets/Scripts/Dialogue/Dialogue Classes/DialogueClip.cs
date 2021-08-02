using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Clip", menuName = "Dialogue/Dialogue Clip")]
public class DialogueClip : ScriptableObject
{
    [Header("DEVELOPER NOTES")]
    [TextArea]
    [Tooltip("Notes For Development Only")]
    [SerializeField] protected string developerNotes;
    public string DeveloperNotes { get => developerNotes; }

    [Header("JOURNAL NOTES")]
    [Tooltip("The journal notes the clip triggers")]
    [SerializeField] protected List<JournalNote> journalNotes;
    public List<JournalNote> JournalNotes { get => journalNotes; }

    [Header("DIALOGUE PROMPT")]
    [TextArea]
    [Tooltip("The NPC dialogue that prompts a response")]
    [SerializeField] protected string dialoguePrompt;
    public string DialoguePrompt { get => dialoguePrompt; }

    [Header("DIALOGUE POOL RESPONSE")]
    [TextArea]
    [Tooltip("The initial response given in the dialogue pool that leads to this clip")]
    [SerializeField] private string dialoguePoolResponse;
    public string DialoguePoolResponse { get => dialoguePoolResponse; }
    [SerializeField] [Range(-5, 5)] private int dialoguePoolResponse_Respect;
    public int DialoguePoolResponse_Respect { get => dialoguePoolResponse_Respect; }
    [SerializeField] protected bool dialoguePoolResponse_isExit;
    public bool DialoguePoolResponse_isExit { get => dialoguePoolResponse_isExit; }

    [Header("DIALOGUE RESPONSES")]
    [TextArea]
    [SerializeField] protected string response_1;
    public string Response_1 { get => response_1; }
    [SerializeField] [Range(-5, 5)] protected int response_1_Clip;
    public int Response_1_Respect { get => response_1_Clip; }
    [SerializeField] protected DialogueClip response_1_NextClip;
    public DialogueClip Response_1_NextClip { get => response_1_NextClip; }
    [SerializeField] protected bool response_1_isExit;
    public bool Response_1_isExit { get => response_1_isExit; }
    
    [Space]
    [TextArea]
    [SerializeField] protected string response_2;
    public string Response_2
    { get => response_2; }
    [SerializeField] [Range(-5, 5)] protected int response_2_Respect;
    public int Response_2_Respect { get => response_2_Respect; }
    [SerializeField] protected DialogueClip response_2_NextClip;
    public DialogueClip Response_2_NextClip { get => response_2_NextClip; }
    [SerializeField] protected bool response_2_isExit;
    public bool Response_2_isExit { get => response_2_isExit; }

    [Space]
    [TextArea]
    [SerializeField] protected string response_3;
    public string Response_3 { get => response_3; }
    [SerializeField] [Range(-5, 5)] protected int response_3_Amiability;
    public int Response_3_Amiability { get => response_3_Amiability; }
    [SerializeField] protected DialogueClip response_3_NextClip;
    public DialogueClip Response_3_NextClip { get => response_3_NextClip; }
    [SerializeField] protected bool response_3_isExit;
    public bool Response_3_isExit { get => response_3_isExit; }

    public virtual void LoadDialogueClip(DialogueClip dc)
    {
        developerNotes = dc.DeveloperNotes;
        journalNotes = new List<JournalNote>();
        foreach (JournalNote jn in dc.journalNotes)
            journalNotes.Add(jn);
        dialoguePrompt = dc.dialoguePrompt;

        response_1 = dc.response_1;
        response_1_Clip = dc.response_1_Clip;
        response_1_isExit = dc.response_1_isExit;
        response_1_NextClip = dc.response_1_NextClip;

        response_2 = dc.response_2;
        response_2_Respect = dc.response_2_Respect;
        response_2_isExit = dc.response_2_isExit;
        response_2_NextClip = dc.response_2_NextClip;

        response_3 = dc.response_3;
        response_3_Amiability = dc.response_3_Amiability;
        response_3_isExit = dc.response_3_isExit;
        response_3_NextClip = dc.response_3_NextClip;
    }
}