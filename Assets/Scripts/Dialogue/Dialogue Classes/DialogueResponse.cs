using UnityEngine;

[System.Serializable]
public class DialogueResponse
{
    [TextArea] [SerializeField] private string responseText;
    [SerializeField] [Range(-10, 10)] protected int response_Respect;
    [SerializeField] private DialogueClip response_NextClip;
    [SerializeField] private bool response_IsCombatStart;
    [SerializeField] private bool response_IsWorldMapStart;
    [SerializeField] private bool response_IsRecruitmentStart;
    [SerializeField] private bool response_IsShopStart;
    [SerializeField] private bool response_IsCloningStart;
    [SerializeField] private bool response_IsExit;

    public string ResponseText { get => responseText; }
    public int Response_Respect { get => response_Respect; }
    public DialogueClip Response_NextClip { get => response_NextClip; }
    public bool Response_IsCombatStart { get => response_IsCombatStart; }
    public bool Response_IsWorldMapStart { get => response_IsWorldMapStart; }
    public bool Response_IsRecruitmentStart { get => response_IsRecruitmentStart; }
    public bool Response_IsShopStart { get => response_IsShopStart; }
    public bool Response_IsCloningStart { get => response_IsCloningStart; }
    public bool Response_IsExit { get => response_IsExit; }

    public void LoadResponse(DialogueResponse dr)
    {
        responseText = dr.ResponseText;
        response_Respect = dr.Response_Respect;
        response_NextClip = dr.Response_NextClip;
        response_IsCombatStart = dr.Response_IsCombatStart;
        response_IsWorldMapStart = dr.Response_IsWorldMapStart;
        response_IsRecruitmentStart = dr.Response_IsRecruitmentStart;
        response_IsShopStart = dr.Response_IsShopStart;
        response_IsCloningStart = dr.Response_IsCloningStart;
        response_IsExit = dr.Response_IsExit;
    }
}
