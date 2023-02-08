using UnityEngine;

[System.Serializable]
public class DialogueResponse
{
    [TextArea][SerializeField] private string responseText;
    [SerializeField] private DialogueClip response_NextClip;
    [SerializeField] private DialogueClip npc_NextClip;
    [SerializeField] private bool response_IsCombatStart;
    [SerializeField] private bool response_IsWorldMapStart;
    [SerializeField] private bool response_IsRecruitmentStart;
    [SerializeField] private bool response_IsActionShopStart;
    [SerializeField] private bool response_IsShopStart;
    [SerializeField] private bool response_IsCloningStart;
    [SerializeField] private bool response_IsNewAugmentStart;
    [SerializeField] private bool response_IsExit;
    [SerializeField] private Location response_TravelLocation;

    public string ResponseText { get => responseText; }
    public DialogueClip Response_NextClip { get => response_NextClip; }
    public DialogueClip NPC_NextClip { get => npc_NextClip; }
    public bool Response_IsCombatStart { get => response_IsCombatStart; }
    public bool Response_IsWorldMapStart { get => response_IsWorldMapStart; }
    public bool Response_IsRecruitmentStart { get => response_IsRecruitmentStart; }
    public bool Response_IsActionShopStart { get => response_IsActionShopStart; }
    public bool Response_IsShopStart { get => response_IsShopStart; }
    public bool Response_IsCloningStart { get => response_IsCloningStart; }
    public bool Response_IsNewAugmentStart { get => response_IsNewAugmentStart; }
    public bool Response_IsExit { get => response_IsExit; }
    public Location Response_TravelLocation { get => response_TravelLocation; }

    public void LoadResponse(DialogueResponse dr)
    {
        responseText = dr.ResponseText;
        response_NextClip = dr.Response_NextClip;
        npc_NextClip = dr.NPC_NextClip;
        response_IsCombatStart = dr.Response_IsCombatStart;
        response_IsWorldMapStart = dr.Response_IsWorldMapStart;
        response_IsRecruitmentStart = dr.Response_IsRecruitmentStart;
        response_IsActionShopStart = dr.Response_IsActionShopStart;
        response_IsShopStart = dr.Response_IsShopStart;
        response_IsCloningStart = dr.Response_IsCloningStart;
        response_IsNewAugmentStart = dr.Response_IsNewAugmentStart;
        response_IsExit = dr.Response_IsExit;
        response_TravelLocation = dr.Response_TravelLocation;
    }
}
