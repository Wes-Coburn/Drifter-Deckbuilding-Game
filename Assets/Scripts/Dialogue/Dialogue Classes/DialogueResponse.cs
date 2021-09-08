using UnityEngine;

[System.Serializable]
public class DialogueResponse
{
    [TextArea] [SerializeField] private string responseText;
    [SerializeField] [Range(-10, 10)] protected int response_Respect;
    [SerializeField] private DialogueClip response_NextClip;
    [SerializeField] private bool response_IsExit;
    [SerializeField] private bool response_IsCombatStart;
    //[SerializeField] private bool response_IsNarrativeStart;
    [SerializeField] private bool response_IsWorldMapStart;
    
    public string ResponseText { get => responseText; }
    public int Response_Respect { get => response_Respect; }
    public DialogueClip Response_NextClip { get => response_NextClip; }
    public bool Response_IsExit { get => response_IsExit; }
    public bool Response_IsCombatStart { get => response_IsCombatStart; }
    public bool Response_IsWorldMapStart { get => response_IsWorldMapStart; }

    public void LoadResponse(DialogueResponse dr)
    {
        responseText = dr.ResponseText;
        response_Respect = dr.Response_Respect;
        response_NextClip = dr.Response_NextClip;
        response_IsExit = dr.Response_IsExit;
        response_IsCombatStart = dr.Response_IsCombatStart;
        response_IsWorldMapStart = dr.response_IsWorldMapStart;
    }
}
